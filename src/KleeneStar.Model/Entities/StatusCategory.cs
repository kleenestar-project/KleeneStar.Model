using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a workflow status category (To Do, In Progress, Done).
    /// </summary>
    public class StatusCategory : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the status category.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the status category (e.g. "In Progress").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the status category.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the visual icon associated with this category.
        /// </summary>
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the color associated with this category.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public StatusCategory()
        {
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the class with a specific identifier.
        /// </summary>
        public StatusCategory(Guid id)
        {
            Id = id;
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
        }
    }
}