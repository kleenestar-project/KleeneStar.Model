using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the KanbanBoardColumn class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestKanbanBoardColumn
    {
        /// <summary>
        /// Verifies that a new KanbanBoardColumn instance is assigned a non-empty unique
        /// identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var column = new KanbanBoardColumn();

            // validation
            Assert.NotEqual(Guid.Empty, column.Id);
        }

        /// <summary>
        /// Sets the properties of a KanbanBoardColumn instance and verifies that the values are
        /// assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("To Do", "#3273A3", 0)]
        [InlineData("Done", null, 3)]
        public void SetProperties(string name, string color, int position)
        {
            // arrange
            var column = new KanbanBoardColumn();
            var categoryId = Guid.NewGuid();

            // act
            column.Name = name;
            column.Color = color;
            column.Position = position;
            column.CategoryId = categoryId;
            column.Key = "client-new-1";

            // validation
            Assert.Equal(name, column.Name);
            Assert.Equal(color, column.Color);
            Assert.Equal(position, column.Position);
            Assert.Equal(categoryId, column.CategoryId);
            Assert.Equal("client-new-1", column.Key);
        }

        /// <summary>
        /// Sets the board reference on a KanbanBoardColumn instance and verifies that the value
        /// is assigned correctly.
        /// </summary>
        [Fact]
        public void SetBoard()
        {
            // arrange
            var column = new KanbanBoardColumn();
            var board = new KanbanBoard { WorkspaceId = Guid.NewGuid(), Kind = "issue" };

            // act
            column.BoardId = board.Id;
            column.Board = board;

            // validation
            Assert.Equal(board.Id, column.BoardId);
            Assert.Equal(board, column.Board);
        }
    }
}
