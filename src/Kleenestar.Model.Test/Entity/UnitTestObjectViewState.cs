using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for <see cref="ObjectViewStateExtensions"/>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestObjectViewState
    {
        /// <summary>
        /// Verifies that each state returns a non-empty well-known identifier.
        /// </summary>
        /// <param name="state">The state under test.</param>
        [Theory]
        [InlineData(ObjectViewState.Active)]
        [InlineData(ObjectViewState.Archived)]
        public void Id_ReturnsNonEmptyGuid(ObjectViewState state)
        {
            Assert.NotEqual(Guid.Empty, state.Id());
        }

        /// <summary>
        /// Verifies that the two states carry different identifiers.
        /// </summary>
        [Fact]
        public void Id_ReturnsUniqueGuids()
        {
            Assert.NotEqual(ObjectViewState.Active.Id(), ObjectViewState.Archived.Id());
        }

        /// <summary>
        /// Verifies that each state returns a non-null text label.
        /// </summary>
        /// <param name="state">The state under test.</param>
        [Theory]
        [InlineData(ObjectViewState.Active)]
        [InlineData(ObjectViewState.Archived)]
        public void Text_ReturnsNonNullString(ObjectViewState state)
        {
            Assert.NotNull(state.Text());
        }

        /// <summary>
        /// Verifies that each state returns a non-null color value.
        /// </summary>
        /// <param name="state">The state under test.</param>
        [Theory]
        [InlineData(ObjectViewState.Active)]
        [InlineData(ObjectViewState.Archived)]
        public void Color_ReturnsNonNullString(ObjectViewState state)
        {
            Assert.NotNull(state.Color());
        }
    }
}
