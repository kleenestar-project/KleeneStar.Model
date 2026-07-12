using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the <see cref="ModelHub"/> object-share surface.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubObjectShare
    {
        private static readonly Guid WorkspaceId = Guid.Parse("3CD3F3EC-5F77-4B04-B133-A803D55C33CC");
        private static readonly Guid ClassId = Guid.Parse("BE0FA3E2-6D44-403E-BE38-BF4C8693F2F2");
        private static readonly Guid ObjectId = Guid.Parse("FB050A02-BE4E-4144-BE40-9F4EBC59E205");
        private static readonly Guid OtherObjectId = Guid.Parse("0C161B13-CF5F-4255-CF51-A05FCD6AF316");
        private static readonly Guid IdentityId = Guid.Parse("2DAFAF49-608F-4149-D503-405D8EADE3F3");
        private static readonly Guid OtherIdentityId = Guid.Parse("3EB0B05A-719F-405A-E614-516E9FBEF404");

        /// <summary>
        /// Seeds the in-memory database with two objects and two identities so each
        /// test can exercise the share CRUD surface against a stable fixture.
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
                db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-s", Name = "workspace" });
            }
            if (!db.Classes.Any(x => x.Id == ClassId))
            {
                db.Classes.Add(new Class { Id = ClassId, Name = "Incident", WorkspaceId = WorkspaceId });
            }
            if (!db.Identities.Any(x => x.Id == IdentityId))
            {
                db.Identities.Add(new Identity { Id = IdentityId, Name = "Share One", Email = "s1@kleenestar.org", PasswordHash = "$test$" });
            }
            if (!db.Identities.Any(x => x.Id == OtherIdentityId))
            {
                db.Identities.Add(new Identity { Id = OtherIdentityId, Name = "Share Two", Email = "s2@kleenestar.org", PasswordHash = "$test$" });
            }
            if (!db.Objects.Any(x => x.Id == ObjectId))
            {
                db.Objects.Add(new KleeneStar.Model.Entities.Object { Id = ObjectId, Key = "SMH-001", Summary = "shared item", WorkspaceId = WorkspaceId, ClassId = ClassId });
            }
            if (!db.Objects.Any(x => x.Id == OtherObjectId))
            {
                db.Objects.Add(new KleeneStar.Model.Entities.Object { Id = OtherObjectId, Key = "SMH-002", Summary = "unshared item", WorkspaceId = WorkspaceId, ClassId = ClassId });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies that every share in the database is returned when no filter
        /// criteria are applied.
        /// </summary>
        [Fact]
        public void AllShares()
        {
            var connectionString = nameof(AllShares);
            SeedFixtures(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.ObjectShares.Add(new ObjectShare { ObjectId = ObjectId, IdentityId = IdentityId, Created = DateTime.UtcNow });
                db.ObjectShares.Add(new ObjectShare { ObjectId = ObjectId, IdentityId = OtherIdentityId, Created = DateTime.UtcNow });
                db.SaveChanges();
            }

            var result = ModelHub.GetObjectShares(new Query<ObjectShare>()).ToList();

            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the query pipeline returns only the shares matching the
        /// supplied predicate.
        /// </summary>
        [Fact]
        public void FilteredShares()
        {
            var connectionString = nameof(FilteredShares);
            SeedFixtures(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.ObjectShares.Add(new ObjectShare { ObjectId = ObjectId, IdentityId = IdentityId, Created = DateTime.UtcNow });
                db.ObjectShares.Add(new ObjectShare { ObjectId = OtherObjectId, IdentityId = IdentityId, Created = DateTime.UtcNow });
                db.SaveChanges();
            }

            var result = ModelHub.GetObjectShares(
                new Query<ObjectShare>().WhereEquals(x => x.ObjectId, ObjectId)).ToList();

            Assert.Single(result);
            Assert.Equal(ObjectId, result[0].ObjectId);
        }

        /// <summary>
        /// Verifies that the materialized rows hydrate the
        /// <see cref="ObjectShare.Identity"/> and <see cref="ObjectShare.Object"/>
        /// navigation properties so callers do not need an extra round-trip to display
        /// the share-holder name.
        /// </summary>
        [Fact]
        public void GetSharesHydratesNavigationProperties()
        {
            var connectionString = nameof(GetSharesHydratesNavigationProperties);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectShare { ObjectId = ObjectId, IdentityId = IdentityId });

            var result = ModelHub.GetObjectShares(
                new Query<ObjectShare>().WhereEquals(x => x.ObjectId, ObjectId)).Single();

            Assert.NotNull(result.Identity);
            Assert.Equal("Share One", result.Identity.Name);
            Assert.NotNull(result.Object);
            Assert.Equal("SMH-001", result.Object.Key);
        }

        /// <summary>
        /// Verifies that adding a share persists the row and back-fills
        /// <see cref="ObjectShare.Created"/> when the caller leaves it unset.
        /// </summary>
        [Fact]
        public void AddSharePersists()
        {
            var connectionString = nameof(AddSharePersists);
            SeedFixtures(connectionString);

            var share = new ObjectShare
            {
                ObjectId = ObjectId,
                IdentityId = IdentityId
            };

            ModelHub.Add(share);

            var loaded = ModelHub.GetObjectShares(
                new Query<ObjectShare>().WhereEquals(x => x.Id, share.Id)).Single();
            Assert.Equal(ObjectId, loaded.ObjectId);
            Assert.Equal(IdentityId, loaded.IdentityId);
            Assert.NotEqual(default, loaded.Created);
        }

        /// <summary>
        /// Verifies that a second <see cref="ModelHub.Add(ObjectShare)"/> for the same
        /// (object, identity) pair is a silent no-op. The unique composite index would
        /// otherwise throw on the second insert, so the early-return guard is
        /// load-bearing.
        /// </summary>
        [Fact]
        public void AddShareWhenPairExistsIsNoOp()
        {
            var connectionString = nameof(AddShareWhenPairExistsIsNoOp);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectShare { ObjectId = ObjectId, IdentityId = IdentityId });
            ModelHub.Add(new ObjectShare { ObjectId = ObjectId, IdentityId = IdentityId });

            using var db = ModelHub.CreateDbContext();
            var entries = db.ObjectShares
                .Where(s => s.ObjectId == ObjectId && s.IdentityId == IdentityId)
                .ToList();
            Assert.Single(entries);
        }

        /// <summary>
        /// Verifies that the same identity can hold shares on two different objects —
        /// the unique index only forbids the duplicate (object, identity) pair, not
        /// the per-identity total.
        /// </summary>
        [Fact]
        public void AddShareSameIdentityDifferentObjects()
        {
            var connectionString = nameof(AddShareSameIdentityDifferentObjects);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectShare { ObjectId = ObjectId, IdentityId = IdentityId });
            ModelHub.Add(new ObjectShare { ObjectId = OtherObjectId, IdentityId = IdentityId });

            using var db = ModelHub.CreateDbContext();
            var entries = db.ObjectShares
                .Where(s => s.IdentityId == IdentityId)
                .ToList();
            Assert.Equal(2, entries.Count);
        }

        /// <summary>
        /// Verifies that removing a share hard-deletes the row.
        /// </summary>
        [Fact]
        public void RemoveShareDeletes()
        {
            var connectionString = nameof(RemoveShareDeletes);
            SeedFixtures(connectionString);

            var share = new ObjectShare { ObjectId = ObjectId, IdentityId = IdentityId };
            ModelHub.Add(share);

            ModelHub.Remove(share);

            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.ObjectShares.Where(s => s.Id == share.Id));
        }

        /// <summary>
        /// Verifies that removing a share whose id is not in the table is a silent
        /// no-op and does not affect unrelated rows.
        /// </summary>
        [Fact]
        public void RemoveUnknownShareIsNoOp()
        {
            var connectionString = nameof(RemoveUnknownShareIsNoOp);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectShare { ObjectId = ObjectId, IdentityId = IdentityId });

            ModelHub.Remove(new ObjectShare { ObjectId = ObjectId, IdentityId = IdentityId });

            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.ObjectShares.Where(s => s.ObjectId == ObjectId && s.IdentityId == IdentityId));
        }
    }
}
