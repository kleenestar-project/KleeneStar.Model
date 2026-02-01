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
        /// Retrieves a collection of categories that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="query">
        /// The query used to filter and select categories. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of categories that match the specified predicate. The 
        /// collection is empty if no categories meet the criteria.
        /// </returns>
        public static IEnumerable<Category> GetCategories(IQuery<Category> query)
        {
            using var db = CreateDbContext();

            var data = db.Categories
               .AsNoTracking();

            return [.. query.Apply(data)]; // materialize query
        }

        /// <summary>
        /// Adds the specified category to the database if it does not already exist.
        /// </summary>
        /// <param name="category">
        /// The category to add to the database. Cannot be null.
        /// </param>
        public static void Add(Category category)
        {
            using var db = CreateDbContext();
            var exists = db.Categories.Any(x => x.Name.Equals(category.Name, StringComparison.InvariantCultureIgnoreCase));

            if (exists)
            {
                return;
            }

            db.Categories.Add(category);
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
            using var db = CreateDbContext();

            var existing = db.Categories
                .FirstOrDefault(x => x.Id == category.Id);

            if (existing == null)
            {
                return;
            }

            db.Categories.Remove(existing);
            db.SaveChanges();
        }
    }
}
