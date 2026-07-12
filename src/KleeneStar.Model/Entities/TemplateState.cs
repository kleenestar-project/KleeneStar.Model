using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a template.
    /// </summary>
    public enum TemplateState
    {
        /// <summary>
        /// Indicates that the template is active and available for use in object creation.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the template is archived and no longer available for use.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for working with the TemplateState enumeration.
    /// </summary>
    public static class TemplateStateExtensions
    {
        /// <summary>
        /// Determines whether the specified state indicates the template is active.
        /// </summary>
        /// <param name="state">The template state to evaluate.</param>
        /// <returns>True if the template is active; otherwise, false.</returns>
        public static bool IsActive(this TemplateState state)
        {
            return state == TemplateState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified template state.
        /// </summary>
        /// <param name="state">The template state for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified template state.</returns>
        public static Guid Id(this TemplateState state)
        {
            return state switch
            {
                TemplateState.Active => Guid.Parse("A1B2C3D4-E5F6-47A8-9BCC-D0E1F2A3B4C5"),
                TemplateState.Archived => Guid.Parse("F7A8B9C0-D1E2-43F4-A5B6-C7D8E9F0A1B2"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the resource key label associated with the specified template state.
        /// </summary>
        /// <param name="state">The template state for which to retrieve the resource key label.</param>
        /// <returns>A string containing the resource key label, or null if not recognized.</returns>
        public static string Text(this TemplateState state)
        {
            return state switch
            {
                TemplateState.Active => "kleenestar.core:state.active.label",
                TemplateState.Archived => "kleenestar.core:state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the CSS class name associated with the specified template state.
        /// </summary>
        /// <param name="state">The template state for which to retrieve the CSS class name.</param>
        /// <returns>A string containing the CSS class name.</returns>
        public static string Color(this TemplateState state)
        {
            return state switch
            {
                TemplateState.Active => TypeColorSelection.Success.ToClass(),
                TemplateState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
