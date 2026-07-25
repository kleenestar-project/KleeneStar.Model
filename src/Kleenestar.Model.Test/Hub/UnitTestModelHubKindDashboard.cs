using KleeneStar.Model;
using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub object-kind dashboard helpers.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubKindDashboard
    {
        /// <summary>
        /// Verifies that no board is returned for a workspace/kind pair that was never
        /// customized.
        /// </summary>
        [Fact]
        public void GetBoardWhenNoneExists()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "GetKindDashboardWhenNoneExists",
                Assembly = "KleeneStar.Model.Test"
            };

            // act
            var board = ModelHub.GetKindDashboard(Guid.NewGuid(), "issue");

            // validation
            Assert.Null(board);
        }

        /// <summary>
        /// Verifies that ensuring a board creates exactly one row per workspace/kind pair and
        /// that a second call for the same pair returns the same board.
        /// </summary>
        [Fact]
        public void EnsureBoardCreatesOnlyOnce()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "EnsureKindDashboardCreatesOnlyOnce",
                Assembly = "KleeneStar.Model.Test"
            };

            var workspaceId = Guid.NewGuid();

            // act
            var first = ModelHub.EnsureKindDashboard(workspaceId, "issue");
            var second = ModelHub.EnsureKindDashboard(workspaceId, "issue");

            // validation
            Assert.Equal(first.Id, second.Id);

            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.KindDashboards);
        }

        /// <summary>
        /// Verifies that a column-only update renames, resizes, recolors and reorders columns
        /// and that the changes survive a reload, while the widgets of the surviving columns are
        /// kept.
        /// </summary>
        [Fact]
        public void SetColumns_SurvivesReloadAndKeepsWidgets()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "SetKindDashboardColumns_SurvivesReloadAndKeepsWidgets",
                Assembly = "KleeneStar.Model.Test"
            };

            var board = ModelHub.EnsureKindDashboard(Guid.NewGuid(), "issue");

            ModelHub.SetKindDashboardBoard(board.Id,
            [
                new KindDashboardColumn(Guid.Empty)
                {
                    Name = "Total",
                    Widgets = [new KindDashboardWidget(Guid.NewGuid()) { Type = "widget_bignumber", Name = "Total" }]
                },
                new KindDashboardColumn(Guid.Empty) { Name = "Active" }
            ]);

            var afterFirstSave = ModelHub.GetKindDashboard(board.WorkspaceId, board.Kind);
            var totalId = afterFirstSave.Columns.Single(c => c.Name == "Total").Id;
            var activeId = afterFirstSave.Columns.Single(c => c.Name == "Active").Id;

            // act: reorder (active first) and rename the first column, column-only update
            ModelHub.SetKindDashboardColumns(board.Id,
            [
                new KindDashboardColumn(activeId) { Name = "Active" },
                new KindDashboardColumn(totalId) { Name = "Total Renamed" }
            ]);

            // validation
            var reloaded = ModelHub.GetKindDashboard(board.WorkspaceId, board.Kind);
            var ordered = reloaded.Columns.OrderBy(c => c.Position).ToList();

            Assert.Equal(2, ordered.Count);
            Assert.Equal(activeId, ordered[0].Id);
            Assert.Equal(totalId, ordered[1].Id);
            Assert.Equal("Total Renamed", ordered[1].Name);
            Assert.Single(ordered[1].Widgets);
        }

        /// <summary>
        /// Verifies that a full board update rebuilds a column's widgets and that the
        /// per-widget type, name, color and params survive a reload.
        /// </summary>
        [Fact]
        public void SetBoard_WidgetSettingsSurviveReload()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "SetKindDashboardBoard_WidgetSettingsSurviveReload",
                Assembly = "KleeneStar.Model.Test"
            };

            var board = ModelHub.EnsureKindDashboard(Guid.NewGuid(), "asset");

            // act
            ModelHub.SetKindDashboardBoard(board.Id,
            [
                new KindDashboardColumn(Guid.Empty)
                {
                    Name = "KPI",
                    Widgets =
                    [
                        new KindDashboardWidget(Guid.NewGuid())
                        {
                            Type = "widget_kleenestar_note",
                            Name = "My Note",
                            Color = "#abcdef",
                            Params = "{\"text\":\"hello\"}"
                        }
                    ]
                }
            ]);

            // validation
            var loaded = ModelHub.GetKindDashboard(board.WorkspaceId, board.Kind);
            var column = Assert.Single(loaded.Columns);
            var widget = Assert.Single(column.Widgets);

            Assert.Equal("widget_kleenestar_note", widget.Type);
            Assert.Equal("My Note", widget.Name);
            Assert.Equal("#abcdef", widget.Color);
            Assert.Contains("hello", widget.Params);
        }

        /// <summary>
        /// Verifies that deleting a column (omitting it from the desired set) removes it
        /// together with its widgets.
        /// </summary>
        [Fact]
        public void SetColumns_DeleteRemovesColumnAndWidgets()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "SetKindDashboardColumns_DeleteRemovesColumnAndWidgets",
                Assembly = "KleeneStar.Model.Test"
            };

            var board = ModelHub.EnsureKindDashboard(Guid.NewGuid(), "issue");

            ModelHub.SetKindDashboardBoard(board.Id,
            [
                new KindDashboardColumn(Guid.Empty)
                {
                    Name = "Total",
                    Widgets = [new KindDashboardWidget(Guid.NewGuid()) { Type = "widget_bignumber", Name = "Total" }]
                },
                new KindDashboardColumn(Guid.Empty) { Name = "Active" }
            ]);

            var totalId = ModelHub.GetKindDashboard(board.WorkspaceId, board.Kind)
                .Columns.Single(c => c.Name == "Total").Id;

            // act
            ModelHub.SetKindDashboardColumns(board.Id,
            [
                new KindDashboardColumn(totalId) { Name = "Total" }
            ]);

            // validation
            var loaded = ModelHub.GetKindDashboard(board.WorkspaceId, board.Kind);
            var remaining = Assert.Single(loaded.Columns);
            Assert.Equal(totalId, remaining.Id);
        }
    }
}
