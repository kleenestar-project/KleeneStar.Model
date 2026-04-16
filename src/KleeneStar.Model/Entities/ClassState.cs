namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a class, indicating whether it is active or deleted.
    /// </summary>
    public enum ClassState
    {
        /// <summary>
        /// Indicates that the class is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }
}
