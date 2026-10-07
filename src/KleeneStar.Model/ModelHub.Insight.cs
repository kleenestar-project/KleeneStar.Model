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
        /// Returns a queryable collection of insights from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND. The query includes related category and widget data for each insight.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned insights. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of insights. The query
        /// includes related categories and widgets and is not tracked by the context.
        /// </returns>
        public static IEnumerable<Insight> GetInsights(IQuery<Insight> query)
        {
            using var db = CreateDbContext();

            return [.. GetInsights(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of insights from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned insights. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of insights. The query
        /// includes related categories and widgets and is not tracked by the context.
        /// </returns>
        public static IEnumerable<Insight> GetInsights(IQuery<Insight> query, KleeneStarDbContext context)
        {
            var data = context.Insights
                .AsNoTracking()
                .Include(d => d.Categories)
                .Include(d => d.Columns)
                    .ThenInclude(c => c.Widgets);

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified insight to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If an insight with the same identifier already exists in the database, this method does nothing.
        /// </remarks>
        /// <param name="insight">
        /// The insight to add. The insight's Id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Insight insight)
        {
            ArgumentNullException.ThrowIfNull(insight);

            using var db = CreateDbContext();

            var query = new Query<Insight>()
                .WhereEquals(x => x.Id, insight.Id);

            if (query.Apply(db.Insights).Any())
            {
                return;
            }

            db.AddEntity(insight, ["Categories"]);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified insight in the database.
        /// </summary>
        /// <param name="insight">
        /// The insight to update. Cannot be null.
        /// </param>
        public static void Update(Insight insight)
        {
            ArgumentNullException.ThrowIfNull(insight);

            using var db = CreateDbContext();

            db.UpdateEntity(insight, ["Categories"]);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified insight from the data store if it exists.
        /// </summary>
        /// <param name="insight">
        /// The insight entity to remove.
        /// </param>
        public static void Remove(Insight insight)
        {
            ArgumentNullException.ThrowIfNull(insight);

            using var db = CreateDbContext();

            RemoveBoardViews(db, insight.Id);
            db.RemoveEntity(insight, ["Categories"]);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Applies a column-only layout change (add, rename, resize, recolor, reorder, delete) to a
        /// dashboard while preserving the widgets of the surviving columns.
        /// </summary>
        /// <remarks>
        /// Each desired column is matched to an existing one by its business id: a column carrying an
        /// existing id updates that column's meta in place, a column carrying <see cref="Guid.Empty"/>
        /// (or an unknown id) is created fresh, and any existing column absent from the desired set is
        /// removed together with its widgets. The list order defines the persisted
        /// <see cref="DashboardColumn.Position"/>.
        /// </remarks>
        /// <param name="insightId">The business id of the insight whose dashboard is updated.</param>
        /// <param name="viewId">The owning tab identifier, or the legacy board when empty.</param>
        /// <param name="columns">
        /// The desired columns in their target order. Widgets on these instances are ignored; only the
        /// column meta is applied. Must not be null.
        /// </param>
        public static void SetDashboardColumns(Guid insightId, IReadOnlyList<DashboardColumn> columns, Guid viewId = default)
        {
            ArgumentNullException.ThrowIfNull(columns);

            using var db = CreateDbContext();

            var insight = db.Insights
                .Include(d => d.Columns.Where(c => c.ViewId == viewId))
                    .ThenInclude(c => c.Widgets)
                .FirstOrDefault(d => d.Id == insightId);

            // only the selected tab participates in column reconciliation
            if (insight is null)
            {
                return;
            }

            ReconcileColumns(db, insight, columns, rebuildWidgets: false, viewId);

            db.SaveChanges();
        }

        /// <summary>
        /// Applies a full board update (a widget being added, deleted, reconfigured or moved) to a
        /// dashboard, rebuilding the widgets of every column from the desired state.
        /// </summary>
        /// <remarks>
        /// Columns are reconciled exactly as in <see cref="SetDashboardColumns"/>; in addition every
        /// surviving or newly created column has its widgets replaced by the desired widgets. Widgets
        /// have no stable identity across saves (the board carries only their type id, name, color and
        /// params), so they are recreated with fresh ids and the list order becomes their
        /// <see cref="Widget.Position"/>.
        /// </remarks>
        /// <param name="insightId">The business id of the insight whose dashboard is updated.</param>
        /// <param name="viewId">The owning tab identifier, or the legacy board when empty.</param>
        /// <param name="columns">
        /// The desired columns, each carrying the widgets it should hold, in their target order. Must
        /// not be null.
        /// </param>
        public static void SetDashboardBoard(Guid insightId, IReadOnlyList<DashboardColumn> columns, Guid viewId = default)
        {
            ArgumentNullException.ThrowIfNull(columns);

            using var db = CreateDbContext();

            var insight = db.Insights
                .Include(d => d.Columns.Where(c => c.ViewId == viewId))
                    .ThenInclude(c => c.Widgets)
                .FirstOrDefault(d => d.Id == insightId);

            // only the selected tab participates in column reconciliation
            if (insight is null)
            {
                return;
            }

            ReconcileColumns(db, insight, columns, rebuildWidgets: true, viewId);

            db.SaveChanges();
        }

        /// <summary>
        /// Reconciles the dashboard columns of a tracked insight against a desired ordered set, optionally
        /// rebuilding the widgets of each column.
        /// </summary>
        /// <param name="db">The tracking database context.</param>
        /// <param name="insight">The tracked insight whose dashboard columns are loaded.</param>
        /// <param name="columns">The desired columns in their target order.</param>
        /// <param name="rebuildWidgets">
        /// When true, the widgets of every surviving or created column are replaced by the desired
        /// widgets; when false, the widgets of surviving columns are left untouched.
        /// </param>
        /// <param name="viewId">The owning tab identifier.</param>
        private static void ReconcileColumns(KleeneStarDbContext db, Insight insight, IReadOnlyList<DashboardColumn> columns, bool rebuildWidgets, Guid viewId)
        {
            var existing = insight.Columns.ToDictionary(c => c.Id);
            var keep = new HashSet<Guid>();

            for (var index = 0; index < columns.Count; index++)
            {
                var desired = columns[index];

                // correlate the desired column to an existing one: first by business id, then by the
                // transient client key a session-new column keeps until the next reload; anything left
                // is a genuinely new column.
                DashboardColumn column = null;

                if (desired.Id != Guid.Empty && existing.TryGetValue(desired.Id, out var byId))
                {
                    column = byId;
                }
                else if (!string.IsNullOrEmpty(desired.Key))
                {
                    column = insight.Columns.FirstOrDefault(c => c.Key == desired.Key);
                }

                if (column is null)
                {
                    column = new DashboardColumn(Guid.NewGuid())
                    {
                        InsightId = insight.Id,
                        ViewId = viewId,
                        Key = desired.Key
                    };
                    insight.Columns.Add(column);
                }

                column.Name = desired.Name;
                column.Size = desired.Size;
                column.Color = desired.Color;
                column.Position = index;
                keep.Add(column.Id);

                if (rebuildWidgets)
                {
                    RebuildWidgets(db, column, desired.Widgets);
                }
            }

            foreach (var column in insight.Columns.Where(c => !keep.Contains(c.Id)).ToList())
            {
                if (column.Widgets is { Count: > 0 })
                {
                    db.Widgets.RemoveRange(column.Widgets);
                }

                insight.Columns.Remove(column);
                db.DashboardColumns.Remove(column);
            }
        }

        /// <summary>
        /// Replaces every widget of a column with fresh widgets built from the desired set, assigning
        /// each a new id and its list position.
        /// </summary>
        /// <param name="db">The tracking database context.</param>
        /// <param name="column">The tracked column whose widgets are replaced.</param>
        /// <param name="widgets">The desired widgets in their target order; may be null or empty.</param>
        private static void RebuildWidgets(KleeneStarDbContext db, DashboardColumn column, IEnumerable<Widget> widgets)
        {
            if (column.Widgets is { Count: > 0 })
            {
                db.Widgets.RemoveRange(column.Widgets);
                column.Widgets.Clear();
            }

            if (widgets is null)
            {
                return;
            }

            var position = 0;

            foreach (var widget in widgets)
            {
                column.Widgets.Add(new Widget(Guid.NewGuid())
                {
                    ColumnId = column.Id,
                    Type = widget.Type,
                    Name = widget.Name,
                    Color = widget.Color,
                    Params = widget.Params,
                    Wql = widget.Wql,
                    Position = position++
                });
            }
        }
    }
}
