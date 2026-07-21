using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a persisted tab inside the objects index of a workspace.
    /// Each <see cref="ObjectView"/> instance maps to one <c>FragmentControlRestTabTemplate</c>
    /// (selected via <see cref="ViewType"/>) and can hold per-instance configuration as JSON.
    /// </summary>
    public class ObjectView : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the view.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the view shown on its tab.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional description of the view.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the object kind whose overview tab control this view belongs to
        /// (e.g. <see cref="ObjectKind.Issue"/> or <see cref="ObjectKind.Asset"/>). A
        /// workspace keeps a separate tab set per kind, so the same layout (Table, List,
        /// …) can exist once for issues and once for assets without colliding.
        /// </summary>
        public string Kind { get; set; } = ObjectKind.Default;

        /// <summary>
        /// Gets or sets the kind of view that should be rendered for this tab.
        /// </summary>
        public ObjectViewType ViewType { get; set; }

        /// <summary>
        /// Gets or sets the optional configuration payload for the view, serialized as JSON.
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Gets or sets the display order of the view within the tab control.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the current state of the view.
        /// </summary>
        public ObjectViewState State { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the owning workspace.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the workspace the view belongs to.
        /// </summary>
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ObjectView()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the view.</param>
        public ObjectView(Guid id)
        {
            Id = id;
        }
    }
}
