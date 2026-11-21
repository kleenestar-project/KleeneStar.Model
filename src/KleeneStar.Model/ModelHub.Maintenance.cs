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
        /// Returns the maintenance notices from the database matching the given criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned notices. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of maintenance notices.
        /// </returns>
        public static IEnumerable<Maintenance> GetMaintenances(IQuery<Maintenance> query)
        {
            using var db = CreateDbContext();

            return [.. GetMaintenances(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns the maintenance notices from the database matching the given criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned notices. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of maintenance notices.
        /// </returns>
        public static IEnumerable<Maintenance> GetMaintenances(IQuery<Maintenance> query, KleeneStarDbContext context)
        {
            var data = context.Maintenances
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified maintenance notice to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a notice with the same id already exists in the database, this method does nothing.
        /// </remarks>
        /// <param name="maintenanceEntry">
        /// The maintenance notice to add. The notice's id property is used to determine uniqueness.
        /// Cannot be null.
        /// </param>
        public static void Add(Maintenance maintenanceEntry)
        {
            ArgumentNullException.ThrowIfNull(maintenanceEntry);

            using var db = CreateDbContext();

            var query = new Query<Maintenance>()
                .WhereEquals(x => x.Id, maintenanceEntry.Id);

            if (query.Apply(db.Maintenances).Any())
            {
                return;
            }

            db.AddEntity(maintenanceEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified maintenance notice in the database.
        /// </summary>
        /// <param name="maintenanceEntry">
        /// The maintenance notice to update. Cannot be null.
        /// </param>
        public static void Update(Maintenance maintenanceEntry)
        {
            ArgumentNullException.ThrowIfNull(maintenanceEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(maintenanceEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
