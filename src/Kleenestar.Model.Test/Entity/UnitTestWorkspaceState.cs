using KleeneStar.Model.Converters;
using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the WorkspaceStateExtensions and WorkspaceStateConverter classes.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestWorkspaceState
    {
        /// <summary>
        /// Verifies that IsActive returns true for the Active state.
        /// </summary>
        [Fact]
        public void IsActive_ReturnsTrue_ForActiveState()
        {
            // act
            var result = WorkspaceState.Active.IsActive();

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
            var result = WorkspaceState.Archived.IsActive();

            // validation
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that each state returns a non-empty unique identifier.
        /// </summary>
        [Theory]
        [InlineData(WorkspaceState.Active)]
        [InlineData(WorkspaceState.Archived)]
        public void Id_ReturnsNonEmptyGuid(WorkspaceState state)
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
            var activeId = WorkspaceState.Active.Id();
            var archivedId = WorkspaceState.Archived.Id();

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
            var id1 = WorkspaceState.Active.Id();
            var id2 = WorkspaceState.Active.Id();

            // validation
            Assert.Equal(id1, id2);
        }

        /// <summary>
        /// Verifies that each state returns a non-null text label.
        /// </summary>
        [Theory]
        [InlineData(WorkspaceState.Active)]
        [InlineData(WorkspaceState.Archived)]
        public void Text_ReturnsNonNullString(WorkspaceState state)
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
        [InlineData(WorkspaceState.Active)]
        [InlineData(WorkspaceState.Archived)]
        public void Color_ReturnsNonNullString(WorkspaceState state)
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
            var converter = new WorkspaceStateConverter();

            // act
            var result = converter.FromRaw(null, typeof(WorkspaceState));

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
            var converter = new WorkspaceStateConverter();
            var rawValue = WorkspaceState.Active.Id().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(WorkspaceState));

            // validation
            Assert.Equal(WorkspaceState.Active, result);
        }

        /// <summary>
        /// Verifies that the converter returns Archived when the raw value is the Archived GUID.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsArchived_ForArchivedGuid()
        {
            // arrange
            var converter = new WorkspaceStateConverter();
            var rawValue = WorkspaceState.Archived.Id().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(WorkspaceState));

            // validation
            Assert.Equal(WorkspaceState.Archived, result);
        }

        /// <summary>
        /// Verifies that the converter returns the raw value when it is not a string.
        /// </summary>
        [Fact]
        public void Converter_FromRaw_ReturnsRawValue_WhenNotString()
        {
            // arrange
            var converter = new WorkspaceStateConverter();
            var rawValue = 42;

            // act
            var result = converter.FromRaw(rawValue, typeof(WorkspaceState));

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
            var converter = new WorkspaceStateConverter();
            var rawValue = Guid.NewGuid().ToString();

            // act
            var result = converter.FromRaw(rawValue, typeof(WorkspaceState));

            // validation
            Assert.Equal(WorkspaceState.Active, result);
        }

        /// <summary>
        /// Verifies that ToRaw returns the Active GUID for the Active state.
        /// </summary>
        [Fact]
        public void Converter_ToRaw_ReturnsActiveGuid_ForActiveState()
        {
            // arrange
            var converter = new WorkspaceStateConverter();

            // act
            var result = converter.ToRaw(WorkspaceState.Active, typeof(WorkspaceState));

            // validation
            Assert.Equal(WorkspaceState.Active.Id(), result);
        }

        /// <summary>
        /// Verifies that ToRaw returns the Archived GUID for the Archived state.
        /// </summary>
        [Fact]
        public void Converter_ToRaw_ReturnsArchivedGuid_ForArchivedState()
        {
            // arrange
            var converter = new WorkspaceStateConverter();

            // act
            var result = converter.ToRaw(WorkspaceState.Archived, typeof(WorkspaceState));

            // validation
            Assert.Equal(WorkspaceState.Archived.Id(), result);
        }
    }
}
