using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides the data-access helpers for <see cref="SavedSearch"/> entities.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns a materialized collection of saved searches matching the given query.
        /// </summary>
        /// <param name="query">The query criteria used to filter the saved searches. Must not be null.</param>
        /// <returns>The filtered saved searches.</returns>
        public static IEnumerable<SavedSearch> GetSavedSearches(IQuery<SavedSearch> query)
        {
            using var db = CreateDbContext();

            return [.. GetSavedSearches(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a collection of saved searches matching the given query, evaluated against
        /// the supplied context.
        /// </summary>
        /// <param name="query">The query criteria used to filter the saved searches. Must not be null.</param>
        /// <param name="context">The context in which the query is executed. Cannot be null.</param>
        /// <returns>The filtered saved searches (not tracked).</returns>
        public static IEnumerable<SavedSearch> GetSavedSearches(IQuery<SavedSearch> query, KleeneStarDbContext context)
        {
            var data = context.SavedSearches
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified saved search to the database if it does not already exist.
        /// </summary>
        /// <param name="savedSearch">The saved search to add. Cannot be null.</param>
        public static void Add(SavedSearch savedSearch)
        {
            ArgumentNullException.ThrowIfNull(savedSearch);

            using var db = CreateDbContext();

            var query = new Query<SavedSearch>()
                .WhereEquals(x => x.Id, savedSearch.Id);

            if (query.Apply(db.SavedSearches).Any())
            {
                return;
            }

            db.AddEntity(savedSearch);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified saved search in the database.
        /// </summary>
        /// <param name="savedSearch">The saved search to update. Cannot be null.</param>
        public static void Update(SavedSearch savedSearch)
        {
            ArgumentNullException.ThrowIfNull(savedSearch);

            using var db = CreateDbContext();

            db.UpdateEntity(savedSearch);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified saved search from the data store if it exists.
        /// </summary>
        /// <param name="savedSearch">The saved search entity to remove.</param>
        public static void Remove(SavedSearch savedSearch)
        {
            ArgumentNullException.ThrowIfNull(savedSearch);

            using var db = CreateDbContext();

            db.RemoveEntity(savedSearch);

            // persist changes
            db.SaveChanges();
        }
    }
}
