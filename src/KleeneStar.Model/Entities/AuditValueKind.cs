using System;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Says how the serialized payloads of an <see cref="AuditDelta"/> are to be read back.
    /// </summary>
    /// <remarks>
    /// The store keeps values as text, because an audit row has to survive the deletion of the
    /// field definition that gave it its type. Without a recorded value kind a later reader is
    /// left guessing from the characters: "2026-08-22" could be a date or a string, "1" could be
    /// a number or a boolean or an ordinal, and a comparison between two revisions would depend
    /// on which guess it made. Recording the kind is what turns the text back into a value.
    /// <para>
    /// The kind describes the payload, not the attribute: an attribute whose type was changed
    /// keeps the kind each event recorded for it at the time, which is exactly what makes the
    /// older events still readable.
    /// </para>
    /// <para>
    /// The values are persisted as their ordinal, so new entries are appended rather than
    /// inserted.
    /// </para>
    /// </remarks>
    public enum AuditValueKind
    {
        /// <summary>
        /// Free text, recorded verbatim.
        /// </summary>
        Text,

        /// <summary>
        /// A number, formatted with the invariant culture so it reads the same everywhere.
        /// </summary>
        Number,

        /// <summary>
        /// A truth value, recorded as <c>true</c> or <c>false</c>.
        /// </summary>
        Boolean,

        /// <summary>
        /// A point in time, recorded as a round-trip ISO 8601 string in UTC.
        /// </summary>
        Timestamp,

        /// <summary>
        /// The identifier of another record, recorded as a GUID in its dashed form. The record
        /// it names may since have been deleted, which is why the log keeps the id rather than
        /// a foreign key.
        /// </summary>
        Reference,

        /// <summary>
        /// A member of an enumeration, recorded as its stable member name rather than its
        /// ordinal, so a later insertion into the enumeration cannot change what past events
        /// mean.
        /// </summary>
        Enumeration,

        /// <summary>
        /// Several values of one attribute, recorded as their individual payloads joined by a
        /// line feed.
        /// </summary>
        Collection,

        /// <summary>
        /// The attribute changed, but its value is a secret the log must not hold: a password
        /// hash, a token, a private key. Both payloads read as a fixed marker. The fact of the
        /// change is auditable; the value is not recoverable from the log.
        /// </summary>
        Redacted,

        /// <summary>
        /// Binary content the log does not carry. The payloads name the size and content type
        /// instead, so the change is auditable without the log becoming a file store.
        /// </summary>
        Binary
    }

    /// <summary>
    /// Provides extension methods for the <see cref="AuditValueKind"/> enumeration.
    /// </summary>
    public static class AuditValueKindExtensions
    {
        /// <summary>
        /// The payload written in place of a value the log must not hold.
        /// </summary>
        public const string RedactedMarker = "[redacted]";

        /// <summary>
        /// Returns the wire token the REST API exchanges the value kind as.
        /// </summary>
        /// <param name="kind">The value kind.</param>
        /// <returns>The lower-case wire token.</returns>
        public static string Token(this AuditValueKind kind)
        {
            return kind switch
            {
                AuditValueKind.Text => "text",
                AuditValueKind.Number => "number",
                AuditValueKind.Boolean => "boolean",
                AuditValueKind.Timestamp => "timestamp",
                AuditValueKind.Reference => "reference",
                AuditValueKind.Enumeration => "enumeration",
                AuditValueKind.Collection => "collection",
                AuditValueKind.Redacted => "redacted",
                AuditValueKind.Binary => "binary",
                _ => "text"
            };
        }

        /// <summary>
        /// Returns the localized text key for the value kind, suitable for passing to
        /// <c>I18N.Translate</c>.
        /// </summary>
        /// <param name="kind">The value kind.</param>
        /// <returns>The translation key.</returns>
        public static string Text(this AuditValueKind kind)
        {
            return string.Concat("kleenestar.core:audit.delta.value.", kind.Token());
        }

        /// <summary>
        /// Parses a wire token into the matching value kind. An unknown, empty or <c>null</c>
        /// token reads as <see cref="AuditValueKind.Text"/>.
        /// </summary>
        /// <param name="token">The wire token.</param>
        /// <returns>The parsed value kind.</returns>
        public static AuditValueKind Parse(string token)
        {
            var normalized = (token?.Trim() ?? string.Empty).ToLowerInvariant();

            foreach (var candidate in Enum.GetValues<AuditValueKind>())
            {
                if (candidate.Token() == normalized)
                {
                    return candidate;
                }
            }

            return AuditValueKind.Text;
        }
    }
}
