using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the state of an identity.
    /// </summary>
    public enum IdentityState
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity is active.
        /// </summary>
        Active,

        /// <summary>
        /// Gets a value indicating whether the resource is currently locked and cannot be modified.
        /// </summary>
        Locked,

        /// <summary>
        /// Indicates that the feature or functionality is disabled.
        /// </summary>
        Disabled
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the IdentityState enumeration.
    /// </summary>
    public static class IdentityStateExtensions
    {
        /// <summary>
        /// Determines whether the specified identity state is active.
        /// </summary>
        /// <param name="state">The identity state to check.</param>
        /// <returns><c>true</c> if the identity state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this IdentityState state)
        {
            return state == IdentityState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified identity state.
        /// </summary>
        /// <remarks>Use this method to obtain a consistent identifier for each defined <see
        /// cref="IdentityState"/> value. The returned GUID is determined by the input state.</remarks>
        /// <param name="state">The identity state for which to retrieve the unique identifier.</param>
        /// <returns>
        /// A <see cref="Guid"/> representing the unique identifier for the specified identity state.
        /// </returns>
        public static Guid Id(this IdentityState state)
        {
            return state switch
            {
                IdentityState.Active => Guid.Parse("F8890536-0222-4371-889B-7C7EB2AC7C23"),
                IdentityState.Locked => Guid.Parse("F1402D2E-F34B-4120-9917-88A33D53F279"),
                IdentityState.Disabled => Guid.Parse("671DB602-8B79-4596-9609-E765DF7397A7"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified identity state.
        /// </summary>
        /// <remarks>
        /// For known values of <c>IdentityState</c>, a predefined text label is returned.
        /// For unknown or unsupported values, <c>null</c> is returned.
        /// </remarks>
        /// <param name="state">
        /// The identity state for which the text label should be retrieved.
        /// </param>
        /// <returns>
        /// A string containing the text label for the specified state; otherwise <c>null</c>
        /// if the state is not supported.
        /// </returns>
        public static string Text(this IdentityState state)
        {
            return state switch
            {
                IdentityState.Active => "kleenestar.core:state.active.label",
                IdentityState.Locked => "kleenestar.core:state.locked.label",
                IdentityState.Disabled => "kleenestar.core:state.disabled.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified identity state.
        /// </summary>
        /// <param name="state">The identity state for which to retrieve the corresponding color selection.</param>
        /// <returns>
        /// A <see cref="TypeColorSelection"/> value that represents the color selection for the given identity state.
        /// </returns>
        public static string Color(this IdentityState state)
        {
            return state switch
            {
                IdentityState.Active => TypeColorSelection.Success.ToClass(),
                IdentityState.Locked => TypeColorSelection.Warning.ToClass(),
                IdentityState.Disabled => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
