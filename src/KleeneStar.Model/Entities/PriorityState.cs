namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a priority, indicating whether it is active or deleted.
    /// </summary>
    public enum PriorityState
    {
        /// <summary>
        /// Indicates that the priority is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }
}
