using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a insight, indicating whether it is active or deleted.
    /// </summary>
    public enum InsightState
    {
        /// <summary>
        /// Indicates that the insight is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the insight has been deleted and is no longer active or accessible.
        /// </summary>
        Deleted
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the InsightState enumeration.
    /// </summary>
    public static class InsightStateExtensions
    {
        /// <summary>
        /// Determines whether the specified insight state is active.
        /// </summary>
        /// <param name="state">The insight state to check.</param>
        /// <returns><c>true</c> if the insight state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this InsightState state)
        {
            return state == InsightState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified insight state.
        /// </summary>
        /// <param name="state">The insight state for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified insight state.</returns>
        public static Guid Id(this InsightState state)
        {
            return state switch
            {
                InsightState.Active => Guid.Parse("3A5F7C9E-B1D2-4E86-A3F7-C9E1B2D4A5F7"),
                InsightState.Deleted => Guid.Parse("7C9E1B3A-D2F4-4A86-E5F7-B1C2D3E4F5A6"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified insight state.
        /// </summary>
        /// <param name="state">The insight state for which the text label should be retrieved.</param>
        /// <returns>A string containing the text label for the specified state; otherwise <c>null</c>.</returns>
        public static string Text(this InsightState state)
        {
            return state switch
            {
                InsightState.Active => "kleenestar.core:state.active.label",
                InsightState.Deleted => "kleenestar.core:state.deleted.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified insight state.
        /// </summary>
        /// <param name="state">The insight state for which to retrieve the corresponding color selection.</param>
        /// <returns>A string representing the color class for the given insight state.</returns>
        public static string Color(this InsightState state)
        {
            return state switch
            {
                InsightState.Active => TypeColorSelection.Success.ToClass(),
                InsightState.Deleted => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
