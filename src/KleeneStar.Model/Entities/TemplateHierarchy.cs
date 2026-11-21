using KleeneStar.Model.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a hierarchy of templates: a work structure that creates several objects at
    /// once, offered beside the templates when an object is created.
    /// </summary>
    /// <remarks>
    /// The hierarchy names a root template (<see cref="RootTemplateId"/>), whose object is the
    /// one the user creates, and a tree of nodes (<see cref="TemplateHierarchyNode"/>), each
    /// referencing an existing template whose object is created below or linked to the object
    /// of its parent node. Templates stay free of structure; one template may stand in several
    /// hierarchies.
    /// </remarks>
    public class TemplateHierarchy : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the hierarchy.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the hierarchy, unique within its workspace.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the hierarchy (an editor document).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the categories of the hierarchy, separated by semicolons.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the icon of the hierarchy.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the state; only an active hierarchy is offered.
        /// </summary>
        [RestConverter<TemplateStateConverter>]
        public TemplateState State { get; set; }

        /// <summary>
        /// Gets or sets the workspace the hierarchy belongs to.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the workspace the hierarchy belongs to.
        /// </summary>
        [JsonIgnore]
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets the template the root object of the hierarchy is created from.
        /// </summary>
        public Guid RootTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the template the root object is created from.
        /// </summary>
        [JsonIgnore]
        public Template RootTemplate { get; set; }

        /// <summary>
        /// Gets or sets the class the root object is created in, or <c>null</c> for the class the
        /// root template is bound to.
        /// </summary>
        public Guid? RootClassId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public TemplateHierarchy()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        public TemplateHierarchy(Guid id)
        {
            Id = id;
        }
    }
}
