using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a further class a template serves beside the class it is bound to.
    /// </summary>
    /// <remarks>
    /// <see cref="Template.ClassId"/> is the owning class: it decides the workspace, the
    /// permission chain and the class a child of a composite is created in. The rows of this
    /// entity name the other classes of the same workspace whose objects may be created from the
    /// template; the owning class is never one of them. A composite unique index on
    /// (TemplateId, ClassId) allows each class once per template.
    /// </remarks>
    public class TemplateClass : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the assignment.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the template.
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        [JsonIgnore]
        public Template Template { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the further class.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the further class.
        /// </summary>
        [JsonIgnore]
        public Class Class { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public TemplateClass()
        {
            Id = Guid.NewGuid();
        }
    }
}
