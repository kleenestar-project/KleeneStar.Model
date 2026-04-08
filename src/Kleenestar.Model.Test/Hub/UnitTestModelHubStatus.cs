using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub statuses.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubStatus
    {
        /// <summary>
        /// Verifies that all workflow states can be retrieved from the database and that the
        /// expected number of workflow states is returned.
        /// </summary>
        [Fact]
        public void AllWorkflowStates()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AllWorkflowStates",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.StatusCategories.Add(new StatusCategory
                {
                    Id = Guid.Parse("2871E154-A629-4317-9CFE-8BA07C0CF7BF"),
                    Name = "Category1"
                });
                db.SaveChanges();
            }

            using (var db = ModelHub.CreateDbContext())
            {
                db.Statuses.Add(new Status
                {
                    Id = Guid.NewGuid(),
                    Name = "Alpha",
                    CategoryId = Guid.Parse("2871E154-A629-4317-9CFE-8BA07C0CF7BF")
                });
                db.Statuses.Add(new Status
                {
                    Id = Guid.NewGuid(),
                    Name = "Beta",
                    CategoryId = Guid.Parse("2871E154-A629-4317-9CFE-8BA07C0CF7BF")
                });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetStatuses(new Query<Status>()).ToList();

            // validation
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the workflow state filtering functionality returns only workflow states matching the
        /// specified predicate.
        /// </summary>
        [Fact]
        public void FilteredWorkflowStates()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "FilteredWorkflowStates",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.StatusCategories.Add(new StatusCategory
                {
                    Id = Guid.Parse("13609082-6293-451B-B49A-20728446C1B7"),
                    Name = "Category1"
                });
                db.SaveChanges();
            }

            using (var db = ModelHub.CreateDbContext())
            {
                db.Statuses.Add(new Status
                {
                    Id = Guid.NewGuid(),
                    Name = "Alpha",
                    CategoryId = Guid.Parse("13609082-6293-451B-B49A-20728446C1B7")
                });
                db.Statuses.Add(new Status
                {
                    Id = Guid.NewGuid(),
                    Name = "Beta",
                    CategoryId = Guid.Parse("13609082-6293-451B-B49A-20728446C1B7")
                });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetStatuses(new Query<Status>().Where(x => x.Name.StartsWith("A")))
                .ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
        }

        /// <summary>
        /// Verifies that a workflow state is added only if it does not already exist in the database.
        /// </summary>
        [Fact]
        public void AddWorkflowStateWhenNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddWorkflowStateWhenNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var workflowState = new Status { Id = Guid.NewGuid(), Name = "A" };

            // act
            ModelHub.Add(workflowState);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Statuses);
        }

        /// <summary>
        /// Verifies that adding a workflow state with an identifier that already exists in the
        /// database does not result in duplicate entries.
        /// </summary>
        [Fact]
        public void AddWorkflowStateWhenIdExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddWorkflowStateWhenIdExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();
            var state1 = new Status { Id = id, Name = "Alpha" };
            var state2 = new Status { Id = id, Name = "Beta" }; // same ID

            // act
            ModelHub.Add(state1);
            ModelHub.Add(state2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Statuses);
        }

        /// <summary>
        /// Updates an existing workflow state in the database and verifies that the changes are persisted.
        /// </summary>
        [Fact]
        public void UpdateExistingWorkflowState()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "UpdateExistingWorkflowState",
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();
            var workflowState = new Status { Id = Guid.NewGuid(), Name = "Original" };
            db.Statuses.Add(workflowState);
            db.SaveChanges();

            // act
            workflowState.Name = "Updated";
            ModelHub.Update(workflowState);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Equal("Updated", db2.Statuses.Single().Name);
        }

        /// <summary>
        /// Removes an existing workflow state from the database and verifies that it has been deleted.
        /// </summary>
        [Fact]
        public void RemoveExistingWorkflowState()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveExistingWorkflowState",
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();
            var workflowState = new Status { Id = Guid.NewGuid(), Name = "A" };
            db.Statuses.Add(workflowState);
            db.SaveChanges();

            // act
            ModelHub.Remove(workflowState);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.Statuses);
        }

        /// <summary>
        /// Verifies that removing a workflow state that does not exist in the database does not
        /// result in an error and leaves the workflow state collection empty.
        /// </summary>
        [Fact]
        public void RemoveWhenWorkflowStateNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveWhenWorkflowStateNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            // act
            ModelHub.Remove(new Status { RawId = 1, Id = Guid.NewGuid() });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Statuses);
        }
    }
}
