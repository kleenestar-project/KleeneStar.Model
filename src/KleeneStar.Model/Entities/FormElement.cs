using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents an element within a form's structural tree. Concrete subtypes are
    /// <see cref="FormGroupElement"/> and <see cref="FormFieldRefElement"/>.
    /// </summary>
    /// <remarks>
    /// Persisted via single-table-per-hierarchy (TPH) inheritance with a discriminator column.
    /// </remarks>
    public abstract class FormElement : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the element.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the form tab this element belongs to.
        /// </summary>
        /// <remarks>
        /// Always set, regardless of nesting depth, to allow loading the entire structure
        /// of a tab in a single query.
        /// </remarks>
        public Guid FormTabId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the parent group element, if any.
        /// <c>null</c> indicates a top-level element directly under the tab.
        /// </summary>
        public Guid? ParentElementId { get; set; }

        /// <summary>
        /// Gets or sets the parent element navigation reference.
        /// </summary>
        [JsonIgnore]
        public FormElement Parent { get; set; }

        /// <summary>
        /// Gets or sets the children of this element. Populated only for groups; always
        /// empty for field-reference elements.
        /// </summary>
        [IndexIgnore]
        public List<FormElement> Children { get; set; } = [];

        /// <summary>
        /// Gets or sets the zero-based ordering position of this element within its parent
        /// (or within the tab when <see cref="ParentElementId"/> is <c>null</c>).
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected FormElement()
        {
            Id = Guid.NewGuid();
        }
    }
}
