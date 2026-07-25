using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the KindDashboard class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestKindDashboard
    {
        /// <summary>
        /// Verifies that a new KindDashboard instance is assigned a non-empty unique identifier
        /// upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var board = new KindDashboard();

            // validation
            Assert.NotEqual(Guid.Empty, board.Id);
        }

        /// <summary>
        /// Sets the properties of a KindDashboard instance and verifies that the values are
        /// assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("issue")]
        [InlineData("asset")]
        public void SetProperties(string kind)
        {
            // arrange
            var board = new KindDashboard();
            var workspaceId = Guid.NewGuid();

            // act
            board.WorkspaceId = workspaceId;
            board.Kind = kind;

            // validation
            Assert.Equal(workspaceId, board.WorkspaceId);
            Assert.Equal(kind, board.Kind);
        }

        /// <summary>
        /// Sets the columns for the board and verifies that the collection is assigned correctly.
        /// </summary>
        [Fact]
        public void SetColumns()
        {
            // arrange
            var board = new KindDashboard();
            var columns = new List<KindDashboardColumn>
            {
                new KindDashboardColumn { Name = "Total" },
                new KindDashboardColumn { Name = "Active" }
            };

            // act
            board.Columns = columns;

            // validation
            Assert.Equal(2, board.Columns.Count);
            Assert.Equal("Total", board.Columns[0].Name);
            Assert.Equal("Active", board.Columns[1].Name);
        }
    }
}
