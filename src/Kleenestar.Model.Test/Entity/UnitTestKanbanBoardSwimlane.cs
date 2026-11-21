using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the KanbanBoardSwimlane class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestKanbanBoardSwimlane
    {
        /// <summary>
        /// Verifies that a new KanbanBoardSwimlane instance is assigned a non-empty unique
        /// identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var swimlane = new KanbanBoardSwimlane();

            // validation
            Assert.NotEqual(Guid.Empty, swimlane.Id);
        }

        /// <summary>
        /// Sets the properties of a KanbanBoardSwimlane instance and verifies that the values
        /// are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Bugs", "#3273A3", "Priority = \"P1\"", 0)]
        [InlineData("Features", null, null, 2)]
        public void SetProperties(string name, string color, string filter, int position)
        {
            // arrange
            var swimlane = new KanbanBoardSwimlane();
            var classId = Guid.NewGuid();

            // act
            swimlane.Name = name;
            swimlane.Color = color;
            swimlane.Filter = filter;
            swimlane.Position = position;
            swimlane.ClassId = classId;
            swimlane.Key = "client-new-1";

            // validation
            Assert.Equal(name, swimlane.Name);
            Assert.Equal(color, swimlane.Color);
            Assert.Equal(filter, swimlane.Filter);
            Assert.Equal(position, swimlane.Position);
            Assert.Equal(classId, swimlane.ClassId);
            Assert.Equal("client-new-1", swimlane.Key);
        }

        /// <summary>
        /// Sets the board reference on a KanbanBoardSwimlane instance and verifies that the
        /// value is assigned correctly.
        /// </summary>
        [Fact]
        public void SetBoard()
        {
            // arrange
            var swimlane = new KanbanBoardSwimlane();
            var board = new KanbanBoard { WorkspaceId = Guid.NewGuid(), Kind = "issue" };

            // act
            swimlane.BoardId = board.Id;
            swimlane.Board = board;

            // validation
            Assert.Equal(board.Id, swimlane.BoardId);
            Assert.Equal(board, swimlane.Board);
        }
    }
}
