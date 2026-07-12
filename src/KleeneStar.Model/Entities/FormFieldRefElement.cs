using System;
using System.Text.Json.Serialization;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a reference to a field defined on the form's class. Display attributes
    /// (name, help text, required flag) are derived from the referenced <see cref="Field"/>;
    /// no per-form overrides are stored.
    /// </summary>
    public class FormFieldRefElement : FormElement
    {
        /// <summary>
        /// Gets or sets the unique identifier of the referenced field.
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// Gets or sets the referenced field navigation reference.
        /// </summary>
        [JsonIgnore]
        public Field Field { get; set; }
    }
}
