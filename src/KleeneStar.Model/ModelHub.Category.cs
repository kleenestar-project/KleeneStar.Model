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
        /// Retrieves a collection of categories that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// An object that defines the filtering, sorting, or projection to apply to the categories.
        /// </param>
        /// <returns>
        /// An enumerable collection of categories that match the specified predicate. The 
        /// collection is empty if no categories meet the criteria.
        /// </returns>
        public static IEnumerable<Category> GetCategories(IQuery<Category> query = null)
        {
            using var db = CreateDbContext();

            return [.. GetCategories(query ?? new Query<Category>(), db)]; // materialize query
        }

        /// <summary>
        /// Retrieves a collection of categories that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// An object that defines the filtering, sorting, or projection to apply to the categories.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of categories that match the specified predicate. The 
        /// collection is empty if no categories meet the criteria.
        /// </returns>
        public static IEnumerable<Category> GetCategories(IQuery<Category> query, KleeneStarDbContext context)
        {
            var data = context.Categories
               .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified category to the database if it does not already exist.
        /// </summary>
        /// <param name="category">
        /// The category to add to the database. Cannot be null.
        /// </param>
        public static void Add(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            using var db = CreateDbContext();
            var exists = db.Categories.Any(x => x.Name.Equals(category.Name, StringComparison.InvariantCultureIgnoreCase));

            if (exists)
            {
                return;
            }

            db.AddEntity(category);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified category from the database if it exists.
        /// </summary>
        /// <param name="category">
        /// The category to remove. The category must have a valid identifier corresponding 
        /// to an existing entry in the database.
        /// </param>
        public static void Remove(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            using var db = CreateDbContext();

            db.RemoveEntity(category, ["Workspaces"]);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes all categories that are not referenced by any workspace.
        /// </summary>
        public static void RemoveOrphanCategories()
        {
            using var db = CreateDbContext();

            // find categories that are not referenced by any workspace
            var usedCategoryIds = db.Set<Workspace>()
                .SelectMany(w => w.Categories.Select(c => c.RawId))
                .Distinct();

            var orphans = db.Set<Category>()
                .Where(c => !usedCategoryIds.Contains(c.RawId))
                .ToList();

            if (orphans.Count == 0)
            {
                return;
            }

            // remove all orphan categories
            foreach (var cat in orphans)
            {
                db.Remove(cat);
            }

            // persist changes
            db.SaveChanges();
        }
    }
}
