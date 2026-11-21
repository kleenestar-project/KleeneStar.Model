using System;

namespace KleeneStar.Model.Attributes
{
    /// <summary>
    /// Marks a property whose change must be recorded but whose value must not be: a password
    /// hash, a token, a secret.
    /// </summary>
    /// <remarks>
    /// A redacted property still produces a delta, with the same
    /// <see cref="Entities.AuditDeltaKind"/> as any other and with both payloads replaced by
    /// <see cref="Entities.AuditValueKindExtensions.RedactedMarker"/>. That is the point: an
    /// administrator quietly resetting somebody's password is exactly the event an audit log
    /// exists to surface, and it would be invisible if the property were ignored outright.
    /// <para>
    /// Because the marker is constant, two consecutive changes to a redacted property produce
    /// two deltas that look alike. They are still distinct events at distinct sequences, so the
    /// log records that the value changed twice - it just cannot say what it changed to, which
    /// is what makes the log safe to read and to export.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class AuditRedactedAttribute : Attribute
    {
    }
}
