namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a form, indicating whether it is active or deleted.
    /// </summary>
    public enum FormState
    {
        /// <summary>
        /// Indicates that the form is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }
}
