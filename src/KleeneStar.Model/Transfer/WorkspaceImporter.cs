using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;

namespace KleeneStar.Model.Transfer
{
    /// <summary>
    /// Reads an export file into the database, as tolerantly as a file written by an older or a
    /// newer version of the application needs.
    /// </summary>
    /// <remarks>
    /// Tolerance means: a table this version does not know is left out, a column it does not
    /// know is left out, a column the file lacks stays empty, a value that cannot be read stays
    /// empty, and a reference to a row that neither the file nor the database holds is cleared
    /// when the reference may be empty, or leaves the row out when the row cannot exist without
    /// it. Rows are kept with their ids, so references inside the file stay intact.
    /// <para>
    /// A workspace that is already present (the same id, key or name) is not imported; neither is
    /// anything it owns. Shared reference data (categories, status categories, relation types) is
    /// reused when a row of the same id or name exists and created otherwise; accounts, groups
    /// and tenants are never created, a reference to one is mapped by id or name.
    /// </para>
    /// <para>
    /// Everything is written in one transaction: an import takes effect as a whole or not at all.
    /// </para>
    /// </remarks>
    internal sealed class WorkspaceImporter
    {
        /// <summary>
        /// A row of the file.
        /// </summary>
        private sealed class Row
        {
            public TransferTableSchema Table { get; init; }

            public IEntityType Type { get; init; }

            public Dictionary<IProperty, object> Values { get; } = [];

            public bool Live { get; set; } = true;

            public bool Reused { get; set; }

            /// <summary>
            /// The present row of the same id an overwrite updates, or <see langword="null"/>.
            /// </summary>
            public object Target { get; set; }

            public string Reason { get; set; }

            public string ReasonDetail { get; set; }

            public object Value(string name)
            {
                var property = Type.FindProperty(name);

                return property is not null && Values.TryGetValue(property, out var value) ? value : null;
            }

            public void Drop(string reason, string detail = null)
            {
                Live = false;
                Reason = reason;
                ReasonDetail = detail;
            }
        }

        private static readonly JsonDocumentOptions _documentOptions = new()
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
            MaxDepth = 256
        };

        private readonly DbContext _context;
        private readonly WorkspaceTransferSchema _schema;
        private readonly WorkspaceTransferReport _report = new();
        private readonly Dictionary<TransferTableSchema, List<Row>> _rows = [];
        private readonly Dictionary<TransferTableSchema, List<Row>> _references = [];
        private readonly Dictionary<(TransferTableSchema, string), Dictionary<object, object>> _map = [];
        private readonly Dictionary<(TransferTableSchema, string), HashSet<object>> _existing = [];
        private readonly List<JsonDocument> _documents = [];

        /// <summary>
        /// The rows of the workspaces an overwrite replaces, per table (tracked).
        /// </summary>
        private Dictionary<TransferTableSchema, List<object>> _replaced = [];

        /// <summary>
        /// The rows of <see cref="_replaced"/> with an id, by id.
        /// </summary>
        private readonly Dictionary<TransferTableSchema, Dictionary<object, object>> _replacedById = [];

        /// <summary>
        /// The rows of <see cref="_replaced"/> the file no longer holds: deleted by the overwrite.
        /// </summary>
        private readonly Dictionary<TransferTableSchema, List<object>> _deleted = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The database context to write to.</param>
        /// <param name="schema">The schema of the model.</param>
        public WorkspaceImporter(DbContext context, WorkspaceTransferSchema schema)
        {
            _context = context;
            _schema = schema;
        }

        /// <summary>
        /// Returns or sets what is told of the progress: the share done (0 to 1) and the phase
        /// (<c>export.table</c> with the table, <c>import.read</c>, <c>import.check</c>,
        /// <c>import.write</c>). May be <see langword="null"/>.
        /// </summary>
        public Action<double, string, string> Progress { get; set; }

        /// <summary>
        /// Imports an export file.
        /// </summary>
        /// <param name="input">The zip archive.</param>
        /// <param name="options">The choices of the import.</param>
        /// <returns>What was imported, left out and left empty.</returns>
        public WorkspaceTransferReport Import(Stream input, WorkspaceImportOptions options)
        {
            options ??= new WorkspaceImportOptions();

            try
            {
                if (!Read(input))
                {
                    return _report;
                }

                Progress?.Invoke(0.30, "import.check", null);
                Select(options);
                ResolveShared();
                ResolveExternal();
                LoadReplaced(options);
                Progress?.Invoke(0.38, "import.check", null);
                CheckPresent();
                CheckUnique();
                Progress?.Invoke(0.45, "import.check", null);
                MapReferences();
                LoadExisting();
                Progress?.Invoke(0.52, "import.check", null);
                Settle();
                DropUnusedShared();
                PlanDeletions();
                Summarize();
                Progress?.Invoke(options.DryRun ? 1 : 0.6, "import.check", null);

                if (_report.Workspaces.All(x => x.Skipped is not null))
                {
                    _report.Error ??= "no-workspace";

                    return _report;
                }

                if (!options.DryRun)
                {
                    try
                    {
                        Write();
                    }
                    catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException or ArgumentException)
                    {
                        _report.Error = "write-failed";
                        _report.Note("write-failed", detail: (ex.InnerException ?? ex).Message);

                        return _report;
                    }
                }

                _report.Succeeded = true;

                return _report;
            }
            finally
            {
                foreach (var document in _documents)
                {
                    document.Dispose();
                }
            }
        }

        /// <summary>
        /// Reads the archive: the manifest, the tables and the references.
        /// </summary>
        /// <returns><see langword="false"/> when the file is no export.</returns>
        private bool Read(Stream input)
        {
            ZipArchive archive;

            try
            {
                archive = new ZipArchive(input, ZipArchiveMode.Read, leaveOpen: true);
            }
            catch (InvalidDataException)
            {
                _report.Error = "not-a-zip";

                return false;
            }

            using (archive)
            {
                var entries = archive.Entries
                    .Where(x => x.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var manifest = entries.FirstOrDefault(x => string.Equals(Normalize(x.FullName), WorkspaceTransferFormat.Manifest, StringComparison.OrdinalIgnoreCase));

                if (manifest is not null && !ReadManifest(manifest))
                {
                    return false;
                }

                var others = entries.Where(x => x != manifest).ToList();

                for (var i = 0; i < others.Count; i++)
                {
                    Progress?.Invoke(0.3 * i / Math.Max(1, others.Count), "import.read", Path.GetFileNameWithoutExtension(others[i].Name));
                    ReadEntry(others[i]);
                }
            }

            if (!_rows.Any(x => x.Key.Role == WorkspaceTransferRole.Root && x.Value.Count > 0))
            {
                _report.Error ??= "no-workspace";

                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads the manifest: refuses a file of another format, notes a newer version.
        /// </summary>
        private bool ReadManifest(ZipArchiveEntry entry)
        {
            try
            {
                using var stream = entry.Open();
                using var document = JsonDocument.Parse(stream, _documentOptions);
                var root = document.RootElement;

                if (root.ValueKind != JsonValueKind.Object)
                {
                    return true;
                }

                if (Member(root, "format") is { ValueKind: JsonValueKind.String } format
                    && !string.Equals(format.GetString(), WorkspaceTransferFormat.Name, StringComparison.OrdinalIgnoreCase))
                {
                    _report.Error = "wrong-format";

                    return false;
                }

                if (Member(root, "formatVersion") is { } version && TransferValue.TryReadScalar(version, typeof(int), out var number) && number is int v)
                {
                    _report.FormatVersion = v;

                    if (v > WorkspaceTransferFormat.Version)
                    {
                        _report.Note("format-newer", detail: v.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                }

                if (Member(root, "schema") is { ValueKind: JsonValueKind.String } schema)
                {
                    _report.Schema = schema.GetString();
                }
            }
            catch (JsonException)
            {
                // an unreadable manifest is no reason to give up the tables
                _report.Note("file-unreadable", detail: WorkspaceTransferFormat.Manifest);
            }

            return true;
        }

        /// <summary>
        /// Reads a table or a reference file.
        /// </summary>
        private void ReadEntry(ZipArchiveEntry entry)
        {
            var path = Normalize(entry.FullName);
            var name = Path.GetFileNameWithoutExtension(entry.Name);
            var table = _schema.Find(name);

            if (table is null)
            {
                var known = WorkspaceTransferCatalog.Find(name);

                _report.Note(known?.Role == WorkspaceTransferRole.Excluded ? "table-excluded" : "table-unknown", name);

                return;
            }

            var reference = table.Role == WorkspaceTransferRole.External
                || path.StartsWith(WorkspaceTransferFormat.ReferencesFolder, StringComparison.OrdinalIgnoreCase);

            JsonDocument document;

            try
            {
                using var stream = entry.Open();

                document = JsonDocument.Parse(stream, _documentOptions);
            }
            catch (JsonException)
            {
                _report.Note("file-unreadable", table.Name);

                return;
            }

            _documents.Add(document);

            var elements = document.RootElement.ValueKind switch
            {
                JsonValueKind.Array => document.RootElement.EnumerateArray().ToList(),
                // a later version may wrap the rows
                JsonValueKind.Object when Member(document.RootElement, "rows") is { ValueKind: JsonValueKind.Array } rows => rows.EnumerateArray().ToList(),
                _ => []
            };

            var target = reference ? _references : _rows;

            if (!target.TryGetValue(table, out var list))
            {
                target[table] = list = [];
            }

            var unknown = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var element in elements.Where(x => x.ValueKind == JsonValueKind.Object))
            {
                var row = ReadRow(table, element, unknown);

                if (row is not null)
                {
                    list.Add(row);
                }
            }

            foreach (var column in unknown)
            {
                _report.Note("column-unknown", table.Name, column);
            }
        }

        /// <summary>
        /// Reads a row: its type, and every property the file has a readable value for.
        /// </summary>
        private Row ReadRow(TransferTableSchema table, JsonElement element, HashSet<string> unknown)
        {
            var members = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);

            foreach (var member in element.EnumerateObject())
            {
                members.TryAdd(member.Name, member.Value);
            }

            var type = TypeOf(table, members);

            if (type is null)
            {
                _report.Note("type-unknown", table.Name);

                return null;
            }

            var row = new Row { Table = table, Type = type };
            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var property in table.Properties(type))
            {
                var column = property.GetColumnName();

                if (!members.TryGetValue(property.Name, out var value) && (column is null || !members.TryGetValue(column, out value)))
                {
                    continue;
                }

                used.Add(property.Name);

                if (column is not null)
                {
                    used.Add(column);
                }

                var fk = table.ForeignKeys.FirstOrDefault(x => x.Property == property);

                if (fk is { ViaDatabaseId: true })
                {
                    // the file names the id of the row; the database id is looked up on write
                    if (TransferValue.TryReadScalar(value, typeof(Guid), out var id))
                    {
                        row.Values[property] = id;
                    }
                    else
                    {
                        _report.Note("value-unreadable", table.Name, property.Name);
                    }

                    continue;
                }

                if (TransferValue.TryRead(value, property, out var read))
                {
                    row.Values[property] = read;
                }
                else
                {
                    _report.Note("value-unreadable", table.Name, property.Name);
                }
            }

            // the columns of the other types of the table are not unknown
            foreach (var name in members.Keys.Where(x => !used.Contains(x) && table.FindProperty(x) is null))
            {
                unknown.Add(name);
            }

            return row;
        }

        /// <summary>
        /// Returns the entity type of a row: the type its discriminator names, or the root.
        /// </summary>
        private static IEntityType TypeOf(TransferTableSchema table, Dictionary<string, JsonElement> members)
        {
            var root = table.EntityType;
            var discriminator = root.FindDiscriminatorProperty();

            if (discriminator is null)
            {
                return root;
            }

            var types = root.GetDerivedTypesInclusive().Where(x => !x.ClrType.IsAbstract || x.HasSharedClrType).ToList();

            if (members.TryGetValue(discriminator.Name, out var element)
                && TransferValue.TryReadScalar(element, discriminator.ClrType, out var value)
                && value is not null)
            {
                var match = types.FirstOrDefault(x => Equals(x.GetDiscriminatorValue(), value));

                if (match is not null)
                {
                    return match;
                }
            }

            return root.ClrType.IsAbstract ? null : root;
        }

        /// <summary>
        /// Leaves out the workspaces that were not chosen.
        /// </summary>
        private void Select(WorkspaceImportOptions options)
        {
            if (options.WorkspaceIds is null)
            {
                return;
            }

            foreach (var row in Rows(WorkspaceTransferRole.Root))
            {
                if (row.Value("Id") is not Guid id || !options.WorkspaceIds.Contains(id))
                {
                    row.Drop("not-selected");
                }
            }
        }

        /// <summary>
        /// Decides for every row of a shared table whether an existing row (same id, or same
        /// natural key) is reused or the row is created.
        /// </summary>
        private void ResolveShared()
        {
            foreach (var table in _schema.Tables.Where(x => x.Role == WorkspaceTransferRole.Shared))
            {
                if (!_rows.TryGetValue(table, out var rows) || rows.Count == 0)
                {
                    continue;
                }

                var existing = TransferQuery.All(_context, table.EntityType).ToList();

                foreach (var row in rows)
                {
                    var match = Match(table, row, existing);

                    if (match is not null)
                    {
                        row.Reused = true;
                        MapRow(table, row, match);
                    }
                    else
                    {
                        // created under its own id: references to it stay as they are
                        Remember(Existing(table, "Id"), row.Value("Id"));
                    }
                }
            }
        }

        /// <summary>
        /// Maps the references to accounts, groups and tenants onto the rows of this
        /// installation: by id, then by name. A reference nobody matches stays unresolved.
        /// </summary>
        private void ResolveExternal()
        {
            foreach (var table in _schema.Tables.Where(x => x.Role == WorkspaceTransferRole.External))
            {
                // an older file may have put them among the tables
                var rows = new List<Row>();

                if (_references.TryGetValue(table, out var references))
                {
                    rows.AddRange(references);
                }

                if (_rows.Remove(table, out var misplaced))
                {
                    rows.AddRange(misplaced);
                }

                if (rows.Count == 0)
                {
                    continue;
                }

                var existing = TransferQuery.All(_context, table.EntityType).ToList();

                foreach (var row in rows)
                {
                    var match = Match(table, row, existing);

                    if (match is not null)
                    {
                        MapRow(table, row, match);
                    }
                    else
                    {
                        _report.Note("reference-unmatched", table.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the existing row a row of a shared or external table is: the one of the same
        /// id, or of the same natural key.
        /// </summary>
        private object Match(TransferTableSchema table, Row row, List<object> existing)
        {
            var id = row.Value("Id");
            var idProperty = table.FindProperty("Id");

            if (id is not null && idProperty is not null)
            {
                var byId = existing.FirstOrDefault(x => Equals(TransferEntity.Get(_context, table.EntityType, x, idProperty), id));

                if (byId is not null)
                {
                    return byId;
                }
            }

            foreach (var name in table.Entry.MatchBy)
            {
                var property = table.FindProperty(name);

                if (property is null || row.Value(name) is not string text || string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                var byKey = existing.FirstOrDefault(x => TransferEntity.Get(_context, table.EntityType, x, property) is string other
                    && string.Equals(other.Trim(), text.Trim(), StringComparison.OrdinalIgnoreCase));

                if (byKey is not null)
                {
                    return byKey;
                }
            }

            return null;
        }

        /// <summary>
        /// Records that the references to a row of the file name an existing row: for every
        /// property references can name (the id, the natural key).
        /// </summary>
        private void MapRow(TransferTableSchema table, Row row, object match)
        {
            foreach (var name in TargetProperties(table))
            {
                var property = table.FindProperty(name);
                var from = row.Value(name);

                if (property is null || from is null)
                {
                    continue;
                }

                var to = TransferEntity.Get(_context, table.EntityType, match, property);

                if (!_map.TryGetValue((table, name), out var map))
                {
                    _map[(table, name)] = map = [];
                }

                map[TransferValue.Key(from)] = to;
                Remember(Existing(table, name), to);
            }
        }

        /// <summary>
        /// Loads, for an overwrite, the rows of the workspaces of the file that are present
        /// (the same id): the workspace and everything it owns, as an export would select it.
        /// </summary>
        private void LoadReplaced(WorkspaceImportOptions options)
        {
            if (!options.Overwrite)
            {
                return;
            }

            var root = _schema.Tables.FirstOrDefault(x => x.Role == WorkspaceTransferRole.Root);

            if (root is null || !_rows.TryGetValue(root, out var workspaces))
            {
                return;
            }

            var idProperty = root.FindProperty("Id");
            var ids = workspaces.Where(x => x.Live).Select(x => x.Value("Id")).Where(x => x is not null).ToList();
            var present = TransferQuery.Load(_context, root.EntityType, [(idProperty, ids)], tracking: false)
                .Select(x => (Guid)TransferEntity.Get(_context, root.EntityType, x, idProperty))
                .ToList();

            if (present.Count == 0)
            {
                return;
            }

            _replaced = new WorkspaceExporter(_context, _schema).Select(present);

            foreach (var (table, entities) in _replaced)
            {
                if (table.FindProperty("Id") is not { } id)
                {
                    continue;
                }

                var map = new Dictionary<object, object>();

                foreach (var entity in entities)
                {
                    map[TransferValue.Key(TransferEntity.Get(_context, TransferEntity.TypeOf(table.EntityType, entity), entity, id))] = entity;
                }

                _replacedById[table] = map;
            }
        }

        /// <summary>
        /// Decides which rows of the overwritten workspaces the file no longer holds: they are
        /// deleted. A table without an id (a join table) is rewritten as a whole.
        /// </summary>
        private void PlanDeletions()
        {
            foreach (var (table, entities) in _replaced)
            {
                var kept = new HashSet<object>(ReferenceEqualityComparer.Instance);

                if (_rows.TryGetValue(table, out var rows))
                {
                    foreach (var row in rows.Where(x => x.Live && x.Target is not null))
                    {
                        kept.Add(row.Target);
                    }
                }

                var deleted = entities.Where(x => !kept.Contains(x)).ToList();

                if (deleted.Count > 0)
                {
                    _deleted[table] = deleted;
                }
            }
        }

        /// <summary>
        /// Leaves out the rows whose id the database already holds: a workspace that is present
        /// is not imported a second time, and neither is what it owns.
        /// </summary>
        private void CheckPresent()
        {
            // in the order of the owners, so a row of a workspace that is not imported is left out
            // as such, not reported as present one table after the other
            foreach (var table in _schema.Tables.Where(x => x.Role is WorkspaceTransferRole.Root or WorkspaceTransferRole.Content && _rows.ContainsKey(x)))
            {
                var rows = _rows[table];

                if (table.Role == WorkspaceTransferRole.Content && table.Memberships.Count > 0)
                {
                    var sources = table.Memberships.ToDictionary
                    (
                        m => m,
                        m => (_rows.TryGetValue(m.Source, out var list) ? list : [])
                            .Where(x => x.Live && !x.Reused)
                            .SelectMany(x => TransferValue.References(x.Value(m.SourceProperty)))
                            .ToHashSet()
                    );

                    foreach (var row in rows.Where(x => x.Live))
                    {
                        var owned = table.Memberships.Any(m => TransferValue.References(row.Values.GetValueOrDefault(m.Property)).Any(sources[m].Contains));

                        // a membership through a later table (a catalog a binding names) is
                        // decided when everything is settled
                        if (!owned && table.Memberships.All(m => _schema.Tables.ToList().IndexOf(m.Source) < _schema.Tables.ToList().IndexOf(table)))
                        {
                            row.Drop("row-owner");
                        }
                    }
                }

                var idProperty = table.FindProperty("Id");

                if (idProperty is null)
                {
                    continue;
                }

                var ids = rows.Where(x => x.Live).Select(x => x.Value("Id")).Where(x => x is not null).ToList();
                var present = TransferQuery.Load(_context, table.EntityType, [(idProperty, ids)], tracking: false)
                    .Select(x => TransferValue.Key(TransferEntity.Get(_context, table.EntityType, x, idProperty)))
                    .ToHashSet();

                var seen = new HashSet<object>();

                foreach (var row in rows.Where(x => x.Live))
                {
                    var id = row.Value("Id");

                    if (id is null)
                    {
                        continue;
                    }

                    if (present.Contains(TransferValue.Key(id)))
                    {
                        // a row of a workspace the import overwrites takes the values of the file
                        if (_replacedById.TryGetValue(table, out var replaced)
                            && replaced.TryGetValue(TransferValue.Key(id), out var target)
                            && TransferEntity.TypeOf(table.EntityType, target) == row.Type)
                        {
                            row.Target = target;
                            seen.Add(TransferValue.Key(id));
                        }
                        else
                        {
                            row.Drop("row-present");
                        }
                    }
                    else if (!seen.Add(TransferValue.Key(id)))
                    {
                        row.Drop("row-duplicate");
                    }
                }
            }
        }

        /// <summary>
        /// Leaves out the rows that would break a unique index: against each other, and against
        /// the database where the index is not made unique by an owner the import creates.
        /// </summary>
        private void CheckUnique()
        {
            foreach (var (table, rows) in _rows.Where(x => x.Key.Role != WorkspaceTransferRole.External))
            {
                var candidates = rows.Where(x => x.Live && !x.Reused).ToList();

                if (candidates.Count == 0)
                {
                    continue;
                }

                var indexes = table.EntityType.GetDerivedTypesInclusive()
                    .SelectMany(x => x.GetDeclaredIndexes())
                    .Where(x => x.IsUnique && x.Properties.All(p => p != table.DatabaseId))
                    .ToList();

                foreach (var index in indexes)
                {
                    var properties = index.Properties.OfType<IProperty>().ToList();

                    if (properties.Count != index.Properties.Count || properties.Count == 0)
                    {
                        continue;
                    }

                    // a reference to a database id names an existing row only after the write;
                    // such an index is left to the database
                    if (properties.Any(p => table.ForeignKeys.Any(f => f.Property == p && f.ViaDatabaseId)))
                    {
                        continue;
                    }

                    var applicable = candidates
                        .Where(x => x.Live && properties.All(p => p.DeclaringType.IsAssignableFrom(x.Type)))
                        .ToList();

                    // in the file
                    var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (var row in applicable)
                    {
                        var tuple = Tuple(properties.Select(p => row.Values.GetValueOrDefault(p)));

                        if (tuple is not null && !seen.Add(tuple))
                        {
                            row.Drop("row-duplicate", string.Join(", ", properties.Select(p => p.Name)));
                        }
                    }

                    // an index through an owner the import creates cannot meet an existing row
                    var owned = properties.Any(p => table.Memberships.Any(m => m.Property == p) || p.Name == "Id");

                    if (owned || table.Role == WorkspaceTransferRole.Shared && properties.All(p => table.Entry.MatchBy.Contains(p.Name)))
                    {
                        continue;
                    }

                    var first = properties[0];
                    var values = applicable.Where(x => x.Live).Select(x => x.Values.GetValueOrDefault(first)).Where(x => x is not null).ToList();

                    if (values.Count == 0)
                    {
                        continue;
                    }

                    var replacedIds = _replacedById.TryGetValue(table, out var byId) ? byId : [];
                    var idProperty = table.FindProperty("Id");

                    // the rows an overwrite replaces collide with nothing: they take the file's
                    // values or are deleted
                    var existing = TransferQuery.Load(_context, table.EntityType, [(first, values)], tracking: false)
                        .Where(x => idProperty is null || !replacedIds.ContainsKey(TransferValue.Key(TransferEntity.Get(_context, table.EntityType, x, idProperty))))
                        .Select(x => Tuple(properties.Select(p => TransferEntity.Get(_context, TransferEntity.TypeOf(table.EntityType, x), x, p))))
                        .Where(x => x is not null)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    foreach (var row in applicable.Where(x => x.Live))
                    {
                        var tuple = Tuple(properties.Select(p => row.Values.GetValueOrDefault(p)));

                        if (tuple is not null && existing.Contains(tuple))
                        {
                            row.Drop("row-conflict", string.Join(", ", properties.Select(p => p.Name)));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Rewrites the references to shared and external rows onto the rows of this
        /// installation they were matched with.
        /// </summary>
        private void MapReferences()
        {
            foreach (var row in _rows.Values.SelectMany(x => x).Where(x => x.Live && !x.Reused))
            {
                foreach (var fk in row.Table.ForeignKeys.Where(x => x.Target.Role is WorkspaceTransferRole.Shared or WorkspaceTransferRole.External))
                {
                    if (!row.Values.TryGetValue(fk.Property, out var value) || value is null
                        || !_map.TryGetValue((fk.Target, fk.TargetProperty), out var map))
                    {
                        continue;
                    }

                    if (fk.Many)
                    {
                        row.Values[fk.Property] = MapList(value, x => map.TryGetValue(TransferValue.Key(x), out var to) ? to : x);
                    }
                    else if (map.TryGetValue(TransferValue.Key(value), out var mapped))
                    {
                        // a reference to a database id carries the id of the row until the write
                        row.Values[fk.Property] = fk.ViaDatabaseId ? mapped : Coerce(mapped, fk.Property, value);
                    }
                }
            }
        }

        /// <summary>
        /// Looks up which referenced rows the database holds that the file does not.
        /// </summary>
        private void LoadExisting()
        {
            var wanted = new Dictionary<(TransferTableSchema, string), HashSet<object>>();

            foreach (var row in _rows.Values.SelectMany(x => x).Where(x => x.Live && !x.Reused))
            {
                foreach (var fk in row.Table.ForeignKeys)
                {
                    if (!row.Values.TryGetValue(fk.Property, out var value) || value is null)
                    {
                        continue;
                    }

                    if (!wanted.TryGetValue((fk.Target, fk.TargetProperty), out var set))
                    {
                        wanted[(fk.Target, fk.TargetProperty)] = set = [];
                    }

                    foreach (var reference in TransferValue.Values(value))
                    {
                        set.Add(reference);
                    }
                }
            }

            foreach (var ((table, name), values) in wanted)
            {
                var property = table.FindProperty(name);

                if (property is null || values.Count == 0)
                {
                    continue;
                }

                var known = Existing(table, name);

                foreach (var row in TransferQuery.Load(_context, table.EntityType, [(property, values)], tracking: false))
                {
                    Remember(known, TransferEntity.Get(_context, TransferEntity.TypeOf(table.EntityType, row), row, property));
                }
            }

            // what an overwrite replaces is there only as far as the file holds it again
            foreach (var ((table, name), known) in _existing)
            {
                if (!_replaced.TryGetValue(table, out var replaced))
                {
                    continue;
                }

                foreach (var entity in replaced)
                {
                    var type = TransferEntity.TypeOf(table.EntityType, entity);

                    if (type.FindProperty(name) is { } property)
                    {
                        foreach (var value in TransferValue.References(TransferEntity.Get(_context, type, entity, property)))
                        {
                            known.Remove(value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Leaves out every row whose owners are not imported and every row that refers to a row
        /// that neither the file nor the database holds - repeated until nothing changes, since
        /// leaving out a row can leave out what depends on it.
        /// </summary>
        private void Settle()
        {
            bool changed;

            do
            {
                changed = false;

                var live = new Dictionary<(TransferTableSchema, string), HashSet<object>>();

                HashSet<object> Live(TransferTableSchema table, string name)
                {
                    if (!live.TryGetValue((table, name), out var set))
                    {
                        set = [];

                        if (_rows.TryGetValue(table, out var rows))
                        {
                            foreach (var row in rows.Where(x => x.Live && !x.Reused))
                            {
                                foreach (var value in TransferValue.References(row.Value(name)))
                                {
                                    set.Add(value);
                                }
                            }
                        }

                        live[(table, name)] = set;
                    }

                    return set;
                }

                foreach (var row in _rows.Values.SelectMany(x => x).Where(x => x.Live && !x.Reused))
                {
                    if (row.Table.Role == WorkspaceTransferRole.Content && row.Table.Memberships.Count > 0)
                    {
                        var owned = row.Table.Memberships.Any(m => TransferValue.References(row.Values.GetValueOrDefault(m.Property))
                            .Any(v => Live(m.Source, m.SourceProperty).Contains(v)));

                        if (!owned)
                        {
                            row.Drop("row-owner");
                            changed = true;

                            continue;
                        }
                    }

                    foreach (var fk in row.Table.ForeignKeys)
                    {
                        if (!row.Values.TryGetValue(fk.Property, out var value) || value is null)
                        {
                            // a row the database cannot hold without the reference the file lacks
                            if (fk.Declared && fk.Required && !fk.Many)
                            {
                                row.Drop("row-reference", fk.Target.Name);
                                changed = true;

                                break;
                            }

                            continue;
                        }

                        bool Resolves(object reference)
                        {
                            var key = TransferValue.Key(reference);

                            return Live(fk.Target, fk.TargetProperty).Contains(key) || Existing(fk.Target, fk.TargetProperty).Contains(key);
                        }

                        if (fk.Many)
                        {
                            var kept = MapList(value, x => Resolves(x) ? x : null);

                            if (Count(kept) != Count(value))
                            {
                                row.Values[fk.Property] = kept;
                                _report.Note("reference-cleared", row.Table.Name, fk.Property.Name);
                            }

                            continue;
                        }

                        if (Resolves(value))
                        {
                            continue;
                        }

                        if (fk.Property.IsNullable && !row.Table.Entry.Requires.Contains(fk.Property.Name, StringComparer.Ordinal))
                        {
                            row.Values[fk.Property] = null;
                            _report.Note("reference-cleared", row.Table.Name, fk.Property.Name);
                        }
                        else if (fk.Declared)
                        {
                            row.Drop("row-reference", fk.Target.Name);
                            changed = true;

                            break;
                        }
                    }
                }
            }
            while (changed);
        }

        /// <summary>
        /// Does not create shared rows nothing imported refers to.
        /// </summary>
        private void DropUnusedShared()
        {
            foreach (var table in _schema.Tables.Where(x => x.Role == WorkspaceTransferRole.Shared))
            {
                if (!_rows.TryGetValue(table, out var rows))
                {
                    continue;
                }

                var referenced = new HashSet<object>();

                foreach (var row in _rows.Values.SelectMany(x => x).Where(x => x.Live && x.Table.Role is WorkspaceTransferRole.Root or WorkspaceTransferRole.Content))
                {
                    foreach (var fk in row.Table.ForeignKeys.Where(x => x.Target == table))
                    {
                        foreach (var value in TransferValue.References(row.Values.GetValueOrDefault(fk.Property)))
                        {
                            referenced.Add((fk.TargetProperty, value));
                        }
                    }
                }

                foreach (var row in rows.Where(x => x.Live && !x.Reused))
                {
                    var used = TargetProperties(table).Any(name => TransferValue.References(row.Value(name)).Any(v => referenced.Contains((name, v))));

                    if (!used)
                    {
                        row.Drop("row-unused");
                    }
                }
            }
        }

        /// <summary>
        /// Fills the report: the workspaces, the tables and why rows were left out.
        /// </summary>
        private void Summarize()
        {
            foreach (var table in _schema.Tables)
            {
                if (!_rows.TryGetValue(table, out var rows))
                {
                    if (!_deleted.ContainsKey(table))
                    {
                        continue;
                    }

                    rows = [];
                }

                _report.Tables.Add(new WorkspaceTransferTableReport
                {
                    Name = table.Name,
                    Rows = rows.Count,
                    Written = rows.Count(x => x.Live && !x.Reused && x.Target is null),
                    Updated = rows.Count(x => x.Live && x.Target is not null),
                    Reused = rows.Count(x => x.Live && x.Reused),
                    Skipped = rows.Count(x => !x.Live),
                    Deleted = _deleted.TryGetValue(table, out var deleted) ? deleted.Count : 0
                });

                foreach (var row in rows.Where(x => !x.Live))
                {
                    if (table.Role == WorkspaceTransferRole.Root)
                    {
                        continue;
                    }

                    // a row of a workspace that is not imported is no news; everything else is
                    if (row.Reason is "row-owner" or "row-unused" or "not-selected")
                    {
                        continue;
                    }

                    _report.Note(row.Reason, table.Name, row.ReasonDetail);
                }

                if (table.Role == WorkspaceTransferRole.Root)
                {
                    foreach (var row in rows)
                    {
                        _report.Workspaces.Add(new WorkspaceTransferWorkspace
                        {
                            Id = row.Value("Id") is Guid id ? id : Guid.Empty,
                            Key = row.Value("Key") as string,
                            Name = row.Value("Name") as string,
                            Replaced = row.Live && row.Target is not null,
                            Skipped = row.Live ? null : row.ReasonDetail is null ? row.Reason : row.Reason + ":" + row.ReasonDetail
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Writes the rows in one transaction: the join tables of overwritten workspaces are
        /// emptied first; then the rows are updated, created and deleted; then the join tables,
        /// which refer to database ids that only exist once the rows are written, are filled.
        /// </summary>
        private void Write()
        {
            var relational = _context.Database.IsRelational();
            using var transaction = relational ? _context.Database.BeginTransaction() : null;

            try
            {
                var tables = _schema.Tables.Where(x => _rows.ContainsKey(x)).ToList();

                // a join table has no id to match by: what the file holds replaces it
                foreach (var (table, entities) in _deleted.Where(x => x.Key.FindProperty("Id") is null))
                {
                    foreach (var entity in entities)
                    {
                        Remove(table, entity);
                    }
                }

                _context.SaveChanges();

                var prepared = 0;
                var preparing = tables.Count(x => !x.HasDatabaseIdReferences);

                foreach (var table in tables.Where(x => !x.HasDatabaseIdReferences))
                {
                    Progress?.Invoke(0.6 + 0.15 * prepared++ / Math.Max(1, preparing), "import.prepare", table.Name);
                    Update(table, _rows[table]);
                    Add(table, _rows[table]);
                }

                var deleting = new HashSet<object>(ReferenceEqualityComparer.Instance);

                foreach (var entity in _deleted.Where(x => x.Key.FindProperty("Id") is not null).SelectMany(x => x.Value))
                {
                    deleting.Add(entity);
                }

                if (deleting.Count > 0)
                {
                    _context.ChangeTracker.DetectChanges();
                    ReleaseDependents(deleting);

                    foreach (var (table, entities) in _deleted.Where(x => x.Key.FindProperty("Id") is not null))
                    {
                        foreach (var entity in entities)
                        {
                            Remove(table, entity);
                        }
                    }

                    // the rows outside the workspace that could not stay without a deleted one
                    foreach (var entity in deleting.Where(x => x is not IDictionary<string, object> && _context.Entry(x).State != EntityState.Deleted))
                    {
                        _context.Remove(entity);
                    }
                }

                Progress?.Invoke(0.78, "import.write", null);
                _context.SaveChanges();
                _context.ChangeTracker.Clear();
                Progress?.Invoke(0.95, "import.write", null);

                foreach (var table in tables.Where(x => x.HasDatabaseIdReferences))
                {
                    TranslateDatabaseIds(table, _rows[table]);
                    Add(table, _rows[table]);
                }

                _context.SaveChanges();
                _context.ChangeTracker.Clear();

                transaction?.Commit();
            }
            catch (Exception)
            {
                transaction?.Rollback();
                _context.ChangeTracker.Clear();

                throw;
            }
        }

        /// <summary>
        /// Adds the new rows of a table to the context.
        /// </summary>
        private void Add(TransferTableSchema table, List<Row> rows)
        {
            foreach (var row in rows.Where(x => x.Live && !x.Reused && x.Target is null))
            {
                var (entity, shadow) = Build(table, row);

                if (row.Type.HasSharedClrType)
                {
                    _context.Set<Dictionary<string, object>>(row.Type.Name).Add((Dictionary<string, object>)entity);
                }
                else
                {
                    _context.Add(entity);

                    foreach (var (property, value) in shadow)
                    {
                        TransferEntity.SetShadow(_context, row.Type, entity, property, value);
                    }
                }
            }
        }

        /// <summary>
        /// Gives the present rows an overwrite keeps the values of the file; a value the file
        /// lacks becomes what a new row has, as on a create.
        /// </summary>
        private void Update(TransferTableSchema table, List<Row> rows)
        {
            foreach (var row in rows.Where(x => x.Live && x.Target is not null))
            {
                var (fresh, shadow) = Build(table, row);

                foreach (var property in table.Properties(row.Type).Where(x => !x.IsShadowProperty() && x.Name != "Id" && !x.IsPrimaryKey()))
                {
                    var value = TransferEntity.Get(_context, row.Type, fresh, property);

                    // a column the file lacks that the database fills by default, or that cannot
                    // be empty, keeps what it holds: an update writes what a create would omit
                    if (!row.Values.ContainsKey(property)
                        && (property.GetDefaultValue() is not null || property.GetDefaultValueSql() is not null || value is null && !property.IsNullable))
                    {
                        continue;
                    }

                    if (!Equals(TransferEntity.Get(_context, row.Type, row.Target, property), value))
                    {
                        TransferEntity.Set(row.Target, property, value);
                    }
                }

                foreach (var (property, value) in shadow)
                {
                    TransferEntity.SetShadow(_context, row.Type, row.Target, property, value);
                }
            }
        }

        /// <summary>
        /// Builds a row of the file as a new entity: the values of the file, the empty text for
        /// a required text the file lacks, and what a new row has for everything else.
        /// </summary>
        private (object Entity, List<(IProperty, object)> Shadow) Build(TransferTableSchema table, Row row)
        {
            var entity = TransferEntity.Create(row.Type);
            var shadow = new List<(IProperty, object)>();

            foreach (var property in table.Properties(row.Type))
            {
                if (row.Values.TryGetValue(property, out var value))
                {
                    if (!TransferEntity.Set(entity, property, value))
                    {
                        shadow.Add((property, value));
                    }
                }
                else if (!property.IsShadowProperty() && entity is not IDictionary<string, object>)
                {
                    Empty(entity, property);
                }
            }

            foreach (var property in table.Properties(row.Type).Where(x => x.ClrType == typeof(string) && !x.IsNullable))
            {
                if (entity is IDictionary<string, object> bag ? !bag.ContainsKey(property.Name) || bag[property.Name] is null : TransferEntity.Get(_context, row.Type, entity, property) is null)
                {
                    if (property.GetDefaultValue() is null && property.GetDefaultValueSql() is null)
                    {
                        TransferEntity.Set(entity, property, string.Empty);
                    }
                }
            }

            return (entity, shadow);
        }

        /// <summary>
        /// Marks a tracked row of a table deleted.
        /// </summary>
        private void Remove(TransferTableSchema table, object entity)
        {
            if (table.EntityType.HasSharedClrType)
            {
                _context.Set<Dictionary<string, object>>(table.EntityType.Name).Remove((Dictionary<string, object>)entity);
            }
            else
            {
                _context.Remove(entity);
            }
        }

        /// <summary>
        /// Releases the rows that refer to a row the overwrite deletes and that the database
        /// does not release itself (no cascade, no set null): an optional reference is cleared; a
        /// row that cannot stay without the reference, or whose catalog entry requires it (a
        /// relation from another workspace into this one), is deleted along. Repeated until
        /// nothing changes.
        /// </summary>
        /// <param name="deleting">The rows deleted; rows deleted along are added.</param>
        private void ReleaseDependents(HashSet<object> deleting)
        {
            bool changed;

            do
            {
                changed = false;

                foreach (var dependentType in _context.Model.GetEntityTypes().Where(x => !x.IsOwned() && !x.HasSharedClrType))
                {
                    foreach (var fk in dependentType.GetDeclaredForeignKeys())
                    {
                        if (fk.DeleteBehavior is DeleteBehavior.Cascade or DeleteBehavior.SetNull || fk.Properties.Count != 1)
                        {
                            continue;
                        }

                        var principalKey = fk.PrincipalKey.Properties[0];
                        var keys = deleting
                            .Where(x => x is not IDictionary<string, object> && fk.PrincipalEntityType.ClrType.IsInstanceOfType(x))
                            .Select(x => principalKey.PropertyInfo?.GetValue(x) ?? principalKey.FieldInfo?.GetValue(x))
                            .Where(x => x is not null)
                            .ToHashSet();

                        if (keys.Count == 0)
                        {
                            continue;
                        }

                        var property = fk.Properties[0];
                        var entry = WorkspaceTransferCatalog.Find(WorkspaceTransferSchema.NameOf(dependentType));
                        var requires = fk.IsRequired || entry?.Requires.Contains(property.Name, StringComparer.Ordinal) == true;

                        foreach (var dependent in TransferQuery.Load(_context, dependentType, [(property, keys)], tracking: true))
                        {
                            if (deleting.Contains(dependent))
                            {
                                continue;
                            }

                            var current = _context.Entry(dependent).Property(property.Name);

                            if (current.CurrentValue is null || !keys.Contains(current.CurrentValue))
                            {
                                continue;
                            }

                            if (requires)
                            {
                                deleting.Add(dependent);
                                changed = true;
                            }
                            else
                            {
                                current.CurrentValue = null;
                            }
                        }
                    }
                }
            }
            while (changed);
        }

        /// <summary>
        /// Leaves a property the file lacks empty: a reference is cleared, any other property
        /// keeps what a new row has.
        /// </summary>
        private static void Empty(object entity, IProperty property)
        {
            if (property.IsNullable && (property.IsForeignKey() || property.Name.EndsWith("Id", StringComparison.Ordinal)) && property.Name != "Id")
            {
                TransferEntity.Set(entity, property, null);
            }
        }

        /// <summary>
        /// Replaces the ids in the references to database ids with the database ids.
        /// </summary>
        private void TranslateDatabaseIds(TransferTableSchema table, List<Row> rows)
        {
            foreach (var fk in table.ForeignKeys.Where(x => x.ViaDatabaseId))
            {
                var live = rows.Where(x => x.Live && !x.Reused).ToList();
                var ids = live.Select(x => x.Values.GetValueOrDefault(fk.Property)).Where(x => x is not null).Distinct().ToList();
                var idProperty = fk.Target.FindProperty(fk.TargetProperty);
                var map = new Dictionary<object, object>();

                if (ids.Count > 0 && idProperty is not null && fk.Target.DatabaseId is not null)
                {
                    foreach (var principal in TransferQuery.Load(_context, fk.Target.EntityType, [(idProperty, ids)], tracking: false))
                    {
                        var type = TransferEntity.TypeOf(fk.Target.EntityType, principal);

                        map[TransferValue.Key(TransferEntity.Get(_context, type, principal, idProperty))] = TransferEntity.Get(_context, type, principal, fk.Target.DatabaseId);
                    }
                }

                foreach (var row in live)
                {
                    var id = row.Values.GetValueOrDefault(fk.Property);

                    if (id is not null && map.TryGetValue(TransferValue.Key(id), out var databaseId))
                    {
                        row.Values[fk.Property] = databaseId;
                    }
                    else if (fk.Property.IsNullable)
                    {
                        row.Values[fk.Property] = null;
                    }
                    else
                    {
                        row.Drop("row-reference", fk.Target.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the live rows of a role.
        /// </summary>
        private IEnumerable<Row> Rows(WorkspaceTransferRole role)
        {
            return _rows.Where(x => x.Key.Role == role).SelectMany(x => x.Value).Where(x => x.Live);
        }

        /// <summary>
        /// Returns the properties of a table that references name: its id and every target
        /// property a reference of another table names.
        /// </summary>
        private IEnumerable<string> TargetProperties(TransferTableSchema table)
        {
            return _schema.Tables
                .SelectMany(x => x.ForeignKeys)
                .Where(x => x.Target == table)
                .Select(x => x.TargetProperty)
                .Append("Id")
                .Distinct(StringComparer.Ordinal);
        }

        /// <summary>
        /// Returns the set of the values the database holds for a property of a table.
        /// </summary>
        private HashSet<object> Existing(TransferTableSchema table, string name)
        {
            if (!_existing.TryGetValue((table, name), out var set))
            {
                _existing[(table, name)] = set = [];
            }

            return set;
        }

        /// <summary>
        /// Adds the comparable forms of a value to a set.
        /// </summary>
        private static void Remember(HashSet<object> set, object value)
        {
            foreach (var reference in TransferValue.References(value))
            {
                set.Add(reference);
            }
        }

        /// <summary>
        /// Returns a value as the type of the property it replaces.
        /// </summary>
        private static object Coerce(object value, IProperty property, object fallback)
        {
            return TransferValue.ToPropertyType(value, property.ClrType).FirstOrDefault() ?? fallback;
        }

        /// <summary>
        /// Returns a list of the same type with every element mapped; a <see langword="null"/>
        /// result leaves the element out.
        /// </summary>
        private static object MapList(object list, Func<object, object> map)
        {
            if (list is not IEnumerable items || list is string)
            {
                return list;
            }

            var result = (IList)Activator.CreateInstance(list.GetType());

            foreach (var item in items)
            {
                var mapped = item is null ? null : map(item);

                if (mapped is not null)
                {
                    result.Add(mapped);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the number of elements of a list.
        /// </summary>
        private static int Count(object list)
        {
            return list is ICollection collection ? collection.Count : list is IEnumerable items ? items.Cast<object>().Count() : 0;
        }

        /// <summary>
        /// Returns the comparable text of the values of an index, or <see langword="null"/>
        /// when one is missing (a database does not compare a missing value).
        /// </summary>
        private static string Tuple(IEnumerable<object> values)
        {
            var parts = new List<string>();

            foreach (var value in values)
            {
                if (value is null)
                {
                    return null;
                }

                parts.Add(TransferValue.Key(value).ToString());
            }

            return string.Join("\u001F", parts);
        }

        /// <summary>
        /// Returns a member of an object ignoring case, or <see langword="null"/>.
        /// </summary>
        private static JsonElement? Member(JsonElement element, string name)
        {
            foreach (var member in element.EnumerateObject())
            {
                if (string.Equals(member.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return member.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a path of the archive with forward slashes.
        /// </summary>
        private static string Normalize(string path)
        {
            return (path ?? string.Empty).Replace('\\', '/').TrimStart('/');
        }
    }
}
