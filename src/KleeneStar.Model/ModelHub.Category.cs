using KleeneStar.Model.Entity;
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
        /// Returns all categories.
        /// </summary>
        public static IEnumerable<Category> Categories
        {
            get
            {
                using var db = CreateDbContext();

                return [.. db.Categories];
            }
        }

        /// <summary>
        /// Retrieves a collection of categories that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="predicate">
        /// An expression that defines the conditions each category must meet to be included 
        /// in the result. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumerable collection of categories that match the specified predicate. The 
        /// collection is empty if no categories meet the criteria.
        /// </returns>
        public static IEnumerable<Category> GetCategories(Expression<Func<Category, bool>> predicate)
        {
            using var db = CreateDbContext();

            return [.. db.Categories.Where(predicate)];
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
