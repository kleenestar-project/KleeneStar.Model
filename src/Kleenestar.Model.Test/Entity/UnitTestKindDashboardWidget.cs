using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the KindDashboardWidget class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestKindDashboardWidget
    {
        /// <summary>
        /// Verifies that a new KindDashboardWidget instance is assigned a non-empty unique
        /// identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var widget = new KindDashboardWidget();

            // validation
            Assert.NotEqual(Guid.Empty, widget.Id);
        }

        /// <summary>
        /// Sets the properties of a KindDashboardWidget instance and verifies that the values
        /// are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("widget_bignumber", "Total", "{\"value\":\"42\"}")]
        [InlineData("widget_info", "Note", null)]
        public void SetProperties(string type, string name, string parameters)
        {
            // arrange
            var widget = new KindDashboardWidget();

            // act
            widget.Type = type;
            widget.Name = name;
            widget.Color = "#3273A3";
            widget.Params = parameters;
            widget.Position = 2;

            // validation
            Assert.Equal(type, widget.Type);
            Assert.Equal(name, widget.Name);
            Assert.Equal("#3273A3", widget.Color);
            Assert.Equal(parameters, widget.Params);
            Assert.Equal(2, widget.Position);
        }

        /// <summary>
        /// Sets the column reference on a KindDashboardWidget instance and verifies that the
        /// value is assigned correctly.
        /// </summary>
        [Fact]
        public void SetColumn()
        {
            // arrange
            var widget = new KindDashboardWidget();
            var column = new KindDashboardColumn { Name = "Total" };

            // act
            widget.ColumnId = column.Id;
            widget.Column = column;

            // validation
            Assert.Equal(column.Id, widget.ColumnId);
            Assert.Equal(column, widget.Column);
        }
    }
}
