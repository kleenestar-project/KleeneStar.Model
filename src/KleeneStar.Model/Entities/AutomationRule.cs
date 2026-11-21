using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// A rule of the automation platform: when something happens to an object (the trigger) and
    /// the object matches (the condition), do something (the actions).
    /// </summary>
    /// <remarks>
    /// <para>
    /// A rule belongs to a workspace and applies to the objects of all its classes, or of one
    /// (<see cref="ClassId"/>). It is not tied to a kind: an issue, an asset, a document - any
    /// object can be assigned, moved, commented, escalated or decomposed by a rule.
    /// </para>
    /// <para>
    /// The trigger is a key of the open trigger catalog (<c>object.created</c>,
    /// <c>transition.executed</c>, <c>schedule</c>, …) with an optional parameter - the target
    /// state of a transition, the interval of a schedule. The condition is a WQL expression over
    /// the object. The actions are a small script, one action per line (<c>assign agent</c>,
    /// <c>transition In Progress</c>, <c>comment internal: Escalated.</c>), validated when the
    /// rule is saved and executed in order by a job of its own.
    /// </para>
    /// </remarks>
    public class AutomationRule : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the rule.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the rule.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the rule.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets whether the rule reacts.
        /// </summary>
        public AutomationRuleState State { get; set; }

        /// <summary>
        /// Gets or sets the workspace whose objects the rule applies to.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the class the rule is narrowed to, or <c>null</c> for every class of the
        /// workspace.
        /// </summary>
        public Guid? ClassId { get; set; }

        /// <summary>
        /// Gets or sets the key of the trigger in the trigger catalog.
        /// </summary>
        public string Trigger { get; set; }

        /// <summary>
        /// Gets or sets the parameter of the trigger, or <c>null</c>.
        /// </summary>
        public string TriggerParameter { get; set; }

        /// <summary>
        /// Gets or sets the WQL condition an object has to meet, or <c>null</c> for every object.
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// Gets or sets the action script, one action per line.
        /// </summary>
        public string Actions { get; set; }

        /// <summary>
        /// Gets or sets the identity the rule acts as - the author of its comments, the actor of
        /// its transitions - or <c>null</c> to act as its creator.
        /// </summary>
        public Guid? RunAsId { get; set; }

        /// <summary>
        /// Gets or sets the identity that created the rule.
        /// </summary>
        public Guid? CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the next time a scheduled rule is evaluated, or <c>null</c>.
        /// </summary>
        public DateTime? NextRun { get; set; }

        /// <summary>
        /// Gets or sets the date and time the rule was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time the rule was updated.
        /// </summary>
        public DateTime Updated { get; set; }
    }

    /// <summary>
    /// Whether an <see cref="AutomationRule"/> reacts. Persisted as the ordinal.
    /// </summary>
    public enum AutomationRuleState
    {
        /// <summary>The rule reacts to its trigger.</summary>
        Active,

        /// <summary>The rule is kept but reacts to nothing.</summary>
        Disabled
    }
}
