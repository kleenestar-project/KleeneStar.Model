using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the <see cref="ModelHub"/> object-watcher surface.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubObjectWatcher
    {
        private static readonly Guid WorkspaceId = Guid.Parse("2BC2F2DB-4E66-4AF3-A022-97F2C44B22BB");
        private static readonly Guid ClassId = Guid.Parse("ADFE92D1-5C33-4F3D-AD27-AE3B7582F1F1");
        private static readonly Guid ObjectId = Guid.Parse("EAF4F9F1-AD3D-4033-AD3F-8F3DAB48D1F1");
        private static readonly Guid OtherObjectId = Guid.Parse("FBE5FAF2-BE4E-4144-BE40-9F4EBC59E2F2");
        private static readonly Guid IdentityId = Guid.Parse("1C8E8D27-4E7D-4F27-B3F1-2E3B6D8BC1F1");
        private static readonly Guid OtherIdentityId = Guid.Parse("2D9F9E38-5F8E-4038-C4F2-3F4C7E9CD2F2");

        /// <summary>
        /// Seeds the in-memory database with two objects and two identities so each
        /// test can exercise the watcher CRUD surface against a stable fixture.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        private static void SeedFixtures(string connectionString)
        {
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig
            {
                ConnectionString = connectionString,
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();

            if (!db.Workspaces.Any(x => x.Id == WorkspaceId))
            {
                db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-w", Name = "workspace" });
            }
            if (!db.Classes.Any(x => x.Id == ClassId))
            {
                db.Classes.Add(new Class { Id = ClassId, Name = "Incident", WorkspaceId = WorkspaceId });
            }
            if (!db.Identities.Any(x => x.Id == IdentityId))
            {
                db.Identities.Add(new Identity { Id = IdentityId, Name = "Watcher One", Email = "w1@kleenestar.org", PasswordHash = "$test$" });
            }
            if (!db.Identities.Any(x => x.Id == OtherIdentityId))
            {
                db.Identities.Add(new Identity { Id = OtherIdentityId, Name = "Watcher Two", Email = "w2@kleenestar.org", PasswordHash = "$test$" });
            }
            if (!db.Objects.Any(x => x.Id == ObjectId))
            {
                db.Objects.Add(new KleeneStar.Model.Entities.Object { Id = ObjectId, Key = "WMH-001", Summary = "watched item", WorkspaceId = WorkspaceId, ClassId = ClassId });
            }
            if (!db.Objects.Any(x => x.Id == OtherObjectId))
            {
                db.Objects.Add(new KleeneStar.Model.Entities.Object { Id = OtherObjectId, Key = "WMH-002", Summary = "unwatched item", WorkspaceId = WorkspaceId, ClassId = ClassId });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies that every watcher in the database is returned when no filter
        /// criteria are applied.
        /// </summary>
        [Fact]
        public void AllWatchers()
        {
            var connectionString = nameof(AllWatchers);
            SeedFixtures(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.ObjectWatchers.Add(new ObjectWatcher { ObjectId = ObjectId, IdentityId = IdentityId, Created = DateTime.UtcNow });
                db.ObjectWatchers.Add(new ObjectWatcher { ObjectId = ObjectId, IdentityId = OtherIdentityId, Created = DateTime.UtcNow });
                db.SaveChanges();
            }

            var result = ModelHub.GetObjectWatchers(new Query<ObjectWatcher>()).ToList();

            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the query pipeline returns only the watchers matching the
        /// supplied predicate.
        /// </summary>
        [Fact]
        public void FilteredWatchers()
        {
            var connectionString = nameof(FilteredWatchers);
            SeedFixtures(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.ObjectWatchers.Add(new ObjectWatcher { ObjectId = ObjectId, IdentityId = IdentityId, Created = DateTime.UtcNow });
                db.ObjectWatchers.Add(new ObjectWatcher { ObjectId = OtherObjectId, IdentityId = IdentityId, Created = DateTime.UtcNow });
                db.SaveChanges();
            }

            var result = ModelHub.GetObjectWatchers(
                new Query<ObjectWatcher>().WhereEquals(x => x.ObjectId, ObjectId)).ToList();

            Assert.Single(result);
            Assert.Equal(ObjectId, result[0].ObjectId);
        }

        /// <summary>
        /// Verifies that the materialized rows hydrate the
        /// <see cref="ObjectWatcher.Identity"/> and
        /// <see cref="ObjectWatcher.Object"/> navigation properties so callers do not
        /// need an extra round-trip to display the watcher name.
        /// </summary>
        [Fact]
        public void GetWatchersHydratesNavigationProperties()
        {
            var connectionString = nameof(GetWatchersHydratesNavigationProperties);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectWatcher { ObjectId = ObjectId, IdentityId = IdentityId });

            var result = ModelHub.GetObjectWatchers(
                new Query<ObjectWatcher>().WhereEquals(x => x.ObjectId, ObjectId)).Single();

            Assert.NotNull(result.Identity);
            Assert.Equal("Watcher One", result.Identity.Name);
            Assert.NotNull(result.Object);
            Assert.Equal("WMH-001", result.Object.Key);
        }

        /// <summary>
        /// Verifies that adding a watcher persists the row and back-fills
        /// <see cref="ObjectWatcher.Created"/> when the caller leaves it unset.
        /// </summary>
        [Fact]
        public void AddWatcherPersists()
        {
            var connectionString = nameof(AddWatcherPersists);
            SeedFixtures(connectionString);

            var watcher = new ObjectWatcher
            {
                ObjectId = ObjectId,
                IdentityId = IdentityId
            };

            ModelHub.Add(watcher);

            var loaded = ModelHub.GetObjectWatchers(
                new Query<ObjectWatcher>().WhereEquals(x => x.Id, watcher.Id)).Single();
            Assert.Equal(ObjectId, loaded.ObjectId);
            Assert.Equal(IdentityId, loaded.IdentityId);
            Assert.NotEqual(default, loaded.Created);
        }

        /// <summary>
        /// Verifies that a second <see cref="ModelHub.Add(ObjectWatcher)"/> for the
        /// same (object, identity) pair is a silent no-op. The unique composite index
        /// would otherwise throw on the second insert, so the early-return guard is
        /// load-bearing.
        /// </summary>
        [Fact]
        public void AddWatcherWhenPairExistsIsNoOp()
        {
            var connectionString = nameof(AddWatcherWhenPairExistsIsNoOp);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectWatcher { ObjectId = ObjectId, IdentityId = IdentityId });
            ModelHub.Add(new ObjectWatcher { ObjectId = ObjectId, IdentityId = IdentityId });

            using var db = ModelHub.CreateDbContext();
            var entries = db.ObjectWatchers
                .Where(w => w.ObjectId == ObjectId && w.IdentityId == IdentityId)
                .ToList();
            Assert.Single(entries);
        }

        /// <summary>
        /// Verifies that the same identity can watch two different objects — the
        /// unique index only forbids the duplicate (object, identity) pair, not the
        /// per-identity total.
        /// </summary>
        [Fact]
        public void AddWatcherSameIdentityDifferentObjects()
        {
            var connectionString = nameof(AddWatcherSameIdentityDifferentObjects);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectWatcher { ObjectId = ObjectId, IdentityId = IdentityId });
            ModelHub.Add(new ObjectWatcher { ObjectId = OtherObjectId, IdentityId = IdentityId });

            using var db = ModelHub.CreateDbContext();
            var entries = db.ObjectWatchers
                .Where(w => w.IdentityId == IdentityId)
                .ToList();
            Assert.Equal(2, entries.Count);
        }

        /// <summary>
        /// Verifies that removing a watcher hard-deletes the row.
        /// </summary>
        [Fact]
        public void RemoveWatcherDeletes()
        {
            var connectionString = nameof(RemoveWatcherDeletes);
            SeedFixtures(connectionString);

            var watcher = new ObjectWatcher { ObjectId = ObjectId, IdentityId = IdentityId };
            ModelHub.Add(watcher);

            ModelHub.Remove(watcher);

            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.ObjectWatchers.Where(w => w.Id == watcher.Id));
        }

        /// <summary>
        /// Verifies that removing a watcher whose id is not in the table is a silent
        /// no-op and does not affect unrelated rows.
        /// </summary>
        [Fact]
        public void RemoveUnknownWatcherIsNoOp()
        {
            var connectionString = nameof(RemoveUnknownWatcherIsNoOp);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectWatcher { ObjectId = ObjectId, IdentityId = IdentityId });

            ModelHub.Remove(new ObjectWatcher { ObjectId = ObjectId, IdentityId = IdentityId });

            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.ObjectWatchers.Where(w => w.ObjectId == ObjectId && w.IdentityId == IdentityId));
        }
    }
}
