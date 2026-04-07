namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a dashboard, indicating whether it is active or deleted.
    /// </summary>
    public enum TypeDashboardState
    {
        /// <summary>
        /// Indicates that the dashboard is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the dashboard has been permanently and irreversibly deleted.
        /// </summary>
        Deleted
    }
}
