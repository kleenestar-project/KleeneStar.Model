namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a status, indicating whether it is active or deleted.
    /// </summary>
    public enum StatusState
    {
        /// <summary>
        /// Indicates that the status is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }
}
