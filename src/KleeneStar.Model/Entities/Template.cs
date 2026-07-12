using KleeneStar.Model.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a template entity.
    /// A template provides predefined field values and metadata to accelerate object creation.
    /// </summary>
    public class Template : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the template.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the system name of the template.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the template.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the category of the template used for grouping in the UI.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this template.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the current state of the template.
        /// </summary>
        [RestConverter<TemplateStateConverter>]
        public TemplateState State { get; set; }

        /// <summary>
        /// Gets or sets the JSON serialized collection of predefined field value presets.
        /// </summary>
        public string Presets { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the class this template is bound to.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the class this template is bound to.
        /// </summary>
        public Class Class { get; set; }

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
        public Template()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the template.</param>
        public Template(Guid id)
        {
            Id = id;
        }
    }
}
