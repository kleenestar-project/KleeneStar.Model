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
        /// Returns the user-defined quickfilters from the database matching the given criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned quickfilters. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of quickfilters.
        /// </returns>
        public static IEnumerable<CustomQuickfilter> GetCustomQuickfilters(IQuery<CustomQuickfilter> query)
        {
            using var db = CreateDbContext();

            return [.. GetCustomQuickfilters(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns the user-defined quickfilters from the database matching the given criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned quickfilters. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of quickfilters.
        /// </returns>
        public static IEnumerable<CustomQuickfilter> GetCustomQuickfilters(IQuery<CustomQuickfilter> query, KleeneStarDbContext context)
        {
            var data = context.CustomQuickfilters
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified quickfilter to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a quickfilter with the same id already exists in the database, this method does nothing.
        /// </remarks>
        /// <param name="quickfilterEntry">
        /// The quickfilter to add. The quickfilter's id property is used to determine uniqueness.
        /// Cannot be null.
        /// </param>
        public static void Add(CustomQuickfilter quickfilterEntry)
        {
            ArgumentNullException.ThrowIfNull(quickfilterEntry);

            using var db = CreateDbContext();

            var query = new Query<CustomQuickfilter>()
                .WhereEquals(x => x.Id, quickfilterEntry.Id);

            if (query.Apply(db.CustomQuickfilters).Any())
            {
                return;
            }

            db.AddEntity(quickfilterEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified quickfilter in the database.
        /// </summary>
        /// <param name="quickfilterEntry">
        /// The quickfilter to update. Cannot be null.
        /// </param>
        public static void Update(CustomQuickfilter quickfilterEntry)
        {
            ArgumentNullException.ThrowIfNull(quickfilterEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(quickfilterEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified quickfilter from the data store if it exists.
        /// </summary>
        /// <param name="quickfilterEntry">
        /// The quickfilter entity to remove.
        /// </param>
        public static void Remove(CustomQuickfilter quickfilterEntry)
        {
            ArgumentNullException.ThrowIfNull(quickfilterEntry);

            using var db = CreateDbContext();

            db.RemoveEntity(quickfilterEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
