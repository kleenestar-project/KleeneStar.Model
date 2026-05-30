using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a tag (label) attached to an object. A composite unique index on
    /// (ObjectId, Name) enforces one tag of a given name per object; removing the row
    /// detaches the tag.
    /// </summary>
    public class ObjectTag : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the tag.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the tag was attached.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the tagged object.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the tagged object.
        /// </summary>
        [JsonIgnore]
        public Object Object { get; set; }

        /// <summary>
        /// Gets or sets the display text of the tag / label.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional badge color of the tag as a CSS color string (e.g. a
        /// hex triplet such as <c>#0d6efd</c>). When <c>null</c> the UI derives a
        /// deterministic color from <see cref="Name"/>.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public ObjectTag()
        {
            Id = Guid.NewGuid();
        }
    }
}
