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
        public static IEnumerable<WorkflowState> GetWorkflowStates(IQuery<WorkflowState> query)
        {
            using var db = CreateDbContext();

            return [.. GetWorkflowStates(query, db)]; // materialize query
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
        public static IEnumerable<WorkflowState> GetWorkflowStates(IQuery<WorkflowState> query, KleeneStarDbContext context)
        {
            var data = context.WorkflowStates
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Returns a queryable collection of workflows from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND. The query includes related category data for each workflow.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned workflows. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of workflows.
        /// </returns>
        public static IEnumerable<Workflow> GetWorkflows(IQuery<Workflow> query)
        {
            using var db = CreateDbContext();

            return [.. GetWorkflows(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of workflows from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned workflows. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of workflows.
        /// </returns>
        public static IEnumerable<Workflow> GetWorkflows(IQuery<Workflow> query, KleeneStarDbContext context)
        {
            var data = context.Workflows
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
        public static void Add(WorkflowState workflowStateEntry)
        {
            ArgumentNullException.ThrowIfNull(workflowStateEntry);

            using var db = CreateDbContext();

            var query = new Query<WorkflowState>()
                .WhereEquals(x => x.Id, workflowStateEntry.Id);

            if (query.Apply(db.WorkflowStates).Any())
            {
                return;
            }

            db.AddEntity(workflowStateEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Adds the specified workflow to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a workflow with the same key (case-insensitive) already exists in the 
        /// database, this method does nothing.
        /// </remarks>
        /// <param name="workflowEntry">
        /// The workflow to add. The workflow's id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Workflow workflowEntry)
        {
            ArgumentNullException.ThrowIfNull(workflowEntry);

            using var db = CreateDbContext();

            var query = new Query<Workflow>()
                .WhereEquals(x => x.Id, workflowEntry.Id);

            if (query.Apply(db.Workflows).Any())
            {
                return;
            }

            db.AddEntity(workflowEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified workflow state in the database.
        /// </summary>
        /// <param name="workflowStateEntry">
        /// The workflow state to update. Cannot be null.
        /// </param>
        public static void Update(WorkflowState workflowStateEntry)
        {
            ArgumentNullException.ThrowIfNull(workflowStateEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(workflowStateEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified workflow in the database.
        /// </summary>
        /// <param name="workflowEntry">
        /// The workflow to update. Cannot be null.
        /// </param>
        public static void Update(Workflow workflowEntry)
        {
            ArgumentNullException.ThrowIfNull(workflowEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(workflowEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified workflow state from the data store if it exists.
        /// </summary>
        /// <param name="workflowStateEntry">
        /// The workflow state entity to remove.
        /// </param>
        public static void Remove(WorkflowState workflowStateEntry)
        {
            ArgumentNullException.ThrowIfNull(workflowStateEntry);

            using var db = CreateDbContext();

            db.RemoveEntity(workflowStateEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified workflow from the data store if it exists.
        /// </summary>
        /// <param name="workflowEntry">
        /// The workflow entity to remove.
        /// </param>
        public static void Remove(Workflow workflowEntry)
        {
            ArgumentNullException.ThrowIfNull(workflowEntry);

            using var db = CreateDbContext();

            db.RemoveEntity(workflowEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
