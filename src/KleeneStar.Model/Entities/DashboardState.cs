using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a dashboard, indicating whether it is active or deleted.
    /// </summary>
    public enum DashboardState
    {
        /// <summary>
        /// Indicates that the dashboard is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the dashboard has been deleted and is no longer active or accessible.
        /// </summary>
        Deleted
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the DashboardState enumeration.
    /// </summary>
    public static class DashboardStateExtensions
    {
        /// <summary>
        /// Determines whether the specified dashboard state is active.
        /// </summary>
        /// <param name="state">The dashboard state to check.</param>
        /// <returns><c>true</c> if the dashboard state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this DashboardState state)
        {
            return state == DashboardState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified dashboard state.
        /// </summary>
        /// <param name="state">The dashboard state for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified dashboard state.</returns>
        public static Guid Id(this DashboardState state)
        {
            return state switch
            {
                DashboardState.Active => Guid.Parse("3A5F7C9E-B1D2-4E86-A3F7-C9E1B2D4A5F7"),
                DashboardState.Deleted => Guid.Parse("7C9E1B3A-D2F4-4A86-E5F7-B1C2D3E4F5A6"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified dashboard state.
        /// </summary>
        /// <param name="state">The dashboard state for which the text label should be retrieved.</param>
        /// <returns>A string containing the text label for the specified state; otherwise <c>null</c>.</returns>
        public static string Text(this DashboardState state)
        {
            return state switch
            {
                DashboardState.Active => "kleenestar.core:state.active.label",
                DashboardState.Deleted => "kleenestar.core:state.deleted.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified dashboard state.
        /// </summary>
        /// <param name="state">The dashboard state for which to retrieve the corresponding color selection.</param>
        /// <returns>A string representing the color class for the given dashboard state.</returns>
        public static string Color(this DashboardState state)
        {
            return state switch
            {
                DashboardState.Active => TypeColorSelection.Success.ToClass(),
                DashboardState.Deleted => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
