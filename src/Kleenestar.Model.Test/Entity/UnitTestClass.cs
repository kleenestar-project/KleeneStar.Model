using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Class class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestClass
    {
        /// <summary>
        /// Verifies that a new Class instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var classEntry = new Class();

            // validation
            Assert.NotEqual(Guid.Empty, classEntry.Id);
        }

        /// <summary>
        /// Sets the properties of a Class instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Class A", "Description A", TypeWorkspaceState.Active)]
        [InlineData("Class B", null, TypeWorkspaceState.Archived)]
        public void SetProperties(string name, string description, TypeWorkspaceState state)
        {
            // arrange
            var classEntry = new Class();

            // act
            classEntry.Name = name;
            classEntry.Description = description;
            classEntry.State = state;

            // validation
            Assert.Equal(name, classEntry.Name);
            Assert.Equal(description, classEntry.Description);
            Assert.Equal(state, classEntry.State);
        }
    }
}
