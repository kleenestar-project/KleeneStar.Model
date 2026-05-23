using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a single filter rule that contributes to the scope of an <see cref="SlaPolicy"/>.
    /// Multiple rules are combined with logical AND when matching tickets.
    /// </summary>
    public class SlaScopeRule : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the rule.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the attribute the rule matches against.
        /// </summary>
        public SlaScopeRuleType RuleType { get; set; }

        /// <summary>
        /// Gets or sets the value the rule matches (e.g. "High", "Enterprise", "VIP-User").
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the policy this rule belongs to.
        /// </summary>
        public Guid PolicyId { get; set; }

        /// <summary>
        /// Gets or sets the policy this rule belongs to.
        /// </summary>
        [JsonIgnore]
        public SlaPolicy Policy { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SlaScopeRule()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the rule.</param>
        public SlaScopeRule(Guid id)
        {
            Id = id;
        }
    }
}
