namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a field, indicating whether it is active or deleted.
    /// </summary>
    public enum FieldState
    {
        /// <summary>
        /// Indicates that the field is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }
}
