using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the worklogs of an object, the newest first.
        /// </summary>
        /// <param name="objectId">The object.</param>
        /// <returns>The worklogs.</returns>
        public static IReadOnlyList<Worklog> GetWorklogs(Guid objectId)
        {
            using var db = CreateDbContext();

            return [.. db.Worklogs.AsNoTracking().Where(x => x.ObjectId == objectId).OrderByDescending(x => x.Started)];
        }

        /// <summary>
        /// Returns the worklogs of a person in a period.
        /// </summary>
        /// <param name="identityId">The person.</param>
        /// <param name="from">The start, inclusive.</param>
        /// <param name="to">The end, exclusive.</param>
        /// <returns>The worklogs, the oldest first.</returns>
        public static IReadOnlyList<Worklog> GetWorklogs(Guid identityId, DateTime from, DateTime to)
        {
            using var db = CreateDbContext();

            return [.. db.Worklogs.AsNoTracking().Where(x => x.IdentityId == identityId && x.Started >= from && x.Started < to).OrderBy(x => x.Started)];
        }

        /// <summary>
        /// Returns the minutes logged per object.
        /// </summary>
        /// <param name="objectIds">The objects.</param>
        /// <returns>The minutes by object.</returns>
        public static Dictionary<Guid, int> GetLoggedMinutes(IEnumerable<Guid> objectIds)
        {
            var ids = objectIds.ToHashSet();

            using var db = CreateDbContext();

            return db.Worklogs.AsNoTracking()
                .Where(x => ids.Contains(x.ObjectId))
                .GroupBy(x => x.ObjectId)
                .Select(x => new { x.Key, Minutes = x.Sum(w => w.Minutes) })
                .ToDictionary(x => x.Key, x => x.Minutes);
        }

        /// <summary>
        /// Returns a worklog.
        /// </summary>
        /// <param name="worklogId">The worklog.</param>
        /// <returns>The worklog, or <see langword="null"/>.</returns>
        public static Worklog GetWorklog(Guid worklogId)
        {
            using var db = CreateDbContext();

            return db.Worklogs.AsNoTracking().FirstOrDefault(x => x.Id == worklogId);
        }

        /// <summary>
        /// Adds, changes or removes a worklog.
        /// </summary>
        /// <param name="worklog">The worklog.</param>
        /// <param name="remove">Whether to remove it.</param>
        public static void SaveWorklog(Worklog worklog, bool remove = false)
        {
            ArgumentNullException.ThrowIfNull(worklog);

            using var db = CreateDbContext();

            var stored = db.Worklogs.FirstOrDefault(x => x.Id == worklog.Id);

            if (remove)
            {
                if (stored is not null)
                {
                    db.Worklogs.Remove(stored);
                }
            }
            else if (stored is null)
            {
                db.Worklogs.Add(worklog);
            }
            else
            {
                stored.Started = worklog.Started;
                stored.Minutes = worklog.Minutes;
                stored.Comment = worklog.Comment;
                stored.Updated = DateTime.UtcNow;
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Returns the planning figures of objects.
        /// </summary>
        /// <param name="objectIds">The objects.</param>
        /// <returns>The rows by object; an object without one is missing.</returns>
        public static Dictionary<Guid, ObjectPlanning> GetObjectPlannings(IEnumerable<Guid> objectIds)
        {
            var ids = objectIds.ToHashSet();

            using var db = CreateDbContext();

            return db.ObjectPlannings.AsNoTracking().Where(x => ids.Contains(x.ObjectId)).ToDictionary(x => x.ObjectId);
        }

        /// <summary>
        /// Returns the objects due with a milestone.
        /// </summary>
        /// <param name="milestoneId">The milestone.</param>
        /// <returns>The object ids.</returns>
        public static IReadOnlyList<Guid> GetMilestoneObjects(Guid milestoneId)
        {
            using var db = CreateDbContext();

            return [.. db.ObjectPlannings.AsNoTracking().Where(x => x.MilestoneId == milestoneId).Select(x => x.ObjectId)];
        }

        /// <summary>
        /// Changes the planning figures of an object, creating its row on first use.
        /// </summary>
        /// <param name="objectId">The object.</param>
        /// <param name="change">The change.</param>
        /// <returns>The row as stored.</returns>
        public static ObjectPlanning UpdateObjectPlanning(Guid objectId, Action<ObjectPlanning> change)
        {
            using var db = CreateDbContext();

            var row = db.ObjectPlannings.FirstOrDefault(x => x.ObjectId == objectId);

            if (row is null)
            {
                row = new ObjectPlanning { Id = Guid.NewGuid(), ObjectId = objectId };
                db.ObjectPlannings.Add(row);
            }

            change(row);
            row.Updated = DateTime.UtcNow;
            db.SaveChanges();

            return row;
        }

        /// <summary>
        /// Returns the milestones of a workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace.</param>
        /// <returns>The milestones, the earliest due first.</returns>
        public static IReadOnlyList<Milestone> GetMilestones(Guid workspaceId)
        {
            using var db = CreateDbContext();

            return [.. db.Milestones.AsNoTracking().Where(x => x.WorkspaceId == workspaceId).OrderBy(x => x.Due == null).ThenBy(x => x.Due).ThenBy(x => x.Name)];
        }

        /// <summary>
        /// Returns a milestone.
        /// </summary>
        /// <param name="milestoneId">The milestone.</param>
        /// <returns>The milestone, or <see langword="null"/>.</returns>
        public static Milestone GetMilestone(Guid milestoneId)
        {
            using var db = CreateDbContext();

            return db.Milestones.AsNoTracking().FirstOrDefault(x => x.Id == milestoneId);
        }

        /// <summary>
        /// Adds, changes or removes a milestone.
        /// </summary>
        /// <param name="milestone">The milestone.</param>
        /// <param name="remove">Whether to remove it.</param>
        public static void SaveMilestone(Milestone milestone, bool remove = false)
        {
            ArgumentNullException.ThrowIfNull(milestone);

            using var db = CreateDbContext();

            var stored = db.Milestones.FirstOrDefault(x => x.Id == milestone.Id);

            if (remove)
            {
                if (stored is not null)
                {
                    // the in-memory provider does not apply SetNull; clear the references here
                    foreach (var row in db.ObjectPlannings.Where(x => x.MilestoneId == milestone.Id))
                    {
                        row.MilestoneId = null;
                    }

                    db.Milestones.Remove(stored);
                }
            }
            else if (stored is null)
            {
                db.Milestones.Add(milestone);
            }
            else
            {
                stored.Name = milestone.Name;
                stored.Description = milestone.Description;
                stored.Due = milestone.Due;
                stored.State = milestone.State;
                stored.Updated = DateTime.UtcNow;
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Returns whether work is logged on a workspace's objects.
        /// </summary>
        /// <param name="workspaceId">The workspace.</param>
        /// <returns><see langword="true"/> when time tracking is on.</returns>
        public static bool GetTimeTracking(Guid workspaceId)
        {
            using var db = CreateDbContext();

            return db.WorkspacePlannings.AsNoTracking().Any(x => x.WorkspaceId == workspaceId && x.TimeTracking);
        }

        /// <summary>
        /// Switches time tracking of a workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace.</param>
        /// <param name="enabled">Whether work is logged.</param>
        public static void SetTimeTracking(Guid workspaceId, bool enabled)
        {
            using var db = CreateDbContext();

            var row = db.WorkspacePlannings.FirstOrDefault(x => x.WorkspaceId == workspaceId);

            if (row is null)
            {
                row = new WorkspacePlanning { Id = Guid.NewGuid(), WorkspaceId = workspaceId };
                db.WorkspacePlannings.Add(row);
            }

            row.TimeTracking = enabled;
            db.SaveChanges();
        }

        /// <summary>
        /// Returns the weekly capacity of a person.
        /// </summary>
        /// <param name="identityId">The person.</param>
        /// <returns>The minutes, or <see langword="null"/> when none was set.</returns>
        public static int? GetWeeklyCapacity(Guid identityId)
        {
            using var db = CreateDbContext();

            return db.IdentityCapacities.AsNoTracking().FirstOrDefault(x => x.IdentityId == identityId)?.WeeklyMinutes;
        }

        /// <summary>
        /// Sets the weekly capacity of a person.
        /// </summary>
        /// <param name="identityId">The person.</param>
        /// <param name="minutes">The minutes per week.</param>
        public static void SetWeeklyCapacity(Guid identityId, int minutes)
        {
            using var db = CreateDbContext();

            var row = db.IdentityCapacities.FirstOrDefault(x => x.IdentityId == identityId);

            if (row is null)
            {
                row = new IdentityCapacity { Id = Guid.NewGuid(), IdentityId = identityId };
                db.IdentityCapacities.Add(row);
            }

            row.WeeklyMinutes = minutes;
            db.SaveChanges();
        }
    }
}
