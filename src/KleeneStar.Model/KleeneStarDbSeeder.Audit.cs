using KleeneStar.Model.Entities;
using KleeneStar.Model.Integrity;
using System;
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
        /// Writes the genesis event of the audit log: the one recording that this installation
        /// populated itself.
        /// </summary>
        /// <remarks>
        /// Unlike every other seeder in this file, this one fabricates nothing. The seeded
        /// commits invent a plausible history for the demonstration objects because a version
        /// dialog with a single entry demonstrates nothing; doing the same to the audit log
        /// would be a straightforward falsification of the record the log exists to be, and
        /// would make the very first thing an auditor reads a lie. The log therefore starts with
        /// exactly one event, and that event is true.
        /// <para>
        /// It also serves a structural purpose: it is the anchor of the hash chain. Every event
        /// appended later seals onto it, so verifying the log from the beginning has somewhere
        /// to begin.
        /// </para>
        /// </remarks>
        /// <param name="db">The database context used for adding the event.</param>
        private static void SeedAudit(KleeneStarDbContext db)
        {
            var timestamp = DateTime.UtcNow;

            var @event = new AuditEvent
            {
                Sequence = 1,
                Timestamp = timestamp,
                Origin = AuditOrigin.System,
                Category = AuditCategory.Lifecycle,
                Action = AuditAction.Seeded,
                Outcome = AuditOutcome.Succeeded,
                Severity = AuditSeverity.Notice,
                Agent = "kleenestar.seeder",
                TargetType = AuditTargetType.Installation,
                TargetKey = "installation",
                CorrelationId = Guid.NewGuid(),
                PreviousHash = string.Empty
            };

            // what the seed actually produced, counted rather than described, so the first entry
            // of the log is a checkable statement about the state it left behind
            var counts = new (string Attribute, int Value)[]
            {
                ("tenants", db.Tenants.Count()),
                ("identities", db.Identities.Count()),
                ("groups", db.Groups.Count()),
                ("workspaces", db.Workspaces.Count()),
                ("classes", db.Classes.Count()),
                ("fields", db.Fields.Count()),
                ("workflows", db.Workflows.Count()),
                ("objects", db.Objects.Count())
            };

            var ordinal = 0;

            foreach (var (attribute, value) in counts)
            {
                @event.Deltas.Add(new AuditDelta
                {
                    EventId = @event.Id,
                    Kind = AuditDeltaKind.Added,
                    Attribute = attribute,
                    ValueKind = AuditValueKind.Number,
                    NewValue = value.ToString(CultureInfo.InvariantCulture),
                    Ordinal = ordinal++
                });
            }

            @event.Hash = AuditSeal.Compute(@event, @event.PreviousHash);

            db.AuditEvents.Add(@event);
        }
    }
}
