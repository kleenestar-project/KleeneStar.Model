using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the SecurityLevel class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestSecurityLevel
    {
        /// <summary>
        /// Verifies that a new SecurityLevel instance is assigned a non-empty unique identifier
        /// upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var securityLevel = new SecurityLevel();

            // validation
            Assert.NotEqual(Guid.Empty, securityLevel.Id);
        }

        /// <summary>
        /// Verifies that the supplied identifier is kept.
        /// </summary>
        [Fact]
        public void InitializeWithId()
        {
            // arrange
            var id = Guid.NewGuid();

            // act
            var securityLevel = new SecurityLevel(id);

            // validation
            Assert.Equal(id, securityLevel.Id);
        }

        /// <summary>
        /// Verifies that a fresh level clears nobody. The empty clearance is what closes a
        /// level, so it has to be the starting state rather than null.
        /// </summary>
        [Fact]
        public void InitializeClearanceEmpty()
        {
            // act
            var securityLevel = new SecurityLevel();

            // validation
            Assert.NotNull(securityLevel.PermittedGroupIds);
            Assert.Empty(securityLevel.PermittedGroupIds);
        }

        /// <summary>
        /// Sets the properties of a SecurityLevel instance and verifies that the values are
        /// assigned correctly.
        /// </summary>
        /// <param name="name">The name of the security level.</param>
        /// <param name="description">The description of the security level.</param>
        /// <param name="rank">The rank of the security level.</param>
        /// <param name="state">The state of the security level.</param>
        [Theory]
        [InlineData("Public", "Everyone", 0, SecurityLevelState.Active)]
        [InlineData("Confidential", null, 20, SecurityLevelState.Archived)]
        public void SetProperties(string name, string description, int rank, SecurityLevelState state)
        {
            // arrange
            var securityLevel = new SecurityLevel();

            // act
            securityLevel.Name = name;
            securityLevel.Description = description;
            securityLevel.Rank = rank;
            securityLevel.State = state;

            // validation
            Assert.Equal(name, securityLevel.Name);
            Assert.Equal(description, securityLevel.Description);
            Assert.Equal(rank, securityLevel.Rank);
            Assert.Equal(state, securityLevel.State);
        }

        /// <summary>
        /// Verifies that the clearance can be assigned.
        /// </summary>
        [Fact]
        public void SetClearance()
        {
            // arrange
            var securityLevel = new SecurityLevel();
            var group = Guid.NewGuid();

            // act
            securityLevel.PermittedGroupIds = [group];

            // validation
            Assert.Equal([group], securityLevel.PermittedGroupIds);
        }
    }
}
