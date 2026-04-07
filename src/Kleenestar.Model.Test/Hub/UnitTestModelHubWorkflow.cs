using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub workflow and workflow state.
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
                db.WorkflowStates.Add(new WorkflowState { Id = Guid.NewGuid(), Name = "Alpha" });
                db.WorkflowStates.Add(new WorkflowState { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetWorkflowStates(new Query<WorkflowState>()).ToList();

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
                db.WorkflowStates.Add(new WorkflowState { Id = Guid.NewGuid(), Name = "Alpha" });
                db.WorkflowStates.Add(new WorkflowState { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetWorkflowStates(new Query<WorkflowState>().Where(x => x.Name.StartsWith("A")))
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

            var workflowState = new WorkflowState { Id = Guid.NewGuid(), Name = "A" };

            // act
            ModelHub.Add(workflowState);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.WorkflowStates);
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
            var state1 = new WorkflowState { Id = id, Name = "Alpha" };
            var state2 = new WorkflowState { Id = id, Name = "Beta" }; // same ID

            // act
            ModelHub.Add(state1);
            ModelHub.Add(state2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.WorkflowStates);
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
            var workflowState = new WorkflowState { Id = Guid.NewGuid(), Name = "Original" };
            db.WorkflowStates.Add(workflowState);
            db.SaveChanges();

            // act
            workflowState.Name = "Updated";
            ModelHub.Update(workflowState);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Equal("Updated", db2.WorkflowStates.Single().Name);
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
            var workflowState = new WorkflowState { Id = Guid.NewGuid(), Name = "A" };
            db.WorkflowStates.Add(workflowState);
            db.SaveChanges();

            // act
            ModelHub.Remove(workflowState);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.WorkflowStates);
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
            ModelHub.Remove(new WorkflowState { RawId = 1, Id = Guid.NewGuid() });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.WorkflowStates);
        }
    }
}
