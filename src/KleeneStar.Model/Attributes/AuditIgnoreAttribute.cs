using System;

namespace KleeneStar.Model.Attributes
{
    /// <summary>
    /// Marks a property that must never appear in the audit log, not even as the fact that it
    /// changed.
    /// </summary>
    /// <remarks>
    /// Use this sparingly. The default is that everything scalar on an audited entity is
    /// recorded, because an audit log with silent holes is worse than no audit log: a reader
    /// cannot tell an attribute that never moved from one the log declined to mention. Ignoring
    /// is right only for properties that carry no information about the record's state - a
    /// cached derivation, a counter the store maintains - not for properties that are merely
    /// sensitive. For those use <see cref="AuditRedactedAttribute"/>, which keeps the change
    /// auditable without keeping the value.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class AuditIgnoreAttribute : Attribute
    {
    }
}
