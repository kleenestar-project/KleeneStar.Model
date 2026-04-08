using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the DashboardColumn class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestDashboardColumn
    {
        /// <summary>
        /// Verifies that a new DashboardColumn instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var column = new DashboardColumn();

            // validation
            Assert.NotEqual(Guid.Empty, column.Id);
        }

        /// <summary>
        /// Sets the properties of a DashboardColumn instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Column A", "small")]
        [InlineData("Column B", null)]
        [InlineData("Column C", "large")]
        public void SetProperties(string name, string size)
        {
            // arrange
            var column = new DashboardColumn();

            // act
            column.Name = name;
            column.Size = size;

            // validation
            Assert.Equal(name, column.Name);
            Assert.Equal(size, column.Size);
        }

        /// <summary>
        /// Sets the dashboard reference on a DashboardColumn instance and verifies that the value is assigned correctly.
        /// </summary>
        [Fact]
        public void SetDashboard()
        {
            // arrange
            var column = new DashboardColumn();
            var dashboard = new Dashboard { Name = "My Dashboard" };

            // act
            column.DashboardId = dashboard.Id;
            column.Dashboard = dashboard;

            // validation
            Assert.Equal(dashboard.Id, column.DashboardId);
            Assert.Equal(dashboard, column.Dashboard);
        }

        /// <summary>
        /// Sets the widgets for the dashboard column and verifies that the collection is assigned correctly.
        /// </summary>
        [Fact]
        public void SetWidgets()
        {
            // arrange
            var column = new DashboardColumn();
            var widgets = new List<Widget>
            {
                new Widget { Name = "Widget A", Wql = "SELECT * FROM objects" },
                new Widget { Name = "Widget B", Wql = "SELECT * FROM classes" }
            };

            // act
            column.Widgets = widgets;

            // validation
            Assert.Equal(2, column.Widgets.Count);
            Assert.Equal("Widget A", column.Widgets[0].Name);
            Assert.Equal("Widget B", column.Widgets[1].Name);
        }
    }
}
