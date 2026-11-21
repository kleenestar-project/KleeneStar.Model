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
        /// Returns a queryable collection of navigator links from the database, optionally filtered
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned navigator links. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of navigator links.
        /// </returns>
        public static IEnumerable<NavigatorLink> GetNavigatorLinks(IQuery<NavigatorLink> query)
        {
            using var db = CreateDbContext();

            return [.. GetNavigatorLinks(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of navigator links from the database, optionally filtered
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned navigator links. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of navigator links.
        /// </returns>
        public static IEnumerable<NavigatorLink> GetNavigatorLinks(IQuery<NavigatorLink> query, KleeneStarDbContext context)
        {
            var data = context.NavigatorLinks
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified navigator link to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a navigator link with the same id already exists in the database, this method does nothing.
        /// </remarks>
        /// <param name="navigatorLinkEntry">
        /// The navigator link to add. The link's id property is used to determine uniqueness.
        /// Cannot be null.
        /// </param>
        public static void Add(NavigatorLink navigatorLinkEntry)
        {
            ArgumentNullException.ThrowIfNull(navigatorLinkEntry);

            using var db = CreateDbContext();

            var query = new Query<NavigatorLink>()
                .WhereEquals(x => x.Id, navigatorLinkEntry.Id);

            if (query.Apply(db.NavigatorLinks).Any())
            {
                return;
            }

            db.AddEntity(navigatorLinkEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified navigator link in the database.
        /// </summary>
        /// <param name="navigatorLinkEntry">
        /// The navigator link to update. Cannot be null.
        /// </param>
        public static void Update(NavigatorLink navigatorLinkEntry)
        {
            ArgumentNullException.ThrowIfNull(navigatorLinkEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(navigatorLinkEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified navigator link from the data store if it exists.
        /// </summary>
        /// <param name="navigatorLinkEntry">
        /// The navigator link entity to remove.
        /// </param>
        public static void Remove(NavigatorLink navigatorLinkEntry)
        {
            ArgumentNullException.ThrowIfNull(navigatorLinkEntry);

            using var db = CreateDbContext();

            db.RemoveEntity(navigatorLinkEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
