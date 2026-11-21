using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Records one act of creating objects - an object from the wizard, or a whole work
    /// structure from a composite template - under the key its client submitted it with.
    /// </summary>
    /// <remarks>
    /// The record makes the creation <em>idempotent</em> and <em>atomic</em>. Idempotent: the
    /// key is unique, so a form submitted twice - a double click, a retry after a timeout whose
    /// first attempt did arrive - finds the record of the first submission and is answered with
    /// what it created instead of creating it again. Atomic: the objects and relations are
    /// listed as they are written, so a creation that fails half-way can take back exactly what
    /// it wrote (<see cref="ObjectCreationState.RolledBack"/>) and leave neither an incomplete
    /// structure nor one a retry would duplicate. A rolled-back record may be run again under
    /// the same key.
    /// </remarks>
    public class ObjectCreation : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the idempotency key the client submitted the creation under. Unique.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the template the structure was created from, or <c>null</c>.
        /// </summary>
        public Guid? TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the object the creation answers with - the one the caller asked for, the
        /// root of a structure - once it exists.
        /// </summary>
        public Guid? RootObjectId { get; set; }

        /// <summary>
        /// Gets or sets where the creation stands.
        /// </summary>
        public ObjectCreationState State { get; set; }

        /// <summary>
        /// Gets or sets the objects written so far, a comma-separated list of ids in the order
        /// they were written.
        /// </summary>
        public string ObjectIds { get; set; }

        /// <summary>
        /// Gets or sets the relations written so far, a comma-separated list of ids.
        /// </summary>
        public string RelationIds { get; set; }

        /// <summary>
        /// Gets or sets what made the creation fail, or <c>null</c>.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the identity on whose behalf the objects were created, or <c>null</c>.
        /// </summary>
        public Guid? IdentityId { get; set; }

        /// <summary>
        /// Gets or sets the date and time the creation started.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time the creation completed or was rolled back.
        /// </summary>
        public DateTime? Finished { get; set; }
    }

    /// <summary>
    /// Where an <see cref="ObjectCreation"/> stands.
    /// </summary>
    /// <remarks>
    /// Persisted as the ordinal; new members are appended.
    /// </remarks>
    public enum ObjectCreationState
    {
        /// <summary>
        /// The creation is under way; a second submission under the same key is refused rather
        /// than run in parallel.
        /// </summary>
        Running,

        /// <summary>
        /// Everything was created; a second submission is answered with the root object.
        /// </summary>
        Completed,

        /// <summary>
        /// The creation failed and what it had written was taken back; the key may be used again.
        /// </summary>
        RolledBack
    }
}
