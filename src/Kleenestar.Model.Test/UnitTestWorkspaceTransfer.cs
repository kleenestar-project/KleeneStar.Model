using KleeneStar.Model.Entities;
using KleeneStar.Model.Settings;
using KleeneStar.Model.Transfer;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace KleeneStar.Model.Test
{
    /// <summary>
    /// Tests the export and import of workspaces on real SQLite files: what belongs to a
    /// workspace travels with it and nothing else does, references to data of the installation
    /// are mapped by name, and the import takes a file of another version as far as it can.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestWorkspaceTransfer : IDisposable
    {
        private readonly string _directory = Path.Combine(Path.GetTempPath(), "kleenestar-transfer-" + Guid.NewGuid().ToString("N"));
        private readonly DatabaseSettings _previous = ModelHub.DatabaseSettings;

        private readonly Guid _aliceId = Guid.NewGuid();
        private readonly Guid _groupId = Guid.NewGuid();
        private readonly Guid _categoryId = Guid.NewGuid();
        private readonly Guid _statusCategoryId = Guid.NewGuid();
        private readonly Guid _workspaceId = Guid.NewGuid();
        private readonly Guid _otherWorkspaceId = Guid.NewGuid();
        private readonly Guid _classId = Guid.NewGuid();
        private readonly Guid _fieldId = Guid.NewGuid();
        private readonly Guid _statusId = Guid.NewGuid();
        private readonly Guid _levelId = Guid.NewGuid();
        private readonly Guid _parentId = Guid.NewGuid();
        private readonly Guid _childId = Guid.NewGuid();
        private readonly Guid _foreignId = Guid.NewGuid();
        private readonly Guid _commentId = Guid.NewGuid();

        /// <summary>
        /// Creates the folder of the databases.
        /// </summary>
        public UnitTestWorkspaceTransfer()
        {
            Directory.CreateDirectory(_directory);
        }

        /// <summary>
        /// Restores the settings and removes the files.
        /// </summary>
        public void Dispose()
        {
            ModelHub.DatabaseSettings = _previous;

            try
            {
                Directory.Delete(_directory, recursive: true);
            }
            catch (IOException)
            {
                // a file still held by the sqlite runtime is left to the temp folder's cleanup
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Every table of the model is classified, and every owner the catalog names exists.
        /// </summary>
        [Fact]
        public void Catalog_ClassifiesEveryTable()
        {
            Use("catalog");

            Assert.Empty(ModelHub.GetUnclassifiedTransferTables());

            using var db = ModelHub.CreateDbContext();
            var names = db.Model.GetEntityTypes().Where(x => x.BaseType is null).Select(WorkspaceTransferSchema.NameOf).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var table in WorkspaceTransferCatalog.Tables)
            {
                Assert.True(names.Contains(table.Name), $"The catalog names {table.Name}, which the model does not have.");

                foreach (var owner in table.Owners)
                {
                    Assert.True(names.Contains(owner.Source), $"{table.Name} is owned by {owner.Source}, which the model does not have.");
                }
            }

            var schema = ModelHub.TransferSchema(db.Model);

            foreach (var table in schema.Tables.Where(x => x.Role == WorkspaceTransferRole.Content))
            {
                Assert.True(table.Memberships.Count == table.Entry.Owners.Count, $"An owner of {table.Name} does not resolve.");
            }

            // a content row must not depend on a row that is never exported
            foreach (var type in db.Model.GetEntityTypes())
            {
                var entry = WorkspaceTransferCatalog.Find(WorkspaceTransferSchema.NameOf(type));

                if (entry?.Role != WorkspaceTransferRole.Content)
                {
                    continue;
                }

                foreach (var fk in type.GetDeclaredForeignKeys().Where(x => x.IsRequired))
                {
                    var principal = WorkspaceTransferCatalog.Find(WorkspaceTransferSchema.NameOf(fk.PrincipalEntityType));

                    Assert.NotEqual(WorkspaceTransferRole.Excluded, principal?.Role);
                }
            }
        }

        /// <summary>
        /// A workspace travels with what belongs to it, references to accounts and groups are
        /// mapped by name onto the target installation, shared reference data is reused or
        /// created, and data of the installation stays behind.
        /// </summary>
        [Fact]
        public void Export_Import_CarriesTheWorkspace()
        {
            Use("source");
            Seed();

            var file = Export(_workspaceId);

            var entries = Entries(file);

            Assert.Contains(WorkspaceTransferFormat.Manifest, entries);
            Assert.Contains("tables/Workspace.json", entries);
            Assert.Contains("tables/Object.json", entries);
            Assert.Contains("tables/WorkspaceCategory.json", entries);
            Assert.Contains("references/Identity.json", entries);
            Assert.DoesNotContain(entries, x => x.Contains("ObjectWatcher", StringComparison.Ordinal));
            Assert.DoesNotContain(entries, x => x.Contains("AuditEvent", StringComparison.Ordinal));
            Assert.DoesNotContain("tables/Identity.json", entries);

            // the account is named, nothing else of it is written
            var identities = Read(file, "references/Identity.json");
            var alice = Assert.Single(identities.EnumerateArray());
            Assert.Equal(["Id", "UserName"], alice.EnumerateObject().Select(x => x.Name).Order());

            // the other workspace stays behind
            Assert.DoesNotContain(Read(file, "tables/Object.json").EnumerateArray(), x => x.GetProperty("Id").GetGuid() == _foreignId);

            // the target installation knows the account, the group and the category by name
            Use("target");

            var bobId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            using (var db = ModelHub.CreateDbContext())
            {
                db.Identities.Add(new Identity(bobId) { Name = "Alice Other", UserName = "ALICE", Email = "a@example.org" });
                db.Groups.Add(new Group(teamId) { Name = "Team" });
                db.Categories.Add(new Category { Id = customerId, Name = "customer" });
                db.SaveChanges();
            }

            var report = Import(file);

            Assert.True(report.Succeeded, report.Error);
            Assert.Null(Assert.Single(report.Workspaces).Skipped);

            using (var db = ModelHub.CreateDbContext())
            {
                var workspace = db.Workspaces.Include(x => x.Categories).Single();

                Assert.Equal("TRX", workspace.Key);
                Assert.Equal(customerId, Assert.Single(workspace.Categories).Id);
                Assert.Equal(2, db.Objects.Count());
                Assert.Equal(_parentId, db.Objects.Single(x => x.Id == _childId).ParentId);
                Assert.Equal("payload", db.Values.Single().Data);

                // the account is the one of the same name
                Assert.Equal(bobId, db.Comments.Single().AuthorId);
                Assert.Equal(bobId, db.CommentLikes.Single().AuthorId);
                Assert.Equal(bobId, db.Objects.Single(x => x.Id == _parentId).CreatorId);

                // the group too, also inside a list
                Assert.Equal(teamId, db.PermissionAssignments.Single().GroupId);
                Assert.Equal([teamId], db.SecurityLevels.Single().PermittedGroupIds);

                // the status category was created with its id
                Assert.Equal(_statusCategoryId, db.Statuses.Single().CategoryId);
                Assert.Equal("To do", db.StatusCategories.Single().Name);

                // the relation inside the workspace survives, the one into the other does not
                var relation = db.ObjectRelations.Single();
                Assert.Equal(_childId, relation.TargetObjectId);
                Assert.Single(db.ObjectRelationTypes);

                // per-user and installation data stays behind
                Assert.Empty(db.ObjectWatchers);
                Assert.Single(db.Identities);
                Assert.Single(db.Groups);
            }

            // a second import finds the workspace present
            var again = Import(file);

            Assert.False(again.Succeeded);
            Assert.Equal("row-present", Assert.Single(again.Workspaces).Skipped);
        }

        /// <summary>
        /// A structure export preserves configuration, omits object data and clears the home
        /// reference without changing the source database.
        /// </summary>
        [Fact]
        public void Export_StructureOnlyCarriesConfigurationWithoutObjects()
        {
            // arrange
            Use("structure-source");
            Seed();

            using (var source = ModelHub.CreateDbContext())
            {
                source.Workspaces.Single(x => x.Id == _workspaceId).HomeId = _parentId;
                source.Templates.Add(new Template { Id = Guid.NewGuid(), ClassId = _classId, Name = "Preset", Presets = "{}" });
                source.PermissionAssignments.Add(new PermissionAssignment(Guid.NewGuid())
                {
                    Scope = "object", ScopeId = _parentId.ToString(), GroupId = _groupId, Policy = "edit", Created = DateTime.UtcNow
                });
                source.SaveChanges();
            }

            // act
            using var stream = new MemoryStream();
            var exported = ModelHub.ExportWorkspaces([_workspaceId], stream, structureOnly: true);
            var file = stream.ToArray();

            // validation
            Assert.True(exported.Succeeded);
            Assert.Single(Read(file, "tables/Class.json").EnumerateArray());
            Assert.Single(Read(file, "tables/Template.json").EnumerateArray());
            Assert.Single(Read(file, "tables/PermissionAssignment.json").EnumerateArray());
            Assert.Equal(JsonValueKind.Null, Assert.Single(Read(file, "tables/Workspace.json").EnumerateArray()).GetProperty("HomeId").ValueKind);

            foreach (var table in new[] { "Object", "Value", "Comment", "CommentLike", "ObjectRelation", "Attachment", "Commit", "Poll", "Appointment" })
            {
                Assert.Empty(Read(file, $"tables/{table}.json").EnumerateArray());
            }

            using (var source = ModelHub.CreateDbContext())
            {
                Assert.Equal(_parentId, source.Workspaces.Single(x => x.Id == _workspaceId).HomeId);
                Assert.Equal(3, source.Objects.Count());
            }

            Use("structure-target");
            var imported = Import(file);
            Assert.True(imported.Succeeded, imported.Error);

            using var target = ModelHub.CreateDbContext();
            Assert.Single(target.Workspaces);
            Assert.Null(target.Workspaces.Single().HomeId);
            Assert.Single(target.Classes);
            Assert.Single(target.Fields);
            Assert.Single(target.Templates);
            Assert.Single(target.Statuses);
            Assert.Empty(target.Objects);
            Assert.Empty(target.Values);
            Assert.Empty(target.Comments);
        }

        /// <summary>
        /// A reference to an account the target installation does not know is cleared where it
        /// may be empty and leaves out the row that cannot exist without it.
        /// </summary>
        [Fact]
        public void Import_LeavesUnknownAccountsOut()
        {
            Use("source2");
            Seed();

            var file = Export(_workspaceId);

            Use("target2");

            var report = Import(file);

            Assert.True(report.Succeeded, report.Error);

            using var db = ModelHub.CreateDbContext();

            Assert.Null(db.Objects.Single(x => x.Id == _parentId).CreatorId);
            Assert.Empty(db.Comments);
            Assert.Empty(db.PermissionAssignments);
            Assert.Empty(db.SecurityLevels.Single().PermittedGroupIds);
            Assert.Contains(report.Notes, x => x.Code == "reference-unmatched" && x.Table == "Identity");
            Assert.Contains(report.Notes, x => x.Code == "row-reference" && x.Table == "Comment");
        }

        /// <summary>
        /// A file of another version is taken as far as it goes: unknown tables and columns are
        /// left out, missing columns stay empty, unreadable values stay empty, and a row that
        /// cannot exist without what the file lacks is left out.
        /// </summary>
        [Fact]
        public void Import_IsTolerant()
        {
            Use("tolerant");

            var workspaceId = Guid.NewGuid();
            var classId = Guid.NewGuid();
            var objectId = Guid.NewGuid();

            var file = Zip(new Dictionary<string, string>
            {
                ["manifest.json"] = """{ "format": "kleenestar.workspaces", "formatVersion": 99, "somethingNew": { "a": 1 } }""",
                ["tables/Workspace.json"] = $$"""[{ "id": "{{workspaceId}}", "key": "OLD", "name": "From the future", "State": "Hibernating", "Colour": "teal", "Sealed": "true" }]""",
                ["tables/Class.json"] = $$"""[{ "Id": "{{classId}}", "WorkspaceId": "{{workspaceId}}", "Name": "Ticket", "Created": 1700000000000 }]""",
                ["tables/Object.json"] = $$"""
                    [
                        { "Id": "{{objectId}}", "WorkspaceId": "{{workspaceId}}", "ClassId": "{{classId}}", "Key": "OLD-1", "StoryPoints": "5" },
                        { "Id": "{{Guid.NewGuid()}}", "WorkspaceId": "{{workspaceId}}", "Key": "OLD-2", "Summary": "no class" }
                    ]
                    """,
                ["tables/Value.json"] = $$"""[{ "Id": "{{Guid.NewGuid()}}", "ObjectId": "{{objectId}}", "Data": "orphan" }]""",
                ["tables/FutureThing.json"] = """[{ "Id": 1 }]""",
                ["tables/AuditEvent.json"] = """[]"""
            });

            var report = Import(file);

            Assert.True(report.Succeeded, report.Error);

            using (var db = ModelHub.CreateDbContext())
            {
                var workspace = db.Workspaces.Single();

                Assert.Equal("OLD", workspace.Key);
                Assert.True(workspace.Sealed);
                Assert.Equal(default, workspace.State);

                var @object = db.Objects.Single();

                Assert.Equal(objectId, @object.Id);
                Assert.Equal(string.Empty, @object.Summary);
                Assert.Empty(db.Values);
            }

            Assert.Contains(report.Notes, x => x.Code == "format-newer");
            Assert.Contains(report.Notes, x => x.Code == "table-unknown" && x.Table == "FutureThing");
            Assert.Contains(report.Notes, x => x.Code == "table-excluded" && x.Table == "AuditEvent");
            Assert.Contains(report.Notes, x => x.Code == "column-unknown" && x.Detail == "Colour");
            Assert.Contains(report.Notes, x => x.Code == "value-unreadable" && x.Detail == "State");
            Assert.Contains(report.Notes, x => x.Code == "row-reference" && x.Table == "Object");
            Assert.Contains(report.Notes, x => x.Code == "row-reference" && x.Table == "Value");
        }

        /// <summary>
        /// A file of another format, or no zip at all, is refused without writing.
        /// </summary>
        [Fact]
        public void Import_RefusesWhatIsNoExport()
        {
            Use("refuse");

            var other = Import(Zip(new Dictionary<string, string> { ["manifest.json"] = """{ "format": "something.else" }""" }));

            Assert.False(other.Succeeded);
            Assert.Equal("wrong-format", other.Error);

            var garbage = Import(Encoding.UTF8.GetBytes("not a zip"));

            Assert.False(garbage.Succeeded);
            Assert.Equal("not-a-zip", garbage.Error);

            var empty = Import(Zip(new Dictionary<string, string> { ["tables/Workspace.json"] = "[]" }));

            Assert.False(empty.Succeeded);
            Assert.Equal("no-workspace", empty.Error);
        }

        /// <summary>
        /// An overwrite gives a present workspace the state of the file: changed rows take the
        /// file's values, rows the file no longer holds are deleted - with what refers to them
        /// from outside - and per-user data of the rows that stay is kept.
        /// </summary>
        [Fact]
        public void Import_OverwritesAPresentWorkspace()
        {
            Use("overwrite");
            Seed();

            var file = Export(_workspaceId);
            var extraId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            using (var db = ModelHub.CreateDbContext())
            {
                db.Objects.Single(x => x.Id == _parentId).Summary = "Changed";
                db.Workspaces.Single(x => x.Id == _workspaceId).Description = "changed";
                db.Fields.Single(x => x.Id == _fieldId).Name = "Renamed";
                db.Objects.Add(new Entities.Object(extraId) { Key = "TRX-3", Summary = "Added later", WorkspaceId = _workspaceId, ClassId = _classId, ParentId = _childId, Created = now, Updated = now });
                db.SaveChanges();

                // a relation from the other workspace into the object the overwrite removes
                db.ObjectRelations.Add(new ObjectRelation(Guid.NewGuid()) { SourceObjectId = _foreignId, TargetObjectId = extraId, TypeKey = "blocks", Created = now, Updated = now });
                db.Values.Add(new Value(Guid.NewGuid()) { ObjectId = extraId, FieldId = _fieldId, Data = "gone", Created = now, Updated = now });
                db.SaveChanges();
            }

            // without the choice the present workspace stays as it is
            var refused = Import(file);

            Assert.False(refused.Succeeded);
            Assert.Equal("row-present", Assert.Single(refused.Workspaces).Skipped);

            var report = Import(file, new WorkspaceImportOptions { Overwrite = true });

            Assert.True(report.Succeeded, report.Error + " " + string.Join(" | ", report.Notes.Select(x => $"{x.Code} {x.Table} {x.Detail}")));
            Assert.True(Assert.Single(report.Workspaces).Replaced);
            Assert.Contains(report.Tables, x => x.Name == "Object" && x.Updated == 2 && x.Deleted == 1);

            using var check = ModelHub.CreateDbContext();

            Assert.Equal("Parent", check.Objects.Single(x => x.Id == _parentId).Summary);
            Assert.Equal("{\"version\":1}", check.Workspaces.Single(x => x.Id == _workspaceId).Description);
            Assert.Equal("Summary", check.Fields.Single(x => x.Id == _fieldId).Name);
            Assert.False(check.Objects.Any(x => x.Id == extraId));
            Assert.Single(check.Values);

            // the relation into the deleted object went with it; the others stay
            Assert.DoesNotContain(check.ObjectRelations, x => x.TargetObjectId == extraId);
            Assert.Contains(check.ObjectRelations, x => x.SourceObjectId == _parentId && x.TargetObjectId == _childId);

            // what is not the file's to say is kept: the watcher, the other workspace, the joins
            Assert.Single(check.ObjectWatchers);
            Assert.Equal(3, check.Objects.Count());
            Assert.Single(check.Workspaces.Include(x => x.Categories).Single(x => x.Id == _workspaceId).Categories);
            Assert.Single(check.Comments);
        }

        /// <summary>
        /// A dry run checks the file and writes nothing.
        /// </summary>
        [Fact]
        public void Import_DryRunWritesNothing()
        {
            Use("dry-source");
            Seed();

            var file = Export(_workspaceId);

            Use("dry-target");

            var report = Import(file, new WorkspaceImportOptions { DryRun = true });

            Assert.True(report.Succeeded, report.Error);
            Assert.Contains(report.Tables, x => x.Name == "Object" && x.Written == 2);

            using var db = ModelHub.CreateDbContext();

            Assert.Empty(db.Workspaces);
        }

        /// <summary>
        /// Points the model hub at a fresh, migrated SQLite file.
        /// </summary>
        private void Use(string name)
        {
            ModelHub.DatabaseSettings = new DatabaseSettings
            {
                Provider = "Microsoft.Data.Sqlite",
                Assembly = "KleeneStar.Model.Sqlite",
                ConnectionString = $"Data Source={Path.Combine(_directory, name + ".db")};Pooling=False"
            };

            using var db = ModelHub.CreateDbContext();

            db.Database.Migrate();
        }

        /// <summary>
        /// Writes a workspace with classes, objects, values, comments, grants and relations, a
        /// second workspace, and data of the installation.
        /// </summary>
        private void Seed()
        {
            var now = DateTime.UtcNow;

            using var db = ModelHub.CreateDbContext();

            db.Identities.Add(new Identity(_aliceId) { Name = "Alice", UserName = "alice", Email = "alice@example.org" });
            db.Groups.Add(new Group(_groupId) { Name = "team" });
            var category = new Category { Id = _categoryId, Name = "Customer" };
            db.Categories.Add(category);
            db.StatusCategories.Add(new StatusCategory { Id = _statusCategoryId, Name = "To do", Created = now, Updated = now });
            db.ObjectRelationTypes.Add(new ObjectRelationType(Guid.NewGuid()) { Key = "blocks", Label = "blocks", Created = now, Updated = now });

            db.Workspaces.Add(new Workspace(_workspaceId) { Key = "TRX", Name = "Transfer", Description = "{\"version\":1}", Categories = [category], Created = now, Updated = now });
            db.Workspaces.Add(new Workspace(_otherWorkspaceId) { Key = "OTH", Name = "Other", Created = now, Updated = now });

            db.Classes.Add(new Class { Id = _classId, Name = "Ticket", WorkspaceId = _workspaceId, Created = now, Updated = now });
            db.Classes.Add(new Class { Id = Guid.NewGuid(), Name = "Ticket", WorkspaceId = _otherWorkspaceId, Created = now, Updated = now });
            db.Fields.Add(new Field(_fieldId) { Name = "Summary", ClassId = _classId, Created = now, Updated = now });
            db.Statuses.Add(new Status(_statusId) { Name = "Open", ClassId = _classId, CategoryId = _statusCategoryId, Created = now, Updated = now });
            db.SecurityLevels.Add(new SecurityLevel(_levelId) { Name = "Internal", ClassId = _classId, PermittedGroupIds = [_groupId], Created = now, Updated = now });

            db.Objects.Add(new Entities.Object(_parentId) { Key = "TRX-1", Summary = "Parent", WorkspaceId = _workspaceId, ClassId = _classId, CreatorId = _aliceId, Created = now, Updated = now });
            db.Objects.Add(new Entities.Object(_childId) { Key = "TRX-2", Summary = "Child", WorkspaceId = _workspaceId, ClassId = _classId, ParentId = _parentId, Created = now, Updated = now });
            db.SaveChanges();

            db.Objects.Add(new Entities.Object(_foreignId) { Key = "OTH-1", Summary = "Foreign", WorkspaceId = _otherWorkspaceId, ClassId = db.Classes.Single(x => x.WorkspaceId == _otherWorkspaceId).Id, Created = now, Updated = now });
            db.Values.Add(new Value(Guid.NewGuid()) { ObjectId = _parentId, FieldId = _fieldId, Data = "payload", Created = now, Updated = now });
            db.Comments.Add(new Comment(_commentId) { ObjectId = _parentId, AuthorId = _aliceId, Content = "{}", Created = now, Updated = now });
            db.CommentLikes.Add(new CommentLike { Id = Guid.NewGuid(), CommentId = _commentId, AuthorId = _aliceId, Created = now });
            db.ObjectRelations.Add(new ObjectRelation(Guid.NewGuid()) { SourceObjectId = _parentId, TargetObjectId = _childId, TypeKey = "blocks", Created = now, Updated = now });
            db.ObjectRelations.Add(new ObjectRelation(Guid.NewGuid()) { SourceObjectId = _parentId, TargetObjectId = _foreignId, TypeKey = "blocks", Created = now, Updated = now });
            db.ObjectWatchers.Add(new ObjectWatcher { Id = Guid.NewGuid(), ObjectId = _parentId, IdentityId = _aliceId, Created = now });
            db.PermissionAssignments.Add(new PermissionAssignment(Guid.NewGuid()) { Scope = "workspace", ScopeId = _workspaceId.ToString(), GroupId = _groupId, Policy = "edit", Created = now });
            db.SaveChanges();
        }

        /// <summary>
        /// Exports workspaces into a byte array.
        /// </summary>
        private static byte[] Export(params Guid[] workspaceIds)
        {
            using var stream = new MemoryStream();

            var report = ModelHub.ExportWorkspaces(workspaceIds, stream);

            Assert.True(report.Succeeded);

            return stream.ToArray();
        }

        /// <summary>
        /// Imports a file.
        /// </summary>
        private static WorkspaceTransferReport Import(byte[] file, WorkspaceImportOptions options = null)
        {
            using var stream = new MemoryStream(file);

            return ModelHub.ImportWorkspaces(stream, options);
        }

        /// <summary>
        /// Returns the entry names of an archive.
        /// </summary>
        private static List<string> Entries(byte[] file)
        {
            using var archive = new ZipArchive(new MemoryStream(file), ZipArchiveMode.Read);

            return [.. archive.Entries.Select(x => x.FullName)];
        }

        /// <summary>
        /// Returns the JSON of an entry.
        /// </summary>
        private static JsonElement Read(byte[] file, string name)
        {
            using var archive = new ZipArchive(new MemoryStream(file), ZipArchiveMode.Read);
            using var stream = archive.GetEntry(name)!.Open();

            return JsonDocument.Parse(stream).RootElement.Clone();
        }

        /// <summary>
        /// Writes an archive of text entries.
        /// </summary>
        private static byte[] Zip(Dictionary<string, string> entries)
        {
            using var stream = new MemoryStream();

            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var (name, content) in entries)
                {
                    using var writer = new StreamWriter(archive.CreateEntry(name).Open());

                    writer.Write(content);
                }
            }

            return stream.ToArray();
        }
    }
}
