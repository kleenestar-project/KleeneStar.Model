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
        /// Returns the branding records from the database matching the given criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned records. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of branding records.
        /// </returns>
        public static IEnumerable<Branding> GetBrandings(IQuery<Branding> query)
        {
            using var db = CreateDbContext();

            return [.. GetBrandings(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns the branding records from the database matching the given criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned records. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of branding records.
        /// </returns>
        public static IEnumerable<Branding> GetBrandings(IQuery<Branding> query, KleeneStarDbContext context)
        {
            var data = context.Brandings
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified branding record to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a record with the same id already exists in the database, this method does nothing.
        /// </remarks>
        /// <param name="brandingEntry">
        /// The branding record to add. The record's id property is used to determine uniqueness.
        /// Cannot be null.
        /// </param>
        public static void Add(Branding brandingEntry)
        {
            ArgumentNullException.ThrowIfNull(brandingEntry);

            using var db = CreateDbContext();

            var query = new Query<Branding>()
                .WhereEquals(x => x.Id, brandingEntry.Id);

            if (query.Apply(db.Brandings).Any())
            {
                return;
            }

            db.AddEntity(brandingEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified branding record in the database.
        /// </summary>
        /// <param name="brandingEntry">
        /// The branding record to update. Cannot be null.
        /// </param>
        public static void Update(Branding brandingEntry)
        {
            ArgumentNullException.ThrowIfNull(brandingEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(brandingEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Writes the branding record, inserting it when the installation has none yet.
        /// </summary>
        /// <remarks>
        /// A singleton is written before it is ever read: the first administrator to open the
        /// settings page saves a record that does not exist, and <see cref="Update(Branding)"/>
        /// alone would throw on it. Rather than making every caller check first, the write itself
        /// decides which of the two operations it is.
        /// </remarks>
        /// <param name="brandingEntry">The branding record to store. Cannot be null.</param>
        public static void Save(Branding brandingEntry)
        {
            ArgumentNullException.ThrowIfNull(brandingEntry);

            using var db = CreateDbContext();

            var query = new Query<Branding>()
                .WhereEquals(x => x.Id, brandingEntry.Id);

            if (query.Apply(db.Brandings).Any())
            {
                db.UpdateEntity(brandingEntry);
            }
            else
            {
                db.AddEntity(brandingEntry);
            }

            // persist changes
            db.SaveChanges();
        }
    }
}
