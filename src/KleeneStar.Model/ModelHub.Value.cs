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
        /// Returns a materialized collection of values from the database.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching values, including object and field navigation.</returns>
        public static IEnumerable<Value> GetValues(IQuery<Value> query)
        {
            using var db = CreateDbContext();

            return [.. GetValues(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of values using the supplied DbContext.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The DbContext.</param>
        /// <returns>The filtered collection.</returns>
        public static IEnumerable<Value> GetValues(IQuery<Value> query, KleeneStarDbContext context)
        {
            var data = context.Values
                .Include(x => x.Field)
                .Include(x => x.Object)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Adds the supplied value to the database when no value with the same id exists.
        /// </summary>
        /// <param name="valueEntry">The value to add.</param>
        public static void Add(Value valueEntry)
        {
            ArgumentNullException.ThrowIfNull(valueEntry);

            using var db = CreateDbContext();

            var query = new Query<Value>()
                .WhereEquals(x => x.Id, valueEntry.Id);

            if (query.Apply(db.Values).Any())
            {
                return;
            }

            if (valueEntry.Created == default)
            {
                valueEntry.Created = DateTime.UtcNow;
            }

            valueEntry.Updated = DateTime.UtcNow;

            db.Values.Add(valueEntry);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the supplied value in the database. Re-loads the existing row and
        /// overwrites the <see cref="Value.Data"/> payload and the timestamp.
        /// </summary>
        /// <param name="valueEntry">The value to update.</param>
        public static void Update(Value valueEntry)
        {
            ArgumentNullException.ThrowIfNull(valueEntry);

            using var db = CreateDbContext();

            var existing = db.Values.FirstOrDefault(x => x.Id == valueEntry.Id);

            if (existing is null)
            {
                return;
            }

            existing.Data = valueEntry.Data;
            existing.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified value from the data store if it exists.
        /// </summary>
        /// <param name="valueEntry">The value to remove.</param>
        public static void Remove(Value valueEntry)
        {
            ArgumentNullException.ThrowIfNull(valueEntry);

            using var db = CreateDbContext();

            var existing = db.Values.FirstOrDefault(x => x.Id == valueEntry.Id);

            if (existing is null)
            {
                return;
            }

            db.Values.Remove(existing);
            db.SaveChanges();
        }
    }
}
