using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a single like on a <see cref="Comment"/> by an <see cref="Identity"/>.
    /// A composite unique index on (CommentId, AuthorId) enforces one like per identity
    /// per comment; toggling off removes the row.
    /// </summary>
    public class CommentLike : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the like.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the like was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the comment the like is attached to.
        /// </summary>
        public Guid CommentId { get; set; }

        /// <summary>
        /// Gets or sets the comment the like is attached to.
        /// </summary>
        [JsonIgnore]
        public Comment Comment { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity that authored the like.
        /// </summary>
        public Guid AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the identity that authored the like.
        /// </summary>
        [JsonIgnore]
        public Identity Author { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public CommentLike()
        {
            Id = Guid.NewGuid();
        }
    }
}
