using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a single escalation step on an <see cref="SlaPolicy"/>: when the configured
    /// time elapses the listed notifiees are alerted.
    /// </summary>
    public class SlaEscalationLevel : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the level.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the 1-based ordinal position of the level within the policy.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the time after which the level fires, expressed in <see cref="Unit"/>.
        /// </summary>
        public int AfterValue { get; set; }

        /// <summary>
        /// Gets or sets the unit in which <see cref="AfterValue"/> is expressed.
        /// </summary>
        public SlaTargetUnit Unit { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated list of notifiees (role or team names).
        /// </summary>
        public string Notify { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the policy this level belongs to.
        /// </summary>
        public Guid PolicyId { get; set; }

        /// <summary>
        /// Gets or sets the policy this level belongs to.
        /// </summary>
        [JsonIgnore]
        public SlaPolicy Policy { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SlaEscalationLevel()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the level.</param>
        public SlaEscalationLevel(Guid id)
        {
            Id = id;
        }
    }
}
