using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with <see cref="Sprint"/> entries and the
    /// sprint assignment columns of <see cref="Entities.Object"/>.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the sprints that match the given query criteria, opening a
        /// short-lived DbContext for the call.
        /// </summary>
        /// <param name="query">The query criteria. Must not be null.</param>
        /// <returns>The matching sprints, materialized.</returns>
        public static IEnumerable<Sprint> GetSprints(IQuery<Sprint> query)
        {
            using var db = CreateDbContext();

            return [.. GetSprints(query, db)];
        }

        /// <summary>
        /// Returns the sprints that match the given query criteria, using the supplied
        /// <paramref name="context"/>. Includes the workspace navigation property.
        /// </summary>
        /// <param name="query">The query criteria. Must not be null.</param>
        /// <param name="context">The query context.</param>
        /// <returns>The matching sprints.</returns>
        public static IEnumerable<Sprint> GetSprints(IQuery<Sprint> query, KleeneStarDbContext context)
        {
            var data = context.Sprints
                .Include(x => x.Workspace)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Inserts the given sprint if no record with the same <see cref="Sprint.Id"/>
        /// already exists.
        /// </summary>
        /// <param name="sprintEntry">The sprint to add. Cannot be null.</param>
        public static void Add(Sprint sprintEntry)
        {
            ArgumentNullException.ThrowIfNull(sprintEntry);

            using var db = CreateDbContext();

            var query = new Query<Sprint>()
                .WhereEquals(x => x.Id, sprintEntry.Id);

            if (query.Apply(db.Sprints).Any())
            {
                return;
            }

            db.AddEntity(sprintEntry);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the scalar properties of the existing sprint identified by
        /// <see cref="Sprint.Id"/>.
        /// </summary>
        /// <param name="sprintEntry">The sprint holding the updated values. Cannot be null.</param>
        public static void Update(Sprint sprintEntry)
        {
            ArgumentNullException.ThrowIfNull(sprintEntry);

            using var db = CreateDbContext();

            var query = new Query<Sprint>()
                .WhereEquals(x => x.Id, sprintEntry.Id);

            var dbEntry = query.Apply(db.Sprints).FirstOrDefault();

            if (dbEntry is null)
            {
                return;
            }

            dbEntry.Name = sprintEntry.Name;
            dbEntry.Goal = sprintEntry.Goal;
            dbEntry.State = sprintEntry.State;
            dbEntry.Start = sprintEntry.Start;
            dbEntry.End = sprintEntry.End;
            dbEntry.Capacity = sprintEntry.Capacity;
            dbEntry.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Removes the sprint identified by <see cref="Sprint.Id"/> and moves the
        /// objects committed to it back to the product backlog of their workspace.
        /// </summary>
        /// <param name="sprintEntry">The sprint to remove. Cannot be null.</param>
        public static void Remove(Sprint sprintEntry)
        {
            ArgumentNullException.ThrowIfNull(sprintEntry);

            using var db = CreateDbContext();

            var query = new Query<Sprint>()
                .WhereEquals(x => x.Id, sprintEntry.Id);

            var dbEntry = query.Apply(db.Sprints).FirstOrDefault();

            if (dbEntry is null)
            {
                return;
            }

            // move committed objects back to the backlog before the sprint disappears,
            // so their ranks restart cleanly behind the existing backlog items
            var backlogTail = db.Objects
                .Where(x => x.WorkspaceId == dbEntry.WorkspaceId && x.SprintId == null)
                .Select(x => (int?)x.SprintRank)
                .Max() ?? 0;

            var committed = db.Objects
                .Where(x => x.SprintId == dbEntry.Id)
                .OrderBy(x => x.SprintRank)
                .ThenBy(x => x.RawId)
                .ToList();

            foreach (var objectEntry in committed)
            {
                objectEntry.SprintId = null;
                objectEntry.SprintRank = ++backlogTail;
                objectEntry.Updated = DateTime.UtcNow;
            }

            db.Remove(dbEntry);
            db.SaveChanges();
        }

        /// <summary>
        /// Returns the objects of the given workspace that belong to the given sprint —
        /// or, when <paramref name="sprintId"/> is <see langword="null"/>, the product
        /// backlog of the workspace — ordered by rank.
        /// </summary>
        /// <param name="workspaceId">The owning workspace id.</param>
        /// <param name="sprintId">The sprint id, or <see langword="null"/> for the backlog.</param>
        /// <returns>The matching objects, materialized and ordered by rank.</returns>
        public static IReadOnlyList<Entities.Object> GetObjectsBySprint(Guid workspaceId, Guid? sprintId)
        {
            using var db = CreateDbContext();

            return [.. db.Objects
                .AsNoTracking()
                .Where(x => x.WorkspaceId == workspaceId && x.SprintId == sprintId)
                .OrderBy(x => x.SprintRank)
                .ThenBy(x => x.RawId)];
        }

        /// <summary>
        /// Moves the object identified by <paramref name="objectId"/> into the given
        /// sprint (or back to the backlog when <paramref name="sprintId"/> is
        /// <see langword="null"/>) and re-ranks both the source and the target group so
        /// the ranks stay dense and 1-based.
        /// </summary>
        /// <param name="objectId">The object to move.</param>
        /// <param name="sprintId">The target sprint, or <see langword="null"/> for the backlog.</param>
        /// <param name="rank">The requested 1-based rank in the target group, or
        /// <see langword="null"/> to append at the end.</param>
        public static void SetObjectSprint(Guid objectId, Guid? sprintId, int? rank)
        {
            using var db = CreateDbContext();

            var objectEntry = db.Objects.FirstOrDefault(x => x.Id == objectId);

            if (objectEntry is null)
            {
                return;
            }

            var sourceSprintId = objectEntry.SprintId;

            // re-rank the group the object leaves
            if (sourceSprintId != sprintId)
            {
                var source = db.Objects
                    .Where(x => x.WorkspaceId == objectEntry.WorkspaceId && x.SprintId == sourceSprintId && x.Id != objectId)
                    .OrderBy(x => x.SprintRank)
                    .ThenBy(x => x.RawId)
                    .ToList();

                for (var i = 0; i < source.Count; i++)
                {
                    source[i].SprintRank = i + 1;
                }
            }

            // insert into the target group at the requested position (append by default)
            var target = db.Objects
                .Where(x => x.WorkspaceId == objectEntry.WorkspaceId && x.SprintId == sprintId && x.Id != objectId)
                .OrderBy(x => x.SprintRank)
                .ThenBy(x => x.RawId)
                .ToList();

            var index = Math.Clamp(rank ?? target.Count + 1, 1, target.Count + 1) - 1;
            target.Insert(index, objectEntry);

            for (var i = 0; i < target.Count; i++)
            {
                target[i].SprintRank = i + 1;
            }

            objectEntry.SprintId = sprintId;
            objectEntry.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Sets the story-point estimate of the object identified by
        /// <paramref name="objectId"/>.
        /// </summary>
        /// <param name="objectId">The object to update.</param>
        /// <param name="points">The estimate, or <see langword="null"/> to clear it.</param>
        public static void SetObjectStoryPoints(Guid objectId, int? points)
        {
            using var db = CreateDbContext();

            var objectEntry = db.Objects.FirstOrDefault(x => x.Id == objectId);

            if (objectEntry is null)
            {
                return;
            }

            objectEntry.StoryPoints = points;
            objectEntry.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }
    }
}
