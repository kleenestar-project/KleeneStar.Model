using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the KindDashboardColumn class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestKindDashboardColumn
    {
        /// <summary>
        /// Verifies that a new KindDashboardColumn instance is assigned a non-empty unique
        /// identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var column = new KindDashboardColumn();

            // validation
            Assert.NotEqual(Guid.Empty, column.Id);
        }

        /// <summary>
        /// Sets the properties of a KindDashboardColumn instance and verifies that the values
        /// are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Total", "33%")]
        [InlineData("Active", null)]
        public void SetProperties(string name, string size)
        {
            // arrange
            var column = new KindDashboardColumn();

            // act
            column.Name = name;
            column.Size = size;
            column.Color = "#3273A3";
            column.Position = 1;
            column.Key = "client-new-1";

            // validation
            Assert.Equal(name, column.Name);
            Assert.Equal(size, column.Size);
            Assert.Equal("#3273A3", column.Color);
            Assert.Equal(1, column.Position);
            Assert.Equal("client-new-1", column.Key);
        }

        /// <summary>
        /// Sets the board reference on a KindDashboardColumn instance and verifies that the
        /// value is assigned correctly.
        /// </summary>
        [Fact]
        public void SetBoard()
        {
            // arrange
            var column = new KindDashboardColumn();
            var board = new KindDashboard { WorkspaceId = Guid.NewGuid(), Kind = "issue" };

            // act
            column.BoardId = board.Id;
            column.Board = board;

            // validation
            Assert.Equal(board.Id, column.BoardId);
            Assert.Equal(board, column.Board);
        }

        /// <summary>
        /// Sets the widgets for the column and verifies that the collection is assigned correctly.
        /// </summary>
        [Fact]
        public void SetWidgets()
        {
            // arrange
            var column = new KindDashboardColumn();
            var widgets = new List<KindDashboardWidget>
            {
                new KindDashboardWidget { Type = "widget_bignumber", Name = "Total" },
                new KindDashboardWidget { Type = "widget_info", Name = "Info" }
            };

            // act
            column.Widgets = widgets;

            // validation
            Assert.Equal(2, column.Widgets.Count);
            Assert.Equal("Total", column.Widgets[0].Name);
            Assert.Equal("Info", column.Widgets[1].Name);
        }
    }
}
