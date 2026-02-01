using KleeneStar.Model.Entity;
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
    public static partial class ModelHub
    {
        /// <summary>
        /// Returns a queryable collection of workspaces from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND. The query includes related category data for each workspace.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned workspaces. Must not be null.
        /// </param>
        /// <returns>An enumeration representing the filtered collection of workspaces. The query
        /// includes related categories and is not tracked by the context.</returns>
        public static IEnumerable<Workspace> GetWorkspaces(IQuery<Workspace> query)
        {
            using var db = CreateDbContext();

            var data = db.Workspaces
                .AsNoTracking()
                .Include(w => w.Categories);

            return [.. query.Apply(data)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of workspaces from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned workspaces. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>An enumeration representing the filtered collection of workspaces. The query
        /// includes related categories and is not tracked by the context.</returns>
        public static IEnumerable<Workspace> GetWorkspaces(IQuery<Workspace> query, KleeneStarDbContext context)
        {
            var data = context.Workspaces
                .AsNoTracking()
                .Include(w => w.Categories);

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified workspace to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a workspace with the same key (case-insensitive) already exists in the 
        /// database, this method does nothing.
        /// </remarks>
        /// <param name="workspace">
        /// The workspace to add. The workspace's Key property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Workspace workspace)
        {
            using var db = CreateDbContext();
            var exists = db.Workspaces.Any(x => x.Key.Equals(workspace.Key, StringComparison.InvariantCultureIgnoreCase));

            if (exists)
            {
                return;
            }

            db.Workspaces.Add(workspace);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified workspace from the data store if it exists.
        /// </summary>
        /// <param name="workspace">
        /// The workspace entity to remove. The workspace is identified by 
        /// its <c>Id</c> property.
        /// </param>
        public static void Remove(Workspace workspace)
        {
            using var db = CreateDbContext();

            var existing = db.Workspaces
                .FirstOrDefault(x => x.Id == workspace.Id);

            if (existing == null)
            {
                return;
            }

            db.Workspaces.Remove(existing);
            db.SaveChanges();
        }
    }
}
