using KleeneStar.Model.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /// Returns or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Returns or sets the unique identifier for the workspace.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Returns or sets the key of the workspace.
        /// </summary>
        [ValidateMinLength(2)]
        public string Key { get; set; }

        /// <summary>
        /// Returns the name of the workspace.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the collection of category names associated with the item.
        /// </summary>
        [RestConverter<CategoryConverter>()]
        public List<Category> Categories { get; set; } = [];

        /// <summary>
        /// Returns the current state of the workspace.
        /// </summary>
        public TypeWorkspaceState State { get; set; }

        /// <summary>
        /// Returns the description of the workspace.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns the icon associated with this workspace.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was updated.
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
