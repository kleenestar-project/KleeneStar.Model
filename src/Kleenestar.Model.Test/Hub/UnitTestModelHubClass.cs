using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubClass
    {
        /// <summary>
        /// Verifies that all classes can be retrieved from the database and that the
        /// expected number of classes is returned.
        /// </summary>
        [Fact]
        public void AllClasses()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AllClasses",
                Assembly = "KleeneStar.Model.Test"
            };

            var workspaceId = Guid.NewGuid();

            using (var db = ModelHub.CreateDbContext())
            {
                db.Workspaces.Add(new Workspace { Id = workspaceId, Name = "W", Key = "W" });
                db.Classes.Add(new Class { Id = Guid.NewGuid(), Name = "Alpha", Key = "ALPHA", WorkspaceId = workspaceId });
                db.Classes.Add(new Class { Id = Guid.NewGuid(), Name = "Beta", Key = "BETA", WorkspaceId = workspaceId });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetClasses(new Query<Class>()).ToList();

            // validation
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the class filtering functionality returns only classes matching the specified
        /// predicate.
        /// </summary>
        [Fact]
        public void FilteredClasses()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "FilteredClasses",
                Assembly = "KleeneStar.Model.Test"
            };

            var workspaceId = Guid.NewGuid();

            using (var db = ModelHub.CreateDbContext())
            {
                db.Workspaces.Add(new Workspace { Id = workspaceId, Name = "W", Key = "W" });
                db.Classes.Add(new Class { Id = Guid.NewGuid(), Name = "Alpha", Key = "ALPHA", WorkspaceId = workspaceId });
                db.Classes.Add(new Class { Id = Guid.NewGuid(), Name = "Beta", Key = "BETA", WorkspaceId = workspaceId });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetClasses(new Query<Class>().Where(x => x.Name.StartsWith("A")))
                .ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
        }

        /// <summary>
        /// Verifies that a class is added only if it does not already exist in the database.
        /// </summary>
        [Fact]
        public void AddClassWhenNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddClassWhenNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var classEntry = new Class { Id = Guid.NewGuid(), Name = "A", Key = "A" };

            // act
            ModelHub.Add(classEntry);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Classes);
        }

        /// <summary>
        /// Verifies that adding a class with an identifier that already exists in the
        /// database does not result in duplicate entries.
        /// </summary>
        [Fact]
        public void AddClassWhenIdExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddClassWhenIdExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();
            var class1 = new Class { Id = id, Name = "Alpha", Key = "ALPHA" };
            var class2 = new Class { Id = id, Name = "Beta", Key = "BETA" }; // same ID

            // act
            ModelHub.Add(class1);
            ModelHub.Add(class2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Classes);
        }

        /// <summary>
        /// Removes an existing class from the database and verifies that it has been deleted.
        /// </summary>
        [Fact]
        public void RemoveExistingClass()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveExistingClass",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();
            var classEntry = new Class { Id = id, Name = "A", Key = "A" };
            db.Classes.Add(classEntry);
            db.SaveChanges();

            // act
            ModelHub.Remove(classEntry);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.Classes);
        }

        /// <summary>
        /// Verifies that removing a class that does not exist in the database does not
        /// result in an error and leaves the class collection empty.
        /// </summary>
        [Fact]
        public void RemoveWhenClassNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveWhenClassNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            // act
            ModelHub.Remove(new Class { RawId = 1, Id = id });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Classes);
        }

        /// <summary>
        /// Updates an existing class in the database and verifies that the changes are persisted.
        /// </summary>
        [Fact]
        public void UpdateExistingClass()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "UpdateExistingClass",
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();
            var classEntry = new Class { Id = Guid.NewGuid(), Name = "Original", Key = "ORIGINAL" };
            db.Classes.Add(classEntry);
            db.SaveChanges();

            // act
            classEntry.Name = "Updated";
            ModelHub.Update(classEntry);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Equal("Updated", db2.Classes.Single().Name);
        }

        /// <summary>
        /// Verifies that adding a class automatically creates a standard form for it.
        /// </summary>
        [Fact]
        public void AddClassCreatesStandardForm()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddClassCreatesStandardForm",
                Assembly = "KleeneStar.Model.Test"
            };

            var classEntry = new Class { Id = Guid.NewGuid(), Name = "TestClass", Key = "TESTCLASS" };

            // act
            ModelHub.Add(classEntry);

            // validation
            using var db = ModelHub.CreateDbContext();
            var forms = db.Forms.Where(f => f.ClassId == classEntry.Id).ToList();
            Assert.Single(forms);
            Assert.Equal("Standard", forms[0].Name);
            Assert.Equal(FormType.Standard, forms[0].FormType);
        }
    }
}
