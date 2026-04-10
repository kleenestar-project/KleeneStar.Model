using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a permission profile that can be assigned to one or more workspaces.
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
        /// Gets or sets the name of the permission profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the permission profile.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the collection of workspaces associated with this permission profile.
        /// </summary>
        [JsonIgnore]
        public List<Workspace> Workspaces { get; set; } = [];

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
