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
        /// Returns a materialized collection of object shares from the database.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching shares, including the related identity and object.</returns>
        public static IEnumerable<ObjectShare> GetObjectShares(IQuery<ObjectShare> query)
        {
            using var db = CreateDbContext();

            return [.. GetObjectShares(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of object shares using the supplied DbContext.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The DbContext.</param>
        /// <returns>The filtered collection.</returns>
        public static IEnumerable<ObjectShare> GetObjectShares(IQuery<ObjectShare> query, KleeneStarDbContext context)
        {
            var data = context.ObjectShares
                .Include(x => x.Identity)
                .Include(x => x.Object)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Adds the supplied share relationship to the database. The unique composite
        /// index on (ObjectId, IdentityId) blocks duplicates on the database side;
        /// adding an already existing share is tolerated silently.
        /// </summary>
        /// <param name="share">The share relationship to add.</param>
        public static void Add(ObjectShare share)
        {
            ArgumentNullException.ThrowIfNull(share);

            using var db = CreateDbContext();

            if (db.ObjectShares.Any(x => x.ObjectId == share.ObjectId && x.IdentityId == share.IdentityId))
            {
                return;
            }

            if (share.Created == default)
            {
                share.Created = DateTime.UtcNow;
            }

            db.ObjectShares.Add(share);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the supplied share relationship from the database. No-op when no
        /// matching row exists.
        /// </summary>
        /// <param name="share">The share relationship to remove.</param>
        public static void Remove(ObjectShare share)
        {
            ArgumentNullException.ThrowIfNull(share);

            using var db = CreateDbContext();

            var existing = db.ObjectShares.FirstOrDefault(x => x.Id == share.Id);

            if (existing is null)
            {
                return;
            }

            db.ObjectShares.Remove(existing);
            db.SaveChanges();
        }
    }
}
