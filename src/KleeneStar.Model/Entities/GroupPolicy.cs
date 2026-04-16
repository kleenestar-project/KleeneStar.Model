using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a policy assignment for a group in a specific context.
    /// Stores the policy name (namespace + class name or attribute name).
    /// </summary>
    public class GroupPolicy : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the group.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the policy name (full type name).
        /// </summary>
        public string Policy { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the group.
        /// </summary>
        public Group Group { get; set; }
    }
}
