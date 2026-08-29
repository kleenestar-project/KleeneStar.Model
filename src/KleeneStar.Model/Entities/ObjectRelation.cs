using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebRelation;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// One semantic connection an object holds - the entity half of the hybrid link system.
    /// A relation either joins two objects of the installation ("INC-1 is blocked by CHG-7") or
    /// points at an address outside it; both are carried by this one structure, because what
    /// a relation means lives in its <see cref="TypeKey"/> and its <see cref="System"/> rather
    /// than in a subclass.
    /// </summary>
    /// <remarks>
    /// A relation is one fact told from two sides and is therefore stored <b>once</b>. The
    /// end that authored it reads it under the label of its type, the end it points at reads
    /// the same row under the inverse label; <see cref="Direction"/> decides whether the
    /// second reading happens at all.
    /// <para>
    /// A relation that stopped holding is marked <see cref="RelationStatus.Obsolete"/> rather
    /// than deleted, because the fact that it once held is part of the history of both ends.
    /// </para>
    /// <para>
    /// <see cref="TargetObjectId"/> is null exactly when the relation is external; the address
    /// then lives in <see cref="TargetUri"/> and its caption in <see cref="TargetTitle"/>.
    /// </para>
    /// </remarks>
    public class ObjectRelation : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the relation. It addresses the relation itself,
        /// which is what the update and the delete of the REST contract operate on, and is
        /// independent of the two ends.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the link system that owns the relation. The system decides how
        /// the target is addressed and which dialog page establishes it.
        /// </summary>
        public string System { get; set; } = RelationSystem.Object;

        /// <summary>
        /// Gets or sets the key of the <see cref="ObjectRelationType"/> that classifies the relation,
        /// for example <c>blocks</c>. The type carries both labels, the accepted classes, the
        /// cardinality and the workflow effect.
        /// </summary>
        public string TypeKey { get; set; }

        /// <summary>
        /// Gets or sets whether the relation is read from its source only or from both ends.
        /// </summary>
        public RelationDirection Direction { get; set; } = RelationDirection.Bidirectional;

        /// <summary>
        /// Gets or sets the lifecycle state of the relation.
        /// </summary>
        public RelationStatus Status { get; set; } = RelationStatus.Active;

        /// <summary>
        /// Gets or sets the unique identifier of the source object - the end the relation was
        /// authored from.
        /// </summary>
        public Guid SourceObjectId { get; set; }

        /// <summary>
        /// Gets or sets the source object navigation property.
        /// </summary>
        [JsonIgnore]
        public Object SourceObject { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the target object, or <see langword="null"/>
        /// when the relation points outside the installation.
        /// </summary>
        public Guid? TargetObjectId { get; set; }

        /// <summary>
        /// Gets or sets the target object navigation property.
        /// </summary>
        [JsonIgnore]
        public Object TargetObject { get; set; }

        /// <summary>
        /// Gets or sets the absolute address an external relation points at. It is null for a
        /// relation between two objects, whose address is derived from the target's route.
        /// </summary>
        public string TargetUri { get; set; }

        /// <summary>
        /// Gets or sets the caption an external relation is rendered under, since an address
        /// alone rarely says what it leads to.
        /// </summary>
        public string TargetTitle { get; set; }

        /// <summary>
        /// Gets or sets the free text note explaining why the two ends belong together. It is
        /// the one field the person establishing the relation writes in their own words.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the identity that established the relation.
        /// </summary>
        public Guid? CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the creating identity navigation property.
        /// </summary>
        [JsonIgnore]
        public Identity CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the open key-value extension of the relation, serialized as JSON. It is
        /// the seam a plugin carries system specific facts on - a pull request number, a page
        /// version - without a schema change, and it is passed through untouched.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = [];

        /// <summary>
        /// Gets or sets the date and time when the relation was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the relation was last updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObjectRelation()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the relation.</param>
        public ObjectRelation(Guid id)
        {
            Id = id;
        }
    }
}
