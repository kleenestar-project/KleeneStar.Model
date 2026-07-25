using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the KanbanBoard class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestKanbanBoard
    {
        /// <summary>
        /// Verifies that a new KanbanBoard instance is assigned a non-empty unique identifier
        /// upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var board = new KanbanBoard();

            // validation
            Assert.NotEqual(Guid.Empty, board.Id);
        }

        /// <summary>
        /// Initializes a KanbanBoard instance with a specific identifier and verifies that the
        /// value is assigned correctly.
        /// </summary>
        [Fact]
        public void InitializeWithId()
        {
            // arrange
            var id = Guid.NewGuid();

            // act
            var board = new KanbanBoard(id);

            // validation
            Assert.Equal(id, board.Id);
        }

        /// <summary>
        /// Sets the properties of a KanbanBoard instance and verifies that the values are
        /// assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("issue", "Name ~ \"Sales\"")]
        [InlineData("asset", null)]
        public void SetProperties(string kind, string filter)
        {
            // arrange
            var board = new KanbanBoard();
            var workspaceId = Guid.NewGuid();

            // act
            board.WorkspaceId = workspaceId;
            board.Kind = kind;
            board.Filter = filter;

            // validation
            Assert.Equal(workspaceId, board.WorkspaceId);
            Assert.Equal(kind, board.Kind);
            Assert.Equal(filter, board.Filter);
        }

        /// <summary>
        /// Sets the columns for the board and verifies that the collection is assigned correctly.
        /// </summary>
        [Fact]
        public void SetColumns()
        {
            // arrange
            var board = new KanbanBoard();
            var columns = new List<KanbanBoardColumn>
            {
                new KanbanBoardColumn { Name = "To Do" },
                new KanbanBoardColumn { Name = "Done" }
            };

            // act
            board.Columns = columns;

            // validation
            Assert.Equal(2, board.Columns.Count);
            Assert.Equal("To Do", board.Columns[0].Name);
            Assert.Equal("Done", board.Columns[1].Name);
        }

        /// <summary>
        /// Sets the swimlanes for the board and verifies that the collection is assigned correctly.
        /// </summary>
        [Fact]
        public void SetSwimlanes()
        {
            // arrange
            var board = new KanbanBoard();
            var swimlanes = new List<KanbanBoardSwimlane>
            {
                new KanbanBoardSwimlane { Name = "Bugs" },
                new KanbanBoardSwimlane { Name = "Features" }
            };

            // act
            board.Swimlanes = swimlanes;

            // validation
            Assert.Equal(2, board.Swimlanes.Count);
            Assert.Equal("Bugs", board.Swimlanes[0].Name);
            Assert.Equal("Features", board.Swimlanes[1].Name);
        }
    }
}
