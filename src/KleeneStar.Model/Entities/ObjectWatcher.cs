using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a watch relationship between an identity and an
    /// object. A composite unique index on (ObjectId, IdentityId) enforces 
    /// one watch per identity per object; "stop watching" removes the row.
    /// </summary>
    public class ObjectWatcher : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the watch relationship.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the watch was added.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the watched object.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the watched object.
        /// </summary>
        [JsonIgnore]
        public Object Object { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the watching identity.
        /// </summary>
        public Guid IdentityId { get; set; }

        /// <summary>
        /// Gets or sets the watching identity.
        /// </summary>
        [JsonIgnore]
        public Identity Identity { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public ObjectWatcher()
        {
            Id = Guid.NewGuid();
        }
    }
}
