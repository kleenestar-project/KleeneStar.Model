using KleeneStar.Model.Entities;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KleeneStar.Model.Integrity
{
    /// <summary>
    /// Computes the hash that chains one <see cref="AuditEvent"/> to the one before it, and
    /// therefore defines what "unchanged" means for the audit log.
    /// </summary>
    /// <remarks>
    /// There is exactly one implementation because there has to be: a verifier that canonicalizes
    /// an event even slightly differently from the writer reports every row as tampered, and a
    /// verifier that canonicalizes less than the writer reports none. Both failures are silent,
    /// and both destroy the only property the chain exists to provide.
    /// <para>
    /// The canonical form is a text encoding with a fixed field order, an explicit separator
    /// that cannot occur inside a field, invariant formatting for every value, and the deltas
    /// folded in in their recorded order. It deliberately is not JSON: a serializer is free to
    /// reorder properties, change how it escapes, or omit defaults between versions, and any of
    /// those would silently invalidate every hash written before the change.
    /// </para>
    /// <para>
    /// What this buys: any edit, deletion, insertion or reordering of a row breaks the chain
    /// from that row onwards, and the break names the first affected sequence. What it does not
    /// buy: it cannot stop somebody who holds the database from rewriting the whole chain from
    /// the point they altered. Detecting that requires an anchor the installation does not
    /// control - an off-box copy of a recent hash, or a signature over it. The chain is what
    /// makes such an anchor cheap: one hash pins every event before it.
    /// </para>
    /// </remarks>
    public static class AuditSeal
    {
        /// <summary>
        /// The separator between canonical fields. A control character, so it cannot occur in a
        /// name or a serialized value and no field can be made to impersonate two.
        /// </summary>
        private const char FieldSeparator = '\u001F';

        /// <summary>
        /// The separator between the canonical records of the individual deltas.
        /// </summary>
        private const char RecordSeparator = '\u001E';

        /// <summary>
        /// The text written for a value that is <c>null</c>, so an absent value and an empty
        /// one do not hash alike.
        /// </summary>
        private const string NullMarker = "\u0000";

        /// <summary>
        /// Computes the hash of an event, chained onto the hash of its predecessor.
        /// </summary>
        /// <remarks>
        /// The predecessor's hash is folded into this event's hash rather than merely stored
        /// beside it. That is the whole mechanism: it makes every event depend on the complete
        /// history before it, so a row cannot be altered without altering every row after it.
        /// </remarks>
        /// <param name="event">The event to seal. Its own <see cref="AuditEvent.Hash"/> is ignored.</param>
        /// <param name="previousHash">
        /// The hash of the preceding event, or <c>null</c> for the first event of the log.
        /// </param>
        /// <returns>The hash as a lower-case hexadecimal string.</returns>
        public static string Compute(AuditEvent @event, string previousHash)
        {
            ArgumentNullException.ThrowIfNull(@event);

            var canonical = Canonicalize(@event, previousHash);
            var digest = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));

            return Convert.ToHexStringLower(digest);
        }

        /// <summary>
        /// Returns whether the recorded hash of an event matches its content and the supplied
        /// predecessor hash.
        /// </summary>
        /// <param name="event">The event to check.</param>
        /// <param name="previousHash">The hash of the preceding event, or <c>null</c>.</param>
        /// <returns><c>true</c> when the event is intact.</returns>
        public static bool Verify(AuditEvent @event, string previousHash)
        {
            if (@event is null || string.IsNullOrEmpty(@event.Hash))
            {
                return false;
            }

            return string.Equals(@event.Hash, Compute(@event, previousHash), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Builds the canonical text form of an event: every field that carries meaning, in a
        /// fixed order, formatted so the same event always produces the same bytes.
        /// </summary>
        /// <remarks>
        /// <see cref="AuditEvent.RawId"/> is excluded because it is assigned by the store and
        /// differs between a database and its restored copy, which must hash alike.
        /// <see cref="AuditEvent.Hash"/> is excluded because it is the output.
        /// </remarks>
        /// <param name="event">The event.</param>
        /// <param name="previousHash">The hash of the preceding event, or <c>null</c>.</param>
        /// <returns>The canonical text.</returns>
        private static string Canonicalize(AuditEvent @event, string previousHash)
        {
            var builder = new StringBuilder();

            Append(builder, previousHash ?? string.Empty);
            Append(builder, @event.Id.ToString("D", CultureInfo.InvariantCulture));
            Append(builder, @event.Sequence.ToString(CultureInfo.InvariantCulture));
            // the round-trip format carries the kind as well as the value, so a row whose
            // timestamp was re-interpreted as local time no longer matches its hash
            Append(builder, @event.Timestamp.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
            Append(builder, @event.Origin.Token());
            Append(builder, @event.Category.Token());
            Append(builder, @event.Action.Token());
            Append(builder, @event.Outcome.Token());
            Append(builder, @event.Severity.Token());
            Append(builder, @event.ActorId);
            Append(builder, @event.ActorName);
            Append(builder, @event.Agent);
            Append(builder, @event.ClientAddress);
            Append(builder, @event.TargetType.Token());
            Append(builder, @event.TargetId);
            Append(builder, @event.TargetKey);
            Append(builder, @event.TargetRevision?.ToString(CultureInfo.InvariantCulture));
            Append(builder, @event.CorrelationId.ToString("D", CultureInfo.InvariantCulture));
            Append(builder, @event.CausationId);

            foreach (var delta in (@event.Deltas ?? []).OrderBy(x => x.Ordinal))
            {
                builder.Append(RecordSeparator);

                Append(builder, delta.Ordinal.ToString(CultureInfo.InvariantCulture));
                Append(builder, delta.Kind.Token());
                Append(builder, delta.Attribute);
                Append(builder, delta.AttributeId);
                Append(builder, delta.ValueKind.Token());
                Append(builder, delta.OldValue);
                Append(builder, delta.NewValue);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Appends one canonical field, distinguishing an absent value from an empty one.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="value">The value, or <c>null</c>.</param>
        private static void Append(StringBuilder builder, string value)
        {
            builder.Append(value ?? NullMarker).Append(FieldSeparator);
        }

        /// <summary>
        /// Appends one canonical field holding an optional identifier.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="value">The identifier, or <c>null</c>.</param>
        private static void Append(StringBuilder builder, Guid? value)
        {
            Append(builder, value?.ToString("D", CultureInfo.InvariantCulture));
        }
    }
}
