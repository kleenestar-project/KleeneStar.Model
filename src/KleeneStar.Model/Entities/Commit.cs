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
    /// Represents the atomic unit of change of a single <see cref="Object"/>: one action,
    /// its author, its time, and the <see cref="Change"/> entries it produced.
    /// </summary>
    /// <remarks>
    /// Commits form an append-only, chronological chain per object. The first commit
    /// (<see cref="CommitType.Created"/>, <see cref="ParentId"/> = <c>null</c>) captures every
    /// populated field; every later commit captures only the delta. The object's
    /// <see cref="Value"/> rows always mirror the head of the chain — both are written inside
    /// the same transaction, so the current state and the head can never diverge.
    /// <para>
    /// The chain carries <b>no foreign keys</b>. <see cref="ObjectId"/>, <see cref="CreatedById"/>
    /// and <see cref="Change.FieldId"/> are plain columns beside a snapshot of the name each id
    /// resolved to at the time of writing. That is deliberate: an audit trail has to outlive the
    /// rows it describes. A cascade from <see cref="Object"/> would erase the history of a deleted
    /// object — which the terminal <see cref="CommitType.Deleted"/> commit exists to preserve —
    /// and a restrict would make deleting an object or an identity impossible once anything had
    /// been recorded about it. The navigation properties below are therefore resolved on read by
    /// the <c>CommitManager</c> rather than mapped, and are <c>null</c> once the referenced row
    /// is gone.
    /// </para>
    /// </remarks>
    public class Commit : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the commit.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the object this commit belongs to.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the key the object carried when the commit was written, e.g.
        /// <c>INC-00123</c>. Snapshotted so a commit stays readable after its object is gone.
        /// </summary>
        public string ObjectKey { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the predecessor commit, or <c>null</c> for the
        /// genesis commit of the chain.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the 1-based position of the commit within its object's chain. Together
        /// with <see cref="ObjectKey"/> it forms the human-readable revision reference exposed
        /// by <see cref="Reference"/>.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the action this commit records.
        /// </summary>
        public CommitType Type { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity that initiated the change, or
        /// <c>null</c> when the change was made by the system.
        /// </summary>
        public Guid? CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the display name the author carried when the commit was written.
        /// Snapshotted so a commit stays attributable after the identity is gone.
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// Gets or sets the date and time the commit was appended.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last written. A commit is
        /// immutable, so this equals <see cref="Created"/> for the whole life of the row; the
        /// column exists only to keep the entity shape uniform across the model.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the optional message describing the intent of the change, analogous to
        /// a commit message in source control.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the field modifications this commit produced, in the order they were
        /// recorded. Only the fields the action touched appear here.
        /// </summary>
        public List<Change> Changes { get; set; } = [];

        /// <summary>
        /// Gets or sets the object this commit belongs to. Not mapped — resolved on read by
        /// the <c>CommitManager</c> and <c>null</c> once the object has been deleted.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Object Object { get; set; }

        /// <summary>
        /// Gets or sets the identity that initiated the change. Not mapped — resolved on read
        /// by the <c>CommitManager</c> and <c>null</c> once the identity has been deleted; use
        /// <see cref="CreatedByName"/> for display in that case.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Identity CreatedBy { get; set; }

        /// <summary>
        /// Gets the stable, human-readable revision reference of the commit, e.g.
        /// <c>INC-00123#4</c>.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public string Reference => string.Concat
        (
            ObjectKey ?? Object?.Key ?? string.Empty,
            "#",
            Number.ToString(CultureInfo.InvariantCulture)
        );

        /// <summary>
        /// Gets whether this commit is the genesis commit of its chain.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public bool IsGenesis => !ParentId.HasValue;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Commit()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the commit.</param>
        public Commit(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Returns the change this commit recorded for the supplied field, or <c>null</c> when
        /// the commit did not touch it.
        /// </summary>
        /// <param name="name">The stable name of the field or system property.</param>
        /// <returns>The change, or <c>null</c>.</returns>
        public Change GetChange(string name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? null
                : Changes?.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
