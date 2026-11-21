using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the most recent time an identity opened an object. A composite unique index on
    /// (<see cref="OwnerId"/>, <see cref="ObjectId"/>) enforces one row per identity per object;
    /// opening the object advances <see cref="LastVisited"/>.
    /// </summary>
    /// <remarks>
    /// The visit backs the object dropdown in the application header: with no search term it lists
    /// the calling identity's most recently opened objects, newest first — the object analogue of
    /// the per-identity <see cref="WorkspaceBookmark"/>. Like the workspace bookmark, the row also
    /// carries the personal <see cref="Favorite"/> (star) flag, so a single row per pair combines
    /// both signals; a row exists once the identity has either visited or starred the object.
    /// </remarks>
    public class ObjectVisit : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the visit.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity that owns this visit. Visits are
        /// personal: each identity only sees its own.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the owning identity.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Identity Owner { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the visited object.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the visited object.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Object Object { get; set; }

        /// <summary>
        /// Gets or sets the date and time the owner last opened the object. Drives the
        /// "recently used" ordering in the object dropdown.
        /// </summary>
        public DateTime LastVisited { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the owner has starred the object.
        /// Starred objects surface in the "starred" quickfilter of the issues overview.
        /// </summary>
        public bool Favorite { get; set; }

        /// <summary>
        /// Gets or sets whether the owner has liked the object.
        /// </summary>
        /// <remarks>
        /// A like is not a star. The star is private and says "I want to find this again"; the
        /// like is public and says "this was worth writing" - what is counted under a post and
        /// shown to everybody. They live on the same row because both are one flag of one
        /// identity about one object, which is what this row already is.
        /// <para>
        /// It is deliberately not a <c>CommentLike</c>: that one belongs to a single remark
        /// inside a discussion, and summing them says how lively the discussion was, not what the
        /// post was worth.
        /// </para>
        /// </remarks>
        public bool Liked { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the visit was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the visit was last written.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public ObjectVisit()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the visit.</param>
        public ObjectVisit(Guid id)
        {
            Id = id;
        }
    }
}
