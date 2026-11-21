using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a security level, indicating whether it is active or archived.
    /// </summary>
    public enum SecurityLevelState
    {
        /// <summary>
        /// Indicates that the security level is fully configured and can be assigned to objects.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the security level is archived and can no longer be assigned. Objects
        /// already classified with it keep their classification and stay guarded by it.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the
    /// SecurityLevelState enumeration.
    /// </summary>
    public static class SecurityLevelStateExtensions
    {
        /// <summary>
        /// Determines whether the specified security level state is active.
        /// </summary>
        /// <param name="state">The security level state to check.</param>
        /// <returns><c>true</c> if the state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this SecurityLevelState state)
        {
            return state == SecurityLevelState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified security level state.
        /// </summary>
        /// <param name="state">The state for which to retrieve the unique identifier.</param>
        /// <returns>
        /// A <see cref="Guid"/> representing the unique identifier for the specified state.
        /// </returns>
        public static Guid Id(this SecurityLevelState state)
        {
            return state switch
            {
                SecurityLevelState.Active => Guid.Parse("2C0FE6D4-9B0B-4B1B-9A6E-6D1A2A2F5C10"),
                SecurityLevelState.Archived => Guid.Parse("9E4B7F62-4C2A-4B0E-8E2F-3B7A1D5C6E21"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified security level state.
        /// </summary>
        /// <param name="state">
        /// The security level state for which the text label should be retrieved.
        /// </param>
        /// <returns>
        /// A string containing the text label for the specified state; otherwise <c>null</c>
        /// if the state is not supported.
        /// </returns>
        public static string Text(this SecurityLevelState state)
        {
            return state switch
            {
                SecurityLevelState.Active => "kleenestar.core:state.active.label",
                SecurityLevelState.Archived => "kleenestar.core:state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified security level state.
        /// </summary>
        /// <param name="state">The state for which to retrieve the corresponding color.</param>
        /// <returns>
        /// A value that represents the color selection for the given state.
        /// </returns>
        public static string Color(this SecurityLevelState state)
        {
            return state switch
            {
                SecurityLevelState.Active => TypeColorSelection.Success.ToClass(),
                SecurityLevelState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
