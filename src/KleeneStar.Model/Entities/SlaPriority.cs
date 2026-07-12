namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Indicates the severity bucket an <see cref="SlaPolicy"/> applies to.
    /// </summary>
    /// <remarks>
    /// This is a coarse classification that the policy uses to align with the priority
    /// scheme of the associated <see cref="Class"/>. It is independent of the granular
    /// <see cref="Priority"/> entries defined per class.
    /// </remarks>
    public enum SlaPriority
    {
        /// <summary>
        /// Lowest severity. Best-effort handling.
        /// </summary>
        Low,

        /// <summary>
        /// Standard severity for routine work.
        /// </summary>
        Medium,

        /// <summary>
        /// Elevated severity requiring accelerated handling.
        /// </summary>
        High,

        /// <summary>
        /// Highest severity. System-critical with the strictest response targets.
        /// </summary>
        Critical
    }
}
