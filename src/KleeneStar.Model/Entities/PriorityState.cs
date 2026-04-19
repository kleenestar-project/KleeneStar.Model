using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a priority, indicating whether it is active or deleted.
    /// </summary>
    public enum PriorityState
    {
        /// <summary>
        /// Indicates that the priority is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the PriorityState enumeration.
    /// </summary>
    public static class PriorityStateExtensions
    {
        /// <summary>
        /// Determines whether the specified priority state is active.
        /// </summary>
        /// <param name="state">The priority state to check.</param>
        /// <returns><c>true</c> if the priority state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this PriorityState state)
        {
            return state == PriorityState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified priority state.
        /// </summary>
        /// <remarks>Use this method to obtain a consistent identifier for each defined <see
        /// cref="PriorityState"/> value. The returned GUID is determined by the input state.</remarks>
        /// <param name="state">The priority state for which to retrieve the unique identifier.</param>
        /// <returns>
        /// A <see cref="Guid"/> representing the unique identifier for the specified priority state.
        /// </returns>
        public static Guid Id(this PriorityState state)
        {
            return state switch
            {
                PriorityState.Active => Guid.Parse("B93530CE-3BD0-4F04-A641-23A5F906345D"),
                PriorityState.Archived => Guid.Parse("360BEE13-2286-42E4-8B98-F22564288448"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified priority state.
        /// </summary>
        /// <remarks>
        /// For known values of <c>PriorityState</c>, a predefined text label is returned.
        /// For unknown or unsupported values, <c>null</c> is returned.
        /// </remarks>
        /// <param name="state">
        /// The priority state for which the text label should be retrieved.
        /// </param>
        /// <returns>
        /// A string containing the text label for the specified state; otherwise <c>null</c>
        /// if the state is not supported.
        /// </returns>
        public static string Text(this PriorityState state)
        {
            return state switch
            {
                PriorityState.Active => "kleenestar.core:state.active.label",
                PriorityState.Archived => "kleenestar.core:state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified priority state.
        /// </summary>
        /// <param name="state">The priority state for which to retrieve the corresponding color selection.</param>
        /// <returns>
        /// A <see cref="TypeColorSelection"/> value that represents the color selection for the given priority state.
        /// </returns>
        public static string Color(this PriorityState state)
        {
            return state switch
            {
                PriorityState.Active => TypeColorSelection.Success.ToClass(),
                PriorityState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
