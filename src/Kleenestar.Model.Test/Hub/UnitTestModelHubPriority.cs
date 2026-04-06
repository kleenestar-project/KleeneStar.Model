using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub priority.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubPriority
    {
        /// <summary>
        /// Verifies that all priorities can be retrieved from the database and that the
        /// expected number of priorities is returned.
        /// </summary>
        [Fact]
        public void AllPriorities()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AllPriorities",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Priorities.Add(new Priority { Id = Guid.NewGuid(), Name = "Alpha" });
                db.Priorities.Add(new Priority { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetPriorities(new Query<Priority>()).ToList();

            // validation
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the priority filtering functionality returns only priorities matching the specified
        /// predicate.
        /// </summary>
        [Fact]
        public void FilteredPriorities()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "FilteredPriorities",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Priorities.Add(new Priority { Id = Guid.NewGuid(), Name = "Alpha" });
                db.Priorities.Add(new Priority { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetPriorities(new Query<Priority>().Where(x => x.Name.StartsWith("A")))
                .ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
        }

        /// <summary>
        /// Verifies that a priority is added only if it does not already exist in the database.
        /// </summary>
        [Fact]
        public void AddPriorityWhenNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddPriorityWhenNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var priority = new Priority { Id = Guid.NewGuid(), Name = "A" };

            // act
            ModelHub.Add(priority);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Priorities);
        }

        /// <summary>
        /// Verifies that adding a priority with an identifier that already exists in the
        /// database does not result in duplicate entries.
        /// </summary>
        [Fact]
        public void AddPriorityWhenIdExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddPriorityWhenIdExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();
            var priority1 = new Priority { Id = id, Name = "Alpha" };
            var priority2 = new Priority { Id = id, Name = "Beta" }; // same ID

            // act
            ModelHub.Add(priority1);
            ModelHub.Add(priority2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Priorities);
        }

        /// <summary>
        /// Removes an existing priority from the database and verifies that it has been deleted.
        /// </summary>
        [Fact]
        public void RemoveExistingPriority()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveExistingPriority",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();
            var priority = new Priority { Id = id, Name = "A" };
            db.Priorities.Add(priority);
            db.SaveChanges();

            // act
            ModelHub.Remove(priority);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.Priorities);
        }

        /// <summary>
        /// Verifies that removing a priority that does not exist in the database does not
        /// result in an error and leaves the priority collection empty.
        /// </summary>
        [Fact]
        public void RemoveWhenPriorityNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveWhenPriorityNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            // act
            ModelHub.Remove(new Priority { RawId = 1, Id = id });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Priorities);
        }
    }
}
