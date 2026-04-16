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
        /// Retrieves a collection of status categories that match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// The query used to filter and select status categories. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of status categories that satisfy the query conditions. The 
        /// collection is empty if no status categories match the query.
        /// </returns>
        public static IEnumerable<StatusCategory> GetStatusCategories(IQuery<StatusCategory> query)
        {
            using var db = CreateDbContext();

            return [.. GetStatusCategories(query, db)]; // materialize query
        }

        /// <summary>
        /// Retrieves a collection of status categories that match the specified query criteria.
        /// </summary>
        /// <param name="query">
        /// The query used to filter and select status categories. Cannot be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of status categories that satisfy the query conditions. The 
        /// collection is empty if no status categories match the query.
        /// </returns>
        public static IEnumerable<StatusCategory> GetStatusCategories(IQuery<StatusCategory> query, KleeneStarDbContext context)
        {
            var data = context.StatusCategories
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Returns a queryable collection of workflow states from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND. The query includes related category data for each workflow state.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned workflow states. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of workflow states.
        /// </returns>
        public static IEnumerable<Status> GetStatuses(IQuery<Status> query)
        {
            using var db = CreateDbContext();

            return [.. GetStatuses(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of workflow states from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned workflow states. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of workflow states.
        /// </returns>
        public static IEnumerable<Status> GetStatuses(IQuery<Status> query, KleeneStarDbContext context)
        {
            var data = context.Statuses
                .Include(x => x.Category)
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified workflow state to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a workflow state with the same key (case-insensitive) already exists in the 
        /// database, this method does nothing.
        /// </remarks>
        /// <param name="workflowStateEntry">
        /// The workflow state to add. The workflow state's id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Status workflowStateEntry)
        {
            ArgumentNullException.ThrowIfNull(workflowStateEntry);

            using var db = CreateDbContext();

            var query = new Query<Status>()
                .WhereEquals(x => x.Id, workflowStateEntry.Id);

            if (query.Apply(db.Statuses).Any())
            {
                return;
            }

            db.AddEntity(workflowStateEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified workflow state in the database.
        /// </summary>
        /// <param name="workflowStateEntry">
        /// The workflow state to update. Cannot be null.
        /// </param>
        public static void Update(Status workflowStateEntry)
        {
            ArgumentNullException.ThrowIfNull(workflowStateEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(workflowStateEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified workflow state from the data store if it exists.
        /// </summary>
        /// <param name="workflowStateEntry">
        /// The workflow state entity to remove.
        /// </param>
        public static void Remove(Status workflowStateEntry)
        {
            ArgumentNullException.ThrowIfNull(workflowStateEntry);

            using var db = CreateDbContext();

            db.RemoveEntity(workflowStateEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
