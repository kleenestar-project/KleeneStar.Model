using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a single measurable target on an <see cref="SlaPolicy"/>, e.g. "first response
    /// within 30 minutes" or "resolution within 5 business days".
    /// </summary>
    public class SlaTarget : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the target.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the target (e.g. "First response").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the milestone the target measures.
        /// </summary>
        public SlaTargetKind Kind { get; set; }

        /// <summary>
        /// Gets or sets the numeric target value, expressed in <see cref="Unit"/>.
        /// </summary>
        public int TargetValue { get; set; }

        /// <summary>
        /// Gets or sets the unit in which <see cref="TargetValue"/> is expressed.
        /// </summary>
        public SlaTargetUnit Unit { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the target was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the target was last updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the policy this target belongs to.
        /// </summary>
        public Guid PolicyId { get; set; }

        /// <summary>
        /// Gets or sets the policy this target belongs to.
        /// </summary>
        [JsonIgnore]
        public SlaPolicy Policy { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SlaTarget()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the target.</param>
        public SlaTarget(Guid id)
        {
            Id = id;
        }
    }
}
