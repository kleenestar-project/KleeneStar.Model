using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with object-to-object links.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the links that satisfy the supplied query. Source and target objects
        /// are eagerly hydrated.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The materialized collection of links.</returns>
        public static IEnumerable<ObjectLink> GetObjectLinks(IQuery<ObjectLink> query)
        {
            using var db = CreateDbContext();

            return [.. GetObjectLinks(query, db)];
        }

        /// <summary>
        /// Returns the links that satisfy the supplied query, executed inside the
        /// supplied DbContext.
        /// </summary>
        public static IEnumerable<ObjectLink> GetObjectLinks(IQuery<ObjectLink> query, KleeneStarDbContext context)
        {
            var data = context.ObjectLinks
                .AsNoTracking()
                .Include(x => x.SourceObject)
                .Include(x => x.TargetObject);

            return query.Apply(data);
        }

        /// <summary>
        /// Adds the supplied link to the database if no entry with the same id exists.
        /// </summary>
        /// <param name="link">The link to add. Cannot be null.</param>
        public static void Add(ObjectLink link)
        {
            ArgumentNullException.ThrowIfNull(link);

            using var db = CreateDbContext();

            var query = new Query<ObjectLink>()
                .WhereEquals(x => x.Id, link.Id);

            if (query.Apply(db.ObjectLinks).Any())
            {
                return;
            }

            if (link.Created == default)
            {
                link.Created = DateTime.UtcNow;
            }

            link.Updated = DateTime.UtcNow;

            db.ObjectLinks.Add(link);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the supplied link from the database.
        /// </summary>
        /// <param name="link">The link to remove. Cannot be null.</param>
        public static void Remove(ObjectLink link)
        {
            ArgumentNullException.ThrowIfNull(link);

            using var db = CreateDbContext();

            var existing = db.ObjectLinks.FirstOrDefault(x => x.Id == link.Id);

            if (existing is null)
            {
                return;
            }

            db.ObjectLinks.Remove(existing);
            db.SaveChanges();
        }
    }
}
