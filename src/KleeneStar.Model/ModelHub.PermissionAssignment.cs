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
        /// Returns the permission assignments matching the given criteria, with the granted group
        /// loaded so the dialog can name it.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned assignments. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of assignments.
        /// </returns>
        public static IEnumerable<PermissionAssignment> GetPermissionAssignments(IQuery<PermissionAssignment> query)
        {
            using var db = CreateDbContext();

            return [.. GetPermissionAssignments(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns the permission assignments matching the given criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned assignments. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of assignments.
        /// </returns>
        public static IEnumerable<PermissionAssignment> GetPermissionAssignments(IQuery<PermissionAssignment> query, KleeneStarDbContext context)
        {
            var data = context.PermissionAssignments
                .Include(x => x.Group)
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified assignment to the database if the same grant is not stored already.
        /// </summary>
        /// <param name="assignmentEntry">
        /// The assignment to add. Cannot be null.
        /// </param>
        public static void Add(PermissionAssignment assignmentEntry)
        {
            ArgumentNullException.ThrowIfNull(assignmentEntry);

            using var db = CreateDbContext();

            // the same policy granted to the same group on the same resource twice says nothing
            // more than granting it once
            var duplicate = db.PermissionAssignments.Any
            (
                x => x.Scope == assignmentEntry.Scope &&
                     x.ScopeId == assignmentEntry.ScopeId &&
                     x.GroupId == assignmentEntry.GroupId &&
                     x.Policy == assignmentEntry.Policy
            );

            if (duplicate)
            {
                return;
            }

            db.AddEntity(assignmentEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified assignment from the data store if it exists.
        /// </summary>
        /// <param name="assignmentEntry">
        /// The assignment to remove.
        /// </param>
        public static void Remove(PermissionAssignment assignmentEntry)
        {
            ArgumentNullException.ThrowIfNull(assignmentEntry);

            using var db = CreateDbContext();

            db.RemoveEntity(assignmentEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
