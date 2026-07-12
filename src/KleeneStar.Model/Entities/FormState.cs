using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a form, indicating whether it is active or deleted.
    /// </summary>
    public enum FormState
    {
        /// <summary>
        /// Indicates that the form is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the FormState enumeration.
    /// </summary>
    public static class FormStateExtensions
    {
        /// <summary>
        /// Determines whether the specified form state is active.
        /// </summary>
        /// <param name="state">The form state to check.</param>
        /// <returns><c>true</c> if the form state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this FormState state)
        {
            return state == FormState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified form state.
        /// </summary>
        /// <remarks>Use this method to obtain a consistent identifier for each defined <see
        /// cref="FormState"/> value. The returned GUID is determined by the input state.</remarks>
        /// <param name="state">The form state for which to retrieve the unique identifier.</param>
        /// <returns>
        /// A <see cref="Guid"/> representing the unique identifier for the specified form state.
        /// </returns>
        public static Guid Id(this FormState state)
        {
            return state switch
            {
                FormState.Active => Guid.Parse("40192E1F-1018-476D-B4B7-E14F4005A682"),
                FormState.Archived => Guid.Parse("7F223B82-FFA0-4FED-997E-C4298D395202"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified form state.
        /// </summary>
        /// <remarks>
        /// For known values of <c>FormState</c>, a predefined text label is returned.
        /// For unknown or unsupported values, <c>null</c> is returned.
        /// </remarks>
        /// <param name="state">
        /// The form state for which the text label should be retrieved.
        /// </param>
        /// <returns>
        /// A string containing the text label for the specified state; otherwise <c>null</c>
        /// if the state is not supported.
        /// </returns>
        public static string Text(this FormState state)
        {
            return state switch
            {
                FormState.Active => "kleenestar.core:state.active.label",
                FormState.Archived => "kleenestar.core:state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified form state.
        /// </summary>
        /// <param name="state">The form state for which to retrieve the corresponding color selection.</param>
        /// <returns>
        /// A <see cref="TypeColorSelection"/> value that represents the color selection for the given form state.
        /// </returns>
        public static string Color(this FormState state)
        {
            return state switch
            {
                FormState.Active => TypeColorSelection.Success.ToClass(),
                FormState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
