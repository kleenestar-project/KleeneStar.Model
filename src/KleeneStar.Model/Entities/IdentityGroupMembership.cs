namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the membership of an identity in a group (m:n relation).
    /// </summary>
    public class IdentityGroupMembership
    {
        /// <summary>
        /// Gets or sets the foreign key to the identity.
        /// </summary>
        public int IdentityId { get; set; }

        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        public Identity Identity { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the group.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public Group Group { get; set; }
    }
}
