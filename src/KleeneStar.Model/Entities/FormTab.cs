using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a tab within a form, grouping form elements under a labeled section.
    /// </summary>
    public class FormTab : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the form tab.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the form that owns this tab.
        /// </summary>
        public Guid FormId { get; set; }

        /// <summary>
        /// Gets or sets the form that owns this tab.
        /// </summary>
        [JsonIgnore]
        public Form Form { get; set; }

        /// <summary>
        /// Gets or sets the display name of the tab.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the zero-based ordering position of the tab inside its form.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the top-level elements (groups and field references) of this tab.
        /// </summary>
        [IndexIgnore]
        public List<FormElement> Elements { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FormTab()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the form tab.
        /// </param>
        public FormTab(Guid id)
        {
            Id = id;
        }
    }
}
