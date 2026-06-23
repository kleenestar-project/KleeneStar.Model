using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the personal relationship between an identity and a workspace: whether the
    /// identity has favorited the workspace and when they last opened it. A composite unique
    /// index on (<see cref="OwnerId"/>, <see cref="WorkspaceId"/>) enforces one bookmark per
    /// identity per workspace.
    /// </summary>
    /// <remarks>
    /// A row exists once the identity has either favorited or visited the workspace; both
    /// signals share the same row, mirroring how <see cref="SavedSearch"/> combines its
    /// starred flag and last-used timestamp. The bookmark backs the global workspace
    /// dropdown — favorites are pinned to the top and the remaining slots show the most
    /// recently visited workspaces, newest first.
    /// </remarks>
    public class WorkspaceBookmark : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the bookmark.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity that owns this bookmark.
        /// Bookmarks are personal: each identity only sees its own.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the owning identity.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Identity Owner { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the bookmarked workspace.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the bookmarked workspace.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the owner has favorited (pinned) the
        /// workspace. Favorites are surfaced at the top of the workspace dropdown.
        /// </summary>
        public bool Favorite { get; set; }

        /// <summary>
        /// Gets or sets the date and time the owner last opened the workspace. Drives the
        /// "recently used" ordering in the workspace dropdown.
        /// </summary>
        public DateTime LastVisited { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the bookmark was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the bookmark was last written.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public WorkspaceBookmark()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the bookmark.</param>
        public WorkspaceBookmark(Guid id)
        {
            Id = id;
        }
    }
}
