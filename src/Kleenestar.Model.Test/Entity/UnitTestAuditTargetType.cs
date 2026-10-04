using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the tokens of the <see cref="AuditTargetType"/> enumeration.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestAuditTargetType
    {
        /// <summary>
        /// Verifies that no two target types share a token, so every token parses back to the
        /// member it came from.
        /// </summary>
        [Fact]
        public void TokensAreUniqueAndParseBack()
        {
            // arrange
            var types = Enum.GetValues<AuditTargetType>();

            // validation
            Assert.Equal(types.Length, types.Select(x => x.Token()).Distinct().Count());
            Assert.All(types, x => Assert.Equal(x, AuditTargetTypeExtensions.Parse(x.Token())));
        }
    }
}
