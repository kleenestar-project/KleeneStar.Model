using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the KleeneStar.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the persisted object-kind dashboard of a workspace/kind pair, including its
        /// columns and widgets, or <see langword="null"/> when the board has never been
        /// customized.
        /// </summary>
        /// <param name="workspaceId">The workspace the board belongs to.</param>
        /// <param name="kind">The object kind the board is scoped to.</param>
        /// <returns>The board, or <see langword="null"/> when none is persisted.</returns>
        public static KindDashboard GetKindDashboard(Guid workspaceId, string kind)
        {
            using var db = CreateDbContext();

            return db.KindDashboards
                .AsNoTracking()
                .Include(b => b.Columns)
                    .ThenInclude(c => c.Widgets)
                .FirstOrDefault(b => b.WorkspaceId == workspaceId && b.Kind == kind);
        }

        /// <summary>
        /// Returns the persisted object-kind dashboard by its business id, including its
        /// columns and widgets, or <see langword="null"/> when no such board exists.
        /// </summary>
        /// <param name="boardId">The business id of the board.</param>
        /// <returns>The board, or <see langword="null"/> when none is persisted.</returns>
        public static KindDashboard GetKindDashboardById(Guid boardId)
        {
            using var db = CreateDbContext();

            return db.KindDashboards
                .AsNoTracking()
                .Include(b => b.Columns)
                    .ThenInclude(c => c.Widgets)
                .FirstOrDefault(b => b.Id == boardId);
        }

        /// <summary>
        /// Returns the persisted object-kind dashboard of a workspace/kind pair, creating an
        /// empty one (no columns) when none exists yet.
        /// </summary>
        /// <param name="workspaceId">The workspace the board belongs to.</param>
        /// <param name="kind">The object kind the board is scoped to.</param>
        /// <returns>The existing or newly created board.</returns>
        public static KindDashboard EnsureKindDashboard(Guid workspaceId, string kind)
        {
            using var db = CreateDbContext();

            var board = db.KindDashboards
                .FirstOrDefault(b => b.WorkspaceId == workspaceId && b.Kind == kind);

            if (board is not null)
            {
                return board;
            }

            board = new KindDashboard(Guid.NewGuid())
            {
                WorkspaceId = workspaceId,
                Kind = kind
            };

            db.KindDashboards.Add(board);
            db.SaveChanges();

            return board;
        }

        /// <summary>
        /// Applies a column-only layout change (add, rename, resize, recolor, reorder, delete)
        /// to an object-kind dashboard while preserving the widgets of the surviving columns.
        /// </summary>
        /// <param name="boardId">The business id of the board to update.</param>
        /// <param name="columns">
        /// The desired columns in their target order. Widgets on these instances are ignored.
        /// Must not be null.
        /// </param>
        public static void SetKindDashboardColumns(Guid boardId, IReadOnlyList<KindDashboardColumn> columns)
        {
            ArgumentNullException.ThrowIfNull(columns);

            using var db = CreateDbContext();

            var board = db.KindDashboards
                .Include(b => b.Columns)
                    .ThenInclude(c => c.Widgets)
                .FirstOrDefault(b => b.Id == boardId);

            if (board is null)
            {
                return;
            }

            ReconcileKindDashboardColumns(db, board, columns, rebuildWidgets: false);

            db.SaveChanges();
        }

        /// <summary>
        /// Applies a full board update (a widget being added, deleted, reconfigured or moved) to
        /// an object-kind dashboard, rebuilding the widgets of every column from the desired
        /// state.
        /// </summary>
        /// <param name="boardId">The business id of the board to update.</param>
        /// <param name="columns">
        /// The desired columns, each carrying the widgets it should hold, in their target order.
        /// Must not be null.
        /// </param>
        public static void SetKindDashboardBoard(Guid boardId, IReadOnlyList<KindDashboardColumn> columns)
        {
            ArgumentNullException.ThrowIfNull(columns);

            using var db = CreateDbContext();

            var board = db.KindDashboards
                .Include(b => b.Columns)
                    .ThenInclude(c => c.Widgets)
                .FirstOrDefault(b => b.Id == boardId);

            if (board is null)
            {
                return;
            }

            ReconcileKindDashboardColumns(db, board, columns, rebuildWidgets: true);

            db.SaveChanges();
        }

        /// <summary>
        /// Reconciles the columns of a tracked object-kind dashboard against a desired ordered
        /// set, optionally rebuilding the widgets of each column.
        /// </summary>
        /// <param name="db">The tracking database context.</param>
        /// <param name="board">The tracked board whose columns are loaded.</param>
        /// <param name="columns">The desired columns in their target order.</param>
        /// <param name="rebuildWidgets">
        /// When true, the widgets of every surviving or created column are replaced by the
        /// desired widgets; when false, the widgets of surviving columns are left untouched.
        /// </param>
        private static void ReconcileKindDashboardColumns(KleeneStarDbContext db, KindDashboard board, IReadOnlyList<KindDashboardColumn> columns, bool rebuildWidgets)
        {
            var existing = board.Columns.ToDictionary(c => c.Id);
            var keep = new HashSet<Guid>();

            for (var index = 0; index < columns.Count; index++)
            {
                var desired = columns[index];
                KindDashboardColumn column = null;

                if (desired.Id != Guid.Empty && existing.TryGetValue(desired.Id, out var byId))
                {
                    column = byId;
                }
                else if (!string.IsNullOrEmpty(desired.Key))
                {
                    column = board.Columns.FirstOrDefault(c => c.Key == desired.Key);
                }

                if (column is null)
                {
                    column = new KindDashboardColumn(Guid.NewGuid())
                    {
                        BoardId = board.Id,
                        Key = desired.Key
                    };
                    board.Columns.Add(column);
                }

                column.Name = desired.Name;
                column.Size = desired.Size;
                column.Color = desired.Color;
                column.Position = index;
                keep.Add(column.Id);

                if (rebuildWidgets)
                {
                    RebuildKindDashboardWidgets(db, column, desired.Widgets);
                }
            }

            foreach (var column in board.Columns.Where(c => !keep.Contains(c.Id)).ToList())
            {
                if (column.Widgets is { Count: > 0 })
                {
                    db.KindDashboardWidgets.RemoveRange(column.Widgets);
                }

                board.Columns.Remove(column);
                db.KindDashboardColumns.Remove(column);
            }
        }

        /// <summary>
        /// Replaces every widget of a column with fresh widgets built from the desired set,
        /// assigning each a new id and its list position.
        /// </summary>
        /// <param name="db">The tracking database context.</param>
        /// <param name="column">The tracked column whose widgets are replaced.</param>
        /// <param name="widgets">The desired widgets in their target order; may be null or empty.</param>
        private static void RebuildKindDashboardWidgets(KleeneStarDbContext db, KindDashboardColumn column, IEnumerable<KindDashboardWidget> widgets)
        {
            if (column.Widgets is { Count: > 0 })
            {
                db.KindDashboardWidgets.RemoveRange(column.Widgets);
                column.Widgets.Clear();
            }

            if (widgets is null)
            {
                return;
            }

            var position = 0;

            foreach (var widget in widgets)
            {
                column.Widgets.Add(new KindDashboardWidget(Guid.NewGuid())
                {
                    ColumnId = column.Id,
                    Type = widget.Type,
                    Name = widget.Name,
                    Color = widget.Color,
                    Params = widget.Params,
                    Position = position++
                });
            }
        }
    }
}
