using KleeneStar.Model.Entity;
using System.Reflection;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Workspace class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestWorkspace
    {
        /// <summary>
        /// Verifies that a new Workspace instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var workspace = new Workspace();

            // validation
            Assert.NotEqual(Guid.Empty, workspace.Id);
        }

        /// <summary>
        /// Sets the properties of a Workspace instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("key-1", "Workspace A", TypeWorkspaceState.Active, "Description A")]
        [InlineData("key-2", "Workspace B", TypeWorkspaceState.Archived, null)]
        public void SetProperties(string key, string name, TypeWorkspaceState state, string description)
        {
            // arrange
            var workspace = new Workspace();
            var icon = ImageIcon.FromString("/icon");

            // act
            workspace.Key = key;
            workspace.Name = name;
            workspace.State = state;
            workspace.Description = description;
            workspace.Icon = icon;

            // validation
            Assert.Equal(key, workspace.Key);
            Assert.Equal(name, workspace.Name);
            Assert.Equal(state, workspace.State);
            Assert.Equal(description, workspace.Description);
            Assert.Equal(icon, workspace.Icon);
        }

        /// <summary>
        /// Sets the categories for the workspace using the specified category names.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("A", "B", "C")]
        public void SetCategories(params string[] categories)
        {
            // arrange
            var workspace = new Workspace();
            var categoriesList = categories?.Select(name => new Category { Name = name })
                .ToList() ?? [];

            // act
            workspace.Categories = categoriesList;

            // validation
            Assert.Equal(categoriesList, workspace.Categories);
        }

        /// <summary>
        /// Verifies that the Id and Icon properties of the Workspace class are decorated with the
        /// RestTableColumnHiddenAttribute.
        /// </summary>
        [Fact]
        public void HiddenAttribute()
        {
            // act
            var idAttr = typeof(Workspace)
                .GetProperty(nameof(Workspace.Id))
                .GetCustomAttribute<RestTableColumnHiddenAttribute>();

            var iconAttr = typeof(Workspace)
                .GetProperty(nameof(Workspace.Icon))
                .GetCustomAttribute<RestTableColumnHiddenAttribute>();

            // validation
            Assert.NotNull(idAttr);
            Assert.NotNull(iconAttr);
        }
    }
}
