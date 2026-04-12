using KleeneStar.Model.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a workspace, which serves as a container for related resources and metadata.
    /// </summary>
    public class Workspace : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the workspace.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the key of the workspace.
        /// </summary>
        [ValidateMinLength(2)]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name of the workspace.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of category names associated with the item.
        /// </summary>
        [RestConverter<CategoryConverter>()]
        public List<Category> Categories { get; set; } = [];

        /// <summary>
        /// Gets or sets the current state of the workspace.
        /// </summary>
        public WorkspaceState State { get; set; }

        /// <summary>
        /// Gets or sets the description of the workspace.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this workspace.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the workspace this workspace inherits from.
        /// </summary>
        public Guid? InheritedId { get; set; }

        /// <summary>
        /// Gets or sets the workspace this workspace inherits from.
        /// </summary>
        [JsonIgnore]
        public Workspace Inherited { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workspace is sealed
        /// and cannot be inherited or structurally modified.
        /// </summary>
        public bool Sealed { get; set; }

        /// <summary>
        /// Gets or sets the access modifier controlling the visibility and accessibility of this workspace.
        /// </summary>
        public WorkspaceAccessModifier AccessModifier { get; set; }

        /// <summary>
        /// Gets or sets the collection of tenants associated with this workspace.
        /// </summary>
        [JsonIgnore]
        public List<Tenant> Tenants { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of classes associated with this workspace.
        /// </summary>
        [JsonIgnore]
        public List<Class> Classes { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of objects associated with this workspace.
        /// </summary>
        [JsonIgnore]
        public List<Object> Objects { get; set; } = [];

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
        public Workspace()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the 
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the workspace.
        /// </param>
        public Workspace(Guid id)
        {
            Id = id;
        }
    }
}
