namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a transition, indicating whether it is active or deleted.
    /// </summary>
    public enum TransitionState
    {
        /// <summary>
        /// Indicates that the transition is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }
}
