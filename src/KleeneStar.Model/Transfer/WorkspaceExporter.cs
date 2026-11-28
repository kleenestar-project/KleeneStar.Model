using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Transfer
{
    /// <summary>
    /// Writes workspaces with everything that belongs to them into an export file.
    /// </summary>
    /// <remarks>
    /// The tables are visited in the order of their owners: the workspaces, then what they own,
    /// then what that owns, and so on; a row belongs to the export when one of its owners does.
    /// The shared and external tables come last, with the rows the content refers to. Every
    /// table is read, written and forgotten before the next, so only the keys of the rows stay
    /// in memory.
    /// </remarks>
    internal sealed class WorkspaceExporter
    {
        private static readonly JsonWriterOptions _writerOptions = new() { Indented = true };

        private readonly DbContext _context;
        private readonly WorkspaceTransferSchema _schema;
        private readonly Dictionary<TransferTableSchema, HashSet<string>> _collect = [];
        private readonly Dictionary<(TransferTableSchema, string), HashSet<object>> _values = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The database context to read from.</param>
        /// <param name="schema">The schema of the model.</param>
        public WorkspaceExporter(DbContext context, WorkspaceTransferSchema schema)
        {
            _context = context;
            _schema = schema;

            // the values a later table is decided by
            foreach (var table in schema.Tables)
            {
                foreach (var membership in table.Memberships)
                {
                    Collect(membership.Source).Add(membership.DatabaseSourceProperty);
                }
            }
        }

        /// <summary>
        /// Returns or sets what is told of the progress: the share done (0 to 1) and the phase
        /// (<c>export.table</c> with the table, <c>import.read</c>, <c>import.check</c>,
        /// <c>import.write</c>). May be <see langword="null"/>.
        /// </summary>
        public Action<double, string, string> Progress { get; set; }

        /// <summary>
        /// Returns or sets the resolver for transferred icon data.
        /// </summary>
        public Func<ImageIcon, ImageIcon> ExportIcon { get; set; }

        /// <summary>
        /// Returns or sets whether objects and their dependent data are omitted.
        /// </summary>
        public bool StructureOnly { get; set; }

        /// <summary>
        /// Writes the export.
        /// </summary>
        /// <param name="workspaceIds">The workspaces to export.</param>
        /// <param name="output">The stream the zip archive is written to.</param>
        /// <returns>What was written.</returns>
        public WorkspaceTransferReport Export(IReadOnlyCollection<Guid> workspaceIds, Stream output)
        {
            var report = new WorkspaceTransferReport
            {
                FormatVersion = WorkspaceTransferFormat.Version,
                Schema = _context.Database.GetMigrations().LastOrDefault()
            };

            var tracking = _context.ChangeTracker.AutoDetectChangesEnabled;
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            try
            {
                using var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true);

                var done = 0;

                foreach (var table in _schema.Tables)
                {
                    Progress?.Invoke((double)done++ / _schema.Tables.Count, "export.table", table.Name);

                    var tableReport = ExportTable(archive, table, workspaceIds, report);

                    if (tableReport is not null)
                    {
                        report.Tables.Add(tableReport);
                    }

                    _context.ChangeTracker.Clear();
                }

                Progress?.Invoke(1, "export.finish", null);

                WriteManifest(archive, report);
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = tracking;
            }

            report.Succeeded = true;

            return report;
        }

        /// <summary>
        /// Reads the rows of a table that belong to the export and writes them.
        /// </summary>
        /// <param name="archive">The archive receiving the table.</param>
        /// <param name="table">The table to export.</param>
        /// <param name="workspaceIds">The selected workspace identifiers.</param>
        /// <param name="report">The report receiving workspace metadata.</param>
        /// <returns>The table report, or null for unused external tables.</returns>
        private WorkspaceTransferTableReport ExportTable(ZipArchive archive, TransferTableSchema table, IReadOnlyCollection<Guid> workspaceIds, WorkspaceTransferReport report)
        {
            var rows = Load(table, workspaceIds);

            if (rows.Count == 0 && table.Role == WorkspaceTransferRole.External)
            {
                return null;
            }

            var translations = Translations(table, rows);
            var external = table.Role == WorkspaceTransferRole.External;
            var folder = external ? WorkspaceTransferFormat.ReferencesFolder : WorkspaceTransferFormat.TablesFolder;
            var entry = archive.CreateEntry(folder + table.Name + ".json", CompressionLevel.Optimal);

            using (var stream = entry.Open())
            using (var writer = new Utf8JsonWriter(stream, _writerOptions))
            {
                writer.WriteStartArray();

                foreach (var row in rows)
                {
                    var type = TransferEntity.TypeOf(table.EntityType, row);

                    writer.WriteStartObject();

                    foreach (var property in Written(table, type))
                    {
                        var value = TransferEntity.Get(_context, type, row, property);

                        writer.WritePropertyName(property.Name);

                        if (StructureOnly && table.ForeignKeys.Any(x => x.Property == property && x.Target.Name == "Object"))
                        {
                            writer.WriteNullValue();
                        }
                        else if (translations.TryGetValue(property, out var translation))
                        {
                            TransferValue.WriteScalar(writer, value is not null && translation.TryGetValue(value, out var id) ? id : null);
                        }
                        else
                        {
                            TransferValue.Write(writer, property, value is ImageIcon icon && ExportIcon is not null ? ExportIcon(icon) : value);
                        }
                    }

                    writer.WriteEndObject();

                    Remember(table, type, row);

                    if (table.Role == WorkspaceTransferRole.Root)
                    {
                        report.Workspaces.Add(new WorkspaceTransferWorkspace
                        {
                            Id = (Guid)TransferEntity.Get(_context, type, row, type.FindProperty("Id")),
                            Key = TransferEntity.Get(_context, type, row, type.FindProperty("Key")) as string,
                            Name = TransferEntity.Get(_context, type, row, type.FindProperty("Name")) as string
                        });
                    }
                }

                writer.WriteEndArray();
            }

            return new WorkspaceTransferTableReport { Name = table.Name, Rows = rows.Count, Written = rows.Count };
        }

        /// <summary>
        /// Returns the rows of the workspaces and of everything they own, tracked by the context,
        /// without writing anything: what an import that overwrites the workspaces replaces.
        /// </summary>
        /// <param name="workspaceIds">The workspaces.</param>
        /// <returns>The rows per table (root and content tables only).</returns>
        public Dictionary<TransferTableSchema, List<object>> Select(IReadOnlyCollection<Guid> workspaceIds)
        {
            var result = new Dictionary<TransferTableSchema, List<object>>();

            foreach (var table in _schema.Tables.Where(x => x.Role is WorkspaceTransferRole.Root or WorkspaceTransferRole.Content))
            {
                var rows = Load(table, workspaceIds, ownedOnly: true);

                foreach (var row in rows)
                {
                    Remember(table, TransferEntity.TypeOf(table.EntityType, row), row);
                }

                result[table] = rows;
            }

            return result;
        }

        /// <summary>
        /// Loads the rows of a table that belong to the workspaces, tracked: the workspaces
        /// themselves, or the rows one of whose owners was loaded before.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="workspaceIds">The workspaces.</param>
        /// <param name="ownedOnly">Whether only owners count, not a row that merely refers to
        /// one of the table (an installation-wide catalog a field is bound to is exported with
        /// the field, but it is not the workspace's).</param>
        private List<object> Load(TransferTableSchema table, IReadOnlyCollection<Guid> workspaceIds, bool ownedOnly = false)
        {
            if (StructureOnly && table.Name == "Object")
            {
                return [];
            }

            var conditions = new List<(IProperty, IEnumerable<object>)>();

            if (table.Role == WorkspaceTransferRole.Root)
            {
                conditions.Add((table.FindProperty("Id"), workspaceIds.Cast<object>()));
            }
            else
            {
                foreach (var membership in table.Memberships)
                {
                    if (ownedOnly && (membership.Property.Name == "Id" || membership.DatabaseProperty == table.DatabaseId))
                    {
                        continue;
                    }

                    if (membership.DatabaseProperty is not null
                        && _values.TryGetValue((membership.Source, membership.DatabaseSourceProperty), out var values)
                        && values.Count > 0)
                    {
                        conditions.Add((membership.DatabaseProperty, values));
                    }
                }
            }

            return conditions.Count == 0
                ? []
                : TransferQuery.Load(_context, table.EntityType, conditions, tracking: true);
        }

        /// <summary>
        /// Returns the properties written for a row: all of them, or for an external table the
        /// id and what the row is recognised by.
        /// </summary>
        private static IEnumerable<IProperty> Written(TransferTableSchema table, IEntityType type)
        {
            var properties = table.Properties(type);

            if (table.Role != WorkspaceTransferRole.External)
            {
                return properties;
            }

            return properties.Where(x => x.Name == "Id" || table.Entry.MatchBy.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns, for every reference of a table to a database id, the ids of the rows the
        /// database ids name.
        /// </summary>
        private Dictionary<IProperty, Dictionary<object, object>> Translations(TransferTableSchema table, List<object> rows)
        {
            var translations = new Dictionary<IProperty, Dictionary<object, object>>();

            foreach (var fk in table.ForeignKeys.Where(x => x.ViaDatabaseId))
            {
                var raw = rows
                    .Select(x => TransferEntity.Get(_context, TransferEntity.TypeOf(table.EntityType, x), x, fk.Property))
                    .Where(x => x is not null)
                    .Distinct()
                    .ToList();

                var map = new Dictionary<object, object>();
                var target = fk.Target;
                var idProperty = target.FindProperty(fk.TargetProperty);

                if (raw.Count > 0 && target.DatabaseId is not null && idProperty is not null)
                {
                    foreach (var principal in TransferQuery.Load(_context, target.EntityType, [(target.DatabaseId, raw)], tracking: false))
                    {
                        var type = TransferEntity.TypeOf(target.EntityType, principal);

                        map[TransferEntity.Get(_context, type, principal, target.DatabaseId)] = TransferEntity.Get(_context, type, principal, idProperty);
                    }
                }

                translations[fk.Property] = map;
            }

            return translations;
        }

        /// <summary>
        /// Keeps the values of a row that later tables are decided by.
        /// </summary>
        private void Remember(TransferTableSchema table, IEntityType type, object row)
        {
            if (!_collect.TryGetValue(table, out var names))
            {
                return;
            }

            foreach (var name in names)
            {
                var property = type.FindProperty(name);

                if (property is null)
                {
                    continue;
                }

                if (!_values.TryGetValue((table, name), out var set))
                {
                    _values[(table, name)] = set = [];
                }

                foreach (var value in TransferValue.Values(TransferEntity.Get(_context, type, row, property)))
                {
                    set.Add(value);
                }
            }
        }

        /// <summary>
        /// Returns the names of the values kept for a table.
        /// </summary>
        private HashSet<string> Collect(TransferTableSchema table)
        {
            if (!_collect.TryGetValue(table, out var names))
            {
                _collect[table] = names = new HashSet<string>(StringComparer.Ordinal);
            }

            return names;
        }

        /// <summary>
        /// Writes the manifest: the format, the workspaces and the columns of every table.
        /// </summary>
        private void WriteManifest(ZipArchive archive, WorkspaceTransferReport report)
        {
            var entry = archive.CreateEntry(WorkspaceTransferFormat.Manifest, CompressionLevel.Optimal);

            using var stream = entry.Open();
            using var writer = new Utf8JsonWriter(stream, _writerOptions);

            writer.WriteStartObject();
            writer.WriteString("format", WorkspaceTransferFormat.Name);
            writer.WriteNumber("formatVersion", WorkspaceTransferFormat.Version);
            writer.WriteString("created", DateTime.UtcNow.ToString("O", System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteString("schema", report.Schema);
            writer.WriteString("application", typeof(WorkspaceExporter).Assembly.GetName().Version?.ToString());

            writer.WriteStartArray("workspaces");

            foreach (var workspace in report.Workspaces)
            {
                writer.WriteStartObject();
                writer.WriteString("id", workspace.Id);
                writer.WriteString("key", workspace.Key);
                writer.WriteString("name", workspace.Name);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            writer.WriteStartArray("tables");

            foreach (var table in _schema.Tables)
            {
                var written = report.Tables.FirstOrDefault(x => x.Name == table.Name);

                if (written is null)
                {
                    continue;
                }

                writer.WriteStartObject();
                writer.WriteString("name", table.Name);
                writer.WriteString("table", table.TableName);
                writer.WriteString("role", table.Role.ToString());
                writer.WriteString("file", (table.Role == WorkspaceTransferRole.External ? WorkspaceTransferFormat.ReferencesFolder : WorkspaceTransferFormat.TablesFolder) + table.Name + ".json");
                writer.WriteNumber("rows", written.Rows);

                if (table.EntityType.FindDiscriminatorProperty() is { } discriminator)
                {
                    writer.WriteString("discriminator", discriminator.Name);
                }

                writer.WriteStartArray("columns");

                var seen = new HashSet<string>(StringComparer.Ordinal);

                foreach (var type in table.EntityType.GetDerivedTypesInclusive())
                {
                    foreach (var property in Written(table, type).Where(x => seen.Add(x.Name)))
                    {
                        var converter = property.GetValueConverter();
                        var clr = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;
                        var fk = table.ForeignKeys.FirstOrDefault(x => x.Property == property);

                        writer.WriteStartObject();
                        writer.WriteString("name", property.Name);
                        writer.WriteString("column", property.GetColumnName() ?? property.Name);
                        writer.WriteString("type", fk is { ViaDatabaseId: true } ? "Guid" : converter?.ProviderClrType.Name ?? (clr.IsEnum ? "enum " + clr.Name : clr.Name));
                        writer.WriteBoolean("nullable", property.IsNullable);

                        if (fk is not null)
                        {
                            writer.WriteString("reference", fk.Target.Name + "." + fk.TargetProperty);
                        }

                        if (type != table.EntityType)
                        {
                            writer.WriteString("type-of", type.ClrType.Name);
                        }

                        writer.WriteEndObject();
                    }
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
