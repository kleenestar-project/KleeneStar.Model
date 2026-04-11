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
        /// Returns a queryable collection of tenants from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND. The query includes related category data for each tenant.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned tenants. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of tenants.
        /// </returns>
        public static IEnumerable<Tenant> GetTenants(IQuery<Tenant> query)
        {
            using var db = CreateDbContext();

            return [.. GetTenants(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of tenants from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned tenants. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of tenants.
        /// </returns>
        public static IEnumerable<Tenant> GetTenants(IQuery<Tenant> query, KleeneStarDbContext context)
        {
            var data = context.Tenants
                .Include(x => x.Workspaces)
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified tenant to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a tenant with the same key (case-insensitive) already exists in the 
        /// database, this method does nothing.
        /// </remarks>
        /// <param name="tenantEntry">
        /// The tenant to add. The tenant's id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Tenant tenantEntry)
        {
            ArgumentNullException.ThrowIfNull(tenantEntry);

            using var db = CreateDbContext();

            var query = new Query<Tenant>()
                .WhereEquals(x => x.Id, tenantEntry.Id);

            if (query.Apply(db.Tenants).Any())
            {
                return;
            }

            db.AddEntity(tenantEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified tenant in the database.
        /// </summary>
        /// <param name="tenantEntry">
        /// The tenant to update. Cannot be null.
        /// </param>
        public static void Update(Tenant tenantEntry)
        {
            ArgumentNullException.ThrowIfNull(tenantEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(tenantEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified tenant from the data store if it exists.
        /// </summary>
        /// <param name="tenantEntry">
        /// The tenant entity to remove.
        /// </param>
        public static void Remove(Tenant tenantEntry)
        {
            ArgumentNullException.ThrowIfNull(tenantEntry);

            using var db = CreateDbContext();

            db.RemoveEntity(tenantEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
