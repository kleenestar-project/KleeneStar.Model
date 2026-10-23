using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// How an object of a class is approved: the steps a request passes, who decides each, and
    /// where the object goes once the request is decided.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The steps are written as lines, one per step, in the order they are passed:
    /// <c>&lt;name&gt;: &lt;any | all | n of&gt; &lt;approvers&gt; [if &lt;WQL&gt;] [within &lt;hours&gt;h]</c> -
    /// approvers are accounts or <c>group:&lt;name&gt;</c>, the condition makes a step apply only
    /// to the objects it matches when the approval is requested.
    /// </para>
    /// <para>
    /// A running approval keeps the steps as they were resolved when it was requested
    /// (<see cref="Approval.Steps"/>): a policy changed later does not change who decides a
    /// request already under way, and the record of a decided one stays what it was.
    /// </para>
    /// </remarks>
    public class ApprovalPolicy : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the policy.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the workspace the policy belongs to.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the class whose objects the policy approves.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the name, unique per workspace.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the steps, one line per step.
        /// </summary>
        public string Steps { get; set; }

        /// <summary>
        /// Gets or sets whether the policy is used.
        /// </summary>
        public ApprovalPolicyState State { get; set; }

        /// <summary>
        /// Gets or sets whether the one who requested an approval may decide it as well. Off
        /// by default: a change is not approved by the person who wants it.
        /// </summary>
        public bool AllowSelfApproval { get; set; }

        /// <summary>
        /// Gets or sets the state whose reaching requests an approval, or <c>null</c> when it is
        /// requested by hand or by a rule only.
        /// </summary>
        public Guid? RequestStatusId { get; set; }

        /// <summary>
        /// Gets or sets the state an approved object is moved to, or <c>null</c>.
        /// </summary>
        public Guid? ApprovedStatusId { get; set; }

        /// <summary>
        /// Gets or sets the state a rejected object is moved to, or <c>null</c>.
        /// </summary>
        public Guid? RejectedStatusId { get; set; }

        /// <summary>
        /// Gets or sets who created the policy.
        /// </summary>
        public Guid? CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time the policy was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time the policy was updated.
        /// </summary>
        public DateTime Updated { get; set; }
    }

    /// <summary>
    /// Whether an <see cref="ApprovalPolicy"/> is used. Persisted as the ordinal.
    /// </summary>
    public enum ApprovalPolicyState
    {
        /// <summary>The policy is used.</summary>
        Active,

        /// <summary>The policy is kept but requests no new approvals.</summary>
        Disabled
    }
}
