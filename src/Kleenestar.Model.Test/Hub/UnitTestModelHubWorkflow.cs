using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub workflow.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubWorkflow
    {
        /// <summary>
        /// Verifies that all workflows can be retrieved from the database and that the
        /// expected number of workflows is returned.
        /// </summary>
        [Fact]
        public void AllWorkflows()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AllWorkflows",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Workflows.Add(new Workflow { Id = Guid.NewGuid(), Name = "Alpha" });
                db.Workflows.Add(new Workflow { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetWorkflows(new Query<Workflow>()).ToList();

            // validation
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the workflow filtering functionality returns only workflows matching the specified
        /// predicate.
        /// </summary>
        [Fact]
        public void FilteredWorkflows()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "FilteredWorkflows",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Workflows.Add(new Workflow { Id = Guid.NewGuid(), Name = "Alpha" });
                db.Workflows.Add(new Workflow { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetWorkflows(new Query<Workflow>().Where(x => x.Name.StartsWith("A")))
                .ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
        }

        /// <summary>
        /// Verifies that a workflow is added only if it does not already exist in the database.
        /// </summary>
        [Fact]
        public void AddWorkflowWhenNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddWorkflowWhenNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var workflow = new Workflow { Id = Guid.NewGuid(), Name = "A" };

            // act
            ModelHub.Add(workflow);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Workflows);
        }

        /// <summary>
        /// Verifies that adding a workflow with an identifier that already exists in the
        /// database does not result in duplicate entries.
        /// </summary>
        [Fact]
        public void AddWorkflowWhenIdExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddWorkflowWhenIdExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();
            var workflow1 = new Workflow { Id = id, Name = "Alpha" };
            var workflow2 = new Workflow { Id = id, Name = "Beta" }; // same ID

            // act
            ModelHub.Add(workflow1);
            ModelHub.Add(workflow2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Workflows);
        }

        /// <summary>
        /// Updates an existing workflow in the database and verifies that the changes are persisted.
        /// </summary>
        [Fact]
        public void UpdateExistingWorkflow()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "UpdateExistingWorkflow",
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();
            var workflow = new Workflow { Id = Guid.NewGuid(), Name = "Original" };
            db.Workflows.Add(workflow);
            db.SaveChanges();

            // act
            workflow.Name = "Updated";
            ModelHub.Update(workflow);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Equal("Updated", db2.Workflows.Single().Name);
        }

        /// <summary>
        /// Removes an existing workflow from the database and verifies that it has been deleted.
        /// </summary>
        [Fact]
        public void RemoveExistingWorkflow()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveExistingWorkflow",
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();
            var workflow = new Workflow { Id = Guid.NewGuid(), Name = "A" };
            db.Workflows.Add(workflow);
            db.SaveChanges();

            // act
            ModelHub.Remove(workflow);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.Workflows);
        }

        /// <summary>
        /// Verifies that removing a workflow that does not exist in the database does not
        /// result in an error and leaves the workflow collection empty.
        /// </summary>
        [Fact]
        public void RemoveWhenWorkflowNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveWhenWorkflowNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            // act
            ModelHub.Remove(new Workflow { RawId = 1, Id = Guid.NewGuid() });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Workflows);
        }
    }
}
