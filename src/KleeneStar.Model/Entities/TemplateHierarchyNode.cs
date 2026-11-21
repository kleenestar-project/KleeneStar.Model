using System;
using System.Collections.Generic;
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
    /// its parent, the account it is assigned to and the relations it is linked to the objects of
    /// other places of the hierarchy with.
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
        /// Gets or sets the identity the object of the node is assigned to, or <c>null</c>.
        /// </summary>
        public Guid? AssigneeId { get; set; }

        /// <summary>
        /// Gets or sets the identity the object of the node is assigned to.
        /// </summary>
        [JsonIgnore]
        public Identity Assignee { get; set; }

        /// <summary>
        /// Gets or sets the links of the node: per entry a relation type and the places of the
        /// hierarchy (other nodes or the root) whose objects the object of the node is joined to.
        /// </summary>
        /// <remarks>
        /// A node is a child, linked or both. A link of a blocking type replaces the former
        /// "waits for": the node <em>is blocked by</em> the objects it has to wait for.
        /// </remarks>
        [IndexIgnore]
        public List<TemplateHierarchyNodeLink> Links { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public TemplateHierarchyNode()
        {
            Id = Guid.NewGuid();
        }
    }
}
