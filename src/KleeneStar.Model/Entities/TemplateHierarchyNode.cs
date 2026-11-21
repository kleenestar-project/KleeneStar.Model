using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a node of a template hierarchy: a reference to an existing template whose
    /// object is created with the object of the parent node.
    /// </summary>
    /// <remarks>
    /// The nodes of a hierarchy form a tree: <see cref="ParentNodeId"/> names the parent node,
    /// <c>null</c> the root of the hierarchy (its root template). What belongs to the place in
    /// the structure rather than to the referenced template is stored on the node: the class the
    /// object is created in, the position among the siblings, whether the object is a child of
    /// its parent and the relation it is linked with, the account it is assigned to and the
    /// sibling it waits for.
    /// </remarks>
    public class TemplateHierarchyNode : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the node.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the hierarchy the node belongs to.
        /// </summary>
        public Guid HierarchyId { get; set; }

        /// <summary>
        /// Gets or sets the hierarchy the node belongs to.
        /// </summary>
        [JsonIgnore]
        public TemplateHierarchy Hierarchy { get; set; }

        /// <summary>
        /// Gets or sets the parent node, or <c>null</c> for a node below the root.
        /// </summary>
        public Guid? ParentNodeId { get; set; }

        /// <summary>
        /// Gets or sets the referenced template.
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the referenced template.
        /// </summary>
        [JsonIgnore]
        public Template Template { get; set; }

        /// <summary>
        /// Gets or sets the class the object of the node is created in, or <c>null</c> for the
        /// class the referenced template is bound to.
        /// </summary>
        /// <remarks>
        /// A template serving several classes needs this choice. A class the referenced template
        /// no longer serves counts as no choice, so the node falls back to the bound class.
        /// </remarks>
        public Guid? ClassId { get; set; }

        /// <summary>
        /// Gets or sets the position of the node in the hierarchy (depth first), which orders
        /// the siblings.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets whether the object of the node is created as a child of the object of its
        /// parent node: below it where the class allows it in the same workspace, and joined to
        /// it by the relation that aggregates progress.
        /// </summary>
        public bool AsChild { get; set; } = true;

        /// <summary>
        /// Gets or sets the key of the relation type (<see cref="ObjectRelationType.Key"/>) the
        /// object of the node is linked to the object of its parent node with, or <c>null</c>.
        /// </summary>
        /// <remarks>
        /// A node is a child, a link or both. The relation runs from the parent object to the
        /// created one, or the other way round where <see cref="RelationInverse"/> is set.
        /// </remarks>
        public string RelationTypeKey { get; set; }

        /// <summary>
        /// Gets or sets whether the link runs from the created object to the parent object.
        /// </summary>
        public bool RelationInverse { get; set; }

        /// <summary>
        /// Gets or sets the identity the object of the node is assigned to, or <c>null</c>.
        /// </summary>
        public Guid? AssigneeId { get; set; }

        /// <summary>
        /// Gets or sets the identity the object of the node is assigned to.
        /// </summary>
        [JsonIgnore]
        public Identity Assignee { get; set; }

        /// <summary>
        /// Gets or sets the id of the sibling node whose object has to be finished before the
        /// object of this node can be, or <c>null</c>.
        /// </summary>
        public Guid? PredecessorId { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public TemplateHierarchyNode()
        {
            Id = Guid.NewGuid();
        }
    }
}
