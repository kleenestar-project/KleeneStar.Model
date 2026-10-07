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
            dbEntry.Color = viewEntry.Color;
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

            RemoveBoardViews(db, dbEntry.WorkspaceId, dbEntry.Id);
            db.Remove(dbEntry);
            db.SaveChanges();
        }

        /// <summary>
        /// Puts the tabs of one overview of a workspace - its views of one object kind - into
        /// the given order in one transaction. A tab the list does not name keeps its place
        /// behind the named ones; an id that names no tab of the overview is ignored.
        /// </summary>
        /// <param name="workspaceId">The workspace whose tabs are ordered.</param>
        /// <param name="kind">The object kind of the overview.</param>
        /// <param name="order">The tab ids in their new order. Cannot be null.</param>
        /// <returns><see langword="true"/> when at least one tab of the overview was named.</returns>
        public static bool SetObjectViewOrder(Guid workspaceId, string kind, IReadOnlyList<Guid> order)
        {
            ArgumentNullException.ThrowIfNull(order);

            using var db = CreateDbContext();

            var views = db.ObjectViews
                .Where(x => x.WorkspaceId == workspaceId && x.Kind == kind)
                .ToList();

            var position = order
                .Select((id, index) => (id, index))
                .GroupBy(x => x.id)
                .ToDictionary(g => g.Key, g => g.First().index);

            if (!views.Any(x => position.ContainsKey(x.Id)))
            {
                return false;
            }

            var ordered = views
                .OrderBy(x => position.TryGetValue(x.Id, out var index) ? index : int.MaxValue)
                .ThenBy(x => x.Order)
                .ToList();

            for (var i = 0; i < ordered.Count; i++)
            {
                if (ordered[i].Order != i)
                {
                    ordered[i].Order = i;
                    ordered[i].Updated = DateTime.UtcNow;
                }
            }

            db.SaveChanges();

            return true;
        }
    }
}
