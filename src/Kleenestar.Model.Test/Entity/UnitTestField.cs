using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Field class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestField
    {
        /// <summary>
        /// Verifies that a new Field instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var field = new Field();

            // validation
            Assert.NotEqual(Guid.Empty, field.Id);
        }

        /// <summary>
        /// Sets the properties of a Field instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Field A", "Description A", TypeWorkspaceState.Active)]
        [InlineData("Field B", null, TypeWorkspaceState.Archived)]
        public void SetProperties(string name, string description, TypeWorkspaceState state)
        {
            // arrange
            var field = new Field();

            // act
            field.Name = name;
            field.Description = description;
            field.State = state;

            // validation
            Assert.Equal(name, field.Name);
            Assert.Equal(description, field.Description);
            Assert.Equal(state, field.State);
        }
    }
}
