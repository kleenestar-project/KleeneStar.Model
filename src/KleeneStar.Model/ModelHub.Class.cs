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
        /// Returns a queryable collection of classes from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND. The query includes related Workspace data for each class.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned classes. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of classes.
        /// </returns>
        public static IEnumerable<Class> GetClasses(IQuery<Class> query)
        {
            using var db = CreateDbContext();

            return [.. GetClasses(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of classes from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned classes. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of classes.
        /// </returns>
        public static IEnumerable<Class> GetClasses(IQuery<Class> query, KleeneStarDbContext context)
        {
            var data = context.Classes
                .Include(x => x.Workspace)
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified class to the database if it does not already exist.
        /// A standard form is automatically created for the class.
        /// </summary>
        /// <remarks>
        /// If a class with the same key (case-insensitive) already exists in the 
        /// database, this method does nothing.
        /// </remarks>
        /// <param name="classEntry">
        /// The class to add. The class's id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Class classEntry)
        {
            ArgumentNullException.ThrowIfNull(classEntry);

            using var db = CreateDbContext();

            var query = new Query<Class>()
                .WhereEquals(x => x.Id, classEntry.Id);

            if (query.Apply(db.Classes).Any())
            {
                return;
            }

            db.AddEntity(classEntry);

            // automatically create a standard form for the new class
            var standardForm = new Form
            {
                Id = Guid.NewGuid(),
                Name = "Standard",
                Description = "Standard form for the class.",
                FormType = FormType.Standard,
                State = FormState.Active,
                ClassId = classEntry.Id,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            db.AddEntity(standardForm);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified class in the database.
        /// </summary>
        /// <param name="classEntry">
        /// The class to update. Cannot be null.
        /// </param>
        public static void Update(Class classEntry)
        {
            ArgumentNullException.ThrowIfNull(classEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(classEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified class from the data store if it exists.
        /// </summary>
        /// <param name="classEntry">
        /// The class entity to remove.
        /// </param>
        public static void Remove(Class classEntry)
        {
            ArgumentNullException.ThrowIfNull(classEntry);

            using var db = CreateDbContext();

            db.RemoveEntity(classEntry, ["Categories"]);

            // persist changes
            db.SaveChanges();
        }
    }
}
