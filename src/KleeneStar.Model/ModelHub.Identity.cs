using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with identities.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns a materialized collection of identities, optionally filtered by a query.
        /// </summary>
        /// <param name="query">The query criteria used to filter the returned identities.</param>
        /// <returns>A materialized enumeration of identities.</returns>
        public static IEnumerable<Identity> GetIdentities(IQuery<Identity> query)
        {
            using var db = CreateDbContext();

            return [.. GetIdentities(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of identities, optionally filtered by a query.
        /// </summary>
        /// <param name="query">The query criteria used to filter the returned identities.</param>
        /// <param name="context">The database context used to execute the query.</param>
        /// <returns>An enumeration representing the filtered collection of identities.</returns>
        public static IEnumerable<Identity> GetIdentities(IQuery<Identity> query, KleeneStarDbContext context)
        {
            var data = context.Identities
                .AsNoTracking()
                .Include(x => x.GroupMemberships)
                    .ThenInclude(x => x.Group);

            return query.Apply(data);
        }

        /// <summary>
        /// Adds the specified identity to the database if it does not already exist.
        /// </summary>
        /// <param name="identity">The identity to add.</param>
        public static void Add(Identity identity)
        {
            ArgumentNullException.ThrowIfNull(identity);

            using var db = CreateDbContext();

            var query = new Query<Identity>()
                .WhereEquals(x => x.Id, identity.Id);

            if (query.Apply(db.Identities).Any())
            {
                return;
            }

            db.AddEntity(identity);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified identity in the database.
        /// </summary>
        /// <param name="identity">The identity to update.</param>
        public static void Update(Identity identity)
        {
            ArgumentNullException.ThrowIfNull(identity);

            using var db = CreateDbContext();

            db.UpdateEntity(identity);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified identity from the database.
        /// </summary>
        /// <param name="identity">The identity to remove.</param>
        public static void Remove(Identity identity)
        {
            ArgumentNullException.ThrowIfNull(identity);

            using var db = CreateDbContext();

            db.RemoveEntity(identity);
            db.SaveChanges();
        }
    }
}