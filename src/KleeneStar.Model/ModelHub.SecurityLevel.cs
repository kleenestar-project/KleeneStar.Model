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
        /// Returns a queryable collection of security levels from the database, optionally
        /// filtered by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned security levels. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of security levels.
        /// </returns>
        public static IEnumerable<SecurityLevel> GetSecurityLevels(IQuery<SecurityLevel> query)
        {
            using var db = CreateDbContext();

            return [.. GetSecurityLevels(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of security levels from the database, optionally
        /// filtered by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned security levels. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or
        /// constraints for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of security levels.
        /// </returns>
        public static IEnumerable<SecurityLevel> GetSecurityLevels(IQuery<SecurityLevel> query, KleeneStarDbContext context)
        {
            var data = context.SecurityLevels
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified security level to the database if it does not already exist.
        /// </summary>
        /// <param name="securityLevelEntry">
        /// The security level to add. The id property is used to determine uniqueness.
        /// Cannot be null.
        /// </param>
        public static void Add(SecurityLevel securityLevelEntry)
        {
            ArgumentNullException.ThrowIfNull(securityLevelEntry);

            using var db = CreateDbContext();

            var query = new Query<SecurityLevel>()
                .WhereEquals(x => x.Id, securityLevelEntry.Id);

            if (query.Apply(db.SecurityLevels).Any())
            {
                return;
            }

            db.AddEntity(securityLevelEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified security level in the database.
        /// </summary>
        /// <param name="securityLevelEntry">
        /// The security level to update. Cannot be null.
        /// </param>
        public static void Update(SecurityLevel securityLevelEntry)
        {
            ArgumentNullException.ThrowIfNull(securityLevelEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(securityLevelEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified security level from the data store if it exists, declassifying
        /// every object that carried it.
        /// </summary>
        /// <remarks>
        /// The foreign key is declared <c>SetNull</c>, but the objects are cleared here as well:
        /// the rows are loaded and written by name, so the change is visible to the store
        /// whatever a provider makes of the delete behaviour, and an object never ends up
        /// pointing at a level that is gone - which the visibility check would have to read as
        /// "classified, cleared for nobody".
        /// </remarks>
        /// <param name="securityLevelEntry">
        /// The security level entity to remove.
        /// </param>
        public static void Remove(SecurityLevel securityLevelEntry)
        {
            ArgumentNullException.ThrowIfNull(securityLevelEntry);

            using var db = CreateDbContext();

            var classified = db.Objects
                .Where(x => x.SecurityLevelId == securityLevelEntry.Id)
                .ToList();

            foreach (var objectEntity in classified)
            {
                objectEntity.SecurityLevelId = null;
            }

            db.RemoveEntity(securityLevelEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
