using KleeneStar.Model.Entities;
using WebExpress.WebUI.WebIcon;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Dashboard class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestDashboard
    {
        /// <summary>
        /// Verifies that a new Dashboard instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var dashboard = new Dashboard();

            // validation
            Assert.NotEqual(Guid.Empty, dashboard.Id);
        }

        /// <summary>
        /// Sets the properties of a Dashboard instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Dashboard A", DashboardState.Active, "Description A")]
        [InlineData("Dashboard B", DashboardState.Deleted, null)]
        public void SetProperties(string name, DashboardState state, string description)
        {
            // arrange
            var dashboard = new Dashboard();
            var icon = ImageIcon.FromString("/icon");

            // act
            dashboard.Name = name;
            dashboard.State = state;
            dashboard.Description = description;
            dashboard.Icon = icon;

            // validation
            Assert.Equal(name, dashboard.Name);
            Assert.Equal(state, dashboard.State);
            Assert.Equal(description, dashboard.Description);
            Assert.Equal(icon, dashboard.Icon);
        }

        /// <summary>
        /// Sets the categories for the dashboard using the specified category names.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("A", "B", "C")]
        public void SetCategories(params string[] categories)
        {
            // arrange
            var dashboard = new Dashboard();
            var categoriesList = categories?.Select(name => new Category { Name = name })
                .ToList() ?? [];

            // act
            dashboard.Categories = categoriesList;

            // validation
            Assert.Equal(categoriesList, dashboard.Categories);
        }

        /// <summary>
        /// Sets the columns for the dashboard and verifies that the collection is assigned correctly.
        /// </summary>
        [Fact]
        public void SetColumns()
        {
            // arrange
            var dashboard = new Dashboard();
            var columns = new List<DashboardColumn>
            {
                new DashboardColumn { Name = "Column A", Size = "small" },
                new DashboardColumn { Name = "Column B" }
            };

            // act
            dashboard.Columns = columns;

            // validation
            Assert.Equal(2, dashboard.Columns.Count);
            Assert.Equal("Column A", dashboard.Columns[0].Name);
            Assert.Equal("Column B", dashboard.Columns[1].Name);
        }
    }
}
