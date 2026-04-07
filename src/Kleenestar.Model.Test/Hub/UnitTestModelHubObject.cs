using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;
using KleeneStarObject = KleeneStar.Model.Entities.Object;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub object.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubObject
    {
        /// <summary>
        /// Verifies that all objects can be retrieved from the database and that the
        /// expected number of objects is returned.
        /// </summary>
        [Fact]
        public void AllObjects()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AllObjects",
                Assembly = "KleeneStar.Model.Test"
            };

            var workspaceId = Guid.NewGuid();

            using (var db = ModelHub.CreateDbContext())
            {
                db.Workspaces.Add(new Workspace { Id = workspaceId, Name = "W", Key = "W" });
                db.Objects.Add(new KleeneStarObject { Id = Guid.NewGuid(), Key = "Alpha", Summary = "Summary A", WorkspaceId = workspaceId });
                db.Objects.Add(new KleeneStarObject { Id = Guid.NewGuid(), Key = "Beta", Summary = "Summary B", WorkspaceId = workspaceId });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetObjects(new Query<KleeneStarObject>()).ToList();

            // validation
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the object filtering functionality returns only objects matching the specified
        /// predicate.
        /// </summary>
        [Fact]
        public void FilteredObjects()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "FilteredObjects",
                Assembly = "KleeneStar.Model.Test"
            };

            var workspaceId = Guid.NewGuid();

            using (var db = ModelHub.CreateDbContext())
            {
                db.Workspaces.Add(new Workspace { Id = workspaceId, Name = "W", Key = "W" });
                db.Objects.Add(new KleeneStarObject { Id = Guid.NewGuid(), Key = "Alpha", Summary = "Summary A", WorkspaceId = workspaceId });
                db.Objects.Add(new KleeneStarObject { Id = Guid.NewGuid(), Key = "Beta", Summary = "Summary B", WorkspaceId = workspaceId });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetObjects(new Query<KleeneStarObject>().Where(x => x.Key.StartsWith("A")))
                .ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Key);
        }

        /// <summary>
        /// Verifies that an object is added only if it does not already exist in the database.
        /// </summary>
        [Fact]
        public void AddObjectWhenNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddObjectWhenNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var obj = new KleeneStarObject { Id = Guid.NewGuid(), Key = "Unique", Summary = "Summary" };

            // act
            ModelHub.Add(obj);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Objects);
        }

        /// <summary>
        /// Verifies that adding an object with an identifier that already exists in the
        /// database does not result in duplicate entries.
        /// </summary>
        [Fact]
        public void AddObjectWhenIdExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddObjectWhenIdExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();
            var obj1 = new KleeneStarObject { Id = id, Key = "Alpha", Summary = "Summary A" };
            var obj2 = new KleeneStarObject { Id = id, Key = "Beta", Summary = "Summary B" }; // same ID

            // act
            ModelHub.Add(obj1);
            ModelHub.Add(obj2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Objects);
        }

        /// <summary>
        /// Removes an existing object from the database and verifies that it has been deleted.
        /// </summary>
        [Fact]
        public void RemoveExistingObject()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveExistingObject",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();
            var obj = new KleeneStarObject { Id = id, Key = "X", Summary = "Summary" };
            db.Objects.Add(obj);
            db.SaveChanges();

            // act
            ModelHub.Remove(obj);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.Objects);
        }

        /// <summary>
        /// Verifies that removing an object that does not exist in the database does not
        /// result in an error and leaves the object collection empty.
        /// </summary>
        [Fact]
        public void RemoveWhenObjectNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveWhenObjectNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            // act
            ModelHub.Remove(new KleeneStarObject { RawId = 1, Id = id });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Objects);
        }

        /// <summary>
        /// Updates an existing object in the database and verifies that the changes are persisted.
        /// </summary>
        [Fact]
        public void UpdateExistingObject()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "UpdateExistingObject",
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();
            var obj = new KleeneStarObject { Id = Guid.NewGuid(), Key = "original-key", Summary = "Original" };
            db.Objects.Add(obj);
            db.SaveChanges();

            // act
            obj.Summary = "Updated";
            ModelHub.Update(obj);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Equal("Updated", db2.Objects.Single().Summary);
        }
    }
}
