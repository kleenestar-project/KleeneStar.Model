using KleeneStar.Model.Entities;
using WebExpress.WebUI.WebIcon;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Insight class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestInsight
    {
        /// <summary>
        /// Verifies that a new Insight instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var insight = new Insight();

            // validation
            Assert.NotEqual(Guid.Empty, insight.Id);
        }

        /// <summary>
        /// Verifies that a new insight is a dashboard unless told otherwise - the type every
        /// former dashboard carries and the one a create naming no type falls back to.
        /// </summary>
        [Fact]
        public void DefaultsToDashboardType()
        {
            // act
            var insight = new Insight();

            // validation
            Assert.Equal("dashboard", Insight.DashboardType);
            Assert.Equal(Insight.DashboardType, insight.Type);
        }

        /// <summary>
        /// Sets the properties of an Insight instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Insight A", InsightState.Active, "Description A")]
        [InlineData("Insight B", InsightState.Deleted, null)]
        public void SetProperties(string name, InsightState state, string description)
        {
            // arrange
            var insight = new Insight();
            var icon = ImageIcon.FromString("/icon");

            // act
            insight.Name = name;
            insight.State = state;
            insight.Description = description;
            insight.Icon = icon;

            // validation
            Assert.Equal(name, insight.Name);
            Assert.Equal(state, insight.State);
            Assert.Equal(description, insight.Description);
            Assert.Equal(icon, insight.Icon);
        }

        /// <summary>
        /// Sets the categories for the insight using the specified category names.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("A", "B", "C")]
        public void SetCategories(params string[] categories)
        {
            // arrange
            var insight = new Insight();
            var categoriesList = categories?.Select(name => new Category { Name = name })
                .ToList() ?? [];

            // act
            insight.Categories = categoriesList;

            // validation
            Assert.Equal(categoriesList, insight.Categories);
        }

        /// <summary>
        /// Sets the columns for the insight and verifies that the collection is assigned correctly.
        /// </summary>
        [Fact]
        public void SetColumns()
        {
            // arrange
            var insight = new Insight();
            var columns = new List<DashboardColumn>
            {
                new DashboardColumn { Name = "Column A", Size = "small" },
                new DashboardColumn { Name = "Column B" }
            };

            // act
            insight.Columns = columns;

            // validation
            Assert.Equal(2, insight.Columns.Count);
            Assert.Equal("Column A", insight.Columns[0].Name);
            Assert.Equal("Column B", insight.Columns[1].Name);
        }
    }
}
