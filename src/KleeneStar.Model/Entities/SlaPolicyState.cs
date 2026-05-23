using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the lifecycle state of an <see cref="SlaPolicy"/>.
    /// </summary>
    public enum SlaPolicyState
    {
        /// <summary>
        /// Indicates that the policy is not yet active and does not bind any tickets.
        /// </summary>
        Draft,

        /// <summary>
        /// Indicates that the policy is fully configured, visible, and currently being enforced.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the policy has been retired and is no longer being enforced.
        /// </summary>
        Inactive,

        /// <summary>
        /// Indicates that the policy is archived for historical reference and no longer modifiable.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for the <see cref="SlaPolicyState"/> enumeration.
    /// </summary>
    public static class SlaPolicyStateExtensions
    {
        /// <summary>
        /// Determines whether the specified policy state is currently being enforced.
        /// </summary>
        /// <param name="state">The policy state to check.</param>
        /// <returns><c>true</c> if the state is <see cref="SlaPolicyState.Active"/>; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this SlaPolicyState state)
        {
            return state == SlaPolicyState.Active;
        }

        /// <summary>
        /// Returns a stable unique identifier for the specified policy state.
        /// </summary>
        /// <param name="state">The policy state.</param>
        /// <returns>A <see cref="Guid"/> representing the policy state.</returns>
        public static Guid Id(this SlaPolicyState state)
        {
            return state switch
            {
                SlaPolicyState.Draft => Guid.Parse("9D2F9BE7-4B7C-4A0E-8D3F-1A2B3C4D5E6F"),
                SlaPolicyState.Active => Guid.Parse("B5A9F1E2-3C4D-4F5E-9A6B-7C8D9E0F1A2B"),
                SlaPolicyState.Inactive => Guid.Parse("C6B0F2E3-4D5E-5A6F-AB7C-8D9E0F1A2B3C"),
                SlaPolicyState.Archived => Guid.Parse("D7C1F3E4-5E6F-6B7A-BC8D-9E0F1A2B3C4D"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the localized text key for the specified policy state.
        /// </summary>
        /// <param name="state">The policy state.</param>
        /// <returns>A translation key, or <c>null</c> when the state is unknown.</returns>
        public static string Text(this SlaPolicyState state)
        {
            return state switch
            {
                SlaPolicyState.Draft => "kleenestar.core:sla.state.draft.label",
                SlaPolicyState.Active => "kleenestar.core:sla.state.active.label",
                SlaPolicyState.Inactive => "kleenestar.core:sla.state.inactive.label",
                SlaPolicyState.Archived => "kleenestar.core:sla.state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color class associated with the specified policy state.
        /// </summary>
        /// <param name="state">The policy state.</param>
        /// <returns>The CSS color-selection class.</returns>
        public static string Color(this SlaPolicyState state)
        {
            return state switch
            {
                SlaPolicyState.Draft => TypeColorSelection.Warning.ToClass(),
                SlaPolicyState.Active => TypeColorSelection.Success.ToClass(),
                SlaPolicyState.Inactive => TypeColorSelection.Secondary.ToClass(),
                SlaPolicyState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
