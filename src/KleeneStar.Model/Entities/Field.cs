using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a field entity.
    /// </summary>
    public class Field : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the field.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the field.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the help text displayed to guide the user.
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Gets or sets the placeholder text shown when the field is empty.
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the current state of the field.
        /// </summary>
        public FieldState State { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this field.
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
        /// Gets or sets the unique identifier of the class associated with this field.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the class associated with the current field.
        /// </summary>
        public Class Class { get; set; }

        /// <summary>
        /// Gets or sets the type reference for the field (e.g., Text, Number, Date).
        /// </summary>
        public string FieldType { get; set; }

        /// <summary>
        /// Gets or sets the cardinality of the field.
        /// </summary>
        public FieldCardinality Cardinality { get; set; }

        /// <summary>
        /// Gets or sets the collection of validation rules associated with this field.
        /// </summary>
        public List<string> ValidationRules { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the optional default specification for this field.
        /// </summary>
        public string DefaultSpec { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field is required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field value must be unique.
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field is deprecated.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Gets or sets the access modifier controlling the visibility of this field.
        /// </summary>
        public AccessModifier AccessModifier { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Field()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the 
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the field.
        /// </param>
        public Field(Guid id)
        {
            Id = id;
        }
    }
}
