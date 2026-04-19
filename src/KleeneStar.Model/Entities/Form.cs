using KleeneStar.Model.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a form entity.
    /// </summary>
    public class Form : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the form.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the form.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the form.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the form, indicating whether it is a standard form or an additional form.
        /// </summary>
        public FormType FormType { get; set; } = FormType.Standard;

        /// <summary>
        /// Gets or sets the current state of the form.
        /// </summary>
        [RestConverter<FormStateConverter>]
        public FormState State { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this form.
        /// </summary>
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
        /// Gets or sets the unique identifier of the class associated with this form.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the class associated with the current form.
        /// </summary>
        public Class Class { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Form()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the 
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the form.
        /// </param>
        public Form(Guid id)
        {
            Id = id;
        }
    }
}
