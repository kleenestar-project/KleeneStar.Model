using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a object entity.
    /// </summary>
    public class Object : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the object.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the key of the object.
        /// </summary>
        [ValidateMinLength(2)]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the summary of the object.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Returns the description of the object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns the current state of the object.
        /// </summary>
        public WorkspaceState State { get; set; }

        /// <summary>
        /// Returns the icon associated with this object.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the workspace associated with this object.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the workspace associated with the current object.
        /// </summary>
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the class associated with this instance.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the class associated with the current context.
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
        /// The unique identifier to assign to the object.
        /// </param>
        public Object(Guid id)
        {
            Id = id;
        }
    }
}
