using KleeneStar.Model.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the KleeneStar.
    /// </summary>
    public static partial class ModelHub
    {
        /// <summary>
        /// Returns all workspaces.
        /// </summary>
        public static IEnumerable<Workspace> Workspaces
        {
            get
            {
                using var db = CreateDbContext();

                return
                [..
                    db.Workspaces
                        .AsNoTracking()
                        .Include(w => w.Categories)
                ];
            }
        }

        /// <summary>
        /// Retrieves a collection of workspaces that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="predicate">
        /// An expression used to filter the workspaces. Only workspaces for which the predicate 
        /// evaluates to true are included in the result. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of workspaces that match the specified predicate. The 
        /// collection is empty if no workspaces meet the criteria.
        /// </returns>
        public static IEnumerable<Workspace> GetWorkspaces(Expression<Func<Workspace, bool>> predicate)
        {
            using var db = CreateDbContext();

            return
            [..
                db.Workspaces
                    .AsNoTracking()
                    .Where(predicate)
                    .Include(w => w.Categories)
            ];
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
