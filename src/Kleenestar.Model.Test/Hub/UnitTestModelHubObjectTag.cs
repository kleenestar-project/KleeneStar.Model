using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the <see cref="ModelHub"/> object-tag surface.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubObjectTag
    {
        private static readonly Guid WorkspaceId = Guid.Parse("3CD3F3EC-5F77-4BF4-B133-A8F3D55C33CC");
        private static readonly Guid ClassId = Guid.Parse("BEEF93E2-6D44-4F4E-BE38-BF4C8693F2E2");
        private static readonly Guid ObjectId = Guid.Parse("FBF5FAF2-BE4E-4144-BE40-9F4EBC59E2F3");
        private static readonly Guid OtherObjectId = Guid.Parse("0CF6FBF3-CF5F-4255-CF51-AF5FCD6AF3F4");

        /// <summary>
        /// Seeds the in-memory database with two objects so each test can exercise the tag
        /// CRUD surface against a stable fixture.
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
                db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-t", Name = "workspace" });
            }
            if (!db.Classes.Any(x => x.Id == ClassId))
            {
                db.Classes.Add(new Class { Id = ClassId, Name = "Incident", WorkspaceId = WorkspaceId });
            }
            if (!db.Objects.Any(x => x.Id == ObjectId))
            {
                db.Objects.Add(new KleeneStar.Model.Entities.Object { Id = ObjectId, Key = "TMH-001", Summary = "tagged item", WorkspaceId = WorkspaceId, ClassId = ClassId });
            }
            if (!db.Objects.Any(x => x.Id == OtherObjectId))
            {
                db.Objects.Add(new KleeneStar.Model.Entities.Object { Id = OtherObjectId, Key = "TMH-002", Summary = "other item", WorkspaceId = WorkspaceId, ClassId = ClassId });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies that every tag in the database is returned when no filter criteria are
        /// applied.
        /// </summary>
        [Fact]
        public void AllTags()
        {
            var connectionString = nameof(AllTags);
            SeedFixtures(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.ObjectTags.Add(new ObjectTag { ObjectId = ObjectId, Name = "Urgent", Created = DateTime.UtcNow });
                db.ObjectTags.Add(new ObjectTag { ObjectId = ObjectId, Name = "Backend", Created = DateTime.UtcNow });
                db.SaveChanges();
            }

            var result = ModelHub.GetObjectTags(new Query<ObjectTag>()).ToList();

            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the query pipeline returns only the tags matching the supplied
        /// predicate.
        /// </summary>
        [Fact]
        public void FilteredTags()
        {
            var connectionString = nameof(FilteredTags);
            SeedFixtures(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.ObjectTags.Add(new ObjectTag { ObjectId = ObjectId, Name = "Urgent", Created = DateTime.UtcNow });
                db.ObjectTags.Add(new ObjectTag { ObjectId = OtherObjectId, Name = "Urgent", Created = DateTime.UtcNow });
                db.SaveChanges();
            }

            var result = ModelHub.GetObjectTags(
                new Query<ObjectTag>().WhereEquals(x => x.ObjectId, ObjectId)).ToList();

            Assert.Single(result);
            Assert.Equal(ObjectId, result[0].ObjectId);
        }

        /// <summary>
        /// Verifies that the materialized rows hydrate the <see cref="ObjectTag.Object"/>
        /// navigation property so callers do not need an extra round-trip.
        /// </summary>
        [Fact]
        public void GetTagsHydratesObject()
        {
            var connectionString = nameof(GetTagsHydratesObject);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectTag { ObjectId = ObjectId, Name = "Urgent" });

            var result = ModelHub.GetObjectTags(
                new Query<ObjectTag>().WhereEquals(x => x.ObjectId, ObjectId)).Single();

            Assert.NotNull(result.Object);
            Assert.Equal("TMH-001", result.Object.Key);
        }

        /// <summary>
        /// Verifies that adding a tag persists the row and back-fills
        /// <see cref="ObjectTag.Created"/> when the caller leaves it unset.
        /// </summary>
        [Fact]
        public void AddTagPersists()
        {
            var connectionString = nameof(AddTagPersists);
            SeedFixtures(connectionString);

            var tag = new ObjectTag
            {
                ObjectId = ObjectId,
                Name = "Urgent",
                Color = "#dc3545"
            };

            ModelHub.Add(tag);

            var loaded = ModelHub.GetObjectTags(
                new Query<ObjectTag>().WhereEquals(x => x.Id, tag.Id)).Single();
            Assert.Equal(ObjectId, loaded.ObjectId);
            Assert.Equal("Urgent", loaded.Name);
            Assert.Equal("#dc3545", loaded.Color);
            Assert.NotEqual(default, loaded.Created);
        }

        /// <summary>
        /// Verifies that a second <see cref="ModelHub.Add(ObjectTag)"/> for the same
        /// (object, name) pair is a silent no-op. The unique composite index would otherwise
        /// throw on the second insert, so the early-return guard is load-bearing.
        /// </summary>
        [Fact]
        public void AddTagWhenPairExistsIsNoOp()
        {
            var connectionString = nameof(AddTagWhenPairExistsIsNoOp);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectTag { ObjectId = ObjectId, Name = "Urgent" });
            ModelHub.Add(new ObjectTag { ObjectId = ObjectId, Name = "Urgent" });

            using var db = ModelHub.CreateDbContext();
            var entries = db.ObjectTags
                .Where(t => t.ObjectId == ObjectId && t.Name == "Urgent")
                .ToList();
            Assert.Single(entries);
        }

        /// <summary>
        /// Verifies that the same object can carry two tags with different names — the unique
        /// index only forbids the duplicate (object, name) pair.
        /// </summary>
        [Fact]
        public void AddTagSameObjectDifferentNames()
        {
            var connectionString = nameof(AddTagSameObjectDifferentNames);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectTag { ObjectId = ObjectId, Name = "Urgent" });
            ModelHub.Add(new ObjectTag { ObjectId = ObjectId, Name = "Backend" });

            using var db = ModelHub.CreateDbContext();
            var entries = db.ObjectTags
                .Where(t => t.ObjectId == ObjectId)
                .ToList();
            Assert.Equal(2, entries.Count);
        }

        /// <summary>
        /// Verifies that removing a tag hard-deletes the row.
        /// </summary>
        [Fact]
        public void RemoveTagDeletes()
        {
            var connectionString = nameof(RemoveTagDeletes);
            SeedFixtures(connectionString);

            var tag = new ObjectTag { ObjectId = ObjectId, Name = "Urgent" };
            ModelHub.Add(tag);

            ModelHub.Remove(tag);

            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.ObjectTags.Where(t => t.Id == tag.Id));
        }

        /// <summary>
        /// Verifies that removing a tag whose id is not in the table is a silent no-op and
        /// does not affect unrelated rows.
        /// </summary>
        [Fact]
        public void RemoveUnknownTagIsNoOp()
        {
            var connectionString = nameof(RemoveUnknownTagIsNoOp);
            SeedFixtures(connectionString);

            ModelHub.Add(new ObjectTag { ObjectId = ObjectId, Name = "Urgent" });

            ModelHub.Remove(new ObjectTag { ObjectId = ObjectId, Name = "Urgent" });

            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.ObjectTags.Where(t => t.ObjectId == ObjectId && t.Name == "Urgent"));
        }
    }
}
