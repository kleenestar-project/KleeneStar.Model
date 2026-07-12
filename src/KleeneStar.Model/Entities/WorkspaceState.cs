using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a workspace type, indicating whether it is active or archived.
    /// </summary>
    public enum WorkspaceState
    {
        /// <summary>
        /// Indicates whether the entity is currently active.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the WorkspaceState enumeration.
    /// </summary>
    public static class WorkspaceStateExtensions
    {
        /// <summary>
        /// Determines whether the specified workspace state is active.
        /// </summary>
        /// <param name="state">The workspace state to check.</param>
        /// <returns><c>true</c> if the workspace state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this WorkspaceState state)
        {
            return state == WorkspaceState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified workspace state.
        /// </summary>
        /// <param name="state">The workspace state for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified workspace state.</returns>
        public static Guid Id(this WorkspaceState state)
        {
            return state switch
            {
                WorkspaceState.Active => Guid.Parse("6E53CE64-5195-4C1E-89F7-369C4ACC00E6"),
                WorkspaceState.Archived => Guid.Parse("ABCF5156-520A-4F9C-934F-0EF3CD32742B"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified workspace state.
        /// </summary>
        /// <param name="state">The workspace state for which the text label should be retrieved.</param>
        /// <returns>A string containing the text label for the specified state; otherwise <c>null</c>.</returns>
        public static string Text(this WorkspaceState state)
        {
            return state switch
            {
                WorkspaceState.Active => "kleenestar.core:state.active.label",
                WorkspaceState.Archived => "kleenestar.core:state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified workspace state.
        /// </summary>
        /// <param name="state">The workspace state for which to retrieve the corresponding color selection.</param>
        /// <returns>A string representing the color class for the given workspace state.</returns>
        public static string Color(this WorkspaceState state)
        {
            return state switch
            {
                WorkspaceState.Active => TypeColorSelection.Success.ToClass(),
                WorkspaceState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
