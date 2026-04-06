using KleeneStar.Model.Entities;
using KleeneStarObject = KleeneStar.Model.Entities.Object;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Object class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestObject
    {
        /// <summary>
        /// Verifies that a new Object instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var obj = new KleeneStarObject();

            // validation
            Assert.NotEqual(Guid.Empty, obj.Id);
        }

        /// <summary>
        /// Sets the properties of an Object instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("key-1", "Summary A", "Description A", TypeWorkspaceState.Active)]
        [InlineData("key-2", null, null, TypeWorkspaceState.Archived)]
        public void SetProperties(string key, string summary, string description, TypeWorkspaceState state)
        {
            // arrange
            var obj = new KleeneStarObject();

            // act
            obj.Key = key;
            obj.Summary = summary;
            obj.Description = description;
            obj.State = state;

            // validation
            Assert.Equal(key, obj.Key);
            Assert.Equal(summary, obj.Summary);
            Assert.Equal(description, obj.Description);
            Assert.Equal(state, obj.State);
        }
    }
}
