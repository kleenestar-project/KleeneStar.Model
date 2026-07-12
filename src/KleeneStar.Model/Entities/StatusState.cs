using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a status, indicating whether it is active or deleted.
    /// </summary>
    public enum StatusState
    {
        /// <summary>
        /// Indicates that the status is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the StatusState enumeration.
    /// </summary>
    public static class StatusStateExtensions
    {
        /// <summary>
        /// Determines whether the specified status state is active.
        /// </summary>
        /// <param name="state">The status state to check.</param>
        /// <returns><c>true</c> if the status state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this StatusState state)
        {
            return state == StatusState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified status state.
        /// </summary>
        /// <remarks>Use this method to obtain a consistent identifier for each defined <see
        /// cref="StatusState"/> value. The returned GUID is determined by the input state.</remarks>
        /// <param name="state">The status state for which to retrieve the unique identifier.</param>
        /// <returns>
        /// A <see cref="Guid"/> representing the unique identifier for the specified status state.
        /// </returns>
        public static Guid Id(this StatusState state)
        {
            return state switch
            {
                StatusState.Active => Guid.Parse("0757B8BB-4C27-4F53-BECC-44BBF6835D30"),
                StatusState.Archived => Guid.Parse("1C023703-7CF0-4538-B768-7535C53CE8DD"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified status state.
        /// </summary>
        /// <remarks>
        /// For known values of <c>StatusState</c>, a predefined text label is returned.
        /// For unknown or unsupported values, <c>null</c> is returned.
        /// </remarks>
        /// <param name="state">
        /// The status state for which the text label should be retrieved.
        /// </param>
        /// <returns>
        /// A string containing the text label for the specified state; otherwise <c>null</c>
        /// if the state is not supported.
        /// </returns>
        public static string Text(this StatusState state)
        {
            return state switch
            {
                StatusState.Active => "kleenestar.core:state.active.label",
                StatusState.Archived => "kleenestar.core:state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified status state.
        /// </summary>
        /// <param name="state">The status state for which to retrieve the corresponding color selection.</param>
        /// <returns>
        /// A <see cref="TypeColorSelection"/> value that represents the color selection for the given status state.
        /// </returns>
        public static string Color(this StatusState state)
        {
            return state switch
            {
                StatusState.Active => TypeColorSelection.Success.ToClass(),
                StatusState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
