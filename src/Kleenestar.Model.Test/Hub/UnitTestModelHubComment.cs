using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the <see cref="ModelHub"/> comment surface.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubComment
    {
        private static readonly Guid WorkspaceId = Guid.Parse("1AB1F1CA-3D55-49E2-9F11-86F1B33A11AA");
        private static readonly Guid ClassId = Guid.Parse("9CFD81C0-4B22-4E2C-9C16-9D2A6471F0F0");
        private static readonly Guid ObjectId = Guid.Parse("D9F3F8E0-9C2C-4F22-9C2E-7E2C9A37C0F0");
        private static readonly Guid AuthorId = Guid.Parse("0B7D7C16-3D6C-4E16-A2E0-1D2A5C7AB0F0");

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
                db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-c", Name = "workspace" });
            }
            if (!db.Classes.Any(x => x.Id == ClassId))
            {
                db.Classes.Add(new Class { Id = ClassId, Name = "Incident", WorkspaceId = WorkspaceId });
            }
            if (!db.Identities.Any(x => x.Id == AuthorId))
            {
                db.Identities.Add(new Identity { Id = AuthorId, Name = "Test Author", Email = "test@kleenestar.org", PasswordHash = "$test$" });
            }
            if (!db.Objects.Any(x => x.Id == ObjectId))
            {
                db.Objects.Add(new KleeneStar.Model.Entities.Object { Id = ObjectId, Key = "INC-001", Summary = "Test incident", WorkspaceId = WorkspaceId, ClassId = ClassId });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies all comments in the database can be retrieved.
        /// </summary>
        [Fact]
        public void AllComments()
        {
            var connectionString = nameof(AllComments);
            SeedFixtures(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.Comments.Add(new Comment { Id = Guid.NewGuid(), ObjectId = ObjectId, AuthorId = AuthorId, Content = "first", State = CommentState.Active });
                db.Comments.Add(new Comment { Id = Guid.NewGuid(), ObjectId = ObjectId, AuthorId = AuthorId, Content = "second", State = CommentState.Active });
                db.SaveChanges();
            }

            var result = ModelHub.GetComments(new Query<Comment>()).ToList();

            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the comment filter pipeline returns only matching entries.
        /// </summary>
        [Fact]
        public void FilteredComments()
        {
            var connectionString = nameof(FilteredComments);
            SeedFixtures(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.Comments.Add(new Comment { Id = Guid.NewGuid(), ObjectId = ObjectId, AuthorId = AuthorId, Content = "hello" });
                db.Comments.Add(new Comment { Id = Guid.NewGuid(), ObjectId = ObjectId, AuthorId = AuthorId, Content = "goodbye" });
                db.SaveChanges();
            }

            var result = ModelHub.GetComments(new Query<Comment>().Where(x => x.Content.StartsWith("h"))).ToList();

            Assert.Single(result);
            Assert.Equal("hello", result[0].Content);
        }

        /// <summary>
        /// Verifies that adding a comment persists it.
        /// </summary>
        [Fact]
        public void AddCommentPersists()
        {
            var connectionString = nameof(AddCommentPersists);
            SeedFixtures(connectionString);

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                ObjectId = ObjectId,
                AuthorId = AuthorId,
                Content = "hello world",
                State = CommentState.Active
            };

            ModelHub.Add(comment);

            var loaded = ModelHub.GetComments(new Query<Comment>().WhereEquals(x => x.Id, comment.Id)).Single();
            Assert.Equal("hello world", loaded.Content);
            Assert.Equal(CommentState.Active, loaded.State);
            Assert.NotEqual(default, loaded.Created);
        }

        /// <summary>
        /// Verifies that adding a comment whose id already exists is a no-op.
        /// </summary>
        [Fact]
        public void AddCommentWhenIdExistsIsNoOp()
        {
            var connectionString = nameof(AddCommentWhenIdExistsIsNoOp);
            SeedFixtures(connectionString);

            var id = Guid.NewGuid();
            ModelHub.Add(new Comment { Id = id, ObjectId = ObjectId, AuthorId = AuthorId, Content = "first" });
            ModelHub.Add(new Comment { Id = id, ObjectId = ObjectId, AuthorId = AuthorId, Content = "second" });

            using var db = ModelHub.CreateDbContext();
            var entries = db.Comments.Where(x => x.Id == id).ToList();
            Assert.Single(entries);
            Assert.Equal("first", entries[0].Content);
        }

        /// <summary>
        /// Verifies that updating a comment overwrites scalar values and bumps Updated.
        /// </summary>
        [Fact]
        public void UpdateCommentChangesContent()
        {
            var connectionString = nameof(UpdateCommentChangesContent);
            SeedFixtures(connectionString);

            var comment = new Comment { Id = Guid.NewGuid(), ObjectId = ObjectId, AuthorId = AuthorId, Content = "original" };
            ModelHub.Add(comment);

            comment.Content = "edited";
            comment.State = CommentState.Edited;
            ModelHub.Update(comment);

            var loaded = ModelHub.GetComments(new Query<Comment>().WhereEquals(x => x.Id, comment.Id)).Single();
            Assert.Equal("edited", loaded.Content);
            Assert.Equal(CommentState.Edited, loaded.State);
        }

        /// <summary>
        /// Verifies that removing a comment hard-deletes the row.
        /// </summary>
        [Fact]
        public void RemoveCommentDeletes()
        {
            var connectionString = nameof(RemoveCommentDeletes);
            SeedFixtures(connectionString);

            var comment = new Comment { Id = Guid.NewGuid(), ObjectId = ObjectId, AuthorId = AuthorId, Content = "to remove" };
            ModelHub.Add(comment);

            ModelHub.Remove(comment);

            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Comments.Where(x => x.Id == comment.Id));
        }
    }
}
