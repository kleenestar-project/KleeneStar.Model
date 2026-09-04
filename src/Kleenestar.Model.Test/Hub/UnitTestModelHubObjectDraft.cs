using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the <see cref="ModelHub"/> object-draft surface - the unpublished
    /// working copy of the prose of an object.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubObjectDraft
    {
        private static readonly Guid WorkspaceId = Guid.Parse("6A1D0C21-4E70-4C4A-9C21-3D6E1F2A7B01");
        private static readonly Guid ClassId = Guid.Parse("6A1D0C21-4E70-4C4A-9C21-3D6E1F2A7B02");
        private static readonly Guid ObjectId = Guid.Parse("6A1D0C21-4E70-4C4A-9C21-3D6E1F2A7B03");
        private static readonly Guid OtherObjectId = Guid.Parse("6A1D0C21-4E70-4C4A-9C21-3D6E1F2A7B04");

        /// <summary>
        /// Seeds the in-memory database with two objects so a draft can be written against one
        /// and asserted absent on the other.
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
                db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-d", Name = "workspace" });
            }
            if (!db.Classes.Any(x => x.Id == ClassId))
            {
                db.Classes.Add(new Class { Id = ClassId, Name = "Page", WorkspaceId = WorkspaceId });
            }
            if (!db.Objects.Any(x => x.Id == ObjectId))
            {
                db.Objects.Add(new KleeneStar.Model.Entities.Object { Id = ObjectId, Key = "DMH-001", Summary = "drafted page", WorkspaceId = WorkspaceId, ClassId = ClassId });
            }
            if (!db.Objects.Any(x => x.Id == OtherObjectId))
            {
                db.Objects.Add(new KleeneStar.Model.Entities.Object { Id = OtherObjectId, Key = "DMH-002", Summary = "other page", WorkspaceId = WorkspaceId, ClassId = ClassId });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies that the first upsert creates the draft row and stamps both timestamps.
        /// </summary>
        [Fact]
        public void UpsertCreatesDraft()
        {
            SeedFixtures(nameof(UpsertCreatesDraft));

            var draft = ModelHub.UpsertObjectDraft(ObjectId, "title", "<p>body</p>", null);

            Assert.NotNull(draft);
            Assert.Equal(ObjectId, draft.ObjectId);
            Assert.Equal("title", draft.Summary);
            Assert.Equal("<p>body</p>", draft.Description);
            Assert.NotEqual(default, draft.Created);
            Assert.NotEqual(default, draft.Updated);
        }

        /// <summary>
        /// Verifies that a later upsert overwrites the same row rather than adding a second -
        /// the read and the write share one context precisely so that two saves cannot race
        /// into two rows the unique index would then refuse.
        /// </summary>
        [Fact]
        public void UpsertOverwritesTheSameRow()
        {
            SeedFixtures(nameof(UpsertOverwritesTheSameRow));

            var first = ModelHub.UpsertObjectDraft(ObjectId, "one", "<p>one</p>", null);
            var second = ModelHub.UpsertObjectDraft(ObjectId, "two", "<p>two</p>", null);

            Assert.Equal(first.Id, second.Id);
            Assert.Equal(first.Created, second.Created);

            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.ObjectDrafts.Where(x => x.ObjectId == ObjectId));
        }

        /// <summary>
        /// Verifies that a draft cannot be opened on an object that does not exist.
        /// </summary>
        [Fact]
        public void UpsertUnknownObjectReturnsNull()
        {
            SeedFixtures(nameof(UpsertUnknownObjectReturnsNull));

            Assert.Null(ModelHub.UpsertObjectDraft(Guid.NewGuid(), "title", "<p>body</p>", null));
        }

        /// <summary>
        /// Verifies that reading answers the draft of the addressed object only.
        /// </summary>
        [Fact]
        public void GetDraftIsScopedToItsObject()
        {
            SeedFixtures(nameof(GetDraftIsScopedToItsObject));

            ModelHub.UpsertObjectDraft(ObjectId, "title", "<p>body</p>", null);

            Assert.NotNull(ModelHub.GetObjectDraft(ObjectId));
            Assert.Null(ModelHub.GetObjectDraft(OtherObjectId));
            Assert.Single(ModelHub.GetObjectDrafts(new Query<ObjectDraft>()));
        }

        /// <summary>
        /// Verifies that removing answers whether anything was there, so a caller can tell
        /// "discarded" from "there was nothing to discard".
        /// </summary>
        [Fact]
        public void RemoveReportsWhetherADraftExisted()
        {
            SeedFixtures(nameof(RemoveReportsWhetherADraftExisted));

            ModelHub.UpsertObjectDraft(ObjectId, "title", "<p>body</p>", null);

            Assert.True(ModelHub.RemoveObjectDraft(ObjectId));
            Assert.False(ModelHub.RemoveObjectDraft(ObjectId));
            Assert.Null(ModelHub.GetObjectDraft(ObjectId));
        }
    }
}
