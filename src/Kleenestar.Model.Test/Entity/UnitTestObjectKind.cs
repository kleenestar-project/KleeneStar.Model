using KleeneStar.Model.Entities;
using KleeneStarObject = KleeneStar.Model.Entities.Object;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the <see cref="ObjectKind"/> key catalogue and the kind
    /// property of the object entity.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestObjectKind
    {
        /// <summary>
        /// Verifies that a freshly constructed object carries the default kind (issue).
        /// </summary>
        [Fact]
        public void NewObject_DefaultsToIssueKind()
        {
            // act
            var obj = new KleeneStarObject();

            // validation
            Assert.Equal(ObjectKind.Issue, obj.Kind);
            Assert.Equal(ObjectKind.Default, obj.Kind);
        }

        /// <summary>
        /// Verifies the normalization rule: null, empty, and whitespace fall back to the
        /// default kind; known and unknown keys are trimmed and lower-cased so add-on
        /// kinds survive normalization.
        /// </summary>
        /// <param name="raw">The raw kind key to normalize.</param>
        /// <param name="expected">The expected normalized key.</param>
        [Theory]
        [InlineData(null, ObjectKind.Issue)]
        [InlineData("", ObjectKind.Issue)]
        [InlineData("   ", ObjectKind.Issue)]
        [InlineData("issue", ObjectKind.Issue)]
        [InlineData("Document", ObjectKind.Document)]
        [InlineData("  BLOG  ", ObjectKind.Blog)]
        [InlineData("Custom-Kind", "custom-kind")]
        public void Normalize_MapsRawKeys(string raw, string expected)
        {
            // act
            var normalized = ObjectKind.Normalize(raw);

            // validation
            Assert.Equal(expected, normalized);
        }

        /// <summary>
        /// Verifies that the well-known kind keys are distinct, lower-case, and stable —
        /// they are persisted, so a change would orphan existing rows.
        /// </summary>
        [Fact]
        public void WellKnownKeys_AreDistinctAndLowerCase()
        {
            // arrange
            var keys = new[] { ObjectKind.Document, ObjectKind.Blog, ObjectKind.Issue };

            // validation
            Assert.Equal(keys.Length, keys.Distinct().Count());
            Assert.All(keys, k => Assert.Equal(k.ToLowerInvariant(), k));
            Assert.Equal("document", ObjectKind.Document);
            Assert.Equal("blog", ObjectKind.Blog);
            Assert.Equal("issue", ObjectKind.Issue);
        }
    }
}
