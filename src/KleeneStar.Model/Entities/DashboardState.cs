namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a dashboard, indicating whether it is active or deleted.
    /// </summary>
    public enum DashboardState
    {
        /// <summary>
        /// Indicates that the dashboard is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the dashboard has been deleted and is no longer active or accessible.
        /// </summary>
        Deleted
    }
}
