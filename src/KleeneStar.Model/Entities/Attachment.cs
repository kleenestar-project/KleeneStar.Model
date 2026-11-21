using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a file attached to an <see cref="Object"/>. The binary payload itself
    /// lives on disk (referenced by <see cref="StoragePath"/>); this entity only carries
    /// the metadata required to list, download and manage the file.
    /// </summary>
    /// <remarks>
    /// Soft deletion is supported through <see cref="State"/>=<see cref="AttachmentState.Deleted"/>;
    /// the row is kept for audit purposes while the file list hides it. The owning object
    /// is referenced through <see cref="ObjectId"/> (cascade delete), the uploading
    /// identity through the optional <see cref="UploaderId"/>.
    /// </remarks>
    public class Attachment : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the attachment.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the original file name including its extension (e.g.
        /// <c>incident-report.pdf</c>) as supplied by the uploader.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the MIME content type of the file (e.g. <c>application/pdf</c>),
        /// used to pick the display icon and the download response header.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the version this row holds among the attachments that share its
        /// <see cref="FileName"/> on the same <see cref="Object"/>. The first upload of a name is
        /// version 1, every further upload of the same name continues the chain.
        /// </summary>
        /// <remarks>
        /// The name is the identity of a file across its versions: attaching a name that is
        /// already there is a new version of that file rather than a second file, and the file
        /// surfaces fold the rows of one name into a single entry with the highest version at the
        /// head. The number is stored rather than derived from <see cref="Created"/>, so the order
        /// of a chain survives two uploads that arrive in the same instant and stays stable when a
        /// version is removed. Zero means the row predates versioning.
        /// </remarks>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the size of the file in bytes.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the storage location of the binary payload, relative to the
        /// application data directory. Retained for reference; the binary payload itself is
        /// persisted in <see cref="Content"/>.
        /// </summary>
        public string StoragePath { get; set; }

        /// <summary>
        /// Gets or sets the binary payload of the attachment. This is the data returned by
        /// the download endpoint. May be <c>null</c> when the row carries metadata only.
        /// </summary>
        [IndexIgnore]
        public byte[] Content { get; set; }

        /// <summary>
        /// Gets or sets an optional human-readable description of the attachment.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current lifecycle state of the attachment.
        /// </summary>
        public AttachmentState State { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the attachment was uploaded.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the attachment metadata was last updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the object the attachment belongs to.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the object the attachment belongs to.
        /// </summary>
        [JsonIgnore]
        public Object Object { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity that uploaded the file, or
        /// <c>null</c> when the uploader is unknown (e.g. seeded data).
        /// </summary>
        public Guid? UploaderId { get; set; }

        /// <summary>
        /// Gets or sets the identity that uploaded the file.
        /// </summary>
        [JsonIgnore]
        public Identity Uploader { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public Attachment()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the supplied id.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the attachment.</param>
        public Attachment(Guid id)
        {
            Id = id;
        }
    }
}
