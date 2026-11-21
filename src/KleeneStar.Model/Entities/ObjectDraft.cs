using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// The unpublished working copy of the prose attributes of an <see cref="Object"/> - its
    /// <see cref="Object.Summary"/> and its rich-text <see cref="Object.Description"/>. The
    /// prose editor of the document and blog kinds writes into this row on every change, and
    /// the published object is only touched when the author publishes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There is at most <b>one draft per object</b> (enforced by a unique index on
    /// <see cref="ObjectId"/>), not one per author. That is deliberate: the editor is a
    /// collaborative surface, so two authors working on the same document are working on the
    /// same text, and a per-author draft would silently fork it. <see cref="UpdaterId"/>
    /// therefore records who wrote last, not who owns the draft.
    /// </para>
    /// <para>
    /// A draft is <b>not</b> a revision. It carries no commit, appears in no
    /// <see cref="Commit"/> chain, and is not replayable - the version history begins where
    /// publishing ends. Abandoning the editor keeps the draft (so editing resumes where it
    /// stopped) while the reading view keeps showing the published text; publishing copies the
    /// draft onto the object inside a normal commit and deletes the row.
    /// </para>
    /// </remarks>
    public class ObjectDraft : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the draft.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the object the draft belongs to.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the object the draft belongs to.
        /// </summary>
        [JsonIgnore]
        public Object Object { get; set; }

        /// <summary>
        /// Gets or sets the unpublished title. <c>null</c> when the draft leaves the published
        /// <see cref="Object.Summary"/> untouched.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the unpublished rich-text body. <c>null</c> when the draft leaves the
        /// published <see cref="Object.Description"/> untouched.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date and time the draft was opened, i.e. when the first
        /// unpublished change was recorded.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the most recent unpublished change. This is what
        /// the editor's save indicator reports.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity that wrote the most recent
        /// change, or <c>null</c> when the writer was not authenticated.
        /// </summary>
        public Guid? UpdaterId { get; set; }

        /// <summary>
        /// Gets or sets the identity that wrote the most recent change.
        /// </summary>
        [JsonIgnore]
        public Identity Updater { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public ObjectDraft()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the draft.</param>
        public ObjectDraft(Guid id)
        {
            Id = id;
        }
    }
}
