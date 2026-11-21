using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// A link of a template hierarchy node: the relation the object of the node is joined to the
    /// objects of other places of the hierarchy with. Stored as part of the node
    /// (<see cref="TemplateHierarchyNode.Links"/>), not as an entity of its own.
    /// </summary>
    /// <remarks>
    /// The link reads from the node: without <see cref="Inverse"/> the relation runs from the
    /// object of the node to the objects of the targets (the node <em>blocks</em> them), with it
    /// the other way round (the node <em>is blocked by</em> them). A target is the id of a node of
    /// the same hierarchy, or <see cref="RootTarget"/> for the object created from the root
    /// template.
    /// </remarks>
    public sealed class TemplateHierarchyNodeLink
    {
        /// <summary>
        /// The target that names the object created from the root template.
        /// </summary>
        public const string RootTarget = "root";

        /// <summary>
        /// Gets or sets the key of the relation type (<see cref="ObjectRelationType.Key"/>).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets whether the relation runs from the targets to the object of the node.
        /// </summary>
        [JsonPropertyName("inverse")]
        public bool Inverse { get; set; }

        /// <summary>
        /// Gets or sets the targets: node ids of the same hierarchy, or <see cref="RootTarget"/>.
        /// </summary>
        [JsonPropertyName("targets")]
        public List<string> Targets { get; set; } = [];
    }
}
