using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model.Transfer
{
    /// <summary>
    /// A reference of a transferred table to a row of another table: a foreign key of the
    /// model or a reference the catalog lists.
    /// </summary>
    internal sealed class TransferForeignKey
    {
        /// <summary>
        /// Returns the property holding the reference.
        /// </summary>
        public IProperty Property { get; init; }

        /// <summary>
        /// Returns the table referred to.
        /// </summary>
        public TransferTableSchema Target { get; init; }

        /// <summary>
        /// Returns the property of the target the value names in the export file.
        /// </summary>
        public string TargetProperty { get; init; }

        /// <summary>
        /// Returns the property of the target the value names in the database. It differs from
        /// <see cref="TargetProperty"/> for a foreign key to a database id, which the file
        /// writes as the id of the row it points to.
        /// </summary>
        public string DatabaseTargetProperty { get; init; }

        /// <summary>
        /// Returns whether the database enforces the reference (a declared foreign key).
        /// </summary>
        public bool Declared { get; init; }

        /// <summary>
        /// Returns whether a row cannot exist without the referenced row.
        /// </summary>
        public bool Required { get; init; }

        /// <summary>
        /// Returns whether the property holds a list of references.
        /// </summary>
        public bool Many { get; init; }

        /// <summary>
        /// Returns whether the database value is a database id that the file replaces with the
        /// id of the row.
        /// </summary>
        public bool ViaDatabaseId => !string.Equals(TargetProperty, DatabaseTargetProperty, StringComparison.Ordinal);
    }

    /// <summary>
    /// A membership condition of a table, resolved against the model.
    /// </summary>
    internal sealed class TransferMembership
    {
        /// <summary>
        /// Returns the property of the table the condition is on.
        /// </summary>
        public IProperty Property { get; init; }

        /// <summary>
        /// Returns the property in the database terms (a database id where the file names an id).
        /// </summary>
        public IProperty DatabaseProperty { get; init; }

        /// <summary>
        /// Returns the table whose rows give the values.
        /// </summary>
        public TransferTableSchema Source { get; init; }

        /// <summary>
        /// Returns the property of the source in the file's terms.
        /// </summary>
        public string SourceProperty { get; init; }

        /// <summary>
        /// Returns the property of the source in the database terms.
        /// </summary>
        public string DatabaseSourceProperty { get; init; }
    }

    /// <summary>
    /// One table of the model as the transfer sees it.
    /// </summary>
    internal sealed class TransferTableSchema
    {
        /// <summary>
        /// Returns the catalog entry.
        /// </summary>
        public WorkspaceTransferTable Entry { get; init; }

        /// <summary>
        /// Returns the root entity type of the table.
        /// </summary>
        public IEntityType EntityType { get; init; }

        /// <summary>
        /// Returns the entity name.
        /// </summary>
        public string Name => Entry.Name;

        /// <summary>
        /// Returns the name of the database table.
        /// </summary>
        public string TableName => EntityType.GetTableName() ?? Name;

        /// <summary>
        /// Returns the role.
        /// </summary>
        public WorkspaceTransferRole Role => Entry.Role;

        /// <summary>
        /// Returns the store-generated primary key (the database id), which is never transferred,
        /// or <see langword="null"/> when the key is made of values.
        /// </summary>
        public IProperty DatabaseId { get; init; }

        /// <summary>
        /// Returns the references to other tables.
        /// </summary>
        public List<TransferForeignKey> ForeignKeys { get; } = [];

        /// <summary>
        /// Returns the membership conditions (owners, or the references of the content for a
        /// shared or external table).
        /// </summary>
        public List<TransferMembership> Memberships { get; } = [];

        /// <summary>
        /// Returns the transferred properties of an entity type of the table.
        /// </summary>
        /// <param name="type">The entity type (the root or a derived type).</param>
        /// <returns>The properties.</returns>
        public IEnumerable<IProperty> Properties(IEntityType type)
        {
            return type.GetProperties().Where(x => DatabaseId is null || x != DatabaseId);
        }

        /// <summary>
        /// Returns the property of a name on any entity type of the table.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The property, or <see langword="null"/>.</returns>
        public IProperty FindProperty(string name)
        {
            return EntityType.GetDerivedTypesInclusive()
                .Select(x => x.FindProperty(name))
                .FirstOrDefault(x => x is not null);
        }

        /// <summary>
        /// Returns whether a table refers to a database id of this one through a property.
        /// </summary>
        public bool HasDatabaseIdReferences => ForeignKeys.Any(x => x.ViaDatabaseId);
    }

    /// <summary>
    /// The model as the transfer sees it: every classified table with its references and
    /// membership conditions, resolved once per model.
    /// </summary>
    internal sealed class WorkspaceTransferSchema
    {
        private readonly Dictionary<string, TransferTableSchema> _tables = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Returns the transferred tables (root, content, shared, external) in an order in which
        /// every owner comes before the tables it owns.
        /// </summary>
        public IReadOnlyList<TransferTableSchema> Tables { get; }

        /// <summary>
        /// Returns the entity types of the model that the catalog does not list.
        /// </summary>
        public IReadOnlyList<string> Unclassified { get; }

        /// <summary>
        /// Builds the schema of a model.
        /// </summary>
        /// <param name="model">The model.</param>
        public WorkspaceTransferSchema(IModel model)
        {
            var unclassified = new List<string>();

            foreach (var type in model.GetEntityTypes().Where(x => x.BaseType is null && !x.IsOwned()))
            {
                var entry = WorkspaceTransferCatalog.Find(NameOf(type));

                if (entry is null)
                {
                    unclassified.Add(NameOf(type));
                    continue;
                }

                if (entry.Role == WorkspaceTransferRole.Excluded)
                {
                    continue;
                }

                var key = type.FindPrimaryKey();
                var databaseId = key is { Properties.Count: 1 } && key.Properties[0].ValueGenerated != ValueGenerated.Never
                    ? key.Properties[0]
                    : null;

                _tables[entry.Name] = new TransferTableSchema { Entry = entry, EntityType = type, DatabaseId = databaseId };
            }

            Unclassified = unclassified;

            foreach (var table in _tables.Values)
            {
                ResolveForeignKeys(table);
            }

            foreach (var table in _tables.Values)
            {
                ResolveMemberships(table);
            }

            Tables = Order();
        }

        /// <summary>
        /// Returns a table by its entity or database table name.
        /// </summary>
        /// <param name="name">The name (compared ignoring case).</param>
        /// <returns>The table, or <see langword="null"/>.</returns>
        public TransferTableSchema Find(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return _tables.TryGetValue(name, out var table)
                ? table
                : _tables.Values.FirstOrDefault(x => string.Equals(x.TableName, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns the entity name of an entity type: its class name, or the name of a join table.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>The name.</returns>
        public static string NameOf(IReadOnlyEntityType type)
        {
            var root = type.GetRootType();

            return root.HasSharedClrType ? root.Name : root.ClrType.Name;
        }

        /// <summary>
        /// Collects the declared foreign keys of every entity type of a table and the references
        /// its catalog entry lists.
        /// </summary>
        /// <param name="table">The table.</param>
        private void ResolveForeignKeys(TransferTableSchema table)
        {
            var seen = new HashSet<IProperty>();

            foreach (var fk in table.EntityType.GetDerivedTypesInclusive().SelectMany(x => x.GetDeclaredForeignKeys()))
            {
                if (fk.Properties.Count != 1 || !seen.Add(fk.Properties[0]))
                {
                    continue;
                }

                var principal = Find(NameOf(fk.PrincipalEntityType));
                var principalProperty = fk.PrincipalKey.Properties[0];

                if (principal is null)
                {
                    // a reference to a table outside the transfer (an excluded table): kept when
                    // the target installation has the row, which only the database can tell
                    continue;
                }

                var viaDatabaseId = principal.DatabaseId == principalProperty && principal.EntityType.FindProperty("Id") is not null;

                table.ForeignKeys.Add(new TransferForeignKey
                {
                    Property = fk.Properties[0],
                    Target = principal,
                    TargetProperty = viaDatabaseId ? "Id" : principalProperty.Name,
                    DatabaseTargetProperty = principalProperty.Name,
                    Declared = true,
                    Required = fk.IsRequired
                });
            }

            foreach (var reference in table.Entry.References)
            {
                var property = table.FindProperty(reference.Property);
                var target = Find(reference.Target);

                if (property is null || target is null || !seen.Add(property))
                {
                    continue;
                }

                table.ForeignKeys.Add(new TransferForeignKey
                {
                    Property = property,
                    Target = target,
                    TargetProperty = reference.TargetProperty,
                    DatabaseTargetProperty = reference.TargetProperty,
                    Declared = false,
                    Required = !property.IsNullable && !IsList(property),
                    Many = IsList(property)
                });
            }
        }

        /// <summary>
        /// Resolves the owners of a content table, and for a shared or external table the
        /// references the transferred tables hold to it.
        /// </summary>
        /// <param name="table">The table.</param>
        private void ResolveMemberships(TransferTableSchema table)
        {
            if (table.Role == WorkspaceTransferRole.Content)
            {
                foreach (var owner in table.Entry.Owners)
                {
                    var property = table.FindProperty(owner.Property);
                    var source = Find(owner.Source);

                    if (property is null || source is null)
                    {
                        continue;
                    }

                    var fk = table.ForeignKeys.FirstOrDefault(x => x.Property == property && x.Target == source);
                    var sourceFk = source.ForeignKeys.FirstOrDefault(x => x.Property.Name == owner.SourceProperty && x.Target == table);

                    table.Memberships.Add(new TransferMembership
                    {
                        Property = property,
                        // an owner reached through a database id is compared in database ids
                        DatabaseProperty = sourceFk is { ViaDatabaseId: true } ? table.DatabaseId : property,
                        Source = source,
                        SourceProperty = owner.SourceProperty,
                        DatabaseSourceProperty = fk?.DatabaseTargetProperty ?? owner.SourceProperty
                    });
                }
            }
            else if (table.Role is WorkspaceTransferRole.Shared or WorkspaceTransferRole.External)
            {
                foreach (var source in _tables.Values.Where(x => x.Role is WorkspaceTransferRole.Root or WorkspaceTransferRole.Content))
                {
                    foreach (var fk in source.ForeignKeys.Where(x => x.Target == table))
                    {
                        table.Memberships.Add(new TransferMembership
                        {
                            Property = table.FindProperty(fk.TargetProperty),
                            DatabaseProperty = table.FindProperty(fk.DatabaseTargetProperty),
                            Source = source,
                            SourceProperty = fk.Property.Name,
                            DatabaseSourceProperty = fk.Property.Name
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Orders the tables: the root first, then the content in the order of its owners, then
        /// the shared and the external tables, which only the content decides about.
        /// </summary>
        /// <returns>The ordered tables.</returns>
        private List<TransferTableSchema> Order()
        {
            var ordered = new List<TransferTableSchema>();
            var placed = new HashSet<TransferTableSchema>();
            var content = _tables.Values
                .Where(x => x.Role == WorkspaceTransferRole.Content)
                .OrderBy(x => x.Name, StringComparer.Ordinal)
                .ToList();

            ordered.AddRange(_tables.Values.Where(x => x.Role == WorkspaceTransferRole.Root));
            placed.UnionWith(ordered);

            while (content.Count > 0)
            {
                var next = content.FirstOrDefault(x => x.Memberships.All(m => placed.Contains(m.Source) || m.Source.Role != WorkspaceTransferRole.Content && m.Source.Role != WorkspaceTransferRole.Root))
                    // a table whose owners wait on each other is placed once one of them is
                    ?? content.FirstOrDefault(x => x.Memberships.Any(m => placed.Contains(m.Source)))
                    ?? content[0];

                ordered.Add(next);
                placed.Add(next);
                content.Remove(next);
            }

            ordered.AddRange(_tables.Values.Where(x => x.Role == WorkspaceTransferRole.Shared).OrderBy(x => x.Name, StringComparer.Ordinal));
            ordered.AddRange(_tables.Values.Where(x => x.Role == WorkspaceTransferRole.External).OrderBy(x => x.Name, StringComparer.Ordinal));

            return ordered;
        }

        /// <summary>
        /// Returns whether a property holds a list.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns><see langword="true"/> for a list.</returns>
        private static bool IsList(IReadOnlyProperty property)
        {
            return property.ClrType != typeof(string) && property.ClrType != typeof(byte[])
                && typeof(System.Collections.IEnumerable).IsAssignableFrom(property.ClrType);
        }
    }
}
