using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebIcon;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub workspace.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubWorkspace
    {
        /// <summary>
        /// Verifies that all workspaces can be retrieved from the database and that the expected number of workspaces
        /// is returned.
        /// </summary>
        [Fact]
        public void AllWorkspaces()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AllWorkspaces",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Workspaces.Add(new Workspace { Id = Guid.NewGuid(), Name = "A", Key = "A" });
                db.Workspaces.Add(new Workspace { Id = Guid.NewGuid(), Name = "B", Key = "B" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetWorkspaces(new Query<Workspace>());

            // validation
            Assert.Equal(2, result.Count());
        }

        /// <summary>
        /// Verifies that the workspace filtering functionality returns only workspaces matching the specified
        /// predicate.
        /// </summary>
        [Fact]
        public void FilteredWorkspaces()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "FilteredWorkspaces",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Workspaces.Add(new Workspace
                {
                    Id = Guid.NewGuid(),
                    Name = "A",
                    Key = "Alpha",
                    Icon = ImageIcon.FromString("/icon")
                });
                db.Workspaces.Add(new Workspace { Id = Guid.NewGuid(), Name = "B", Key = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetWorkspaces(new Query<Workspace>().Where(x => x.Key.StartsWith("A")))
                .ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Key);
            Assert.Equal("/icon", result[0].Icon?.Uri?.ToString());
        }

        /// <summary>
        /// Verifies that a workspace is added only if it does not already exist in the database.
        /// </summary>
        [Fact]
        public void AddWorkspaceWhenNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddWorkspaceWhenNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var ws = new Workspace { Id = Guid.NewGuid(), Name = "A", Key = "Unique" };

            // act
            ModelHub.Add(ws);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Workspaces);
        }

        /// <summary>
        /// Verifies that adding a workspace with a key that differs only in case from an 
        /// existing workspace does not result in duplicate entries.
        /// </summary>
        [Fact]
        public void AddWorkspaceWhenKeyExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddWorkspaceWhenKeyExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var ws1 = new Workspace { Id = Guid.NewGuid(), Name = "A", Key = "Same" };
            var ws2 = new Workspace { Id = Guid.NewGuid(), Name = "B", Key = "same" }; // case-insensitive

            // act
            ModelHub.Add(ws1);
            ModelHub.Add(ws2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Workspaces);
        }

        /// <summary>
        /// Removes an existing workspace from the database and verifies that it has been deleted.
        /// </summary>
        [Fact]
        public void RemoveExistingWorkspace()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveExistingWorkspace",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();
            var workspace = new Workspace { Id = id, Name = "A", Key = "X" };
            db.Workspaces.Add(workspace);
            db.SaveChanges();

            // act
            ModelHub.Remove(workspace);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.Workspaces);
        }

        /// <summary>
        /// Verifies that removing a workspace that does not exist in the database does not 
        /// result in an error and leaves the workspace collection empty.
        /// </summary>
        [Fact]
        public void RemoveWhenWorkspaceNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveWhenWorkspaceNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            // act
            ModelHub.Remove(new Workspace { RawId = 1, Id = id });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Workspaces);
        }

        /// <summary>
        /// Updates an existing workspace in the database and verifies that the changes are persisted.
        /// </summary>
        [Fact]
        public void UpdateExistingWorkspace()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "UpdateExistingWorkspace",
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();
            var workspace = new Workspace { Id = Guid.NewGuid(), Name = "Original", Key = "K" };
            db.Workspaces.Add(workspace);
            db.SaveChanges();

            // act
            workspace.Name = "Updated";
            ModelHub.Update(workspace);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Equal("Updated", db2.Workspaces.Single().Name);
        }

    }
}
