using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Widget class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestWidget
    {
        /// <summary>
        /// Verifies that a new Widget instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var widget = new Widget();

            // validation
            Assert.NotEqual(Guid.Empty, widget.Id);
        }

        /// <summary>
        /// Sets the properties of a Widget instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Revenue Chart", "SELECT revenue FROM objects WHERE type = 'revenue'")]
        [InlineData("Region Filter", null)]
        public void SetProperties(string name, string wql)
        {
            // arrange
            var widget = new Widget();

            // act
            widget.Name = name;
            widget.Wql = wql;

            // validation
            Assert.Equal(name, widget.Name);
            Assert.Equal(wql, widget.Wql);
        }

        /// <summary>
        /// Sets the column reference on a Widget instance and verifies that the value is assigned correctly.
        /// </summary>
        [Fact]
        public void SetColumn()
        {
            // arrange
            var widget = new Widget();
            var column = new DashboardColumn { Name = "Main Column" };

            // act
            widget.ColumnId = column.Id;
            widget.Column = column;

            // validation
            Assert.Equal(column.Id, widget.ColumnId);
            Assert.Equal(column, widget.Column);
        }
    }
}
