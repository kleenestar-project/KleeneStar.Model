using KleeneStar.Model;
using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub Kanban board helpers.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubKanban
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
                ConnectionString = "GetBoardWhenNoneExists",
                Assembly = "KleeneStar.Model.Test"
            };

            // act
            var board = ModelHub.GetKanbanBoard(Guid.NewGuid(), "issue");

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
                ConnectionString = "EnsureBoardCreatesOnlyOnce",
                Assembly = "KleeneStar.Model.Test"
            };

            var workspaceId = Guid.NewGuid();

            // act
            var first = ModelHub.EnsureKanbanBoard(workspaceId, "issue");
            var second = ModelHub.EnsureKanbanBoard(workspaceId, "issue");

            // validation
            Assert.Equal(first.Id, second.Id);

            using var db = ModelHub.CreateDbContext();
            Assert.Single(db.KanbanBoards);
        }

        /// <summary>
        /// Verifies that ensuring boards for different kinds of the same workspace creates
        /// separate boards.
        /// </summary>
        [Fact]
        public void EnsureBoardIsScopedByKind()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "EnsureBoardIsScopedByKind",
                Assembly = "KleeneStar.Model.Test"
            };

            var workspaceId = Guid.NewGuid();

            // act
            var issueBoard = ModelHub.EnsureKanbanBoard(workspaceId, "issue");
            var assetBoard = ModelHub.EnsureKanbanBoard(workspaceId, "asset");

            // validation
            Assert.NotEqual(issueBoard.Id, assetBoard.Id);

            using var db = ModelHub.CreateDbContext();
            Assert.Equal(2, db.KanbanBoards.Count());
        }

        /// <summary>
        /// Verifies that setting columns on a board creates the desired columns in the given
        /// order and links each to its category.
        /// </summary>
        [Fact]
        public void SetColumnsCreatesColumns()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "SetColumnsCreatesColumns",
                Assembly = "KleeneStar.Model.Test"
            };

            var board = ModelHub.EnsureKanbanBoard(Guid.NewGuid(), "issue");
            var categoryId = Guid.NewGuid();

            var columns = new List<KanbanBoardColumn>
            {
                new KanbanBoardColumn(Guid.Empty) { Name = "To Do", CategoryId = categoryId, Key = "c1" },
                new KanbanBoardColumn(Guid.Empty) { Name = "Done", Key = "c2" }
            };

            // act
            ModelHub.SetKanbanColumns(board.Id, columns);

            // validation
            var reloaded = ModelHub.GetKanbanBoard(board.WorkspaceId, board.Kind);
            var ordered = reloaded.Columns.OrderBy(c => c.Position).ToList();

            Assert.Equal(2, ordered.Count);
            Assert.Equal("To Do", ordered[0].Name);
            Assert.Equal(categoryId, ordered[0].CategoryId);
            Assert.Equal(0, ordered[0].Position);
            Assert.Equal("Done", ordered[1].Name);
            Assert.Equal(1, ordered[1].Position);
        }

        /// <summary>
        /// Verifies that a column re-submitted with its persisted business id is renamed and
        /// reordered in place rather than duplicated, while an omitted column is deleted.
        /// </summary>
        [Fact]
        public void SetColumnsReconcilesByIdAndDeletesOmitted()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "SetColumnsReconcilesByIdAndDeletesOmitted",
                Assembly = "KleeneStar.Model.Test"
            };

            var board = ModelHub.EnsureKanbanBoard(Guid.NewGuid(), "issue");

            ModelHub.SetKanbanColumns(board.Id,
            [
                new KanbanBoardColumn(Guid.Empty) { Name = "To Do", Key = "c1" },
                new KanbanBoardColumn(Guid.Empty) { Name = "Done", Key = "c2" }
            ]);

            var afterFirstSave = ModelHub.GetKanbanBoard(board.WorkspaceId, board.Kind);
            var toDoId = afterFirstSave.Columns.Single(c => c.Name == "To Do").Id;

            // act: rename the surviving column by its real id and drop the other
            ModelHub.SetKanbanColumns(board.Id,
            [
                new KanbanBoardColumn(toDoId) { Name = "Backlog" }
            ]);

            // validation
            var reloaded = ModelHub.GetKanbanBoard(board.WorkspaceId, board.Kind);
            var column = Assert.Single(reloaded.Columns);

            Assert.Equal(toDoId, column.Id);
            Assert.Equal("Backlog", column.Name);
        }

        /// <summary>
        /// Verifies that setting swimlanes on a board creates the desired swimlanes in the
        /// given order and links each to its class.
        /// </summary>
        [Fact]
        public void SetSwimlanesCreatesSwimlanes()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "SetSwimlanesCreatesSwimlanes",
                Assembly = "KleeneStar.Model.Test"
            };

            var board = ModelHub.EnsureKanbanBoard(Guid.NewGuid(), "issue");
            var classId = Guid.NewGuid();

            var swimlanes = new List<KanbanBoardSwimlane>
            {
                new KanbanBoardSwimlane(Guid.Empty) { Name = "Bugs", ClassId = classId, Key = "s1" }
            };

            // act
            ModelHub.SetKanbanSwimlanes(board.Id, swimlanes);

            // validation
            var reloaded = ModelHub.GetKanbanBoard(board.WorkspaceId, board.Kind);
            var swimlane = Assert.Single(reloaded.Swimlanes);

            Assert.Equal("Bugs", swimlane.Name);
            Assert.Equal(classId, swimlane.ClassId);
            Assert.Equal(0, swimlane.Position);
        }

        /// <summary>
        /// Verifies that the board-level filter can be set and cleared.
        /// </summary>
        [Fact]
        public void SetFilterUpdatesBoard()
        {
            // arrange
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = "SetFilterUpdatesBoard",
                Assembly = "KleeneStar.Model.Test"
            };

            var board = ModelHub.EnsureKanbanBoard(Guid.NewGuid(), "issue");

            // act
            ModelHub.SetKanbanFilter(board.Id, "Priority = \"P1\"");

            // validation
            Assert.Equal("Priority = \"P1\"", ModelHub.GetKanbanBoard(board.WorkspaceId, board.Kind)?.Filter);

            // act: clear
            ModelHub.SetKanbanFilter(board.Id, null);

            // validation
            Assert.Null(ModelHub.GetKanbanBoard(board.WorkspaceId, board.Kind)?.Filter);
        }
    }
}
