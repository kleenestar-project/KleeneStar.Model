using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the lifecycle state of a <see cref="Calendar"/>.
    /// </summary>
    public enum CalendarState
    {
        /// <summary>
        /// The calendar is fully configured and currently used to evaluate working hours.
        /// </summary>
        Active,

        /// <summary>
        /// The calendar is archived and no longer used to evaluate working hours.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for the <see cref="CalendarState"/> enumeration.
    /// </summary>
    public static class CalendarStateExtensions
    {
        /// <summary>
        /// Determines whether the specified state is active.
        /// </summary>
        /// <param name="state">The state to check.</param>
        /// <returns><c>true</c> if the state is <see cref="CalendarState.Active"/>.</returns>
        public static bool IsActive(this CalendarState state)
        {
            return state == CalendarState.Active;
        }

        /// <summary>
        /// Returns a stable unique identifier for the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The state id.</returns>
        public static Guid Id(this CalendarState state)
        {
            return state switch
            {
                CalendarState.Active => Guid.Parse("3F62EB7A-9C56-4B17-87FE-9C0E2A1F4C12"),
                CalendarState.Archived => Guid.Parse("AB1C8FE0-0A8E-4F19-8512-2E5B6C2A37F2"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the localized text key for the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The translation key, or <c>null</c>.</returns>
        public static string Text(this CalendarState state)
        {
            return state switch
            {
                CalendarState.Active => "kleenestar.core:calendar.state.active.label",
                CalendarState.Archived => "kleenestar.core:calendar.state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color class associated with the state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The CSS color-selection class.</returns>
        public static string Color(this CalendarState state)
        {
            return state switch
            {
                CalendarState.Active => TypeColorSelection.Success.ToClass(),
                CalendarState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
