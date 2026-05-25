using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a typed directional link between two <see cref="Object"/> entities,
    /// e.g. "INC-1 is blocked by CHG-7" or "BUG-3 duplicates BUG-2".
    /// </summary>
    public class ObjectLink : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the link.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the source object.
        /// </summary>
        public Guid SourceObjectId { get; set; }

        /// <summary>
        /// Gets or sets the source object navigation property.
        /// </summary>
        [JsonIgnore]
        public Object SourceObject { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the target object.
        /// </summary>
        public Guid TargetObjectId { get; set; }

        /// <summary>
        /// Gets or sets the target object navigation property.
        /// </summary>
        [JsonIgnore]
        public Object TargetObject { get; set; }

        /// <summary>
        /// Gets or sets the relation semantics. The relation reads as
        /// "<see cref="SourceObject"/> <c>RelationType</c> <see cref="TargetObject"/>".
        /// </summary>
        public ObjectLinkRelationType RelationType { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the link was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the link was last updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObjectLink()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the link.</param>
        public ObjectLink(Guid id)
        {
            Id = id;
        }
    }
}
