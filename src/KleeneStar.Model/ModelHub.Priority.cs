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
        /// Returns a queryable collection of priorities from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned priorities. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of priorities.
        /// </returns>
        public static IEnumerable<Priority> GetPriorities(IQuery<Priority> query)
        {
            using var db = CreateDbContext();

            return [.. GetPriorities(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of priorities from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned priorities. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of priorities.
        /// </returns>
        public static IEnumerable<Priority> GetPriorities(IQuery<Priority> query, KleeneStarDbContext context)
        {
            var data = context.Priorities
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified priority to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a priority with the same key (case-insensitive) already exists in the 
        /// database, this method does nothing.
        /// </remarks>
        /// <param name="priorityEntry">
        /// The priority to add. The priority's id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Priority priorityEntry)
        {
            ArgumentNullException.ThrowIfNull(priorityEntry);

            using var db = CreateDbContext();

            var query = new Query<Priority>()
                .WhereEquals(x => x.Id, priorityEntry.Id);

            if (query.Apply(db.Priorities).Any())
            {
                return;
            }

            db.AddEntity(priorityEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified priority in the database.
        /// </summary>
        /// <param name="priorityEntry">
        /// The priority to update. Cannot be null.
        /// </param>
        public static void Update(Priority priorityEntry)
        {
            ArgumentNullException.ThrowIfNull(priorityEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(priorityEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified priority from the data store if it exists.
        /// </summary>
        /// <param name="priorityEntry">
        /// The priority entity to remove.
        /// </param>
        public static void Remove(Priority priorityEntry)
        {
            ArgumentNullException.ThrowIfNull(priorityEntry);

            using var db = CreateDbContext();

            db.RemoveEntity(priorityEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
