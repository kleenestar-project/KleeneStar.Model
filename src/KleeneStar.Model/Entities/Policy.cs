using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a global policy that bundles one or more permissions
    /// (e.g., workspace_admin_policy, workspace_edit_policy).
    /// </summary>
    public class Policy : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the policy.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the policy (e.g., workspace_admin_policy).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the policy.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the collection of permissions included in this policy.
        /// </summary>
        [JsonIgnore]
        public List<Permission> Permissions { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of permission profiles assigned to this policy.
        /// </summary>
        [JsonIgnore]
        public List<PermissionProfile> PermissionProfiles { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Policy()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the policy.
        /// </param>
        public Policy(Guid id)
        {
            Id = id;
        }
    }
}
