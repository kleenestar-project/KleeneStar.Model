namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the state of an identity.
    /// </summary>
    public enum IdentityState
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity is active.
        /// </summary>
        Active,

        /// <summary>
        /// Gets a value indicating whether the resource is currently locked and cannot be modified.
        /// </summary>
        Locked,

        /// <summary>
        /// Indicates that the feature or functionality is disabled.
        /// </summary>
        Disabled
    }
}
