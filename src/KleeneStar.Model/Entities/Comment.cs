using KleeneStar.Model.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a comment posted on an <see cref="Object"/>. Comments form a flat or
    /// threaded discussion under each object and are authored by an <see cref="Identity"/>.
    /// </summary>
    /// <remarks>
    /// Self-referencing parent FK <see cref="ParentCommentId"/> enables reply threads.
    /// Soft deletion is supported through <see cref="State"/>=<see cref="CommentState.Deleted"/>
    /// together with <see cref="DeletedAt"/>; the row is kept so child replies still
    /// resolve to a parent.
    /// </remarks>
    public class Comment : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the comment.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the markdown/plain-text content of the comment.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the current lifecycle state of the comment.
        /// </summary>
        [RestConverter<CommentStateConverter>]
        public CommentState State { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the comment was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the comment was last updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the comment was soft-deleted, or
        /// <c>null</c> when the comment is not deleted.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the comment is pinned to the top of
        /// the thread. Pinned comments sort before everything else and survive the
        /// chronological ordering.
        /// </summary>
        public bool IsPinned { get; set; }

        /// <summary>
        /// Gets or sets the likes attached to the comment.
        /// </summary>
        [JsonIgnore]
        public List<CommentLike> Likes { get; set; } = [];

        /// <summary>
        /// Gets or sets the emoji reactions attached to the comment.
        /// </summary>
        [JsonIgnore]
        public List<CommentReaction> Reactions { get; set; } = [];

        /// <summary>
        /// Gets or sets the unique identifier of the object the comment is attached to.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the object the comment is attached to.
        /// </summary>
        [JsonIgnore]
        public Object Object { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity that authored the comment.
        /// </summary>
        public Guid AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the identity that authored the comment.
        /// </summary>
        [JsonIgnore]
        public Identity Author { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the parent comment when this comment
        /// is a reply, or <c>null</c> for top-level comments.
        /// </summary>
        public Guid? ParentCommentId { get; set; }

        /// <summary>
        /// Gets or sets the parent comment when this comment is a reply.
        /// </summary>
        [JsonIgnore]
        public Comment ParentComment { get; set; }

        /// <summary>
        /// Gets or sets the replies attached to this comment.
        /// </summary>
        [JsonIgnore]
        public List<Comment> Replies { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public Comment()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the supplied id.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the comment.</param>
        public Comment(Guid id)
        {
            Id = id;
        }
    }
}
