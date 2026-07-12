using System;
using System.Collections.Generic;
using KleeneStar.Model.Entities;

namespace KleeneStar.Model.Forms
{
    /// <summary>
    /// Provider-agnostic snapshot of a form's complete structural tree, used to decouple
    /// the persistence layer from the WebExpress editor DTO.
    /// </summary>
    public sealed class FormStructureSnapshot
    {
        /// <summary>
        /// Gets or sets the display name of the form.
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// Gets or sets the description of the form.
        /// </summary>
        public string FormDescription { get; set; }

        /// <summary>
        /// Gets or sets the ordered list of tabs.
        /// </summary>
        public List<TabSnapshot> Tabs { get; set; } = [];
    }

    /// <summary>
    /// Snapshot of a form tab.
    /// </summary>
    public sealed class TabSnapshot
    {
        /// <summary>
        /// Gets or sets the display name of the tab.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ordered list of top-level child elements.
        /// </summary>
        public List<NodeSnapshot> Children { get; set; } = [];
    }

    /// <summary>
    /// Base class for snapshots of structural nodes (groups and field references).
    /// </summary>
    public abstract class NodeSnapshot
    {
    }

    /// <summary>
    /// Snapshot of a layout group.
    /// </summary>
    public sealed class GroupSnapshot : NodeSnapshot
    {
        /// <summary>
        /// Gets or sets the optional label of the group.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the layout strategy of the group.
        /// </summary>
        public FormGroupLayout Layout { get; set; } = FormGroupLayout.Vertical;

        /// <summary>
        /// Gets or sets the ordered list of child nodes.
        /// </summary>
        public List<NodeSnapshot> Children { get; set; } = [];
    }

    /// <summary>
    /// Snapshot of a reference to a field defined on the form's class.
    /// </summary>
    public sealed class FieldRefSnapshot : NodeSnapshot
    {
        /// <summary>
        /// Gets or sets the unique identifier of the referenced field.
        /// </summary>
        public Guid FieldId { get; set; }
    }
}
