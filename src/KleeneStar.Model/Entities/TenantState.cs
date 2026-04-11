namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a tenant, indicating whether it is active or deleted.
    /// </summary>
    public enum TenantState
    {
        /// <summary>
        /// Indicates that the tenant is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }
}
