using KleeneStar.Model.Entities;
using KleeneStar.Model.Integrity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the audit log.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Serializes the appends of one process, so the read of the head and the write of the
        /// successor cannot interleave.
        /// </summary>
        /// <remarks>
        /// The transaction and the unique index on the sequence are what make a lost race
        /// impossible; this gate is what makes it rare. Without it two threads appending at the
        /// same moment would routinely read the same head, and one of them would have to be
        /// rolled back and retried by the caller - for an audit write, that means an event that
        /// might not get recorded at all.
        /// </remarks>
        private static readonly object _auditGate = new();

        /// <summary>
        /// Appends an event to the audit log, assigning its position in the sequence and sealing
        /// it onto the chain.
        /// </summary>
        /// <remarks>
        /// This is the only write path of the audit store, and it is the reason the log can be
        /// trusted at all. The sequence number, the predecessor hash and the event's own hash
        /// are all resolved <b>inside</b> the transaction from the current head rather than by
        /// the caller: a caller that could choose its own position could insert into the past,
        /// and a caller that could choose its own hash could seal a lie.
        /// <para>
        /// <see cref="AuditEvent.Timestamp"/> is likewise taken here when the caller left it
        /// unset, and always normalized to UTC. A log whose rows carry the local time of
        /// whichever server recorded them cannot be read as one order.
        /// </para>
        /// </remarks>
        /// <param name="event">
        /// The event to append. Its <see cref="AuditEvent.Sequence"/>,
        /// <see cref="AuditEvent.PreviousHash"/> and <see cref="AuditEvent.Hash"/> are assigned
        /// here and overwrite whatever the caller set.
        /// </param>
        /// <returns>The appended event, carrying its assigned position and seal.</returns>
        public static AuditEvent AddAuditEvent(AuditEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);

            lock (_auditGate)
            {
                using var db = CreateDbContext();
                using var transaction = db.Database.BeginTransaction();

                var head = db.AuditEvents
                    .AsNoTracking()
                    .OrderByDescending(x => x.Sequence)
                    .FirstOrDefault();

                @event.Sequence = (head?.Sequence ?? 0) + 1;
                @event.PreviousHash = head?.Hash ?? string.Empty;

                @event.Timestamp = @event.Timestamp == default
                    ? DateTime.UtcNow
                    : @event.Timestamp.ToUniversalTime();

                var ordinal = 0;

                foreach (var delta in @event.Deltas ?? [])
                {
                    delta.EventId = @event.Id;
                    delta.Ordinal = ordinal++;
                }

                // sealed last, because the seal covers the values assigned above
                @event.Hash = AuditSeal.Compute(@event, @event.PreviousHash);

                db.AuditEvents.Add(@event);
                db.SaveChanges();
                transaction.Commit();

                return @event;
            }
        }

        /// <summary>
        /// Returns a materialized collection of audit events from the database.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching events, each with its deltas in recorded order.</returns>
        public static IEnumerable<AuditEvent> GetAuditEvents(IQuery<AuditEvent> query)
        {
            using var db = CreateDbContext();

            return [.. GetAuditEvents(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of audit events using the supplied DbContext.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The DbContext.</param>
        /// <returns>The filtered collection.</returns>
        public static IEnumerable<AuditEvent> GetAuditEvents(IQuery<AuditEvent> query, KleeneStarDbContext context)
        {
            var data = context.AuditEvents
                .Include(x => x.Deltas)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Returns the number of audit events that satisfy the supplied query without
        /// materializing a single row. Counts against the bare event set, so the deltas are
        /// not dragged along. Callers must leave paging off the query.
        /// </summary>
        /// <remarks>
        /// This is the filtered counterpart of <see cref="GetAuditEventCount"/>, which
        /// reports the length of the whole chain.
        /// </remarks>
        /// <param name="query">The query criteria used to filter the counted events.</param>
        /// <returns>The number of matching audit events.</returns>
        public static int CountAuditEvents(IQuery<AuditEvent> query)
        {
            using var db = CreateDbContext();

            return query.Apply(db.AuditEvents.AsNoTracking()).Count();
        }

        /// <summary>
        /// Returns a single audit event by its unique identifier, with its deltas.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        /// <returns>The event, or <c>null</c> when no event matches.</returns>
        public static AuditEvent GetAuditEvent(Guid eventId)
        {
            using var db = CreateDbContext();

            var @event = db.AuditEvents
                .Include(x => x.Deltas)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == eventId);

            OrderDeltas(@event);

            return @event;
        }

        /// <summary>
        /// Returns a single audit event by its position in the sequence, with its deltas.
        /// </summary>
        /// <param name="sequence">The 1-based position.</param>
        /// <returns>The event, or <c>null</c> when the log has no such position.</returns>
        public static AuditEvent GetAuditEvent(long sequence)
        {
            using var db = CreateDbContext();

            var @event = db.AuditEvents
                .Include(x => x.Deltas)
                .AsNoTracking()
                .FirstOrDefault(x => x.Sequence == sequence);

            OrderDeltas(@event);

            return @event;
        }

        /// <summary>
        /// Returns the head of the log - the newest event - or <c>null</c> when the log is
        /// empty.
        /// </summary>
        /// <returns>The head event, or <c>null</c>.</returns>
        public static AuditEvent GetAuditHead()
        {
            using var db = CreateDbContext();

            var @event = db.AuditEvents
                .Include(x => x.Deltas)
                .AsNoTracking()
                .OrderByDescending(x => x.Sequence)
                .FirstOrDefault();

            OrderDeltas(@event);

            return @event;
        }

        /// <summary>
        /// Returns the number of events the log holds.
        /// </summary>
        /// <returns>The length of the log.</returns>
        public static long GetAuditEventCount()
        {
            using var db = CreateDbContext();

            return db.AuditEvents.LongCount();
        }

        /// <summary>
        /// Returns a contiguous slice of the log in sequence order, for verification and export.
        /// </summary>
        /// <param name="fromSequence">The position to start at, inclusive. 0 for the beginning.</param>
        /// <param name="take">The largest number of events to return; 0 for all of them.</param>
        /// <returns>The slice, oldest first.</returns>
        public static IReadOnlyList<AuditEvent> GetAuditRange(long fromSequence, int take)
        {
            using var db = CreateDbContext();

            var query = db.AuditEvents
                .Include(x => x.Deltas)
                .AsNoTracking()
                .Where(x => x.Sequence >= fromSequence)
                .OrderBy(x => x.Sequence);

            var slice = take > 0
                ? query.Take(take).ToList()
                : query.ToList();

            foreach (var @event in slice)
            {
                OrderDeltas(@event);
            }

            return slice;
        }

        /// <summary>
        /// Returns every event recorded about one record, oldest first.
        /// </summary>
        /// <remarks>
        /// The target type takes part in the lookup as well as the id. Ids are unique, so it is
        /// not needed to disambiguate - it is there so the index on (TargetType, TargetId,
        /// Sequence) can serve the query, which is what keeps the projection of a record cheap
        /// as the log grows.
        /// </remarks>
        /// <param name="targetType">The kind of record.</param>
        /// <param name="targetId">The durable id of the record.</param>
        /// <param name="upToSequence">
        /// The position to stop at, inclusive; 0 for the whole trail.
        /// </param>
        /// <returns>The trail, oldest first. Empty when nothing was recorded about the record.</returns>
        public static IReadOnlyList<AuditEvent> GetAuditTrail(AuditTargetType targetType, Guid targetId, long upToSequence = 0)
        {
            using var db = CreateDbContext();

            var query = db.AuditEvents
                .Include(x => x.Deltas)
                .AsNoTracking()
                .Where(x => x.TargetType == targetType && x.TargetId == targetId);

            if (upToSequence > 0)
            {
                query = query.Where(x => x.Sequence <= upToSequence);
            }

            var trail = query.OrderBy(x => x.Sequence).ToList();

            foreach (var @event in trail)
            {
                OrderDeltas(@event);
            }

            return trail;
        }

        /// <summary>
        /// Returns every event of one activity, oldest first.
        /// </summary>
        /// <param name="correlationId">The correlation id shared by the events.</param>
        /// <returns>The events, oldest first.</returns>
        public static IReadOnlyList<AuditEvent> GetAuditActivity(Guid correlationId)
        {
            using var db = CreateDbContext();

            var events = db.AuditEvents
                .Include(x => x.Deltas)
                .AsNoTracking()
                .Where(x => x.CorrelationId == correlationId)
                .OrderBy(x => x.Sequence)
                .ToList();

            foreach (var @event in events)
            {
                OrderDeltas(@event);
            }

            return events;
        }

        /// <summary>
        /// Removes every event recorded before the supplied moment.
        /// </summary>
        /// <remarks>
        /// Pruning is deliberately not automatic and deliberately not exposed to the managers
        /// that write to the log. An audit trail that trims itself is one an attacker can make
        /// trim the evidence, and a retention rule belongs to the operator of the installation
        /// rather than to the code that happens to record an event.
        /// <para>
        /// The removal leaves the chain of the surviving events intact - each still seals onto
        /// the hash of its predecessor - but the oldest survivor's predecessor is gone, so
        /// verification can no longer be anchored at the genesis event. The caller is expected
        /// to record a <see cref="AuditAction.Pruned"/> event naming the range and the terminal
        /// hash that went, which is what keeps the gap itself part of the record.
        /// </para>
        /// </remarks>
        /// <param name="before">The moment, in UTC. Events at or after it are kept.</param>
        /// <returns>
        /// The number of events removed, the highest sequence among them, and the hash the
        /// removed range ended on.
        /// </returns>
        public static (int Count, long LastSequence, string LastHash) PruneAuditEvents(DateTime before)
        {
            using var db = CreateDbContext();
            using var transaction = db.Database.BeginTransaction();

            var horizon = before.ToUniversalTime();

            var doomed = db.AuditEvents
                .Where(x => x.Timestamp < horizon)
                .OrderBy(x => x.Sequence)
                .ToList();

            if (doomed.Count == 0)
            {
                return (0, 0, null);
            }

            var last = doomed[^1];
            var result = (doomed.Count, last.Sequence, last.Hash);

            db.AuditEvents.RemoveRange(doomed);
            db.SaveChanges();
            transaction.Commit();

            return result;
        }

        /// <summary>
        /// Restores the recorded order of an event's deltas, which the store does not guarantee
        /// on read.
        /// </summary>
        /// <param name="event">The event whose deltas are ordered. May be <c>null</c>.</param>
        private static void OrderDeltas(AuditEvent @event)
        {
            if (@event?.Deltas is { Count: > 1 })
            {
                @event.Deltas = [.. @event.Deltas.OrderBy(x => x.Ordinal)];
            }
        }
    }
}
