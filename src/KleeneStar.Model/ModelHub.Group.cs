using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with groups.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns a materialized collection of groups, optionally filtered by a query.
        /// </summary>
        /// <param name="query">The query criteria used to filter the returned groups.</param>
        /// <returns>A materialized enumeration of groups.</returns>
        public static IEnumerable<Group> GetGroups(IQuery<Group> query)
        {
            using var db = CreateDbContext();

            return [.. GetGroups(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of groups, optionally filtered by a query.
        /// </summary>
        /// <param name="query">The query criteria used to filter the returned groups.</param>
        /// <param name="context">The database context used to execute the query.</param>
        /// <returns>An enumeration representing the filtered collection of groups.</returns>
        public static IEnumerable<Group> GetGroups(IQuery<Group> query, KleeneStarDbContext context)
        {
            var data = context.Groups
                .AsNoTracking()
                .Include(x => x.GroupPolicies)
                .Include(x => x.GroupMemberships)
                    .ThenInclude(x => x.Identity);

            return query.Apply(data);
        }

        /// <summary>
        /// Adds the specified group to the database if it does not already exist.
        /// </summary>
        /// <param name="group">The group to add.</param>
        public static void Add(Group group)
        {
            ArgumentNullException.ThrowIfNull(group);

            using var db = CreateDbContext();

            var query = new Query<Group>()
                .WhereEquals(x => x.Id, group.Id);

            if (query.Apply(db.Groups).Any())
            {
                return;
            }

            db.AddEntity(group);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified group in the database.
        /// </summary>
        /// <param name="group">The group to update.</param>
        public static void Update(Group group)
        {
            ArgumentNullException.ThrowIfNull(group);

            using var db = CreateDbContext();

            db.UpdateEntity(group);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified group from the database.
        /// </summary>
        /// <param name="group">The group to remove.</param>
        public static void Remove(Group group)
        {
            ArgumentNullException.ThrowIfNull(group);

            using var db = CreateDbContext();

            db.RemoveEntity(group);
            db.SaveChanges();
        }
    }
}