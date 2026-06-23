using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides the data-access helpers for <see cref="WorkspaceBookmark"/> entities.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns a materialized collection of workspace bookmarks matching the given query,
        /// including the related owner and workspace.
        /// </summary>
        /// <param name="query">The query criteria used to filter the bookmarks. Must not be null.</param>
        /// <returns>The filtered bookmarks.</returns>
        public static IEnumerable<WorkspaceBookmark> GetWorkspaceBookmarks(IQuery<WorkspaceBookmark> query)
        {
            using var db = CreateDbContext();

            return [.. GetWorkspaceBookmarks(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a collection of workspace bookmarks matching the given query, evaluated against
        /// the supplied context. The owner and workspace navigations are eagerly loaded.
        /// </summary>
        /// <param name="query">The query criteria used to filter the bookmarks. Must not be null.</param>
        /// <param name="context">The context in which the query is executed. Cannot be null.</param>
        /// <returns>The filtered bookmarks (not tracked).</returns>
        public static IEnumerable<WorkspaceBookmark> GetWorkspaceBookmarks(IQuery<WorkspaceBookmark> query, KleeneStarDbContext context)
        {
            var data = context.WorkspaceBookmarks
                .Include(x => x.Owner)
                .Include(x => x.Workspace)
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Returns the single bookmark of the supplied identity for the supplied workspace, or
        /// <see langword="null"/> when the identity has neither favorited nor visited it.
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="workspaceId">The id of the workspace.</param>
        /// <returns>The bookmark, or <see langword="null"/>.</returns>
        public static WorkspaceBookmark GetWorkspaceBookmark(Guid ownerId, Guid workspaceId)
        {
            using var db = CreateDbContext();

            return db.WorkspaceBookmarks
                .Include(x => x.Workspace)
                .AsNoTracking()
                .FirstOrDefault(x => x.OwnerId == ownerId && x.WorkspaceId == workspaceId);
        }

        /// <summary>
        /// Inserts or updates the bookmark of the supplied identity for the supplied workspace.
        /// The unique composite index on (Owner, Workspace) guarantees a single row per pair; an
        /// existing row is mutated in place. Returns <see langword="null"/> when either the owner
        /// or the workspace does not exist (the foreign keys would otherwise reject the write).
        /// </summary>
        /// <param name="ownerId">The id of the owning identity.</param>
        /// <param name="workspaceId">The id of the workspace.</param>
        /// <param name="favorite">
        /// When set, the new favorite state; when <see langword="null"/>, the favorite state is
        /// left untouched (used by "record visit").
        /// </param>
        /// <param name="recordVisit">
        /// When <see langword="true"/>, the last-visited timestamp is advanced to now (used by
        /// "record visit"); when <see langword="false"/>, it is left untouched.
        /// </param>
        /// <returns>The persisted bookmark, or <see langword="null"/>.</returns>
        public static WorkspaceBookmark UpsertWorkspaceBookmark(Guid ownerId, Guid workspaceId, bool? favorite, bool recordVisit)
        {
            using var db = CreateDbContext();

            var ownerExists = db.Identities.AsNoTracking().Any(i => i.Id == ownerId);
            var workspaceExists = db.Workspaces.AsNoTracking().Any(w => w.Id == workspaceId);
            if (!ownerExists || !workspaceExists)
            {
                return null;
            }

            var now = DateTime.UtcNow;
            var bookmark = db.WorkspaceBookmarks
                .FirstOrDefault(x => x.OwnerId == ownerId && x.WorkspaceId == workspaceId);

            if (bookmark is null)
            {
                bookmark = new WorkspaceBookmark
                {
                    OwnerId = ownerId,
                    WorkspaceId = workspaceId,
                    Favorite = favorite ?? false,
                    LastVisited = recordVisit ? now : default,
                    Created = now,
                    Updated = now
                };

                db.WorkspaceBookmarks.Add(bookmark);
            }
            else
            {
                if (favorite.HasValue)
                {
                    bookmark.Favorite = favorite.Value;
                }

                if (recordVisit)
                {
                    bookmark.LastVisited = now;
                }

                bookmark.Updated = now;
            }

            db.SaveChanges();

            return bookmark;
        }
    }
}
