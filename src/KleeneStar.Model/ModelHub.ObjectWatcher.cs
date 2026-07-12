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
        /// Returns a materialized collection of object watchers from the database.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching watchers, including the related identity.</returns>
        public static IEnumerable<ObjectWatcher> GetObjectWatchers(IQuery<ObjectWatcher> query)
        {
            using var db = CreateDbContext();

            return [.. GetObjectWatchers(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of object watchers using the supplied DbContext.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The DbContext.</param>
        /// <returns>The filtered collection.</returns>
        public static IEnumerable<ObjectWatcher> GetObjectWatchers(IQuery<ObjectWatcher> query, KleeneStarDbContext context)
        {
            var data = context.ObjectWatchers
                .Include(x => x.Identity)
                .Include(x => x.Object)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Adds the supplied watch relationship to the database. The unique composite
        /// index on (ObjectId, IdentityId) blocks duplicates on the database side;
        /// callers should check for an existing row via
        /// <see cref="GetObjectWatchers(IQuery{ObjectWatcher})"/> first when the
        /// duplicate must be tolerated silently.
        /// </summary>
        /// <param name="watcher">The watch relationship to add.</param>
        public static void Add(ObjectWatcher watcher)
        {
            ArgumentNullException.ThrowIfNull(watcher);

            using var db = CreateDbContext();

            if (db.ObjectWatchers.Any(x => x.ObjectId == watcher.ObjectId && x.IdentityId == watcher.IdentityId))
            {
                return;
            }

            if (watcher.Created == default)
            {
                watcher.Created = DateTime.UtcNow;
            }

            db.ObjectWatchers.Add(watcher);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the supplied watch relationship from the database. No-op when no
        /// matching row exists.
        /// </summary>
        /// <param name="watcher">The watch relationship to remove.</param>
        public static void Remove(ObjectWatcher watcher)
        {
            ArgumentNullException.ThrowIfNull(watcher);

            using var db = CreateDbContext();

            var existing = db.ObjectWatchers.FirstOrDefault(x => x.Id == watcher.Id);

            if (existing is null)
            {
                return;
            }

            db.ObjectWatchers.Remove(existing);
            db.SaveChanges();
        }
    }
}
