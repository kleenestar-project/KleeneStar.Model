using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
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
        /// Returns a queryable collection of objects from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND. The query includes related Workspace data for each object.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned objects. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of objects.
        /// </returns>
        public static IEnumerable<Object> GetObjects(IQuery<Object> query)
        {
            using var db = CreateDbContext();

            return [.. GetObjects(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of objects from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned objects. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of objects.
        /// </returns>
        public static IEnumerable<Object> GetObjects(IQuery<Object> query, KleeneStarDbContext context)
        {
            var data = context.Objects
                .AsNoTracking()
                .Include(x => x.Workspace);

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified object to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a object with the same key (case-insensitive) already exists in the 
        /// database, this method does nothing.
        /// </remarks>
        /// <param name="objectEntry">
        /// The object to add. The object's id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Object objectEntry)
        {
            System.ArgumentNullException.ThrowIfNull(objectEntry);

            using var db = CreateDbContext();

            var query = new Query<Object>()
                .WhereEquals(x => x.Id, objectEntry.Id);

            if (query.Apply(db.Objects).Any())
            {
                return;
            }

            db.AddEntity(objectEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified object in the database.
        /// </summary>
        /// <param name="objectEntry">
        /// The object to update. Cannot be null.
        /// </param>
        public static void Update(Object objectEntry)
        {
            System.ArgumentNullException.ThrowIfNull(objectEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(objectEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified object from the data store if it exists.
        /// </summary>
        /// <param name="objectEntry">
        /// The object entity to remove.
        /// </param>
        public static void Remove(Object objectEntry)
        {
            System.ArgumentNullException.ThrowIfNull(objectEntry);

            using var db = CreateDbContext();

            db.RemoveEntity(objectEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
