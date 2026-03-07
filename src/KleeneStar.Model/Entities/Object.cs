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
    public class Object : IEntity
    {
        /// <summary>
        /// Returns or sets the database id.
        /// </summary>
        [RestHidden]
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Returns or sets the unique identifier for the class.
        /// </summary>
        [RestHidden()]
        public Guid Id { get; set; }

        /// <summary>
        /// Returns or sets the key of the workspace.
        /// </summary>
        [ValidateMinLength(2)]
        [RestTableColumnName("Key")]
        [RestTitle]
        public string Key { get; set; }

        /// <summary>
        /// Returns or sets the summary of the class.
        /// </summary>
        [RestTableColumnName("Summary")]
        [RestText]
        public string Summary { get; set; }

        /// <summary>
        /// Returns the description of the workspace.
        /// </summary>
        [RestTableColumnName("Description")]
        [RestDescription]
        public string Description { get; set; }

        /// <summary>
        /// Returns the current state of the workspace.
        /// </summary>
        [RestTableColumnName("State")]
        public TypeWorkspaceState State { get; set; }

        /// <summary>
        /// Returns the icon associated with this workspace.
        /// </summary>
        [RestHidden]
        [RestIcon]
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was created.
        /// </summary>
        [RestHidden()]
        public DateTime Created { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was updated.
        /// </summary>
        [RestHidden()]
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
        /// Returns or sets the unique identifier of the class associated with this instance.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Returns or sets the class associated with the current context.
        /// </summary>
        public Class Class { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Object()
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
        public Object(Guid id)
        {
            Id = id;
        }
    }
}
