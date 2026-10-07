using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the tabs of an insight (<see cref="InsightView"/>).
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the tabs that match the given query criteria, opening a short-lived
        /// DbContext for the call.
        /// </summary>
        /// <param name="query">The query criteria. Must not be null.</param>
        /// <returns>The matching tabs, materialized.</returns>
        public static IEnumerable<InsightView> GetInsightViews(IQuery<InsightView> query)
        {
            using var db = CreateDbContext();

            return [.. GetInsightViews(query, db)];
        }

        /// <summary>
        /// Returns the tabs that match the given query criteria, using the supplied context.
        /// </summary>
        /// <param name="query">The query criteria. Must not be null.</param>
        /// <param name="context">The query context.</param>
        /// <returns>The matching tabs; not materialized.</returns>
        public static IEnumerable<InsightView> GetInsightViews(IQuery<InsightView> query, KleeneStarDbContext context)
        {
            var data = context.InsightViews
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Inserts a tab unless one with the same <see cref="InsightView.Id"/> exists.
        /// </summary>
        /// <param name="view">The tab to add. Cannot be null.</param>
        public static void Add(InsightView view)
        {
            ArgumentNullException.ThrowIfNull(view);

            using var db = CreateDbContext();

            if (db.InsightViews.Any(x => x.Id == view.Id))
            {
                return;
            }

            db.AddEntity(view);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the scalar properties of an existing tab. The insight it belongs to does
        /// not change.
        /// </summary>
        /// <param name="view">The tab holding the updated values. Cannot be null.</param>
        public static void Update(InsightView view)
        {
            ArgumentNullException.ThrowIfNull(view);

            using var db = CreateDbContext();

            var entry = db.InsightViews.FirstOrDefault(x => x.Id == view.Id);

            if (entry is null)
            {
                return;
            }

            entry.Name = view.Name;
            entry.ViewType = view.ViewType;
            entry.Configuration = view.Configuration;
            entry.Order = view.Order;
            entry.State = view.State;
            entry.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Removes a tab.
        /// </summary>
        /// <param name="view">The tab to remove. Cannot be null.</param>
        public static void Remove(InsightView view)
        {
            ArgumentNullException.ThrowIfNull(view);

            using var db = CreateDbContext();

            var entry = db.InsightViews.FirstOrDefault(x => x.Id == view.Id);

            if (entry is null)
            {
                return;
            }

            RemoveBoardViews(db, entry.InsightId, entry.Id);
            db.Remove(entry);
            db.SaveChanges();
        }

        /// <summary>
        /// Puts the tabs of an insight into the given order in one transaction. A tab the list
        /// does not name keeps its place behind the named ones; an id that names no tab of the
        /// insight is ignored.
        /// </summary>
        /// <param name="insightId">The insight whose tabs are ordered.</param>
        /// <param name="order">The tab ids in their new order. Cannot be null.</param>
        /// <returns><see langword="true"/> when at least one tab of the insight was named.</returns>
        public static bool SetInsightViewOrder(Guid insightId, IReadOnlyList<Guid> order)
        {
            ArgumentNullException.ThrowIfNull(order);

            using var db = CreateDbContext();

            var views = db.InsightViews
                .Where(x => x.InsightId == insightId)
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
