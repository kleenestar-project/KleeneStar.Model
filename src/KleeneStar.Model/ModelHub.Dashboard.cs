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
        /// Returns a queryable collection of dashboards from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND. The query includes related category and widget data for each dashboard.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned dashboards. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of dashboards. The query
        /// includes related categories and widgets and is not tracked by the context.
        /// </returns>
        public static IEnumerable<Dashboard> GetDashboards(IQuery<Dashboard> query)
        {
            using var db = CreateDbContext();

            return [.. GetDashboards(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of dashboards from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned dashboards. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of dashboards. The query
        /// includes related categories and widgets and is not tracked by the context.
        /// </returns>
        public static IEnumerable<Dashboard> GetDashboards(IQuery<Dashboard> query, KleeneStarDbContext context)
        {
            var data = context.Dashboards
                .AsNoTracking()
                .Include(d => d.Categories)
                .Include(d => d.Columns)
                    .ThenInclude(c => c.Widgets);

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified dashboard to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a dashboard with the same identifier already exists in the database, this method does nothing.
        /// </remarks>
        /// <param name="dashboard">
        /// The dashboard to add. The dashboard's Id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Dashboard dashboard)
        {
            ArgumentNullException.ThrowIfNull(dashboard);

            using var db = CreateDbContext();

            var query = new Query<Dashboard>()
                .WhereEquals(x => x.Id, dashboard.Id);

            if (query.Apply(db.Dashboards).Any())
            {
                return;
            }

            db.AddEntity(dashboard, ["Categories"]);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified dashboard in the database.
        /// </summary>
        /// <param name="dashboard">
        /// The dashboard to update. Cannot be null.
        /// </param>
        public static void Update(Dashboard dashboard)
        {
            ArgumentNullException.ThrowIfNull(dashboard);

            using var db = CreateDbContext();

            db.UpdateEntity(dashboard, ["Categories"]);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified dashboard from the data store if it exists.
        /// </summary>
        /// <param name="dashboard">
        /// The dashboard entity to remove.
        /// </param>
        public static void Remove(Dashboard dashboard)
        {
            ArgumentNullException.ThrowIfNull(dashboard);

            using var db = CreateDbContext();

            db.RemoveEntity(dashboard, ["Categories"]);

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
        /// <param name="dashboardId">The business id of the dashboard to update.</param>
        /// <param name="columns">
        /// The desired columns in their target order. Widgets on these instances are ignored; only the
        /// column meta is applied. Must not be null.
        /// </param>
        public static void SetDashboardColumns(Guid dashboardId, IReadOnlyList<DashboardColumn> columns)
        {
            ArgumentNullException.ThrowIfNull(columns);

            using var db = CreateDbContext();

            var dashboard = db.Dashboards
                .Include(d => d.Columns)
                    .ThenInclude(c => c.Widgets)
                .FirstOrDefault(d => d.Id == dashboardId);

            if (dashboard is null)
            {
                return;
            }

            ReconcileColumns(db, dashboard, columns, rebuildWidgets: false);

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
        /// <param name="dashboardId">The business id of the dashboard to update.</param>
        /// <param name="columns">
        /// The desired columns, each carrying the widgets it should hold, in their target order. Must
        /// not be null.
        /// </param>
        public static void SetDashboardBoard(Guid dashboardId, IReadOnlyList<DashboardColumn> columns)
        {
            ArgumentNullException.ThrowIfNull(columns);

            using var db = CreateDbContext();

            var dashboard = db.Dashboards
                .Include(d => d.Columns)
                    .ThenInclude(c => c.Widgets)
                .FirstOrDefault(d => d.Id == dashboardId);

            if (dashboard is null)
            {
                return;
            }

            ReconcileColumns(db, dashboard, columns, rebuildWidgets: true);

            db.SaveChanges();
        }

        /// <summary>
        /// Reconciles the columns of a tracked dashboard against a desired ordered set, optionally
        /// rebuilding the widgets of each column.
        /// </summary>
        /// <param name="db">The tracking database context.</param>
        /// <param name="dashboard">The tracked dashboard whose columns are loaded.</param>
        /// <param name="columns">The desired columns in their target order.</param>
        /// <param name="rebuildWidgets">
        /// When true, the widgets of every surviving or created column are replaced by the desired
        /// widgets; when false, the widgets of surviving columns are left untouched.
        /// </param>
        private static void ReconcileColumns(KleeneStarDbContext db, Dashboard dashboard, IReadOnlyList<DashboardColumn> columns, bool rebuildWidgets)
        {
            var existing = dashboard.Columns.ToDictionary(c => c.Id);
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
                    column = dashboard.Columns.FirstOrDefault(c => c.Key == desired.Key);
                }

                if (column is null)
                {
                    column = new DashboardColumn(Guid.NewGuid())
                    {
                        DashboardId = dashboard.Id,
                        Key = desired.Key
                    };
                    dashboard.Columns.Add(column);
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

            foreach (var column in dashboard.Columns.Where(c => !keep.Contains(c.Id)).ToList())
            {
                if (column.Widgets is { Count: > 0 })
                {
                    db.Widgets.RemoveRange(column.Widgets);
                }

                dashboard.Columns.Remove(column);
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
