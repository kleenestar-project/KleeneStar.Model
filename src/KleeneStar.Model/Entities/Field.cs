using KleeneStar.Model.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
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
        [RestConverter<FieldStateConverter>]
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
        [RestConverter<FieldTypeConverter>]
        public FieldType FieldType { get; set; }

        /// <summary>
        /// Gets or sets the cardinality of the field.
        /// </summary>
        [RestConverter<FieldCardinalityConverter>]
        public FieldCardinality Cardinality { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of values this field must contain (cardinality lower bound).
        /// </summary>
        public int CardinalityMin { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of values this field may contain (cardinality upper bound).
        /// Ignored when <see cref="CardinalityUnlimited"/> is <c>true</c>.
        /// </summary>
        public int CardinalityMax { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether the field accepts an unlimited number of values.
        /// When <c>true</c>, <see cref="CardinalityMax"/> is ignored.
        /// </summary>
        public bool CardinalityUnlimited { get; set; }

        /// <summary>
        /// Gets or sets the regular expression pattern used to validate text or string field values.
        /// Applies only to text-based field types.
        /// </summary>
        public string RegexPattern { get; set; }

        /// <summary>
        /// Gets or sets the list of selectable option values for enumerable field types (e.g., Selection).
        /// </summary>
        public List<string> Options { get; set; } = [];

        /// <summary>
        /// Gets or sets the unique identifier of the workflow assigned to this field.
        /// Applies only to fields of type <see cref="FieldType.Workflow"/>.
        /// </summary>
        public Guid? WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the default priority assigned to this field.
        /// Applies only to fields of type <see cref="FieldType.Reference"/> with priority semantics.
        /// </summary>
        public Guid? DefaultPriorityId { get; set; }

        /// <summary>
        /// Gets or sets the list of unique identifiers of the selected (allowed) priorities for this field.
        /// Applies only to priority-type fields.
        /// </summary>
        public List<Guid> SelectedPriorityIds { get; set; } = [];

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
        [RestConverter<AccessModifierConverter>]
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
