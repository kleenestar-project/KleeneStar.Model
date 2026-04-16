using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub category.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubCategory
    {
        /// <summary>
        /// Verifies that all categories can be retrieved from the database and that the 
        /// expected number of workspaces is returned.
        [Fact]
        public void AllCategories()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AllCategories",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Categories.Add(new Category { Id = Guid.NewGuid(), Name = "Alpha" });
                db.Categories.Add(new Category { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetCategories(new Query<Category>()).ToList();

            // validation
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the category filtering functionality returns only categories matching the specified
        /// predicate.
        /// </summary>
        [Fact]
        public void FilteredCategories()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "FilteredCategories",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Categories.Add(new Category { Id = Guid.NewGuid(), Name = "Alpha" });
                db.Categories.Add(new Category { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetCategories(new Query<Category>().Where(x => x.Name.StartsWith("A")))
                .ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
        }

        /// <summary>
        /// Verifies that a category is added only if it does not already exist in the database.
        /// </summary>
        [Fact]
        public void AddCategoryWhenNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddCategoryWhenNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var cat = new Category { Id = Guid.NewGuid(), Name = "A" };

            // act
            ModelHub.Add(cat);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Categories);
        }

        /// <summary>
        /// Verifies that adding a category with a name that differs only in case from an 
        /// existing category does not result in duplicate entries.
        /// </summary>
        [Fact]
        public void AddCategoryWhenNameExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddCategoryWhenKeyExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var cat1 = new Category { Id = Guid.NewGuid(), Name = "Alpha" };
            var cat2 = new Category { Id = Guid.NewGuid(), Name = "alpha" }; // case-insensitive

            // act
            ModelHub.Add(cat1);
            ModelHub.Add(cat2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Categories);
        }

        /// <summary>
        /// Verifies that removing a workspace with a non-existent category does 
        /// not affect the database.
        /// </summary>
        [Fact]
        public void RemoveWhenCategoryNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveWhenCategoryNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            // act
            ModelHub.Remove(new Category { Id = id });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Categories);
        }

    }
}
