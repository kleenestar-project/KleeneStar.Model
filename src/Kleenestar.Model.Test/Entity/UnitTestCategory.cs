using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Category class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestCategory
    {
        /// <summary>
        /// Verifies that a new Workspace instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var category = new Category();

            // validation
            Assert.NotEqual(Guid.Empty, category.Id);
        }

        /// <summary>
        /// Sets the properties of a Workspace instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Category A", "Description A")]
        [InlineData("Category B", null)]
        public void SetProperties(string name, string description)
        {
            // arrange
            var category = new Category();

            // act
            category.Name = name;
            category.Description = description;

            // validation
            Assert.Equal(name, category.Name);
            Assert.Equal(description, category.Description);
        }
    }
}
