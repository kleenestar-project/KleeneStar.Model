using KleeneStar.Model.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a workflow state entity.
    /// </summary>
    public class Status : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the workflow state.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the workflow state.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the workflow state.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated category.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the status category associated with the current workflow state.
        /// </summary>
        public StatusCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the current state of theworkflow state.
        /// </summary>
        [RestConverter<StatusStateConverter>]
        public StatusState State { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this workflow state.
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
        /// Gets or sets the unique identifier of the class associated with this workflow state.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the class associated with the current workflow state.
        /// </summary>
        public Class Class { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Status()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the 
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the workflow state.
        /// </param>
        public Status(Guid id)
        {
            Id = id;
        }
    }
}
