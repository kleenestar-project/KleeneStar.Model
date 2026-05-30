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
        /// Returns a materialized collection of object tags from the database.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching tags, including the related object.</returns>
        public static IEnumerable<ObjectTag> GetObjectTags(IQuery<ObjectTag> query)
        {
            using var db = CreateDbContext();

            return [.. GetObjectTags(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of object tags using the supplied DbContext.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The DbContext.</param>
        /// <returns>The filtered collection.</returns>
        public static IEnumerable<ObjectTag> GetObjectTags(IQuery<ObjectTag> query, KleeneStarDbContext context)
        {
            var data = context.ObjectTags
                .Include(x => x.Object)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Adds the supplied tag to the database. The unique composite index on
        /// (ObjectId, Name) blocks duplicates on the database side; this method additionally
        /// returns early when a tag with the same name already exists on the object so the
        /// duplicate is tolerated silently.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        public static void Add(ObjectTag tag)
        {
            ArgumentNullException.ThrowIfNull(tag);

            using var db = CreateDbContext();

            if (db.ObjectTags.Any(x => x.ObjectId == tag.ObjectId && x.Name == tag.Name))
            {
                return;
            }

            if (tag.Created == default)
            {
                tag.Created = DateTime.UtcNow;
            }

            db.ObjectTags.Add(tag);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the supplied tag from the database. No-op when no matching row exists.
        /// </summary>
        /// <param name="tag">The tag to remove.</param>
        public static void Remove(ObjectTag tag)
        {
            ArgumentNullException.ThrowIfNull(tag);

            using var db = CreateDbContext();

            var existing = db.ObjectTags.FirstOrDefault(x => x.Id == tag.Id);

            if (existing is null)
            {
                return;
            }

            db.ObjectTags.Remove(existing);
            db.SaveChanges();
        }
    }
}
