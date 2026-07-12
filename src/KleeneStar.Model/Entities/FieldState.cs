using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a field, indicating whether it is active or deleted.
    /// </summary>
    public enum FieldState
    {
        /// <summary>
        /// Indicates that the field is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the FieldState enumeration.
    /// </summary>
    public static class FieldStateExtensions
    {
        /// <summary>
        /// Determines whether the specified field state is active.
        /// </summary>
        /// <param name="state">The field state to check.</param>
        /// <returns><c>true</c> if the field state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this FieldState state)
        {
            return state == FieldState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified field state.
        /// </summary>
        /// <remarks>Use this method to obtain a consistent identifier for each defined <see
        /// cref="FieldState"/> value. The returned GUID is determined by the input state.</remarks>
        /// <param name="state">The field state for which to retrieve the unique identifier.</param>
        /// <returns>
        /// A <see cref="Guid"/> representing the unique identifier for the specified field state.
        /// </returns>
        public static Guid Id(this FieldState state)
        {
            return state switch
            {
                FieldState.Active => Guid.Parse("71B050B5-CA09-4274-B6B2-A88FD3A38112"),
                FieldState.Archived => Guid.Parse("4627A26F-CB8A-45D5-8ECA-170491C3A52F"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified field state.
        /// </summary>
        /// <remarks>
        /// For known values of <c>FieldState</c>, a predefined text label is returned.
        /// For unknown or unsupported values, <c>null</c> is returned.
        /// </remarks>
        /// <param name="state">
        /// The field state for which the text label should be retrieved.
        /// </param>
        /// <returns>
        /// A string containing the text label for the specified state; otherwise <c>null</c>
        /// if the state is not supported.
        /// </returns>
        public static string Text(this FieldState state)
        {
            return state switch
            {
                FieldState.Active => "kleenestar.core:state.active.label",
                FieldState.Archived => "kleenestar.core:state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified field state.
        /// </summary>
        /// <param name="state">The field state for which to retrieve the corresponding color selection.</param>
        /// <returns>
        /// A <see cref="TypeColorSelection"/> value that represents the color selection for the given field state.
        /// </returns>
        public static string Color(this FieldState state)
        {
            return state switch
            {
                FieldState.Active => TypeColorSelection.Success.ToClass(),
                FieldState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
