using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// A request for approval of an object, passing the steps of a policy.
    /// </summary>
    /// <remarks>
    /// The record is the audit trail of the approval: who asked, which steps applied, who
    /// decided each and why (<see cref="Decisions"/>), and how it ended. It outlives its policy
    /// - the name and the steps are copied - and goes only with its object.
    /// </remarks>
    public class Approval : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the approval.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the object to approve.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the policy the approval follows; it may be gone since.
        /// </summary>
        public Guid PolicyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the policy when the approval was requested.
        /// </summary>
        public string PolicyName { get; set; }

        /// <summary>
        /// Gets or sets the steps as they were resolved when the approval was requested
        /// (JSON): name, mode, quorum, approvers, due time, and whether the step applies.
        /// </summary>
        public string Steps { get; set; }

        /// <summary>
        /// Gets or sets whether the requester may decide as well, copied from the policy.
        /// </summary>
        public bool AllowSelfApproval { get; set; }

        /// <summary>
        /// Gets or sets the state of the approval.
        /// </summary>
        public ApprovalState State { get; set; }

        /// <summary>
        /// Gets or sets the index of the step awaiting decisions.
        /// </summary>
        public int CurrentStep { get; set; }

        /// <summary>
        /// Gets or sets who requested the approval; empty for a rule or the workflow.
        /// </summary>
        public Guid RequestedById { get; set; }

        /// <summary>
        /// Gets or sets what the requester said.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the date and time the approval was requested.
        /// </summary>
        public DateTime Requested { get; set; }

        /// <summary>
        /// Gets or sets the date and time the current step began.
        /// </summary>
        public DateTime StepStarted { get; set; }

        /// <summary>
        /// Gets or sets the date and time the approval ended, or <c>null</c> while it runs.
        /// </summary>
        public DateTime? Completed { get; set; }

        /// <summary>
        /// Gets or sets why the object could not be moved after the decision, or <c>null</c>.
        /// </summary>
        public string Outcome { get; set; }

        /// <summary>
        /// Gets or sets the concurrency token: two approvers deciding at once must not both
        /// complete the same step.
        /// </summary>
        public Guid Version { get; set; }

        /// <summary>
        /// Gets or sets the decisions taken.
        /// </summary>
        public List<ApprovalDecision> Decisions { get; set; } = [];
    }

    /// <summary>
    /// The state of an <see cref="Approval"/>. Persisted as the ordinal.
    /// </summary>
    public enum ApprovalState
    {
        /// <summary>The approval awaits decisions.</summary>
        Pending,

        /// <summary>Every applying step approved.</summary>
        Approved,

        /// <summary>A step rejected.</summary>
        Rejected,

        /// <summary>The requester or an administrator withdrew the request.</summary>
        Cancelled
    }

    /// <summary>
    /// A decision of an approver on a step of an <see cref="Approval"/>.
    /// </summary>
    public class ApprovalDecision : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the decision.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the approval.
        /// </summary>
        public Guid ApprovalId { get; set; }

        /// <summary>
        /// Gets or sets the index of the step decided.
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// Gets or sets who decided.
        /// </summary>
        public Guid IdentityId { get; set; }

        /// <summary>
        /// Gets or sets the verdict.
        /// </summary>
        public ApprovalVerdict Verdict { get; set; }

        /// <summary>
        /// Gets or sets the reason given.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the decision.
        /// </summary>
        public DateTime Decided { get; set; }
    }

    /// <summary>
    /// The verdict of an <see cref="ApprovalDecision"/>. Persisted as the ordinal.
    /// </summary>
    public enum ApprovalVerdict
    {
        /// <summary>The approver agrees.</summary>
        Approve,

        /// <summary>The approver refuses; the approval ends.</summary>
        Reject
    }
}
