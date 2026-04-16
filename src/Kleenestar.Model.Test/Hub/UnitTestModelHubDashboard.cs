using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebIcon;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub dashboard.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubDashboard
    {
        /// <summary>
        /// Verifies that all dashboards can be retrieved from the database and that the expected
        /// number of dashboards is returned.
        /// </summary>
        [Fact]
        public void AllDashboards()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AllDashboards",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Dashboards.Add(new Dashboard { Id = Guid.NewGuid(), Name = "A" });
                db.Dashboards.Add(new Dashboard { Id = Guid.NewGuid(), Name = "B" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetDashboards(new Query<Dashboard>());

            // validation
            Assert.Equal(2, result.Count());
        }

        /// <summary>
        /// Verifies that the dashboard filtering functionality returns only dashboards matching
        /// the specified predicate.
        /// </summary>
        [Fact]
        public void FilteredDashboards()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "FilteredDashboards",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Dashboards.Add(new Dashboard
                {
                    Id = Guid.NewGuid(),
                    Name = "Alpha",
                    Icon = ImageIcon.FromString("/icon")
                });
                db.Dashboards.Add(new Dashboard { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetDashboards(new Query<Dashboard>().Where(x => x.Name.StartsWith("A")))
                .ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
            Assert.Equal("/icon", result[0].Icon?.Uri?.ToString());
        }

        /// <summary>
        /// Verifies that a dashboard is added only if it does not already exist in the database.
        /// </summary>
        [Fact]
        public void AddDashboardWhenNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddDashboardWhenNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var dashboard = new Dashboard { Id = Guid.NewGuid(), Name = "Unique" };

            // act
            ModelHub.Add(dashboard);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Dashboards);
        }

        /// <summary>
        /// Verifies that adding a dashboard with an identifier that already exists in the database
        /// does not result in duplicate entries.
        /// </summary>
        [Fact]
        public void AddDashboardWhenIdExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "AddDashboardWhenIdExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();
            var dashboard1 = new Dashboard { Id = id, Name = "Alpha" };
            var dashboard2 = new Dashboard { Id = id, Name = "Beta" }; // same ID

            // act
            ModelHub.Add(dashboard1);
            ModelHub.Add(dashboard2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Dashboards);
        }

        /// <summary>
        /// Removes an existing dashboard from the database and verifies that it has been deleted.
        /// </summary>
        [Fact]
        public void RemoveExistingDashboard()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveExistingDashboard",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();
            var dashboard = new Dashboard { Id = id, Name = "A" };
            db.Dashboards.Add(dashboard);
            db.SaveChanges();

            // act
            ModelHub.Remove(dashboard);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.Dashboards);
        }

        /// <summary>
        /// Verifies that removing a dashboard that does not exist in the database does not
        /// result in an error and leaves the dashboard collection empty.
        /// </summary>
        [Fact]
        public void RemoveWhenDashboardNotExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "RemoveWhenDashboardNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            // act
            ModelHub.Remove(new Dashboard { RawId = 1, Id = id });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Dashboards);
        }

        /// <summary>
        /// Updates an existing dashboard in the database and verifies that the changes are persisted.
        /// </summary>
        [Fact]
        public void UpdateExistingDashboard()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "UpdateExistingDashboard",
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();
            var dashboard = new Dashboard { Id = Guid.NewGuid(), Name = "Original" };
            db.Dashboards.Add(dashboard);
            db.SaveChanges();

            // act
            dashboard.Name = "Updated";
            ModelHub.Update(dashboard);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Equal("Updated", db2.Dashboards.Single().Name);
        }
    }
}
