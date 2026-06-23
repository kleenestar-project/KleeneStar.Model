using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a saved search, indicating whether it is active or deleted.
    /// </summary>
    /// <remarks>
    /// Persisted as an integer ordinal — new values must be appended at the end so the
    /// ordinals of existing rows are not shifted.
    /// </remarks>
    public enum SavedSearchState
    {
        /// <summary>
        /// Indicates that the saved search is visible and runnable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the saved search has been deleted and is no longer visible.
        /// </summary>
        Deleted
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the
    /// <see cref="SavedSearchState"/> enumeration.
    /// </summary>
    public static class SavedSearchStateExtensions
    {
        /// <summary>
        /// Determines whether the specified saved-search state is active.
        /// </summary>
        /// <param name="state">The saved-search state to check.</param>
        /// <returns><c>true</c> if the state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this SavedSearchState state)
        {
            return state == SavedSearchState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified saved-search state.
        /// </summary>
        /// <param name="state">The saved-search state for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified state.</returns>
        public static Guid Id(this SavedSearchState state)
        {
            return state switch
            {
                SavedSearchState.Active => Guid.Parse("2E4D6C8A-0B1F-4C3D-9E5A-7B8C9D0E1F23"),
                SavedSearchState.Deleted => Guid.Parse("9F8E7D6C-5B4A-4392-8170-6E5D4C3B2A19"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified saved-search state.
        /// </summary>
        /// <param name="state">The saved-search state for which the text label should be retrieved.</param>
        /// <returns>A string containing the text label for the specified state; otherwise <c>null</c>.</returns>
        public static string Text(this SavedSearchState state)
        {
            return state switch
            {
                SavedSearchState.Active => "kleenestar.core:state.active.label",
                SavedSearchState.Deleted => "kleenestar.core:state.deleted.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified saved-search state.
        /// </summary>
        /// <param name="state">The saved-search state for which to retrieve the corresponding color selection.</param>
        /// <returns>A string representing the color class for the given state.</returns>
        public static string Color(this SavedSearchState state)
        {
            return state switch
            {
                SavedSearchState.Active => TypeColorSelection.Success.ToClass(),
                SavedSearchState.Deleted => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
