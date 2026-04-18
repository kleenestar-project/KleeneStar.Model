using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a class, indicating whether it is active or deleted.
    /// </summary>
    public enum ClassState
    {
        /// <summary>
        /// Indicates that the class is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the ClassState enumeration.
    /// </summary>
    public static class ClassStateExtensions
    {
        /// <summary>
        /// Determines whether the specified class state is active.
        /// </summary>
        /// <param name="state">The class state to check.</param>
        /// <returns><c>true</c> if the class state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this ClassState state)
        {
            return state == ClassState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified class state.
        /// </summary>
        /// <remarks>Use this method to obtain a consistent identifier for each defined <see
        /// cref="ClassState"/> value. The returned GUID is determined by the input state.</remarks>
        /// <param name="state">The class state for which to retrieve the unique identifier.</param>
        /// <returns>
        /// A <see cref="Guid"/> representing the unique identifier for the specified class state.
        /// </returns>
        public static Guid Id(this ClassState state)
        {
            return state switch
            {
                ClassState.Active => Guid.Parse("F732D35B-AF61-4E46-ABAE-26868A518367"),
                ClassState.Archived => Guid.Parse("0302997E-78CB-4CC5-8A1B-BD77B1227F07"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified class state.
        /// </summary>
        /// <remarks>
        /// For known values of <c>ClassState</c>, a predefined text label is returned.
        /// For unknown or unsupported values, <c>null</c> is returned.
        /// </remarks>
        /// <param name="state">
        /// The class state for which the text label should be retrieved.
        /// </param>
        /// <returns>
        /// A string containing the text label for the specified state; otherwise <c>null</c>
        /// if the state is not supported.
        /// </returns>
        public static string Text(this ClassState state)
        {
            return state switch
            {
                ClassState.Active => "kleenestar.core:state.active.label",
                ClassState.Archived => "kleenestar.core:state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified class state.
        /// </summary>
        /// <param name="state">The class state for which to retrieve the corresponding color selection.</param>
        /// <returns>
        /// A <see cref="TypeColorSelection"/> value that represents the color selection for the given class state.
        /// </returns>
        public static string Color(this ClassState state)
        {
            return state switch
            {
                ClassState.Active => TypeColorSelection.Success.ToClass(),
                ClassState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
