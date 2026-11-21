using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a navigator link, indicating whether it is shown in the app navigator.
    /// </summary>
    public enum NavigatorLinkState
    {
        /// <summary>
        /// Indicates that the link is shown in the app navigator.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the link is retained but hidden from the app navigator.
        /// </summary>
        Hidden
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the NavigatorLinkState enumeration.
    /// </summary>
    public static class NavigatorLinkStateExtensions
    {
        /// <summary>
        /// Determines whether the specified navigator link state is active.
        /// </summary>
        /// <param name="state">The navigator link state to check.</param>
        /// <returns><c>true</c> if the navigator link state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this NavigatorLinkState state)
        {
            return state == NavigatorLinkState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified navigator link state.
        /// </summary>
        /// <param name="state">The navigator link state for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified state.</returns>
        public static Guid Id(this NavigatorLinkState state)
        {
            return state switch
            {
                NavigatorLinkState.Active => Guid.Parse("7B2E9C41-4D8A-4F16-9C3E-1A5D6B8F2E70"),
                NavigatorLinkState.Hidden => Guid.Parse("3F6A1D89-2C7B-4E05-8B14-9D2E4C7A6F31"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified navigator link state.
        /// </summary>
        /// <param name="state">The navigator link state for which the text label should be retrieved.</param>
        /// <returns>A string containing the text label for the specified state; otherwise <c>null</c>.</returns>
        public static string Text(this NavigatorLinkState state)
        {
            return state switch
            {
                NavigatorLinkState.Active => "kleenestar.core:state.active.label",
                NavigatorLinkState.Hidden => "kleenestar.core:state.hidden.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified navigator link state.
        /// </summary>
        /// <param name="state">The navigator link state for which to retrieve the corresponding color selection.</param>
        /// <returns>A string representing the color class for the given navigator link state.</returns>
        public static string Color(this NavigatorLinkState state)
        {
            return state switch
            {
                NavigatorLinkState.Active => TypeColorSelection.Success.ToClass(),
                NavigatorLinkState.Hidden => TypeColorSelection.Secondary.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
