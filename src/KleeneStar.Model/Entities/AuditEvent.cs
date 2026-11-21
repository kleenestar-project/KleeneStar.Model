using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents one fact the installation records about itself: what happened, who caused it,
    /// when, to which record, and what it changed.
    /// </summary>
    /// <remarks>
    /// The audit log is a single append-only sequence for the whole installation, unlike the
    /// per-object <see cref="Commit"/> chain it sits beside. The two are not redundant. A commit
    /// is the version history of one object and exists so a user can read and restore it; an
    /// audit event is the forensic record of the installation and exists so a reader who trusts
    /// nothing can reconstruct what occurred. Object mutations therefore appear in both: the
    /// commit chain carries them for the object, and the audit log carries them for the
    /// installation, linked by <see cref="TargetRevision"/>.
    /// <para>
    /// <b>Time base.</b> <see cref="Timestamp"/> is always UTC, taken once at the moment of
    /// recording and never rewritten. It is not sufficient on its own: two events inside the
    /// same clock tick would be indistinguishable, and a clock adjustment would reorder the
    /// past. <see cref="Sequence"/> is therefore the authoritative order — a gap-free counter
    /// assigned inside the append transaction. Reconstruct by sequence; display by timestamp.
    /// </para>
    /// <para>
    /// <b>No foreign keys.</b> Like <see cref="Commit"/>, this row carries none.
    /// <see cref="ActorId"/> and <see cref="TargetId"/> are plain columns beside a snapshot of
    /// the name each resolved to at the time of writing. An audit trail whose rows could be
    /// erased by deleting the identity or the record they describe would be worthless precisely
    /// in the case it exists for. The navigation properties are resolved on read by the
    /// <c>AuditManager</c> and stay <c>null</c> once the referenced row is gone.
    /// </para>
    /// <para>
    /// <b>Integrity.</b> Each event carries the hash of its own canonical content chained onto
    /// <see cref="PreviousHash"/>. Editing, deleting, reordering or inserting a row breaks the
    /// chain from that point on, which the <c>AuditManager</c> detects and names. This makes the
    /// log tamper-evident, not tamper-proof: it cannot stop somebody with write access to the
    /// database, it can only make sure they cannot do it unnoticed.
    /// </para>
    /// </remarks>
    public class AuditEvent : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the event.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the 1-based position of the event in the installation-wide sequence.
        /// This is the authoritative order of the log: it is assigned inside the append
        /// transaction, is unique, and does not depend on the clock.
        /// </summary>
        public long Sequence { get; set; }

        /// <summary>
        /// Gets or sets the moment the event was recorded, in UTC. Immutable for the life of
        /// the row.
        /// </summary>
        /// <remarks>
        /// Always stored in UTC so the log stays comparable across servers, deployments and
        /// daylight-saving boundaries. The user interface converts to the reader's zone on
        /// display; nothing converts on write.
        /// </remarks>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets what set the event in motion.
        /// </summary>
        public AuditOrigin Origin { get; set; }

        /// <summary>
        /// Gets or sets the functional area the event belongs to.
        /// </summary>
        public AuditCategory Category { get; set; }

        /// <summary>
        /// Gets or sets what was done.
        /// </summary>
        public AuditAction Action { get; set; }

        /// <summary>
        /// Gets or sets whether the action took effect.
        /// </summary>
        public AuditOutcome Outcome { get; set; }

        /// <summary>
        /// Gets or sets how much attention the event warrants.
        /// </summary>
        public AuditSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity responsible for the event, or
        /// <c>null</c> when no identity was resolved: a system task, or an authentication
        /// attempt whose credential named nobody.
        /// </summary>
        public Guid? ActorId { get; set; }

        /// <summary>
        /// Gets or sets the display name the actor carried when the event was recorded.
        /// Snapshotted so the event stays attributable after the identity is gone.
        /// </summary>
        public string ActorName { get; set; }

        /// <summary>
        /// Gets or sets the stable name of the non-human party that acted, for events whose
        /// <see cref="Origin"/> is not <see cref="AuditOrigin.User"/>: the scheduled task, the
        /// API version, the portal. <c>null</c> for a person acting through the interface.
        /// </summary>
        public string Agent { get; set; }

        /// <summary>
        /// Gets or sets the network address the request arrived from, or <c>null</c> when the
        /// event did not come from a request.
        /// </summary>
        public string ClientAddress { get; set; }

        /// <summary>
        /// Gets or sets the kind of record the event is about.
        /// </summary>
        public AuditTargetType TargetType { get; set; }

        /// <summary>
        /// Gets or sets the durable identifier of the record the event is about, or <c>null</c>
        /// when the event is not about a particular record. The id outlives the record, which
        /// is what lets the trail of a deleted record still be read.
        /// </summary>
        public Guid? TargetId { get; set; }

        /// <summary>
        /// Gets or sets the human-readable name the target carried when the event was recorded
        /// (an object key, a class name). Snapshotted so the event stays readable after the
        /// record is gone.
        /// </summary>
        public string TargetKey { get; set; }

        /// <summary>
        /// Gets or sets the version the target reached through this event, or <c>null</c> when
        /// the target is not versioned.
        /// </summary>
        /// <remarks>
        /// For an object this is the number of the <see cref="Commit"/> the event corresponds
        /// to, which is what ties an audit entry to a revision the user can open and restore.
        /// Together with <see cref="TargetId"/> it turns a set of events into an ordered
        /// sequence per record, independent of the installation-wide <see cref="Sequence"/>.
        /// </remarks>
        public int? TargetRevision { get; set; }

        /// <summary>
        /// Gets or sets the identifier shared by every event of one activity, so the events a
        /// single user action produced can be read as the unit they were.
        /// </summary>
        /// <remarks>
        /// One action rarely produces one event. Deleting a class removes its fields, its forms
        /// and its objects; each of those is a fact worth recording on its own, and all of them
        /// belong to the same decision. The correlation id is what lets a reader recover that
        /// decision from its consequences.
        /// </remarks>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the id of the event that caused this one, or <c>null</c> when it was
        /// caused directly by the activity. Turns a correlated set into a tree.
        /// </summary>
        public Guid? CausationId { get; set; }

        /// <summary>
        /// Gets or sets the hash of the preceding event in the chain, or the empty string for
        /// the first event the log holds.
        /// </summary>
        public string PreviousHash { get; set; }

        /// <summary>
        /// Gets or sets the hash over this event's canonical content and
        /// <see cref="PreviousHash"/>, as computed by <c>AuditSeal</c>.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the attribute-level state changes the event produced, in the order they
        /// were recorded. An event with no deltas is legitimate: it records that something
        /// happened which changed no attribute.
        /// </summary>
        public List<AuditDelta> Deltas { get; set; } = [];

        /// <summary>
        /// Gets or sets the identity responsible for the event. Not mapped - resolved on read
        /// by the <c>AuditManager</c> and <c>null</c> once the identity has been deleted; use
        /// <see cref="ActorName"/> for display in that case.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Identity Actor { get; set; }

        /// <summary>
        /// Gets the stable, human-readable reference of the event, e.g. <c>AUD-000042</c>.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public string Reference => string.Concat
        (
            "AUD-",
            Sequence.ToString("D6", CultureInfo.InvariantCulture)
        );

        /// <summary>
        /// Gets whether this event is the first the log holds.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public bool IsGenesis => string.IsNullOrEmpty(PreviousHash);

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEvent()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the event.</param>
        public AuditEvent(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Returns the delta this event recorded for the supplied attribute, or <c>null</c>
        /// when the event did not touch it.
        /// </summary>
        /// <param name="name">The stable name of the attribute.</param>
        /// <returns>The delta, or <c>null</c>.</returns>
        public AuditDelta GetDelta(string name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? null
                : Deltas?.FirstOrDefault(x => string.Equals(x.Attribute, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
