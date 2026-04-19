using KleeneStar.Model.Converters;
using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the IdentityStateExtensions and IdentityStateConverter classes.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestIdentityState
    {
        /// <summary>
        /// Verifies that IsActive returns true for the Active state.
        /// </summary>
        [Fact]
        public void IsActive_ReturnsTrue_ForActiveState()
        {
            // act
            var result = IdentityState.Active.IsActive();

            // validation
            Assert.True(result);
        }

        /// <summary>
        /// Verifies that IsActive returns false for the Locked state.
        /// </summary>
        [Fact]
        public void IsActive_ReturnsFalse_ForLockedState()
        {
            // act
            var result = IdentityState.Locked.IsActive();

            // validation
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that IsActive returns false for the Disabled state.
        /// </summary>
        [Fact]
        public void IsActive_ReturnsFalse_ForDisabledState()
        {
            // act
            var result = IdentityState.Disabled.IsActive();

            // validation
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that each state returns a non-empty unique identifier.
        /// </summary>
        [Theory]
        [InlineData(IdentityState.Active)]
        [InlineData(IdentityState.Locked)]
        [InlineData(IdentityState.Disabled)]
        public void Id_ReturnsNonEmptyGuid(IdentityState state)
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
            var activeId = IdentityState.Active.Id();
            var lockedId = IdentityState.Locked.Id();
            var disabledId = IdentityState.Disabled.Id();

            // validation
            Assert.NotEqual(activeId, lockedId);
            Assert.NotEqual(activeId, disabledId);
            Assert.NotEqual(lockedId, disabledId);
        }

        /// <summary>
        /// Verifies that the Id method returns consistent values across multiple calls.
        /// </summary>
        [Fact]
        public void Id_ReturnsConsistentValue()
        {
            // act
            var id1 = IdentityState.Active.Id();
            var id2 = IdentityState.Active.Id();

            // validation
            Assert.Equal(id1, id2);
        }

        /// <summary>
        /// Verifies that each state returns a non-null text label.
        /// </summary>
        [Theory]
        [InlineData(IdentityState.Active)]
        [InlineData(IdentityState.Locked)]
        [InlineData(IdentityState.Disabled)]
        public void Text_ReturnsNonNullString(IdentityState state)
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
        [InlineData(IdentityState.Active)]
        [InlineData(IdentityState.Locked)]
        [InlineData(IdentityState.Disabled)]
        public void Color_ReturnsNonNullString(IdentityState state)
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
            var converter = new IdentityStateConverter();

            // act
            var result = converter.FromRaw(null, typeof(IdentityState));

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
            var converter = new IdentityStateConverter();
            var rawValue = IdentityState.Active.Id().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(IdentityState));

            // validation
            Assert.Equal(IdentityState.Active, result);
        }

        /// <summary>
        /// Verifies that the converter returns Locked when the raw value is the Locked GUID.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsLocked_ForLockedGuid()
        {
            // arrange
            var converter = new IdentityStateConverter();
            var rawValue = IdentityState.Locked.Id().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(IdentityState));

            // validation
            Assert.Equal(IdentityState.Locked, result);
        }

        /// <summary>
        /// Verifies that the converter returns Disabled when the raw value is the Disabled GUID.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsDisabled_ForDisabledGuid()
        {
            // arrange
            var converter = new IdentityStateConverter();
            var rawValue = IdentityState.Disabled.Id().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(IdentityState));

            // validation
            Assert.Equal(IdentityState.Disabled, result);
        }

        /// <summary>
        /// Verifies that the converter returns the raw value when it is not a string.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsRawValue_WhenNotString()
        {
            // arrange
            var converter = new IdentityStateConverter();
            var rawValue = 42;

            // act
            var result = converter.FromRaw(rawValue, typeof(IdentityState));

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
            var converter = new IdentityStateConverter();
            var rawValue = Guid.NewGuid().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(IdentityState));

            // validation
            Assert.Equal(IdentityState.Active, result);
        }

        /// <summary>
        /// Verifies that ToRaw returns the Active GUID for the Active state.
        /// </summary>
        [Fact]
        public void Converter_ToRaw_ReturnsActiveGuid_ForActiveState()
        {
            // arrange
            var converter = new IdentityStateConverter();

            // act
            var result = converter.ToRaw(IdentityState.Active, typeof(IdentityState));

            // validation
            Assert.Equal(IdentityState.Active.Id(), result);
        }

        /// <summary>
        /// Verifies that ToRaw returns the Locked GUID for the Locked state.
        /// </summary>
        [Fact]
        public void Converter_ToRaw_ReturnsLockedGuid_ForLockedState()
        {
            // arrange
            var converter = new IdentityStateConverter();

            // act
            var result = converter.ToRaw(IdentityState.Locked, typeof(IdentityState));

            // validation
            Assert.Equal(IdentityState.Locked.Id(), result);
        }

        /// <summary>
        /// Verifies that ToRaw returns the Disabled GUID for the Disabled state.
        /// </summary>
        [Fact]
        public void Converter_ToRaw_ReturnsDisabledGuid_ForDisabledState()
        {
            // arrange
            var converter = new IdentityStateConverter();

            // act
            var result = converter.ToRaw(IdentityState.Disabled, typeof(IdentityState));

            // validation
            Assert.Equal(IdentityState.Disabled.Id(), result);
        }
    }
}
