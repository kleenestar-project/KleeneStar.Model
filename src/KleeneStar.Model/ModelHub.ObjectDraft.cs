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
        /// Returns a materialized collection of object drafts from the database.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching drafts, including the related object.</returns>
        public static IEnumerable<ObjectDraft> GetObjectDrafts(IQuery<ObjectDraft> query)
        {
            using var db = CreateDbContext();

            return [.. GetObjectDrafts(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of object drafts using the supplied DbContext.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The DbContext.</param>
        /// <returns>The filtered collection.</returns>
        public static IEnumerable<ObjectDraft> GetObjectDrafts(IQuery<ObjectDraft> query, KleeneStarDbContext context)
        {
            var data = context.ObjectDrafts
                .Include(x => x.Object)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Returns the draft of the supplied object, or <see langword="null"/> when the object
        /// carries no unpublished changes.
        /// </summary>
        /// <param name="objectId">The id of the object whose draft is read.</param>
        /// <returns>The draft, or <see langword="null"/>.</returns>
        public static ObjectDraft GetObjectDraft(Guid objectId)
        {
            using var db = CreateDbContext();

            return db.ObjectDrafts
                .AsNoTracking()
                .FirstOrDefault(x => x.ObjectId == objectId);
        }

        /// <summary>
        /// Writes the supplied prose values as the unpublished draft of the object, creating the
        /// row on the first change and overwriting it on every later one. Read and write share a
        /// single context so two saves arriving together cannot produce two draft rows for the
        /// same object - which the unique index would refuse.
        /// </summary>
        /// <param name="objectId">The id of the object being drafted.</param>
        /// <param name="summary">The unpublished title.</param>
        /// <param name="description">The unpublished rich-text body.</param>
        /// <param name="updaterId">The identity that wrote the change, or <c>null</c>.</param>
        /// <returns>The persisted draft, or <see langword="null"/> when the object does not
        /// exist.</returns>
        public static ObjectDraft UpsertObjectDraft(Guid objectId, string summary, string description, Guid? updaterId)
        {
            using var db = CreateDbContext();

            if (!db.Objects.AsNoTracking().Any(x => x.Id == objectId))
            {
                return null;
            }

            var now = DateTime.UtcNow;
            var draft = db.ObjectDrafts.FirstOrDefault(x => x.ObjectId == objectId);

            if (draft is null)
            {
                draft = new ObjectDraft
                {
                    ObjectId = objectId,
                    Created = now
                };

                db.ObjectDrafts.Add(draft);
            }

            draft.Summary = summary;
            draft.Description = description;
            draft.Updated = now;
            draft.UpdaterId = updaterId;

            db.SaveChanges();

            return draft;
        }

        /// <summary>
        /// Removes the draft of the supplied object. No-op when the object carries none.
        /// </summary>
        /// <param name="objectId">The id of the object whose draft is discarded.</param>
        /// <returns><see langword="true"/> when a row existed and was removed.</returns>
        public static bool RemoveObjectDraft(Guid objectId)
        {
            using var db = CreateDbContext();

            var draft = db.ObjectDrafts.FirstOrDefault(x => x.ObjectId == objectId);

            if (draft is null)
            {
                return false;
            }

            db.ObjectDrafts.Remove(draft);
            db.SaveChanges();

            return true;
        }
    }
}
