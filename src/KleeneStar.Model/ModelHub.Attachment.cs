using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .Select(a => new Attachment
                {
                    RawId = a.RawId,
                    Id = a.Id,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    Size = a.Size,
                    StoragePath = a.StoragePath,
                    Description = a.Description,
                    State = a.State,
                    Created = a.Created,
                    Updated = a.Updated,
                    ObjectId = a.ObjectId,
                    UploaderId = a.UploaderId
                })
                .AsNoTracking()
                .ToList();
        }

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
        /// when they are not set by the caller.
        /// </summary>
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

            db.Attachments.Add(attachment);
            db.SaveChanges();
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
