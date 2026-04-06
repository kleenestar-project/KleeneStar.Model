using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Priority class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestPriority
    {
        /// <summary>
        /// Verifies that a new Priority instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var priority = new Priority();

            // validation
            Assert.NotEqual(Guid.Empty, priority.Id);
        }

        /// <summary>
        /// Sets the properties of a Priority instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Priority A", "Description A", TypeWorkspaceState.Active)]
        [InlineData("Priority B", null, TypeWorkspaceState.Archived)]
        public void SetProperties(string name, string description, TypeWorkspaceState state)
        {
            // arrange
            var priority = new Priority();

            // act
            priority.Name = name;
            priority.Description = description;
            priority.State = state;

            // validation
            Assert.Equal(name, priority.Name);
            Assert.Equal(description, priority.Description);
            Assert.Equal(state, priority.State);
        }
    }
}
