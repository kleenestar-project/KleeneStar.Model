using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebIcon;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub insight.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubInsight
    {
        /// <summary>
        /// Verifies that all insights can be retrieved from the database and that the expected
        /// number of insights is returned.
        /// </summary>
        [Fact]
        public void AllInsights()
        {
            // arrange
            ModelHub.DatabaseSettings = new KleeneStar.Model.Settings.DatabaseSettings()
            {
                ConnectionString = "AllInsights",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Insights.Add(new Insight { Id = Guid.NewGuid(), Name = "A" });
                db.Insights.Add(new Insight { Id = Guid.NewGuid(), Name = "B" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetInsights(new Query<Insight>());

            // validation
            Assert.Equal(2, result.Count());
        }

        /// <summary>
        /// Verifies that the insight filtering functionality returns only insights matching
        /// the specified predicate.
        /// </summary>
        [Fact]
        public void FilteredInsights()
        {
            // arrange
            ModelHub.DatabaseSettings = new KleeneStar.Model.Settings.DatabaseSettings()
            {
                ConnectionString = "FilteredInsights",
                Assembly = "KleeneStar.Model.Test"
            };

            using (var db = ModelHub.CreateDbContext())
            {
                db.Insights.Add(new Insight
                {
                    Id = Guid.NewGuid(),
                    Name = "Alpha",
                    Icon = ImageIcon.FromString("/icon")
                });
                db.Insights.Add(new Insight { Id = Guid.NewGuid(), Name = "Beta" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetInsights(new Query<Insight>().Where(x => x.Name.StartsWith("A")))
                .ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
            Assert.Equal("/icon", result[0].Icon?.Uri?.ToString());
        }

        /// <summary>
        /// Verifies that an insight is added only if it does not already exist in the database.
        /// </summary>
        [Fact]
        public void AddInsightWhenNotExists()
        {
            // arrange
            ModelHub.DatabaseSettings = new KleeneStar.Model.Settings.DatabaseSettings()
            {
                ConnectionString = "AddInsightWhenNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var insight = new Insight { Id = Guid.NewGuid(), Name = "Unique" };

            // act
            ModelHub.Add(insight);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Insights);
        }

        /// <summary>
        /// Verifies that adding an insight with an identifier that already exists in the database
        /// does not result in duplicate entries.
        /// </summary>
        [Fact]
        public void AddInsightWhenIdExists()
        {
            // arrange
            ModelHub.DatabaseSettings = new KleeneStar.Model.Settings.DatabaseSettings()
            {
                ConnectionString = "AddInsightWhenIdExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();
            var insight1 = new Insight { Id = id, Name = "Alpha" };
            var insight2 = new Insight { Id = id, Name = "Beta" }; // same ID

            // act
            ModelHub.Add(insight1);
            ModelHub.Add(insight2);

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.Insights);
        }

        /// <summary>
        /// Removes an existing insight from the database and verifies that it has been deleted.
        /// </summary>
        [Fact]
        public void RemoveExistingInsight()
        {
            // arrange
            ModelHub.DatabaseSettings = new KleeneStar.Model.Settings.DatabaseSettings()
            {
                ConnectionString = "RemoveExistingInsight",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();
            var insight = new Insight { Id = id, Name = "A" };
            db.Insights.Add(insight);
            db.SaveChanges();

            // act
            ModelHub.Remove(insight);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Empty(db2.Insights);
        }

        /// <summary>
        /// Verifies that removing an insight that does not exist in the database does not
        /// result in an error and leaves the insight collection empty.
        /// </summary>
        [Fact]
        public void RemoveWhenInsightNotExists()
        {
            // arrange
            ModelHub.DatabaseSettings = new KleeneStar.Model.Settings.DatabaseSettings()
            {
                ConnectionString = "RemoveWhenInsightNotExists",
                Assembly = "KleeneStar.Model.Test"
            };

            var id = Guid.NewGuid();

            // act
            ModelHub.Remove(new Insight { RawId = 1, Id = id });

            // validation
            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Insights);
        }

        /// <summary>
        /// Updates an existing insight in the database and verifies that the changes are persisted.
        /// </summary>
        [Fact]
        public void UpdateExistingInsight()
        {
            // arrange
            ModelHub.DatabaseSettings = new KleeneStar.Model.Settings.DatabaseSettings()
            {
                ConnectionString = "UpdateExistingInsight",
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();
            var insight = new Insight { Id = Guid.NewGuid(), Name = "Original" };
            db.Insights.Add(insight);
            db.SaveChanges();

            // act
            insight.Name = "Updated";
            ModelHub.Update(insight);

            // validation
            using var db2 = ModelHub.CreateDbContext();
            Assert.Equal("Updated", db2.Insights.Single().Name);
        }

        /// <summary>
        /// Verifies that a board saved against an insight of the dashboard type becomes its columns
        /// and widgets.
        /// </summary>
        [Fact]
        public void SetDashboardBoardOnDashboardInsight()
        {
            // arrange
            ModelHub.DatabaseSettings = new KleeneStar.Model.Settings.DatabaseSettings()
            {
                ConnectionString = "SetDashboardBoardOnDashboardInsight",
                Assembly = "KleeneStar.Model.Test"
            };

            var insight = new Insight { Id = Guid.NewGuid(), Name = "Board", Type = Insight.DashboardType };
            ModelHub.Add(insight);

            // act
            ModelHub.SetDashboardBoard(insight.Id,
            [
                new DashboardColumn { Name = "A", Widgets = [new Widget { Name = "W", Type = "widget_info" }] }
            ]);

            // validation
            var column = Assert.Single(ModelHub.GetInsights(new Query<Insight>()).Single().Columns);
            Assert.Equal("A", column.Name);
            Assert.Single(column.Widgets);
        }

        /// <summary>
        /// Verifies that columns and widgets - the content of the dashboard type - are not written
        /// into an insight of another type.
        /// </summary>
        [Fact]
        public void SetDashboardBoardIgnoresOtherTypes()
        {
            // arrange
            ModelHub.DatabaseSettings = new KleeneStar.Model.Settings.DatabaseSettings()
            {
                ConnectionString = "SetDashboardBoardIgnoresOtherTypes",
                Assembly = "KleeneStar.Model.Test"
            };

            var insight = new Insight { Id = Guid.NewGuid(), Name = "Calendar", Type = "calendar" };
            ModelHub.Add(insight);

            // act
            ModelHub.SetDashboardBoard(insight.Id, [new DashboardColumn { Name = "A" }]);
            ModelHub.SetDashboardColumns(insight.Id, [new DashboardColumn { Name = "B" }]);

            // validation
            Assert.Empty(ModelHub.GetInsights(new Query<Insight>()).Single().Columns);
        }
    }
}
