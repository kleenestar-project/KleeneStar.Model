using KleeneStar.Model.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a service-level-agreement policy that binds response, resolution and escalation
    /// expectations to a subset of tickets belonging to a single <see cref="Class"/>.
    /// </summary>
    /// <remarks>
    /// A policy aggregates a <see cref="SlaCalendar"/>, a list of <see cref="SlaScopeRule"/>s
    /// (combined with logical AND when matching tickets), a list of <see cref="SlaTarget"/>s
    /// (response/resolution/etc.), and a list of <see cref="SlaEscalationLevel"/>s.
    /// </remarks>
    public class SlaPolicy : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the policy.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the policy.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the free-text description of the policy.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current lifecycle state of the policy.
        /// </summary>
        [RestConverter<SlaPolicyStateConverter>]
        public SlaPolicyState State { get; set; }

        /// <summary>
        /// Gets or sets the severity bucket the policy applies to.
        /// </summary>
        public SlaPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the calendar that controls when the policy clock runs.
        /// </summary>
        public SlaCalendar Calendar { get; set; }

        /// <summary>
        /// Gets or sets the channels through which breach notifications are dispatched.
        /// </summary>
        public SlaNotificationChannels Notifications { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated list of status names that pause the SLA clock
        /// (e.g. "Waiting for customer, Scheduled maintenance").
        /// </summary>
        public string PauseOn { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with the policy.
        /// </summary>
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the class the policy is bound to.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the class the policy is bound to.
        /// </summary>
        [JsonIgnore]
        public Class Class { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity that owns the policy.
        /// </summary>
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the identity that owns the policy.
        /// </summary>
        [JsonIgnore]
        public Identity Owner { get; set; }

        /// <summary>
        /// Gets or sets the targets (response, resolution, ...) defined for the policy.
        /// </summary>
        [JsonIgnore]
        public List<SlaTarget> Targets { get; set; } = [];

        /// <summary>
        /// Gets or sets the scope rules that decide which tickets the policy applies to.
        /// </summary>
        [JsonIgnore]
        public List<SlaScopeRule> Scope { get; set; } = [];

        /// <summary>
        /// Gets or sets the escalation levels of the policy.
        /// </summary>
        [JsonIgnore]
        public List<SlaEscalationLevel> Escalations { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SlaPolicy()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the policy.</param>
        public SlaPolicy(Guid id)
        {
            Id = id;
        }
    }
}
