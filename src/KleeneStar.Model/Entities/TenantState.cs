using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a tenant, indicating whether it is active or deleted.
    /// </summary>
    public enum TenantState
    {
        /// <summary>
        /// Indicates that the tenant is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the TenantState enumeration.
    /// </summary>
    public static class TenantStateExtensions
    {
        /// <summary>
        /// Determines whether the specified tenant state is active.
        /// </summary>
        /// <param name="state">The tenant state to check.</param>
        /// <returns><c>true</c> if the tenant state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this TenantState state)
        {
            return state == TenantState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified tenant state.
        /// </summary>
        /// <param name="state">The tenant state for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified tenant state.</returns>
        public static Guid Id(this TenantState state)
        {
            return state switch
            {
                TenantState.Active => Guid.Parse("30FEC6C3-6738-49BA-AEFC-EB4FE87F4F51"),
                TenantState.Archived => Guid.Parse("EC383ED7-0AE5-4DE3-A829-9D82DC4A4CB6"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified tenant state.
        /// </summary>
        /// <param name="state">The tenant state for which the text label should be retrieved.</param>
        /// <returns>A string containing the text label for the specified state; otherwise <c>null</c>.</returns>
        public static string Text(this TenantState state)
        {
            return state switch
            {
                TenantState.Active => "kleenestar.core:state.active.label",
                TenantState.Archived => "kleenestar.core:state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified tenant state.
        /// </summary>
        /// <param name="state">The tenant state for which to retrieve the corresponding color selection.</param>
        /// <returns>A string representing the color class for the given tenant state.</returns>
        public static string Color(this TenantState state)
        {
            return state switch
            {
                TenantState.Active => TypeColorSelection.Success.ToClass(),
                TenantState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
