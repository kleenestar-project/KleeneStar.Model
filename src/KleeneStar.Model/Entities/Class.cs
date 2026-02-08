using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a class entity.
    /// </summary>
    public class Class : IEntity
    {
        /// <summary>
        /// Returns or sets the database id.
        /// </summary>
        [RestTableColumnHidden]
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Returns or sets the unique identifier for the class.
        /// </summary>
        [RestTableColumnHidden()]
        public Guid Id { get; set; }

        /// <summary>
        /// Returns or sets the name of the class.
        /// </summary>
        [RestTableColumnName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Returns the description of the workspace.
        /// </summary>
        [RestTableColumnName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Returns the icon associated with this workspace.
        /// </summary>
        [RestTableColumnHidden]
        [RestTableRowIcon]
        [RestDropdownIcon]
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was created.
        /// </summary>
        [RestTableColumnHidden()]
        public DateTime Created { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was updated.
        /// </summary>
        [RestTableColumnHidden()]
        public DateTime Updated { get; set; }

        /// <summary>
        /// Returns or sets the unique identifier of the workspace associated with this instance.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Returns or sets the workspace associated with the current context.
        /// </summary>
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Class()
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
        public Class(Guid id)
        {
            Id = id;
        }
    }
}
