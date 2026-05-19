using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with <see cref="ObjectView"/> entries.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the persisted object views that match the given query criteria, opening
        /// a short-lived DbContext for the call.
        /// </summary>
        /// <param name="query">The query criteria. Must not be null.</param>
        /// <returns>The matching views, materialized.</returns>
        public static IEnumerable<ObjectView> GetObjectViews(IQuery<ObjectView> query)
        {
            using var db = CreateDbContext();

            return [.. GetObjectViews(query, db)];
        }

        /// <summary>
        /// Returns the persisted object views that match the given query criteria, using the
        /// supplied <paramref name="context"/>. Includes the workspace navigation property.
        /// </summary>
        /// <param name="query">The query criteria. Must not be null.</param>
        /// <param name="context">The query context.</param>
        public static IEnumerable<ObjectView> GetObjectViews(IQuery<ObjectView> query, KleeneStarDbContext context)
        {
            var data = context.ObjectViews
                .Include(x => x.Workspace)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Inserts the given object view if no record with the same <see cref="ObjectView.Id"/>
        /// already exists.
        /// </summary>
        /// <param name="viewEntry">The view to add. Cannot be null.</param>
        public static void Add(ObjectView viewEntry)
        {
            ArgumentNullException.ThrowIfNull(viewEntry);

            using var db = CreateDbContext();

            var query = new Query<ObjectView>()
                .WhereEquals(x => x.Id, viewEntry.Id);

            if (query.Apply(db.ObjectViews).Any())
            {
                return;
            }

            db.AddEntity(viewEntry);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the scalar properties of the existing object view identified by
        /// <see cref="ObjectView.Id"/>.
        /// </summary>
        /// <param name="viewEntry">The view holding the updated values. Cannot be null.</param>
        public static void Update(ObjectView viewEntry)
        {
            ArgumentNullException.ThrowIfNull(viewEntry);

            using var db = CreateDbContext();

            var query = new Query<ObjectView>()
                .WhereEquals(x => x.Id, viewEntry.Id);

            var dbEntry = query.Apply(db.ObjectViews).FirstOrDefault();

            if (dbEntry is null)
            {
                return;
            }

            dbEntry.Name = viewEntry.Name;
            dbEntry.Description = viewEntry.Description;
            dbEntry.ViewType = viewEntry.ViewType;
            dbEntry.Configuration = viewEntry.Configuration;
            dbEntry.Order = viewEntry.Order;
            dbEntry.State = viewEntry.State;
            dbEntry.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Removes the object view identified by <see cref="ObjectView.Id"/>.
        /// </summary>
        /// <param name="viewEntry">The view to remove. Cannot be null.</param>
        public static void Remove(ObjectView viewEntry)
        {
            ArgumentNullException.ThrowIfNull(viewEntry);

            using var db = CreateDbContext();

            var query = new Query<ObjectView>()
                .WhereEquals(x => x.Id, viewEntry.Id);

            var dbEntry = query.Apply(db.ObjectViews).FirstOrDefault();

            if (dbEntry is null)
            {
                return;
            }

            db.Remove(dbEntry);
            db.SaveChanges();
        }
    }
}
