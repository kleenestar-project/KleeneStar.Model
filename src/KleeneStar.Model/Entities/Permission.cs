using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a single atomic permission that controls access to a specific operation
    /// (e.g., workspace_read, workspace_update).
    /// </summary>
    public class Permission : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the permission.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the permission (e.g., workspace_read).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the permission.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the collection of policies that include this permission.
        /// </summary>
        [JsonIgnore]
        public List<Policy> Policies { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Permission()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the permission.
        /// </param>
        public Permission(Guid id)
        {
            Id = id;
        }
    }
}
