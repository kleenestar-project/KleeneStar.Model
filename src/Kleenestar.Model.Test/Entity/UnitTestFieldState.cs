using KleeneStar.Model.Converters;
using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the FieldStateExtensions and FieldStateConverter classes.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestFieldState
    {
        /// <summary>
        /// Verifies that IsActive returns true for the Active state.
        /// </summary>
        [Fact]
        public void IsActive_ReturnsTrue_ForActiveState()
        {
            // act
            var result = FieldState.Active.IsActive();

            // validation
            Assert.True(result);
        }

        /// <summary>
        /// Verifies that IsActive returns false for the Archived state.
        /// </summary>
        [Fact]
        public void IsActive_ReturnsFalse_ForArchivedState()
        {
            // act
            var result = FieldState.Archived.IsActive();

            // validation
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that each state returns a non-empty unique identifier.
        /// </summary>
        [Theory]
        [InlineData(FieldState.Active)]
        [InlineData(FieldState.Archived)]
        public void Id_ReturnsNonEmptyGuid(FieldState state)
        {
            // act
            var id = state.Id();

            // validation
            Assert.NotEqual(Guid.Empty, id);
        }

        /// <summary>
        /// Verifies that each state has a unique identifier.
        /// </summary>
        [Fact]
        public void Id_ReturnsUniqueGuids()
        {
            // act
            var activeId = FieldState.Active.Id();
            var archivedId = FieldState.Archived.Id();

            // validation
            Assert.NotEqual(activeId, archivedId);
        }

        /// <summary>
        /// Verifies that the Id method returns consistent values across multiple calls.
        /// </summary>
        [Fact]
        public void Id_ReturnsConsistentValue()
        {
            // act
            var id1 = FieldState.Active.Id();
            var id2 = FieldState.Active.Id();

            // validation
            Assert.Equal(id1, id2);
        }

        /// <summary>
        /// Verifies that each state returns a non-null text label.
        /// </summary>
        [Theory]
        [InlineData(FieldState.Active)]
        [InlineData(FieldState.Archived)]
        public void Text_ReturnsNonNullString(FieldState state)
        {
            // act
            var text = state.Text();

            // validation
            Assert.NotNull(text);
        }

        /// <summary>
        /// Verifies that each state returns a non-null color value.
        /// </summary>
        [Theory]
        [InlineData(FieldState.Active)]
        [InlineData(FieldState.Archived)]
        public void Color_ReturnsNonNullString(FieldState state)
        {
            // act
            var color = state.Color();

            // validation
            Assert.NotNull(color);
        }

        /// <summary>
        /// Verifies that the converter returns null when the raw value is null.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsNull_WhenRawValueIsNull()
        {
            // arrange
            var converter = new FieldStateConverter();

            // act
            var result = converter.FromRaw(null, typeof(FieldState));

            // validation
            Assert.Null(result);
        }

        /// <summary>
        /// Verifies that the converter returns Active when the raw value is the Active GUID.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsActive_ForActiveGuid()
        {
            // arrange
            var converter = new FieldStateConverter();
            var rawValue = FieldState.Active.Id().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(FieldState));

            // validation
            Assert.Equal(FieldState.Active, result);
        }

        /// <summary>
        /// Verifies that the converter returns Archived when the raw value is the Archived GUID.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsArchived_ForArchivedGuid()
        {
            // arrange
            var converter = new FieldStateConverter();
            var rawValue = FieldState.Archived.Id().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(FieldState));

            // validation
            Assert.Equal(FieldState.Archived, result);
        }

        /// <summary>
        /// Verifies that the converter returns the raw value when it is not a string.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsRawValue_WhenNotString()
        {
            // arrange
            var converter = new FieldStateConverter();
            var rawValue = 42;

            // act
            var result = converter.FromRaw(rawValue, typeof(FieldState));

            // validation
            Assert.Equal(42, result);
        }

        /// <summary>
        /// Verifies that the converter returns Active when the raw value is an unknown GUID.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsActive_ForUnknownGuid()
        {
            // arrange
            var converter = new FieldStateConverter();
            var rawValue = Guid.NewGuid().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(FieldState));

            // validation
            Assert.Equal(FieldState.Active, result);
        }

        /// <summary>
        /// Verifies that ToRaw returns the Active GUID for the Active state.
        /// </summary>
        [Fact]
        public void Converter_ToRaw_ReturnsActiveGuid_ForActiveState()
        {
            // arrange
            var converter = new FieldStateConverter();

            // act
            var result = converter.ToRaw(FieldState.Active, typeof(FieldState));

            // validation
            Assert.Equal(FieldState.Active.Id(), result);
        }

        /// <summary>
        /// Verifies that ToRaw returns the Archived GUID for the Archived state.
        /// </summary>
        [Fact]
        public void Converter_ToRaw_ReturnsArchivedGuid_ForArchivedState()
        {
            // arrange
            var converter = new FieldStateConverter();

            // act
            var result = converter.ToRaw(FieldState.Archived, typeof(FieldState));

            // validation
            Assert.Equal(FieldState.Archived.Id(), result);
        }
    }
}
