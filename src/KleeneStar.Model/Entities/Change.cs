using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a single field modification inside a <see cref="Commit"/>: what changed,
    /// what it was, and what it became.
    /// </summary>
    /// <remarks>
    /// A change is keyed by <see cref="Name"/> — the stable name of the class field
    /// (<c>AffectedCI</c>) or of a system property of the object (<c>summary</c>,
    /// <c>assignee</c>, …). <see cref="FieldId"/> is set for the former and <c>null</c> for the
    /// latter, which is what keeps the history schema-aware: a class field can be resolved back
    /// to its <see cref="Field"/> definition for a localized display name and type-specific
    /// formatting, while a system property still has somewhere to live.
    /// <para>
    /// Like <see cref="Commit"/>, the row carries no foreign key. A field that has since been
    /// deleted leaves <see cref="Field"/> unresolvable; <see cref="Name"/> is the snapshot that
    /// keeps the entry readable anyway.
    /// </para>
    /// </remarks>
    public class Change : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the change.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the commit this change belongs to.
        /// </summary>
        public Guid CommitId { get; set; }

        /// <summary>
        /// Gets or sets the commit this change belongs to.
        /// </summary>
        [JsonIgnore]
        public Commit Commit { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the class field this change modifies, or
        /// <c>null</c> when the change modifies a system property of the object.
        /// </summary>
        public Guid? FieldId { get; set; }

        /// <summary>
        /// Gets or sets the stable name of the modified attribute: the <see cref="Entities.Field.Name"/>
        /// of a class field, or the lower-case name of a system property of the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the serialized value before the change, or <c>null</c> when the
        /// attribute had none.
        /// </summary>
        public string OldValue { get; set; }

        /// <summary>
        /// Gets or sets the serialized value after the change, or <c>null</c> when the
        /// attribute was cleared.
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// Gets or sets the 0-based position of the change inside its commit, so the order the
        /// changes were recorded in survives a round trip through the store.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the field definition this change modifies. Not mapped — resolved on
        /// read by the <c>CommitManager</c> and <c>null</c> for a system property or for a
        /// field that has since been deleted.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Field Field { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Change()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the change.</param>
        public Change(Guid id)
        {
            Id = id;
        }
    }
}
