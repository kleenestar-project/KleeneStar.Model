namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Defines when an <see cref="SlaPolicy"/> clock runs.
    /// </summary>
    public enum SlaCalendar
    {
        /// <summary>
        /// The policy clock runs continuously (24 hours a day, 7 days a week).
        /// </summary>
        TwentyFourSeven,

        /// <summary>
        /// The policy clock runs only during business hours on weekdays (Mon-Fri, 08-18).
        /// </summary>
        BusinessHours,

        /// <summary>
        /// The policy clock runs during extended weekday hours (Mon-Fri, 06-22).
        /// </summary>
        ExtendedBusinessHours,

        /// <summary>
        /// The policy clock runs only during the configured nightly maintenance window.
        /// </summary>
        NightShift,

        /// <summary>
        /// The policy uses a custom calendar described in the free-text description.
        /// </summary>
        Custom
    }
}
