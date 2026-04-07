using System;
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
        /// Returns the description of the field.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns the current state of the field.
        /// </summary>
        public TypeWorkspaceState State { get; set; }

        /// <summary>
        /// Returns the icon associated with this field.
        /// </summary>
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was updated.
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
