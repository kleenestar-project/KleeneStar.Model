using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a permission profile that defines which policy a group receives
    /// in a specific workspace. A profile is always scoped to exactly one workspace.
    /// </summary>
    public class PermissionProfile : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the permission profile.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the group assigned to this profile.
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// Gets or sets the group assigned to this profile.
        /// </summary>
        [JsonIgnore]
        public Group Group { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the policy assigned to this profile.
        /// </summary>
        public Guid PolicyId { get; set; }

        /// <summary>
        /// Gets or sets the policy assigned to this profile.
        /// </summary>
        [JsonIgnore]
        public Policy Policy { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the workspace this profile is scoped to.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the workspace this profile is scoped to.
        /// </summary>
        [JsonIgnore]
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PermissionProfile()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the permission profile.
        /// </param>
        public PermissionProfile(Guid id)
        {
            Id = id;
        }
    }
}
