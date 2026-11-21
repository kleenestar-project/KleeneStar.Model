using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the <see cref="Sprint"/> entity and the
    /// <see cref="SprintStateExtensions"/>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestSprint
    {
        /// <summary>
        /// Verifies that the default constructor assigns a fresh unique identifier.
        /// </summary>
        [Fact]
        public void DefaultConstructor_AssignsId()
        {
            var sprint = new Sprint();

            Assert.NotEqual(Guid.Empty, sprint.Id);
        }

        /// <summary>
        /// Verifies that the id constructor keeps the supplied identifier.
        /// </summary>
        [Fact]
        public void IdConstructor_KeepsId()
        {
            var id = Guid.NewGuid();

            var sprint = new Sprint(id);

            Assert.Equal(id, sprint.Id);
        }

        /// <summary>
        /// Verifies that every sprint state maps to the REST status string the scrum
        /// controls expect.
        /// </summary>
        /// <param name="state">The state under test.</param>
        /// <param name="expected">The expected REST status string.</param>
        [Theory]
        [InlineData(SprintState.Planned, "planned")]
        [InlineData(SprintState.Active, "active")]
        [InlineData(SprintState.Completed, "closed")]
        public void Code_ReturnsRestStatus(SprintState state, string expected)
        {
            Assert.Equal(expected, state.Code());
        }

        /// <summary>
        /// Verifies that the REST status strings parse back into the matching state and
        /// that unknown values fall back to <see cref="SprintState.Planned"/>.
        /// </summary>
        /// <param name="code">The REST status string under test.</param>
        /// <param name="expected">The expected state.</param>
        [Theory]
        [InlineData("planned", SprintState.Planned)]
        [InlineData("active", SprintState.Active)]
        [InlineData("closed", SprintState.Completed)]
        [InlineData("completed", SprintState.Completed)]
        [InlineData("ACTIVE", SprintState.Active)]
        [InlineData("garbage", SprintState.Planned)]
        [InlineData(null, SprintState.Planned)]
        public void FromCode_ParsesRestStatus(string code, SprintState expected)
        {
            Assert.Equal(expected, SprintStateExtensions.FromCode(code));
        }

        /// <summary>
        /// Verifies that Code and FromCode round-trip for every state.
        /// </summary>
        /// <param name="state">The state under test.</param>
        [Theory]
        [InlineData(SprintState.Planned)]
        [InlineData(SprintState.Active)]
        [InlineData(SprintState.Completed)]
        public void Code_FromCode_RoundTrip(SprintState state)
        {
            Assert.Equal(state, SprintStateExtensions.FromCode(state.Code()));
        }
    }
}
