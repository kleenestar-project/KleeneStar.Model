namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a workflow, indicating whether it is active or deleted.
    /// </summary>
    public enum WorkflowState
    {
        /// <summary>
        /// Indicates that the workflow is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }
}
