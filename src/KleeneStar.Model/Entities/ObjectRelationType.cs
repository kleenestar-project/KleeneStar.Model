using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebRelation;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// The durable definition of a relation an <see cref="ObjectRelation"/> may carry - the
    /// administrative half of the hybrid link system. One row states one fact told from two
    /// sides ("blocks" / "is blocked by") together with the rules under which it may be
    /// established: which classes it accepts as a target, how many relations of it may meet at
    /// each end, and what it does to the workflow of its source.
    /// </summary>
    /// <remarks>
    /// The meaning of a relation lives here rather than in a C# enum, which is what lets an
    /// administrator invent one without a code change. There is no fixed set: the table is
    /// the whole catalog, and at runtime its rows are published into
    /// <see cref="RelationRegistry"/> - replacing whatever the framework registered by
    /// default - so every surface offers exactly what this installation defined.
    /// <para>
    /// <see cref="Key"/> rather than <see cref="Id"/> is what a stored relation references,
    /// because the registry, the wire contract and the client all address a type by its
    /// stable string id. The <see cref="Guid"/> exists only to give the row the identity
    /// every entity of this model carries.
    /// </para>
    /// </remarks>
    public class ObjectRelationType : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the relation type.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the stable wire id of the relation, for example <c>blocks</c>. It is
        /// what a stored relation references and what the client sends back, so it never changes
        /// once relations carry it.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets how the relation reads on the object the relation was created from, or
        /// the i18n key it is translated through.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets how the same relation reads on the other end, or the i18n key of it.
        /// A symmetric relation carries its <see cref="Label"/> here as well.
        /// </summary>
        public string InverseLabel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether both ends are named alike ("similar to"),
        /// in which case the counterpart cannot drift away from the label.
        /// </summary>
        public bool Symmetric { get; set; }

        /// <summary>
        /// Gets or sets the id of the link system that offers the relation - the native
        /// object system, the native web system, or one a plugin registered.
        /// </summary>
        public string System { get; set; } = RelationSystem.Object;

        /// <summary>
        /// Gets the names of the classes a target may have. Left empty, every class is
        /// accepted.
        /// </summary>
        /// <remarks>
        /// The classes are held by name rather than by id because the name is what the wire
        /// carries at both ends: the target reference of a relation states the class it resolves
        /// to, and the framework validates the reference against this list by that string. A
        /// renamed class therefore has to be re-picked here, which is the price of a target
        /// rule a reader can understand without a lookup.
        /// </remarks>
        public List<string> TargetClasses { get; set; } = [];

        /// <summary>
        /// Gets or sets how many relations of the relation may meet at each end.
        /// </summary>
        public RelationCardinality Cardinality { get; set; } = RelationCardinality.ManyToMany;

        /// <summary>
        /// Gets or sets what a relation of the relation does to the workflow of its source.
        /// </summary>
        public RelationEffect Effect { get; set; } = RelationEffect.None;

        /// <summary>
        /// Gets or sets a value indicating whether the relation may still be used. A
        /// deactivated relation keeps rendering its relations but is no longer offered.
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Gets or sets the symbolic icon name rendered in front of the relation's group
        /// heading and of every row below it.
        /// </summary>
        public string Icon { get; set; } = "link";

        /// <summary>
        /// Gets or sets the position of the relation in the administered order, which is the
        /// order the link surface groups by.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the explanation shown to the person picking the relation, or the
        /// i18n key of it.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the relation was defined.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the relation was last changed.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObjectRelationType()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the relation type.</param>
        public ObjectRelationType(Guid id)
        {
            Id = id;
        }
    }
}
