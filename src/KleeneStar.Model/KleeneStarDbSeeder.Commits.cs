using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Populates the <c>Commit</c> and <c>Change</c> tables with a plausible history for every
        /// seeded object, so the version history dialog has a chain to show from the very first
        /// launch instead of a single entry that demonstrates nothing.
        /// </summary>
        /// <remarks>
        /// The chains are constructed backwards from the state the objects were seeded in, which
        /// is the property that matters: replaying a chain must arrive at exactly the object's
        /// current <see cref="Value"/> rows and system properties, or the history would be lying
        /// about the present. Each object therefore gets
        /// <list type="number">
        /// <item>a genesis commit carrying its full state as it would have been created —
        /// unassigned, and at the entry status of its workflow;</item>
        /// <item>a transition commit whenever the object's workflow field has since moved off
        /// that entry status;</item>
        /// <item>an update commit whenever the object has an assignee.</item>
        /// </list>
        /// A commit that would carry no change is not written, so an object that was seeded
        /// unassigned and at its entry status simply has a chain of one.
        /// </remarks>
        /// <param name="db">The database context used for adding the new commits.</param>
        private static void SeedCommits(KleeneStarDbContext db)
        {
            var objects = db.Objects
                .AsNoTracking()
                .OrderBy(o => o.Created)
                .ThenBy(o => o.Key)
                .ToList();

            if (objects.Count == 0)
            {
                return;
            }

            var identities = db.Identities
                .AsNoTracking()
                .ToDictionary(x => x.Id, x => x.Name);

            var fields = db.Fields
                .AsNoTracking()
                .Where(f => f.State == FieldState.Active && !f.Deprecated)
                .ToList();

            var fieldsById = fields.ToDictionary(x => x.Id, x => x);

            var valuesByObject = db.Values
                .AsNoTracking()
                .ToList()
                .GroupBy(x => x.ObjectId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var entryStatusByWorkflow = ResolveEntryStatuses(db);

            foreach (var entity in objects)
            {
                var values = valuesByObject.TryGetValue(entity.Id, out var rows) ? rows : [];
                var workflowValue = values.FirstOrDefault(x =>
                    fieldsById.TryGetValue(x.FieldId, out var field) &&
                    field.FieldType == FieldType.Workflow);

                var entryStatus = workflowValue is not null &&
                    fieldsById.TryGetValue(workflowValue.FieldId, out var workflowField) &&
                    workflowField.WorkflowId.HasValue &&
                    entryStatusByWorkflow.TryGetValue(workflowField.WorkflowId.Value, out var start)
                        ? start
                        : null;

                var author = entity.CreatorId;
                var authorName = author.HasValue && identities.TryGetValue(author.Value, out var name) ? name : null;

                var number = 0;
                Guid? parent = null;

                // 1 - the genesis commit: everything the object carries, but as it would have
                // looked at creation time
                var genesis = NewCommit(entity, ref number, ref parent, CommitType.Created, author, authorName, entity.Created, null);

                AddSystemChange(genesis, "key", null, entity.Key);
                AddSystemChange(genesis, "summary", null, entity.Summary);
                AddSystemChange(genesis, "description", null, entity.Description);
                AddSystemChange(genesis, "state", null, entity.State.ToString());
                AddSystemChange(genesis, "kind", null, entity.Kind);
                AddSystemChange(genesis, "parent", null, entity.ParentId?.ToString());
                AddSystemChange(genesis, "sprint", null, entity.SprintId?.ToString());
                AddSystemChange(genesis, "storypoints", null, entity.StoryPoints?.ToString(CultureInfo.InvariantCulture));

                foreach (var value in values.OrderBy(x => fieldsById.TryGetValue(x.FieldId, out var f) ? f.Name : string.Empty))
                {
                    if (!fieldsById.TryGetValue(value.FieldId, out var field))
                    {
                        continue;
                    }

                    // the workflow field starts at the entry status; the transition below moves
                    // it to where the object actually stands
                    var initial = value == workflowValue && entryStatus is not null ? entryStatus : value.Data;

                    AddFieldChange(genesis, field, null, initial);
                }

                db.Commits.Add(genesis);

                // 2 - the transition that brought the object to the status it is in now
                if (workflowValue is not null &&
                    entryStatus is not null &&
                    !string.Equals(entryStatus, workflowValue.Data, StringComparison.OrdinalIgnoreCase) &&
                    fieldsById.TryGetValue(workflowValue.FieldId, out var statusField))
                {
                    var transition = NewCommit(entity, ref number, ref parent, CommitType.Transitioned, author, authorName, Later(entity, 1), null);

                    AddFieldChange(transition, statusField, entryStatus, workflowValue.Data);

                    db.Commits.Add(transition);
                }

                // 3 - the edit that put the object in someone's hands
                if (entity.AssigneeId.HasValue)
                {
                    var assignedBy = entity.UpdaterId ?? author;
                    var assignedByName = assignedBy.HasValue && identities.TryGetValue(assignedBy.Value, out var updater) ? updater : null;

                    var assignment = NewCommit(entity, ref number, ref parent, CommitType.Updated, assignedBy, assignedByName, Later(entity, 2), null);

                    AddSystemChange(assignment, "assignee", null, entity.AssigneeId.Value.ToString());

                    db.Commits.Add(assignment);
                }
            }
        }

        /// <summary>
        /// Returns the name of the entry status of each workflow: the one marked as a start
        /// state, or the first participating status when the workflow marks none.
        /// </summary>
        /// <param name="db">The database context.</param>
        /// <returns>The entry status name per workflow id.</returns>
        private static Dictionary<Guid, string> ResolveEntryStatuses(KleeneStarDbContext db)
        {
            var workflows = db.Workflows
                .AsNoTracking()
                .Include(x => x.Statuses)
                .Include(x => x.WorkflowStatuses)
                .ToList();

            var result = new Dictionary<Guid, string>();

            foreach (var workflow in workflows)
            {
                var statuses = workflow.Statuses ?? [];

                if (statuses.Count == 0)
                {
                    continue;
                }

                var startId = (workflow.WorkflowStatuses ?? [])
                    .Where(x => x.IsStart)
                    .Select(x => (Guid?)x.StatusId)
                    .FirstOrDefault();

                var start = startId.HasValue
                    ? statuses.FirstOrDefault(x => x.Id == startId.Value)
                    : null;

                result[workflow.Id] = (start ?? statuses[0]).Name;
            }

            return result;
        }

        /// <summary>
        /// Creates the next commit of an object's chain, advancing the running number and the
        /// predecessor the caller carries.
        /// </summary>
        /// <param name="entity">The object the chain belongs to.</param>
        /// <param name="number">The running revision number.</param>
        /// <param name="parent">The running predecessor id.</param>
        /// <param name="type">The action the commit records.</param>
        /// <param name="authorId">The author id.</param>
        /// <param name="authorName">The author name snapshot.</param>
        /// <param name="timestamp">The commit timestamp.</param>
        /// <param name="message">The commit message.</param>
        /// <returns>The commit.</returns>
        private static Commit NewCommit(Entities.Object entity, ref int number, ref Guid? parent, CommitType type, Guid? authorId, string authorName, DateTime timestamp, string message)
        {
            var commit = new Commit
            {
                ObjectId = entity.Id,
                ObjectKey = entity.Key,
                ParentId = parent,
                Number = ++number,
                Type = type,
                CreatedById = authorId,
                CreatedByName = authorName,
                Created = timestamp,
                Updated = timestamp,
                Message = message,
                Changes = []
            };

            parent = commit.Id;

            return commit;
        }

        /// <summary>
        /// Appends a change describing a system property of the object, skipping the entry when
        /// the property never held anything.
        /// </summary>
        /// <param name="commit">The commit the change is appended to.</param>
        /// <param name="name">The property name.</param>
        /// <param name="oldValue">The value before.</param>
        /// <param name="newValue">The value after.</param>
        private static void AddSystemChange(Commit commit, string name, string oldValue, string newValue)
        {
            if (string.Equals(oldValue, newValue, StringComparison.Ordinal))
            {
                return;
            }

            commit.Changes.Add(new Change
            {
                CommitId = commit.Id,
                Name = name,
                OldValue = oldValue,
                NewValue = newValue,
                Ordinal = commit.Changes.Count
            });
        }

        /// <summary>
        /// Appends a change describing a class field of the object, skipping the entry when the
        /// field never held anything.
        /// </summary>
        /// <param name="commit">The commit the change is appended to.</param>
        /// <param name="field">The field.</param>
        /// <param name="oldValue">The payload before.</param>
        /// <param name="newValue">The payload after.</param>
        private static void AddFieldChange(Commit commit, Field field, string oldValue, string newValue)
        {
            if (string.Equals(oldValue, newValue, StringComparison.Ordinal))
            {
                return;
            }

            commit.Changes.Add(new Change
            {
                CommitId = commit.Id,
                FieldId = field.Id,
                Name = field.Name,
                OldValue = oldValue,
                NewValue = newValue,
                Ordinal = commit.Changes.Count
            });
        }

        /// <summary>
        /// Returns a timestamp a number of hours after the object was created, never later than
        /// the object's own last-updated stamp, so a seeded chain cannot claim to have changed an
        /// object after the object says it last changed.
        /// </summary>
        /// <param name="entity">The object.</param>
        /// <param name="hours">The offset in hours.</param>
        /// <returns>The timestamp.</returns>
        private static DateTime Later(Entities.Object entity, int hours)
        {
            var candidate = entity.Created.AddHours(hours);

            return entity.Updated > entity.Created && candidate > entity.Updated
                ? entity.Updated
                : candidate;
        }
    }
}
