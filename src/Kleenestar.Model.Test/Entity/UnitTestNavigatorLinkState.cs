using KleeneStar.Model.Converters;
using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the NavigatorLinkStateExtensions and NavigatorLinkStateConverter classes.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestNavigatorLinkState
    {
        /// <summary>
        /// Verifies that IsActive returns true for the Active state.
        /// </summary>
        [Fact]
        public void IsActive_ReturnsTrue_ForActiveState()
        {
            // act
            var result = NavigatorLinkState.Active.IsActive();

            // validation
            Assert.True(result);
        }

        /// <summary>
        /// Verifies that IsActive returns false for the Hidden state, because the state decides
        /// whether the link appears in the app navigator.
        /// </summary>
        [Fact]
        public void IsActive_ReturnsFalse_ForHiddenState()
        {
            // act
            var result = NavigatorLinkState.Hidden.IsActive();

            // validation
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that each state returns a non-empty unique identifier.
        /// </summary>
        [Theory]
        [InlineData(NavigatorLinkState.Active)]
        [InlineData(NavigatorLinkState.Hidden)]
        public void Id_ReturnsNonEmptyGuid(NavigatorLinkState state)
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
            var activeId = NavigatorLinkState.Active.Id();
            var hiddenId = NavigatorLinkState.Hidden.Id();

            // validation
            Assert.NotEqual(activeId, hiddenId);
        }

        /// <summary>
        /// Verifies that the identifiers do not collide with those of the tenant states, since both
        /// travel through the same selection payloads.
        /// </summary>
        [Fact]
        public void Id_DoesNotCollideWithTenantState()
        {
            // act
            var ids = new[]
            {
                NavigatorLinkState.Active.Id(),
                NavigatorLinkState.Hidden.Id(),
                TenantState.Active.Id(),
                TenantState.Archived.Id()
            };

            // validation
            Assert.Equal(ids.Length, ids.Distinct().Count());
        }

        /// <summary>
        /// Verifies that the Id method returns consistent values across multiple calls.
        /// </summary>
        [Fact]
        public void Id_ReturnsConsistentValue()
        {
            // act
            var id1 = NavigatorLinkState.Active.Id();
            var id2 = NavigatorLinkState.Active.Id();

            // validation
            Assert.Equal(id1, id2);
        }

        /// <summary>
        /// Verifies that each state returns a non-null text label.
        /// </summary>
        [Theory]
        [InlineData(NavigatorLinkState.Active)]
        [InlineData(NavigatorLinkState.Hidden)]
        public void Text_ReturnsNonNullString(NavigatorLinkState state)
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
        [InlineData(NavigatorLinkState.Active)]
        [InlineData(NavigatorLinkState.Hidden)]
        public void Color_ReturnsNonNullString(NavigatorLinkState state)
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
            var converter = new NavigatorLinkStateConverter();

            // act
            var result = converter.FromRaw(null, typeof(NavigatorLinkState));

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
            var converter = new NavigatorLinkStateConverter();
            var rawValue = NavigatorLinkState.Active.Id().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(NavigatorLinkState));

            // validation
            Assert.Equal(NavigatorLinkState.Active, result);
        }

        /// <summary>
        /// Verifies that the converter returns Hidden when the raw value is the Hidden GUID.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsHidden_ForHiddenGuid()
        {
            // arrange
            var converter = new NavigatorLinkStateConverter();
            var rawValue = NavigatorLinkState.Hidden.Id().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(NavigatorLinkState));

            // validation
            Assert.Equal(NavigatorLinkState.Hidden, result);
        }

        /// <summary>
        /// Verifies that the converter reads the identifier out of a semicolon separated payload,
        /// which is the shape a selection control submits.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReadsSemicolonSeparatedPayload()
        {
            // arrange
            var converter = new NavigatorLinkStateConverter();
            var rawValue = $"{NavigatorLinkState.Hidden.Id()};";

            // act
            var result = converter.FromRaw(rawValue, typeof(NavigatorLinkState));

            // validation
            Assert.Equal(NavigatorLinkState.Hidden, result);
        }

        /// <summary>
        /// Verifies that the converter returns the raw value when it is not a string.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsRawValue_WhenNotString()
        {
            // arrange
            var converter = new NavigatorLinkStateConverter();
            var rawValue = 42;

            // act
            var result = converter.FromRaw(rawValue, typeof(NavigatorLinkState));

            // validation
            Assert.Equal(42, result);
        }

        /// <summary>
        /// Verifies that the converter falls back to Active when the raw value is an unknown GUID,
        /// so an unrecognized payload leaves the link usable rather than hiding it.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsActive_ForUnknownGuid()
        {
            // arrange
            var converter = new NavigatorLinkStateConverter();
            var rawValue = Guid.NewGuid().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(NavigatorLinkState));

            // validation
            Assert.Equal(NavigatorLinkState.Active, result);
        }

        /// <summary>
        /// Verifies that ToRaw returns the Active GUID for the Active state.
        /// </summary>
        [Fact]
        public void Converter_ToRaw_ReturnsActiveGuid_ForActiveState()
        {
            // arrange
            var converter = new NavigatorLinkStateConverter();

            // act
            var result = converter.ToRaw(NavigatorLinkState.Active, typeof(NavigatorLinkState));

            // validation
            Assert.Equal(NavigatorLinkState.Active.Id(), result);
        }

        /// <summary>
        /// Verifies that ToRaw returns the Hidden GUID for the Hidden state.
        /// </summary>
        [Fact]
        public void Converter_ToRaw_ReturnsHiddenGuid_ForHiddenState()
        {
            // arrange
            var converter = new NavigatorLinkStateConverter();

            // act
            var result = converter.ToRaw(NavigatorLinkState.Hidden, typeof(NavigatorLinkState));

            // validation
            Assert.Equal(NavigatorLinkState.Hidden.Id(), result);
        }

        /// <summary>
        /// Verifies that a state survives a conversion round trip in both directions.
        /// </summary>
        /// <param name="state">The state to round trip.</param>
        [Theory]
        [InlineData(NavigatorLinkState.Active)]
        [InlineData(NavigatorLinkState.Hidden)]
        public void Converter_RoundTrip_PreservesState(NavigatorLinkState state)
        {
            // arrange
            var converter = new NavigatorLinkStateConverter();

            // act
            var raw = converter.ToRaw(state, typeof(NavigatorLinkState));
            var result = converter.FromRaw(raw.ToString(), typeof(NavigatorLinkState));

            // validation
            Assert.Equal(state, result);
        }
    }
}
