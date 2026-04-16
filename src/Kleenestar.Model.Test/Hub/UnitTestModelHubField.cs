using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub field.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubField
    {
        /// <summary>
        /// Verifies that all fields can be retrieved from the database and that the
        /// expected number of fields is returned.
        /// </summary>
        [Fact]
        public void AllFields()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AllFields",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Fields.Add(new Field { Id = Guid.NewGuid(), Name = "Alpha" });
                db.Fields.Add(new Field { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetFields(new Query<Field>()).ToList();

            // validation
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the field filtering functionality returns only fields matching the specified
        /// predicate.
        /// </summary>
        [Fact]
        public void FilteredFields()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "FilteredFields",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Fields.Add(new Field { Id = Guid.NewGuid(), Name = "Alpha" });
                db.Fields.Add(new Field { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetFields(new Query<Field>().Where(x => x.Name.StartsWith("A")))
                .ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
        }

        /// <summary>
        /// Verifies that a field is added only if it does not already exist in the database.
        /// </summary>
        [Fact]
        public void AddFieldWhenNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddFieldWhenNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var field = new Field { Id = Guid.NewGuid(), Name = "A" };

            // act
            ModelHub.Add(field);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Fields);
        }

        /// <summary>
        /// Verifies that adding a field with an identifier that already exists in the
        /// database does not result in duplicate entries.
        /// </summary>
        [Fact]
        public void AddFieldWhenIdExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddFieldWhenIdExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();
            var field1 = new Field { Id = id, Name = "Alpha" };
            var field2 = new Field { Id = id, Name = "Beta" }; // same ID

            // act
            ModelHub.Add(field1);
            ModelHub.Add(field2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Fields);
        }

        /// <summary>
        /// Removes an existing field from the database and verifies that it has been deleted.
        /// </summary>
        [Fact]
        public void RemoveExistingField()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveExistingField",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();
            var field = new Field { Id = id, Name = "A" };
            db.Fields.Add(field);
            db.SaveChanges();

            // act
            ModelHub.Remove(field);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.Fields);
        }

        /// <summary>
        /// Verifies that removing a field that does not exist in the database does not
        /// result in an error and leaves the field collection empty.
        /// </summary>
        [Fact]
        public void RemoveWhenFieldNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveWhenFieldNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            // act
            ModelHub.Remove(new Field { RawId = 1, Id = id });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Fields);
        }

        /// <summary>
        /// Updates an existing field in the database and verifies that the changes are persisted.
        /// </summary>
        [Fact]
        public void UpdateExistingField()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "UpdateExistingField",
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();
            var field = new Field { Id = Guid.NewGuid(), Name = "Original" };
            db.Fields.Add(field);
            db.SaveChanges();

            // act
            field.Name = "Updated";
            ModelHub.Update(field);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Equal("Updated", db2.Fields.Single().Name);
        }
    }
}
