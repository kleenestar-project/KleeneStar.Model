using System;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the day a week starts on for an identity. Governs the leading column of
    /// calendars, schedules and sprint boards.
    /// </summary>
    public enum WeekStart
    {
        /// <summary>
        /// The week starts on Monday (ISO 8601, the default).
        /// </summary>
        Monday,

        /// <summary>
        /// The week starts on Sunday.
        /// </summary>
        Sunday,

        /// <summary>
        /// The week starts on Saturday.
        /// </summary>
        Saturday
    }

    /// <summary>
    /// Provides extension methods for working with values of the <see cref="WeekStart"/> enumeration.
    /// </summary>
    public static class WeekStartExtensions
    {
        /// <summary>
        /// Returns the stable identifier associated with the specified week start.
        /// </summary>
        /// <remarks>
        /// The ids are fixed so a selection control can round-trip the value through the REST
        /// layer without depending on the ordinal of the enumeration.
        /// </remarks>
        /// <param name="weekStart">The week start for which to retrieve the identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the specified week start.</returns>
        public static Guid Id(this WeekStart weekStart)
        {
            return weekStart switch
            {
                WeekStart.Sunday => Guid.Parse("6B4F0C1E-9A83-4E27-9C1D-2F0A5E8B7D34"),
                WeekStart.Saturday => Guid.Parse("F2A7D519-3C64-4B08-A5E1-7D9C4B60F812"),
                _ => Guid.Parse("A93C6D07-51B8-4F2A-8E63-0C7A1D5B49E6")
            };
        }

        /// <summary>
        /// Returns the translation key of the textual label for the specified week start.
        /// </summary>
        /// <param name="weekStart">The week start for which the label should be retrieved.</param>
        /// <returns>The translation key of the label.</returns>
        public static string Text(this WeekStart weekStart)
        {
            return weekStart switch
            {
                WeekStart.Sunday => "kleenestar.core:profile.account.weekstart.sunday",
                WeekStart.Saturday => "kleenestar.core:profile.account.weekstart.saturday",
                _ => "kleenestar.core:profile.account.weekstart.monday"
            };
        }

        /// <summary>
        /// Converts the week start into the corresponding <see cref="DayOfWeek"/> value so
        /// calendars and schedules can lay their columns out accordingly.
        /// </summary>
        /// <param name="weekStart">The week start to convert.</param>
        /// <returns>The day of week the calendar week begins with.</returns>
        public static DayOfWeek ToDayOfWeek(this WeekStart weekStart)
        {
            return weekStart switch
            {
                WeekStart.Sunday => DayOfWeek.Sunday,
                WeekStart.Saturday => DayOfWeek.Saturday,
                _ => DayOfWeek.Monday
            };
        }
    }
}
