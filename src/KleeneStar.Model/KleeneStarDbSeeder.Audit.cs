using KleeneStar.Model.Entities;
using KleeneStar.Model.Integrity;
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
        /// The agent the genesis event of the audit log names.
        /// </summary>
        public const string AuditAgent = "kleenestar.model.seeder";

        /// <summary>
        /// Writes the genesis event of the audit log: sequence 1, action
        /// <see cref="AuditAction.Seeded"/>, one <see cref="AuditDelta"/> per counted entity set.
        /// </summary>
        /// <remarks>
        /// The event is the anchor of the hash chain (<see cref="AuditEvent.PreviousHash"/> empty).
        /// Its deltas count the rows the base seed left in the store; a plugin seed running later
        /// appends its own event through <see cref="RecordSeed"/>.
        /// </remarks>
        /// <param name="db">The database context used for adding the event.</param>
        private static void SeedAudit(KleeneStarDbContext db)
        {
            var @event = CreateSeedEvent(AuditAgent, CountSeededRows(db));

            @event.Sequence = 1;
            @event.PreviousHash = string.Empty;
            @event.Hash = AuditSeal.Compute(@event, @event.PreviousHash);

            db.AuditEvents.Add(@event);
        }

        /// <summary>
        /// Appends an <see cref="AuditAction.Seeded"/> event to the audit log through the given
        /// context and saves it: the next sequence number after the current head, sealed onto the
        /// head's hash, one <see cref="AuditDelta"/> per counted entity set
        /// (<see cref="CountSeededRows"/>).
        /// </summary>
        /// <remarks>
        /// For a seed that writes through its own context instead of the managers. The head is
        /// read and the event written under the gate of
        /// <see cref="ModelHub.AddAuditEvent(AuditEvent)"/>, inside one transaction.
        /// </remarks>
        /// <param name="db">The database context. Cannot be null.</param>
        /// <param name="agent">The agent the event names, for example the plugin id. Cannot be blank.</param>
        public static void RecordSeed(KleeneStarDbContext db, string agent)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentException.ThrowIfNullOrWhiteSpace(agent);

            lock (ModelHub._auditGate)
            {
                var transaction = db.Database.IsRelational() && db.Database.CurrentTransaction is null
                    ? db.Database.BeginTransaction()
                    : null;

                using (transaction)
                {
                    var head = db.AuditEvents
                        .AsNoTracking()
                        .OrderByDescending(x => x.Sequence)
                        .FirstOrDefault();

                    var @event = CreateSeedEvent(agent, CountSeededRows(db));

                    @event.Sequence = (head?.Sequence ?? 0) + 1;
                    @event.PreviousHash = head?.Hash ?? string.Empty;
                    @event.Hash = AuditSeal.Compute(@event, @event.PreviousHash);

                    db.AuditEvents.Add(@event);
                    db.SaveChanges();
                    transaction?.Commit();
                }
            }
        }

        /// <summary>
        /// Counts the rows of the entity sets a seed event reports.
        /// </summary>
        /// <param name="db">The database context.</param>
        /// <returns>The pairs of attribute name and row count, in reporting order.</returns>
        private static IReadOnlyList<(string Attribute, int Value)> CountSeededRows(KleeneStarDbContext db)
        {
            return
            [
                ("tenants", db.Tenants.Count()),
                ("identities", db.Identities.Count()),
                ("groups", db.Groups.Count()),
                ("workspaces", db.Workspaces.Count()),
                ("classes", db.Classes.Count()),
                ("fields", db.Fields.Count()),
                ("workflows", db.Workflows.Count()),
                ("objects", db.Objects.Count())
            ];
        }

        /// <summary>
        /// Creates an unsealed <see cref="AuditAction.Seeded"/> event of the installation.
        /// </summary>
        /// <param name="agent">The agent the event names.</param>
        /// <param name="counts">The counted entity sets, one delta each.</param>
        /// <returns>The event without sequence, predecessor hash and hash.</returns>
        private static AuditEvent CreateSeedEvent(string agent, IReadOnlyList<(string Attribute, int Value)> counts)
        {
            var @event = new AuditEvent
            {
                Timestamp = DateTime.UtcNow,
                Origin = AuditOrigin.System,
                Category = AuditCategory.Lifecycle,
                Action = AuditAction.Seeded,
                Outcome = AuditOutcome.Succeeded,
                Severity = AuditSeverity.Notice,
                Agent = agent,
                TargetType = AuditTargetType.Installation,
                TargetKey = "installation",
                CorrelationId = Guid.NewGuid()
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

            return @event;
        }
    }
}
