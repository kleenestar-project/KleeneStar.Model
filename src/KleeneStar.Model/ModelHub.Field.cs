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
        /// Returns a queryable collection of fields from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND. The query includes related category data for each field.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned fields. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of fields.
        /// </returns>
        public static IEnumerable<Field> GetFields(IQuery<Field> query)
        {
            using var db = CreateDbContext();

            return [.. GetFields(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of fields from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned fields. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of fields.
        /// </returns>
        public static IEnumerable<Field> GetFields(IQuery<Field> query, KleeneStarDbContext context)
        {
            var data = context.Fields
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified field to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a field with the same key (case-insensitive) already exists in the 
        /// database, this method does nothing.
        /// </remarks>
        /// <param name="fieldEntry">
        /// The field to add. The field's id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Field fieldEntry)
        {
            ArgumentNullException.ThrowIfNull(fieldEntry);

            using var db = CreateDbContext();

            var query = new Query<Field>()
                .WhereEquals(x => x.Id, fieldEntry.Id);

            if (query.Apply(db.Fields).Any())
            {
                return;
            }

            db.AddEntity(fieldEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified field in the database.
        /// </summary>
        /// <param name="fieldEntry">
        /// The field to update. Cannot be null.
        /// </param>
        public static void Update(Field fieldEntry)
        {
            ArgumentNullException.ThrowIfNull(fieldEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(fieldEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified field from the data store if it exists.
        /// </summary>
        /// <param name="fieldEntry">
        /// The field entity to remove.
        /// </param>
        public static void Remove(Field fieldEntry)
        {
            ArgumentNullException.ThrowIfNull(fieldEntry);

            using var db = CreateDbContext();

            db.RemoveEntity(fieldEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
