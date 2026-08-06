using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Grants a group a policy on one resource: the assignment the permission dialogs administer
    /// (Identity → Group → Policy → Permission, narrowed to the thing being protected).
    /// </summary>
    /// <remarks>
    /// The policy is held by its registered name rather than by a foreign key, because policies are
    /// declared in code as <c>IIdentityPolicy</c> components and are what the guards check. A table
    /// of policies would have to be kept in step with those classes by hand, and drifting apart is
    /// how the earlier attempt at this ended up granting nothing.
    ///
    /// The resource is addressed by a scope and an id rather than by one foreign key per kind, so a
    /// further kind of resource needs no schema change — only a new scope name.
    /// </remarks>
    public class PermissionAssignment : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the assignment.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the group the policy is granted to.
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// Gets or sets the group the policy is granted to.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Group Group { get; set; }

        /// <summary>
        /// Gets or sets the registered name of the granted policy, for example
        /// <c>workspace_admin_policy</c>.
        /// </summary>
        public string Policy { get; set; }

        /// <summary>
        /// Gets or sets the kind of resource the grant applies to, for example <c>workspace</c>.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the resource within its scope, as it appears in the route
        /// of the page the dialog was opened from.
        /// </summary>
        public string ScopeId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the assignment was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public PermissionAssignment()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign.</param>
        public PermissionAssignment(Guid id)
        {
            Id = id;
        }
    }
}
