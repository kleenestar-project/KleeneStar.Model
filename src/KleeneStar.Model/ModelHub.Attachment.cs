using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the KleeneStar.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns a materialized collection of attachments from the database.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching attachments, including object and uploader navigation.</returns>
        public static IEnumerable<Attachment> GetAttachments(IQuery<Attachment> query)
        {
            using var db = CreateDbContext();

            return [.. GetAttachments(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of attachments using the supplied DbContext.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The DbContext.</param>
        /// <returns>The filtered collection.</returns>
        public static IEnumerable<Attachment> GetAttachments(IQuery<Attachment> query, KleeneStarDbContext context)
        {
            var data = context.Attachments
                .Include(x => x.Object)
                .Include(x => x.Uploader)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Returns the lightweight metadata of every attachment of the supplied object,
        /// ordered chronologically. The binary <see cref="Attachment.Content"/> is
        /// deliberately NOT loaded so the file-list card never transfers blob data; use
        /// <see cref="GetAttachment(Guid)"/> to obtain the payload for download.
        /// </summary>
        /// <param name="objectId">The id of the owning object.</param>
        /// <returns>The attachments' metadata. The collection may be empty.</returns>
        public static IEnumerable<Attachment> GetAttachmentsByObject(Guid objectId)
        {
            using var db = CreateDbContext();

            return db.Attachments
                .Where(a => a.ObjectId == objectId)
                .OrderBy(a => a.Created)
                .Select(AttachmentMetadata)
                .AsNoTracking()
                .ToList();
        }

        /// <summary>
        /// The projection that reads an attachment without its binary
        /// <see cref="Attachment.Content"/>. It is shared by every metadata read so a column
        /// added to the entity is carried by all of them or by none, rather than by whichever
        /// one was remembered.
        /// </summary>
        private static readonly Expression<Func<Attachment, Attachment>> AttachmentMetadata = a => new Attachment
        {
            RawId = a.RawId,
            Id = a.Id,
            FileName = a.FileName,
            ContentType = a.ContentType,
            Version = a.Version,
            Size = a.Size,
            StoragePath = a.StoragePath,
            Description = a.Description,
            State = a.State,
            Created = a.Created,
            Updated = a.Updated,
            ObjectId = a.ObjectId,
            UploaderId = a.UploaderId
        };

        /// <summary>
        /// Returns the single attachment with the supplied id including its binary
        /// <see cref="Attachment.Content"/>, or <c>null</c> when no row matches.
        /// </summary>
        /// <param name="id">The attachment id.</param>
        /// <returns>The attachment, or <c>null</c>.</returns>
        public static Attachment GetAttachment(Guid id)
        {
            using var db = CreateDbContext();

            return db.Attachments
                .AsNoTracking()
                .FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Adds the supplied attachment to the database when no attachment with the same id
        /// exists. Stamps <see cref="Attachment.Created"/> / <see cref="Attachment.Updated"/>
        /// when they are not set by the caller, and assigns the next
        /// <see cref="Attachment.Version"/> of its file name when the caller left it unset.
        /// </summary>
        /// <remarks>
        /// The version is assigned here rather than by the caller so the number is read and
        /// written against the same context: a caller that queried the chain first and inserted
        /// afterwards would hand two simultaneous uploads of one name the same number.
        /// </remarks>
        /// <param name="attachment">The attachment to add.</param>
        public static void Add(Attachment attachment)
        {
            ArgumentNullException.ThrowIfNull(attachment);

            using var db = CreateDbContext();

            if (db.Attachments.Any(x => x.Id == attachment.Id))
            {
                return;
            }

            if (attachment.Created == default)
            {
                attachment.Created = DateTime.UtcNow;
            }

            attachment.Updated = DateTime.UtcNow;

            if (attachment.Version <= 0)
            {
                attachment.Version = GetAttachmentVersionCeiling(db, attachment.ObjectId, attachment.FileName) + 1;
            }

            db.Attachments.Add(attachment);
            db.SaveChanges();
        }

        /// <summary>
        /// Returns the highest version currently attached to the object under the supplied file
        /// name, or zero when the name is new to the object.
        /// </summary>
        /// <param name="db">The context the lookup runs in.</param>
        /// <param name="objectId">The id of the owning object.</param>
        /// <param name="fileName">The file name whose chain is measured.</param>
        /// <returns>The highest version of the name, or zero.</returns>
        private static int GetAttachmentVersionCeiling(KleeneStarDbContext db, Guid objectId, string fileName)
        {
            return db.Attachments
                .Where(x => x.ObjectId == objectId && x.FileName == fileName)
                .Max(x => (int?)x.Version) ?? 0;
        }

        /// <summary>
        /// Updates the supplied attachment in the database. Re-loads the existing row and
        /// overwrites the mutable metadata only (file name, description and state); the
        /// stored binary payload is never rewritten through this path.
        /// </summary>
        /// <param name="attachment">The attachment to update.</param>
        public static void Update(Attachment attachment)
        {
            ArgumentNullException.ThrowIfNull(attachment);

            using var db = CreateDbContext();

            var existing = db.Attachments.FirstOrDefault(x => x.Id == attachment.Id);

            if (existing is null)
            {
                return;
            }

            existing.FileName = attachment.FileName;
            existing.Description = attachment.Description;
            existing.State = attachment.State;
            existing.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Writes the description of a single attachment and returns the changed row.
        /// </summary>
        /// <remarks>
        /// The row is read through the metadata projection and attached with only the two changed
        /// properties marked, so the file's binary <see cref="Attachment.Content"/> is neither
        /// read nor written to change a caption beside it. <c>ExecuteUpdate</c> would say the same
        /// thing in one statement, but it is relational-only and the provider here is loaded by
        /// reflection - this way stays true for whatever provider the installation configures.
        /// </remarks>
        /// <param name="id">The attachment id.</param>
        /// <param name="description">The new description, or <c>null</c> to clear it.</param>
        /// <returns>The changed attachment, or <c>null</c> when no row matches.</returns>
        public static Attachment SetAttachmentDescription(Guid id, string description)
        {
            using var db = CreateDbContext();

            var existing = db.Attachments
                .Where(x => x.Id == id)
                .Select(AttachmentMetadata)
                .AsNoTracking()
                .FirstOrDefault();

            if (existing is null)
            {
                return null;
            }

            existing.Description = description;
            existing.Updated = DateTime.UtcNow;

            var entry = db.Attach(existing);
            entry.Property(x => x.Description).IsModified = true;
            entry.Property(x => x.Updated).IsModified = true;

            db.SaveChanges();

            return existing;
        }

        /// <summary>
        /// Removes the specified attachment from the data store if it exists.
        /// </summary>
        /// <param name="attachment">The attachment entity to remove.</param>
        public static void Remove(Attachment attachment)
        {
            ArgumentNullException.ThrowIfNull(attachment);

            using var db = CreateDbContext();

            var existing = db.Attachments.FirstOrDefault(x => x.Id == attachment.Id);

            if (existing is null)
            {
                return;
            }

            db.Attachments.Remove(existing);
            db.SaveChanges();
        }
    }
}
