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
        /// Returns the single visit row of the supplied identity for the supplied object, or
        /// <see langword="null"/> when the identity has neither visited nor starred it.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <returns>The visit, or <see langword="null"/>.</returns>
        public static ObjectVisit GetObjectVisit(Guid ownerId, Guid objectId)
        {
            using var db = CreateDbContext();

            return db.ObjectVisits
                .Include(x => x.Object)
                .AsNoTracking()
                .FirstOrDefault(x => x.OwnerId == ownerId && x.ObjectId == objectId);
        }

        /// <summary>
        /// Inserts or updates the visit row of the supplied identity for the supplied object.
        /// The unique composite index on (Owner, Object) guarantees a single row per pair; an
        /// existing row is mutated in place. Returns <see langword="null"/> when either the
        /// owner or the object does not exist (the foreign keys would otherwise reject the
        /// write).
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="objectId">The id of the object.</param>
        /// <param name="favorite">
        /// When set, the new starred state; when <see langword="null"/>, the starred state is
        /// left untouched (used by "record visit").
        /// </param>
        /// <param name="recordVisit">
        /// When <see langword="true"/>, the last-visited timestamp is advanced to now (used by
        /// "record visit"); when <see langword="false"/>, it is left untouched.
        /// </param>
        /// <param name="liked">
        /// When set, the new liked state; when <see langword="null"/>, the liked state is left
        /// untouched. Every caller that is not the like itself passes null, which is why it is
        /// optional: recording a visit must not silently take somebody's like away.
        /// </param>
        /// <returns>The persisted visit, or <see langword="null"/>.</returns>
        public static ObjectVisit UpsertObjectVisit(Guid ownerId, Guid objectId, bool? favorite, bool recordVisit, bool? liked = null)
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
                    Favorite = favorite ?? false,
                    Liked = liked ?? false,
                    LastVisited = recordVisit ? now : default,
                    Created = now,
                    Updated = now
                };

                db.ObjectVisits.Add(visit);
            }
            else
            {
                if (favorite.HasValue)
                {
                    visit.Favorite = favorite.Value;
                }

                if (liked.HasValue)
                {
                    visit.Liked = liked.Value;
                }

                if (recordVisit)
                {
                    visit.LastVisited = now;
                }

                visit.Updated = now;
            }

            db.SaveChanges();

            return visit;
        }
    }
}
