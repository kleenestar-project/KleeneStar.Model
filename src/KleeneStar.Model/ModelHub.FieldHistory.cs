using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// One recorded change of a field value: which object, which field, when, and from what to
    /// what - the row of a commit's change list flattened together with the commit's time.
    /// </summary>
    /// <param name="ObjectId">The object whose value changed.</param>
    /// <param name="FieldId">The field whose value changed.</param>
    /// <param name="Changed">When the commit carrying the change was recorded (UTC).</param>
    /// <param name="OldValue">The serialized value before, or null when the field had none.</param>
    /// <param name="NewValue">The serialized value after, or null when it was removed.</param>
    public sealed record FieldChange(Guid ObjectId, Guid FieldId, DateTime Changed, string OldValue, string NewValue);

    /// <summary>
    /// Provides the bulk read of the value history the reports are computed from.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns every recorded change of the given fields, oldest first.
        /// </summary>
        /// <remarks>
        /// The reports ask "when did each of these objects enter which state", which is the
        /// history of one field per class - the workflow field. Reading it per object would be
        /// a query per object; reading it per field is one query for the whole set, and the
        /// caller narrows it to the objects it is about. The commit chain stays the authority:
        /// this is a projection of it, not a second record.
        /// </remarks>
        /// <param name="fieldIds">The fields whose changes are read. Cannot be null.</param>
        /// <returns>The changes, ordered by the time of their commit.</returns>
        public static IReadOnlyList<FieldChange> GetFieldChanges(IReadOnlyCollection<Guid> fieldIds)
        {
            ArgumentNullException.ThrowIfNull(fieldIds);

            if (fieldIds.Count == 0)
            {
                return [];
            }

            using var db = CreateDbContext();

            var ids = fieldIds.Distinct().ToList();

            return
            [
                .. db.Changes
                    .AsNoTracking()
                    .Where(x => x.FieldId != null && ids.Contains(x.FieldId.Value))
                    .Join
                    (
                        db.Commits.AsNoTracking(),
                        change => change.CommitId,
                        commit => commit.Id,
                        (change, commit) => new
                        {
                            commit.ObjectId,
                            FieldId = change.FieldId.Value,
                            commit.Created,
                            commit.Number,
                            change.Ordinal,
                            change.OldValue,
                            change.NewValue
                        }
                    )
                    .AsEnumerable()
                    .OrderBy(x => x.Created)
                    .ThenBy(x => x.Number)
                    .ThenBy(x => x.Ordinal)
                    .Select(x => new FieldChange(x.ObjectId, x.FieldId, x.Created, x.OldValue, x.NewValue))
            ];
        }
    }
}
