using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides the data-access helpers for <see cref="ObjectVisit"/> entities.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns a materialized collection of object visits matching the given query, including
        /// the related owner and object.
        /// </summary>
        /// <param name="query">The query criteria used to filter the visits. Must not be null.</param>
        /// <returns>The filtered visits.</returns>
        public static IEnumerable<ObjectVisit> GetObjectVisits(IQuery<ObjectVisit> query)
        {
            using var db = CreateDbContext();

            return [.. GetObjectVisits(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a collection of object visits matching the given query, evaluated against the
        /// supplied context. The owner and object navigations are eagerly loaded.
        /// </summary>
        /// <param name="query">The query criteria used to filter the visits. Must not be null.</param>
        /// <param name="context">The context in which the query is executed. Cannot be null.</param>
        /// <returns>The filtered visits (not tracked).</returns>
        public static IEnumerable<ObjectVisit> GetObjectVisits(IQuery<ObjectVisit> query, KleeneStarDbContext context)
        {
            var data = context.ObjectVisits
                .Include(x => x.Owner)
                .Include(x => x.Object)
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Inserts or updates the visit of the supplied identity for the supplied object, advancing
        /// its last-visited timestamp to now. The unique composite index on (Owner, Object)
        /// guarantees a single row per pair; an existing row is mutated in place. Returns
        /// <see langword="null"/> when either the owner or the object does not exist (the foreign
        /// keys would otherwise reject the write).
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <returns>The persisted visit, or <see langword="null"/>.</returns>
        public static ObjectVisit UpsertObjectVisit(Guid ownerId, Guid objectId)
        {
            using var db = CreateDbContext();

            var ownerExists = db.Identities.AsNoTracking().Any(i => i.Id == ownerId);
            var objectExists = db.Objects.AsNoTracking().Any(o => o.Id == objectId);
            if (!ownerExists || !objectExists)
            {
                return null;
            }

            var now = DateTime.UtcNow;
            var visit = db.ObjectVisits
                .FirstOrDefault(x => x.OwnerId == ownerId && x.ObjectId == objectId);

            if (visit is null)
            {
                visit = new ObjectVisit
                {
                    OwnerId = ownerId,
                    ObjectId = objectId,
                    LastVisited = now,
                    Created = now,
                    Updated = now
                };

                db.ObjectVisits.Add(visit);
            }
            else
            {
                visit.LastVisited = now;
                visit.Updated = now;
            }

            db.SaveChanges();

            return visit;
        }
    }
}
