using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebExpress.WebCore.WebIdentity;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a global user group (e.g., "Marketing", "Admin", "Engineering").
    /// </summary>
    public class Group : IEntity, IIdentityGroup
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the group.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional description of the group.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Navigation property for persisted policy assignments.
        /// </summary>
        public List<GroupPolicy> GroupPolicies { get; set; } = [];

        /// <summary>
        /// Navigation property for identity memberships (m:n).
        /// </summary>
        public List<IdentityGroupMembership> GroupMemberships { get; set; } = [];

        /// <summary>
        /// Gets the collection of policy names associated with the identity group.
        /// </summary>
        IEnumerable<string> IIdentityGroup.Policies => GroupPolicies.Select(x => x.Policy);

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Group()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the group.
        /// </param>
        public Group(Guid id)
        {
            Id = id;
        }
    }
}
