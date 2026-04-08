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
        /// Returns a queryable collection of dashboards from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND. The query includes related category and widget data for each dashboard.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned dashboards. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of dashboards. The query
        /// includes related categories and widgets and is not tracked by the context.
        /// </returns>
        public static IEnumerable<Dashboard> GetDashboards(IQuery<Dashboard> query)
        {
            using var db = CreateDbContext();

            return [.. GetDashboards(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of dashboards from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned dashboards. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of dashboards. The query
        /// includes related categories and widgets and is not tracked by the context.
        /// </returns>
        public static IEnumerable<Dashboard> GetDashboards(IQuery<Dashboard> query, KleeneStarDbContext context)
        {
            var data = context.Dashboards
                .AsNoTracking()
                .Include(d => d.Categories)
                .Include(d => d.Columns)
                    .ThenInclude(c => c.Widgets);

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified dashboard to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a dashboard with the same identifier already exists in the database, this method does nothing.
        /// </remarks>
        /// <param name="dashboard">
        /// The dashboard to add. The dashboard's Id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Dashboard dashboard)
        {
            ArgumentNullException.ThrowIfNull(dashboard);

            using var db = CreateDbContext();

            var query = new Query<Dashboard>()
                .WhereEquals(x => x.Id, dashboard.Id);

            if (query.Apply(db.Dashboards).Any())
            {
                return;
            }

            db.AddEntity(dashboard, ["Categories"]);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified dashboard in the database.
        /// </summary>
        /// <param name="dashboard">
        /// The dashboard to update. Cannot be null.
        /// </param>
        public static void Update(Dashboard dashboard)
        {
            ArgumentNullException.ThrowIfNull(dashboard);

            using var db = CreateDbContext();

            db.UpdateEntity(dashboard, ["Categories"]);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified dashboard from the data store if it exists.
        /// </summary>
        /// <param name="dashboard">
        /// The dashboard entity to remove.
        /// </param>
        public static void Remove(Dashboard dashboard)
        {
            ArgumentNullException.ThrowIfNull(dashboard);

            using var db = CreateDbContext();

            db.RemoveEntity(dashboard, ["Categories"]);

            // persist changes
            db.SaveChanges();
        }
    }
}
