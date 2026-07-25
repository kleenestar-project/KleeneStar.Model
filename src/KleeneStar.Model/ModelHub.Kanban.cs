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
        /// Returns the persisted Kanban board of a workspace/kind pair, including its columns
        /// and swimlanes, or <see langword="null"/> when the board has never been customized.
        /// </summary>
        /// <param name="workspaceId">The workspace the board belongs to.</param>
        /// <param name="kind">The object kind the board is scoped to.</param>
        /// <returns>The board, or <see langword="null"/> when none is persisted.</returns>
        public static KanbanBoard GetKanbanBoard(Guid workspaceId, string kind)
        {
            using var db = CreateDbContext();

            return db.KanbanBoards
                .AsNoTracking()
                .Include(b => b.Columns)
                .Include(b => b.Swimlanes)
                .FirstOrDefault(b => b.WorkspaceId == workspaceId && b.Kind == kind);
        }

        /// <summary>
        /// Returns the persisted Kanban board by its business id, including its columns and
        /// swimlanes, or <see langword="null"/> when no such board exists.
        /// </summary>
        /// <param name="boardId">The business id of the board.</param>
        /// <returns>The board, or <see langword="null"/> when none is persisted.</returns>
        public static KanbanBoard GetKanbanBoardById(Guid boardId)
        {
            using var db = CreateDbContext();

            return db.KanbanBoards
                .AsNoTracking()
                .Include(b => b.Columns)
                .Include(b => b.Swimlanes)
                .FirstOrDefault(b => b.Id == boardId);
        }

        /// <summary>
        /// Returns the persisted Kanban board of a workspace/kind pair, creating an empty one
        /// (no columns, no swimlanes) when none exists yet.
        /// </summary>
        /// <param name="workspaceId">The workspace the board belongs to.</param>
        /// <param name="kind">The object kind the board is scoped to.</param>
        /// <returns>The existing or newly created board.</returns>
        public static KanbanBoard EnsureKanbanBoard(Guid workspaceId, string kind)
        {
            using var db = CreateDbContext();

            var board = db.KanbanBoards
                .FirstOrDefault(b => b.WorkspaceId == workspaceId && b.Kind == kind);

            if (board is not null)
            {
                return board;
            }

            board = new KanbanBoard(Guid.NewGuid())
            {
                WorkspaceId = workspaceId,
                Kind = kind
            };

            db.KanbanBoards.Add(board);
            db.SaveChanges();

            return board;
        }

        /// <summary>
        /// Applies a column layout change (add / rename / recolor / reorder / delete) to a
        /// Kanban board.
        /// </summary>
        /// <remarks>
        /// Each desired column is matched to an existing one by its business id, then by its
        /// transient client key; anything left over is created fresh. An existing column absent
        /// from the desired set is removed. The list order defines the persisted
        /// <see cref="KanbanBoardColumn.Position"/>.
        /// </remarks>
        /// <param name="boardId">The business id of the board to update.</param>
        /// <param name="columns">The desired columns in their target order. Must not be null.</param>
        public static void SetKanbanColumns(Guid boardId, IReadOnlyList<KanbanBoardColumn> columns)
        {
            ArgumentNullException.ThrowIfNull(columns);

            using var db = CreateDbContext();

            var board = db.KanbanBoards
                .Include(b => b.Columns)
                .FirstOrDefault(b => b.Id == boardId);

            if (board is null)
            {
                return;
            }

            var existing = board.Columns.ToDictionary(c => c.Id);
            var keep = new HashSet<Guid>();

            for (var index = 0; index < columns.Count; index++)
            {
                var desired = columns[index];
                KanbanBoardColumn column = null;

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
                    column = new KanbanBoardColumn(Guid.NewGuid())
                    {
                        BoardId = board.Id,
                        Key = desired.Key
                    };
                    board.Columns.Add(column);
                }

                column.Name = desired.Name;
                column.Color = desired.Color;
                column.CategoryId = desired.CategoryId;
                column.Position = index;
                keep.Add(column.Id);
            }

            foreach (var column in board.Columns.Where(c => !keep.Contains(c.Id)).ToList())
            {
                board.Columns.Remove(column);
                db.KanbanBoardColumns.Remove(column);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Applies a swimlane layout change (add / rename / reorder / delete) to a Kanban board.
        /// </summary>
        /// <remarks>
        /// Each desired swimlane is matched to an existing one by its business id, then by its
        /// transient client key; anything left over is created fresh. An existing swimlane absent
        /// from the desired set is removed. The list order defines the persisted
        /// <see cref="KanbanBoardSwimlane.Position"/>.
        /// </remarks>
        /// <param name="boardId">The business id of the board to update.</param>
        /// <param name="swimlanes">The desired swimlanes in their target order. Must not be null.</param>
        public static void SetKanbanSwimlanes(Guid boardId, IReadOnlyList<KanbanBoardSwimlane> swimlanes)
        {
            ArgumentNullException.ThrowIfNull(swimlanes);

            using var db = CreateDbContext();

            var board = db.KanbanBoards
                .Include(b => b.Swimlanes)
                .FirstOrDefault(b => b.Id == boardId);

            if (board is null)
            {
                return;
            }

            var existing = board.Swimlanes.ToDictionary(s => s.Id);
            var keep = new HashSet<Guid>();

            for (var index = 0; index < swimlanes.Count; index++)
            {
                var desired = swimlanes[index];
                KanbanBoardSwimlane swimlane = null;

                if (desired.Id != Guid.Empty && existing.TryGetValue(desired.Id, out var byId))
                {
                    swimlane = byId;
                }
                else if (!string.IsNullOrEmpty(desired.Key))
                {
                    swimlane = board.Swimlanes.FirstOrDefault(s => s.Key == desired.Key);
                }

                if (swimlane is null)
                {
                    swimlane = new KanbanBoardSwimlane(Guid.NewGuid())
                    {
                        BoardId = board.Id,
                        Key = desired.Key
                    };
                    board.Swimlanes.Add(swimlane);
                }

                swimlane.Name = desired.Name;
                swimlane.Filter = desired.Filter;
                swimlane.ClassId = desired.ClassId;
                swimlane.Position = index;
                keep.Add(swimlane.Id);
            }

            foreach (var swimlane in board.Swimlanes.Where(s => !keep.Contains(s.Id)).ToList())
            {
                board.Swimlanes.Remove(swimlane);
                db.KanbanBoardSwimlanes.Remove(swimlane);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Applies the board-level WQL filter (submitted through the board settings dialog) to a
        /// Kanban board.
        /// </summary>
        /// <param name="boardId">The business id of the board to update.</param>
        /// <param name="filter">The WQL filter to persist, or null to clear it.</param>
        public static void SetKanbanFilter(Guid boardId, string filter)
        {
            using var db = CreateDbContext();

            var board = db.KanbanBoards.FirstOrDefault(b => b.Id == boardId);

            if (board is null)
            {
                return;
            }

            board.Filter = filter;

            db.SaveChanges();
        }
    }
}
