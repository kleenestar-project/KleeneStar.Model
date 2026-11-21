using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents one attribute-level state change inside an <see cref="AuditEvent"/>: which
    /// attribute moved, in which direction, from what, to what.
    /// </summary>
    /// <remarks>
    /// The delta is what makes the audit log a record of state rather than a record of activity.
    /// An event saying "the class was updated" is not reconstructable; the same event carrying
    /// <c>Modified name: "Bug" -> "Defect"</c> is. Replaying every delta of a target in sequence
    /// order therefore reproduces the state that target held at any point, without the log ever
    /// storing a full snapshot per event.
    /// <para>
    /// A delta is keyed by <see cref="Attribute"/> - the stable name of a class field
    /// (<c>AffectedCI</c>) or of a property of the record (<c>name</c>, <c>state</c>, ...).
    /// <see cref="AttributeId"/> is set for the former and <c>null</c> for the latter, which is
    /// what keeps the log schema-aware: a class field can be resolved back to its
    /// <see cref="Field"/> definition for a localized label and type-specific formatting, while
    /// a plain property still has somewhere to live.
    /// </para>
    /// <para>
    /// Like <see cref="AuditEvent"/>, the row carries no foreign key to the field. A field that
    /// has since been deleted leaves <see cref="Field"/> unresolvable;
    /// <see cref="Attribute"/> is the snapshot that keeps the entry readable anyway.
    /// </para>
    /// </remarks>
    public class AuditDelta : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the delta.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the event this delta belongs to.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the event this delta belongs to.
        /// </summary>
        [JsonIgnore]
        public AuditEvent Event { get; set; }

        /// <summary>
        /// Gets or sets what this delta did to the attribute: brought it into existence, moved
        /// it, or took it away. Stored rather than inferred from the payloads.
        /// </summary>
        public AuditDeltaKind Kind { get; set; }

        /// <summary>
        /// Gets or sets the stable name of the attribute that changed: the
        /// <see cref="Entities.Field.Name"/> of a class field, or the lower-case name of a
        /// property of the record.
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the class field this delta describes, or
        /// <c>null</c> when it describes a plain property of the record.
        /// </summary>
        public Guid? AttributeId { get; set; }

        /// <summary>
        /// Gets or sets how <see cref="OldValue"/> and <see cref="NewValue"/> are to be read
        /// back.
        /// </summary>
        public AuditValueKind ValueKind { get; set; }

        /// <summary>
        /// Gets or sets the serialized value the attribute held before the event. Carries no
        /// meaning when <see cref="Kind"/> is <see cref="AuditDeltaKind.Added"/>.
        /// </summary>
        public string OldValue { get; set; }

        /// <summary>
        /// Gets or sets the serialized value the attribute holds after the event. Carries no
        /// meaning when <see cref="Kind"/> is <see cref="AuditDeltaKind.Removed"/>.
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// Gets or sets the 0-based position of the delta inside its event, so the order the
        /// deltas were recorded in survives a round trip through the store.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the field definition this delta describes. Not mapped - resolved on
        /// read by the <c>AuditManager</c> and <c>null</c> for a plain property or for a field
        /// that has since been deleted.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Field Field { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditDelta()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the delta.</param>
        public AuditDelta(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Creates a delta recording that an attribute came into existence.
        /// </summary>
        /// <param name="attribute">The stable name of the attribute.</param>
        /// <param name="value">The serialized value it now holds.</param>
        /// <param name="valueKind">How the value is to be read back.</param>
        /// <param name="attributeId">The class field id, or <c>null</c> for a plain property.</param>
        /// <returns>The delta.</returns>
        public static AuditDelta Added(string attribute, string value, AuditValueKind valueKind = AuditValueKind.Text, Guid? attributeId = null)
        {
            return new AuditDelta
            {
                Kind = AuditDeltaKind.Added,
                Attribute = attribute,
                AttributeId = attributeId,
                ValueKind = valueKind,
                OldValue = null,
                NewValue = value
            };
        }

        /// <summary>
        /// Creates a delta recording that an attribute moved from one value to another.
        /// </summary>
        /// <param name="attribute">The stable name of the attribute.</param>
        /// <param name="oldValue">The serialized value it held before.</param>
        /// <param name="newValue">The serialized value it holds after.</param>
        /// <param name="valueKind">How the values are to be read back.</param>
        /// <param name="attributeId">The class field id, or <c>null</c> for a plain property.</param>
        /// <returns>The delta.</returns>
        public static AuditDelta Modified(string attribute, string oldValue, string newValue, AuditValueKind valueKind = AuditValueKind.Text, Guid? attributeId = null)
        {
            return new AuditDelta
            {
                Kind = AuditDeltaKind.Modified,
                Attribute = attribute,
                AttributeId = attributeId,
                ValueKind = valueKind,
                OldValue = oldValue,
                NewValue = newValue
            };
        }

        /// <summary>
        /// Creates a delta recording that an attribute ceased to exist, preserving what it held.
        /// </summary>
        /// <param name="attribute">The stable name of the attribute.</param>
        /// <param name="value">The serialized value it held before.</param>
        /// <param name="valueKind">How the value is to be read back.</param>
        /// <param name="attributeId">The class field id, or <c>null</c> for a plain property.</param>
        /// <returns>The delta.</returns>
        public static AuditDelta Removed(string attribute, string value, AuditValueKind valueKind = AuditValueKind.Text, Guid? attributeId = null)
        {
            return new AuditDelta
            {
                Kind = AuditDeltaKind.Removed,
                Attribute = attribute,
                AttributeId = attributeId,
                ValueKind = valueKind,
                OldValue = value,
                NewValue = null
            };
        }
    }
}
