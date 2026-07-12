using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a share relationship between an object and an identity. Sharing
    /// grants the linked identity read/comment access to the object (e.g. a portal
    /// issue) without making it the requester. A composite unique index on
    /// (ObjectId, IdentityId) enforces one share per identity per object; revoking
    /// a share removes the row.
    /// </summary>
    public class ObjectShare : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the share relationship.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the share was granted.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the shared object.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the shared object.
        /// </summary>
        [JsonIgnore]
        public Object Object { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity the object is shared with.
        /// </summary>
        public Guid IdentityId { get; set; }

        /// <summary>
        /// Gets or sets the identity the object is shared with.
        /// </summary>
        [JsonIgnore]
        public Identity Identity { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public ObjectShare()
        {
            Id = Guid.NewGuid();
        }
    }
}
