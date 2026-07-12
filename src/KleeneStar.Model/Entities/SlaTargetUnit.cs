namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the unit in which an <see cref="SlaTarget.TargetValue"/> is expressed.
    /// </summary>
    public enum SlaTargetUnit
    {
        /// <summary>
        /// The target is expressed in minutes.
        /// </summary>
        Minutes,

        /// <summary>
        /// The target is expressed in hours.
        /// </summary>
        Hours,

        /// <summary>
        /// The target is expressed in calendar days.
        /// </summary>
        Days,

        /// <summary>
        /// The target is expressed in business days (calendar-aware).
        /// </summary>
        BusinessDays
    }
}
