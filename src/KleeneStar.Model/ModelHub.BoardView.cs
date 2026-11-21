using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides lifecycle operations for the configurations owned by board tabs.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Copies an insight tab's dashboard and Kanban configuration with fresh entity identifiers.
        /// </summary>
        /// <param name="sourceId">The source insight identifier.</param>
        /// <param name="targetId">The target insight identifier.</param>
        /// <param name="sourceViewId">The source tab identifier.</param>
        /// <param name="targetViewId">The newly created target tab identifier.</param>
        public static void CopyInsightBoard(Guid sourceId, Guid targetId, Guid sourceViewId, Guid targetViewId)
        {
            using var db = CreateDbContext();
            var columns = db.DashboardColumns.Include(x => x.Widgets)
                .Where(x => x.InsightId == sourceId && x.ViewId == sourceViewId).ToList();

            foreach (var source in columns)
            {
                var column = new DashboardColumn
                {
                    InsightId = targetId,
                    ViewId = targetViewId,
                    Name = source.Name,
                    Size = source.Size,
                    Color = source.Color,
                    Position = source.Position,
                    Key = source.Key
                };
                column.Widgets = source.Widgets.Select(widget => new Widget
                {
                    ColumnId = column.Id,
                    Name = widget.Name,
                    Type = widget.Type,
                    Color = widget.Color,
                    Params = widget.Params,
                    Position = widget.Position,
                    Wql = widget.Wql
                }).ToList();
                db.DashboardColumns.Add(column);
            }

            var sourceBoard = db.KanbanBoards.Include(x => x.Columns).Include(x => x.Swimlanes)
                .SingleOrDefault(x => x.WorkspaceId == sourceId && x.Kind == "insight" && x.ViewId == sourceViewId);
            if (sourceBoard is not null)
            {
                var board = new KanbanBoard
                {
                    WorkspaceId = targetId,
                    Kind = sourceBoard.Kind,
                    ViewId = targetViewId,
                    Filter = sourceBoard.Filter
                };
                board.Columns = sourceBoard.Columns.Select(column => new KanbanBoardColumn
                {
                    BoardId = board.Id,
                    Name = column.Name,
                    Color = column.Color,
                    Position = column.Position,
                    Key = column.Key,
                    CategoryId = column.CategoryId,
                    Statuses = column.Statuses
                }).ToList();
                board.Swimlanes = sourceBoard.Swimlanes.Select(lane => new KanbanBoardSwimlane
                {
                    BoardId = board.Id,
                    Name = lane.Name,
                    Color = lane.Color,
                    Position = lane.Position,
                    Key = lane.Key,
                    ClassId = lane.ClassId,
                    Filter = lane.Filter
                }).ToList();
                db.KanbanBoards.Add(board);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Deletes the board graphs owned by one tab or by an entire workspace or insight.
        /// </summary>
        /// <param name="db">The context of the enclosing deletion transaction.</param>
        /// <param name="ownerId">The workspace or insight that owns the boards.</param>
        /// <param name="viewId">The tab to delete, or null to delete every board of the owner.</param>
        private static void RemoveBoardViews(KleeneStarDbContext db, Guid ownerId, Guid? viewId = null)
        {
            db.KanbanBoards.RemoveRange(db.KanbanBoards.Include(x => x.Columns).Include(x => x.Swimlanes)
                .Where(x => x.WorkspaceId == ownerId && (viewId == null || x.ViewId == viewId)));
            db.KindDashboards.RemoveRange(db.KindDashboards.Include(x => x.Columns).ThenInclude(x => x.Widgets)
                .Where(x => x.WorkspaceId == ownerId && (viewId == null || x.ViewId == viewId)));
            db.DashboardColumns.RemoveRange(db.DashboardColumns.Include(x => x.Widgets)
                .Where(x => x.InsightId == ownerId && (viewId == null || x.ViewId == viewId)));
        }
    }
}
