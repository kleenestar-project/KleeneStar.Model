using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents an emoji reaction on a <see cref="Comment"/> by an <see cref="Identity"/>.
    /// A composite unique index on (CommentId, AuthorId, Emoji) enforces one of each emoji
    /// per identity per comment; toggling off removes the row.
    /// </summary>
    public class CommentReaction : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the reaction.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the emoji that was used for the reaction (typically a single
        /// Unicode grapheme cluster, e.g. "🚀" or "👍").
        /// </summary>
        public string Emoji { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the reaction was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the comment the reaction is attached to.
        /// </summary>
        public Guid CommentId { get; set; }

        /// <summary>
        /// Gets or sets the comment the reaction is attached to.
        /// </summary>
        [JsonIgnore]
        public Comment Comment { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity that authored the reaction.
        /// </summary>
        public Guid AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the identity that authored the reaction.
        /// </summary>
        [JsonIgnore]
        public Identity Author { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public CommentReaction()
        {
            Id = Guid.NewGuid();
        }
    }
}
