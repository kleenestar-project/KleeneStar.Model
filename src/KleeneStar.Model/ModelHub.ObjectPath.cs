using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// The materialized paths of objects (<see cref="ObjectPath"/>) and the recursive operation
    /// runs (<see cref="ObjectOperation"/>).
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// The path segment of an object: its id in <c>N</c> format followed by a slash.
        /// </summary>
        /// <param name="objectId">The object.</param>
        /// <returns>The segment.</returns>
        public static string PathSegment(Guid objectId)
        {
            return objectId.ToString("N") + "/";
        }

        /// <summary>
        /// Narrows the paths to a subtree: every row whose path starts with the prefix. On a
        /// relational store a range on the indexed column (the character after "/" is "0"), so the
        /// index is used and SQLite's case-insensitive LIKE is avoided; elsewhere a prefix test.
        /// </summary>
        private static IQueryable<ObjectPath> Below(KleeneStarDbContext db, Guid workspaceId, string prefix)
        {
            var paths = db.ObjectPaths.AsNoTracking().Where(x => x.WorkspaceId == workspaceId);

            if (!db.Database.IsRelational())
            {
                return paths.Where(x => x.Path.StartsWith(prefix));
            }

            var upper = prefix[..^1] + "0";

            return paths.Where(x => x.Path.CompareTo(prefix) >= 0 && x.Path.CompareTo(upper) < 0);
        }

        /// <summary>
        /// Returns the path row of an object.
        /// </summary>
        /// <param name="objectId">The object.</param>
        /// <returns>The row, or <see langword="null"/>.</returns>
        public static ObjectPath GetObjectPath(Guid objectId)
        {
            using var db = CreateDbContext();

            return db.ObjectPaths.AsNoTracking().FirstOrDefault(x => x.ObjectId == objectId);
        }

        /// <summary>
        /// Returns the path rows of several objects.
        /// </summary>
        /// <param name="objectIds">The objects.</param>
        /// <returns>The rows by object.</returns>
        public static IReadOnlyDictionary<Guid, ObjectPath> GetObjectPaths(IEnumerable<Guid> objectIds)
        {
            var ids = objectIds.Distinct().ToList();

            using var db = CreateDbContext();

            return db.ObjectPaths.AsNoTracking()
                .Where(x => ids.Contains(x.ObjectId))
                .ToList()
                .GroupBy(x => x.ObjectId)
                .ToDictionary(x => x.Key, x => x.First());
        }

        /// <summary>
        /// Writes the path row of an object (insert or update).
        /// </summary>
        /// <param name="objectId">The object.</param>
        /// <param name="workspaceId">Its workspace.</param>
        /// <param name="path">The path.</param>
        /// <param name="depth">The number of ancestors.</param>
        public static void SetObjectPath(Guid objectId, Guid workspaceId, string path, int depth)
        {
            using var db = CreateDbContext();

            var row = db.ObjectPaths.FirstOrDefault(x => x.ObjectId == objectId);

            if (row is null)
            {
                db.ObjectPaths.Add(new ObjectPath { ObjectId = objectId, WorkspaceId = workspaceId, Path = path, Depth = depth });
            }
            else
            {
                row.WorkspaceId = workspaceId;
                row.Path = path;
                row.Depth = depth;
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Rewrites the paths of a moved subtree: every path starting with the old prefix gets the
        /// new one, and its depth shifts by the difference - one statement on a relational store.
        /// </summary>
        /// <param name="workspaceId">The workspace.</param>
        /// <param name="oldPrefix">The prefix below the moved object before the move.</param>
        /// <param name="newPrefix">The prefix below it after the move.</param>
        /// <param name="depthDelta">The change of depth.</param>
        /// <returns>The number of rows rewritten.</returns>
        public static int MoveObjectPaths(Guid workspaceId, string oldPrefix, string newPrefix, int depthDelta)
        {
            using var db = CreateDbContext();

            var length = oldPrefix.Length;

            if (db.Database.IsRelational())
            {
                var upper = oldPrefix[..^1] + "0";

                return db.ObjectPaths
                    .Where(x => x.WorkspaceId == workspaceId && x.Path.CompareTo(oldPrefix) >= 0 && x.Path.CompareTo(upper) < 0)
                    .ExecuteUpdate(s => s
                        .SetProperty(x => x.Path, x => newPrefix + x.Path.Substring(length))
                        .SetProperty(x => x.Depth, x => x.Depth + depthDelta));
            }

            // the in-memory store of the tests has no bulk update
            var rows = db.ObjectPaths.Where(x => x.WorkspaceId == workspaceId).ToList()
                .Where(x => x.Path.StartsWith(oldPrefix, StringComparison.Ordinal))
                .ToList();

            foreach (var row in rows)
            {
                row.Path = newPrefix + row.Path[length..];
                row.Depth += depthDelta;
            }

            db.SaveChanges();

            return rows.Count;
        }

        /// <summary>
        /// Returns the ids of the subtree below a prefix, at most <paramref name="limit"/>, the
        /// shallow ones first.
        /// </summary>
        /// <param name="workspaceId">The workspace.</param>
        /// <param name="prefix">The prefix below the root.</param>
        /// <param name="limit">The most ids answered.</param>
        /// <returns>The ids.</returns>
        public static IReadOnlyList<(Guid ObjectId, int Depth)> GetSubtree(Guid workspaceId, string prefix, int limit)
        {
            using var db = CreateDbContext();

            return [.. Below(db, workspaceId, prefix)
                .OrderBy(x => x.Depth)
                .Take(limit)
                .Select(x => new { x.ObjectId, x.Depth })
                .AsEnumerable()
                .Select(x => (x.ObjectId, x.Depth))];
        }

        /// <summary>
        /// Counts the subtree below a prefix.
        /// </summary>
        /// <param name="workspaceId">The workspace.</param>
        /// <param name="prefix">The prefix below the root.</param>
        /// <returns>The number of objects.</returns>
        public static int CountSubtree(Guid workspaceId, string prefix)
        {
            using var db = CreateDbContext();

            return Below(db, workspaceId, prefix).Count();
        }

        /// <summary>
        /// Returns the greatest depth below a prefix, or <c>-1</c> for an empty subtree.
        /// </summary>
        /// <param name="workspaceId">The workspace.</param>
        /// <param name="prefix">The prefix below the root.</param>
        /// <returns>The depth.</returns>
        public static int MaxDepthBelow(Guid workspaceId, string prefix)
        {
            using var db = CreateDbContext();

            return Below(db, workspaceId, prefix).Select(x => (int?)x.Depth).Max() ?? -1;
        }

        /// <summary>
        /// Returns every path row (the verifier compares them with the parent relation).
        /// </summary>
        /// <returns>The rows.</returns>
        public static IReadOnlyList<ObjectPath> GetAllObjectPaths()
        {
            using var db = CreateDbContext();

            return [.. db.ObjectPaths.AsNoTracking()];
        }

        /// <summary>
        /// Writes many path rows at once (insert or update) and removes the rows of objects that
        /// are gone - the fill at start and the verifier's rebuild.
        /// </summary>
        /// <param name="rows">The rows that should exist: object, workspace, path, depth.</param>
        /// <param name="stale">Path rows whose object no longer exists.</param>
        /// <returns>The number of rows written or removed.</returns>
        public static int WriteObjectPaths(IEnumerable<(Guid ObjectId, Guid WorkspaceId, string Path, int Depth)> rows, IEnumerable<Guid> stale = null)
        {
            var wanted = rows.ToList();
            var changes = 0;

            using var db = CreateDbContext();

            var ids = wanted.Select(x => x.ObjectId).ToList();
            var existing = new Dictionary<Guid, ObjectPath>();

            foreach (var chunk in ids.Chunk(500))
            {
                foreach (var row in db.ObjectPaths.Where(x => chunk.Contains(x.ObjectId)))
                {
                    existing[row.ObjectId] = row;
                }
            }

            foreach (var row in wanted)
            {
                if (existing.TryGetValue(row.ObjectId, out var current))
                {
                    if (current.Path != row.Path || current.Depth != row.Depth || current.WorkspaceId != row.WorkspaceId)
                    {
                        current.Path = row.Path;
                        current.Depth = row.Depth;
                        current.WorkspaceId = row.WorkspaceId;
                        changes++;
                    }
                }
                else
                {
                    db.ObjectPaths.Add(new ObjectPath { ObjectId = row.ObjectId, WorkspaceId = row.WorkspaceId, Path = row.Path, Depth = row.Depth });
                    changes++;
                }
            }

            var gone = (stale ?? []).ToList();

            foreach (var chunk in gone.Chunk(500))
            {
                foreach (var row in db.ObjectPaths.Where(x => chunk.Contains(x.ObjectId)))
                {
                    db.ObjectPaths.Remove(row);
                    changes++;
                }
            }

            db.SaveChanges();

            return changes;
        }

        /// <summary>
        /// Adds an operation run with its objects.
        /// </summary>
        /// <param name="operation">The run.</param>
        /// <param name="objectIds">The objects in processing order.</param>
        public static void AddObjectOperation(ObjectOperation operation, IEnumerable<Guid> objectIds)
        {
            using var db = CreateDbContext();

            db.ObjectOperations.Add(operation);

            var position = 0;

            foreach (var id in objectIds)
            {
                db.ObjectOperationItems.Add(new ObjectOperationItem { OperationId = operation.Id, ObjectId = id, Position = position++ });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Returns an operation run by id.
        /// </summary>
        public static ObjectOperation GetObjectOperation(Guid operationId)
        {
            using var db = CreateDbContext();

            return db.ObjectOperations.AsNoTracking().FirstOrDefault(x => x.Id == operationId);
        }

        /// <summary>
        /// Returns an operation run by its idempotency key.
        /// </summary>
        public static ObjectOperation GetObjectOperationByKey(string key)
        {
            using var db = CreateDbContext();

            return db.ObjectOperations.AsNoTracking().FirstOrDefault(x => x.Key == key);
        }

        /// <summary>
        /// Returns the runs waiting for or being processed, oldest first.
        /// </summary>
        public static IReadOnlyList<ObjectOperation> GetOpenObjectOperations(int limit)
        {
            using var db = CreateDbContext();

            return [.. db.ObjectOperations.AsNoTracking()
                .Where(x => x.State == ObjectOperationState.Pending || x.State == ObjectOperationState.Running)
                .OrderBy(x => x.Created)
                .Take(limit)];
        }

        /// <summary>
        /// Returns the latest completed run of a kind on a root.
        /// </summary>
        public static ObjectOperation GetLatestObjectOperation(Guid rootId, string kind)
        {
            using var db = CreateDbContext();

            return db.ObjectOperations.AsNoTracking()
                .Where(x => x.RootId == rootId && x.Kind == kind && x.State == ObjectOperationState.Completed)
                .OrderByDescending(x => x.Finished)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the objects of a run in processing order.
        /// </summary>
        public static IReadOnlyList<ObjectOperationItem> GetObjectOperationItems(Guid operationId)
        {
            using var db = CreateDbContext();

            return [.. db.ObjectOperationItems.AsNoTracking().Where(x => x.OperationId == operationId).OrderBy(x => x.Position)];
        }

        /// <summary>
        /// Stores the state, counters and error of a run.
        /// </summary>
        public static void UpdateObjectOperation(ObjectOperation operation)
        {
            using var db = CreateDbContext();

            var row = db.ObjectOperations.FirstOrDefault(x => x.Id == operation.Id);

            if (row is null)
            {
                return;
            }

            row.State = operation.State;
            row.Total = operation.Total;
            row.Done = operation.Done;
            row.Error = operation.Error;
            row.ReferenceId = operation.ReferenceId;
            row.TargetWorkspaceId = operation.TargetWorkspaceId;
            row.Options = operation.Options;
            row.Updated = operation.Updated;
            row.Finished = operation.Finished;

            db.SaveChanges();
        }

        /// <summary>
        /// Marks one object of a run as done.
        /// </summary>
        public static void MarkObjectOperationItem(Guid operationId, Guid objectId, Guid? resultId = null)
        {
            using var db = CreateDbContext();

            var row = db.ObjectOperationItems.FirstOrDefault(x => x.OperationId == operationId && x.ObjectId == objectId);

            if (row is not null && (!row.Done || resultId.HasValue))
            {
                row.Done = true;
                row.ResultId = resultId ?? row.ResultId;
                db.SaveChanges();
            }
        }
    }
}
