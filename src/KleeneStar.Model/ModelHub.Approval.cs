using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the approval policies of a workspace, or of every workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace, or <see langword="null"/> for all.</param>
        /// <returns>The policies, ordered by name.</returns>
        public static IReadOnlyList<ApprovalPolicy> GetApprovalPolicies(Guid? workspaceId)
        {
            using var db = CreateDbContext();

            var policies = db.ApprovalPolicies.AsNoTracking();

            if (workspaceId is Guid id)
            {
                policies = policies.Where(x => x.WorkspaceId == id);
            }

            return [.. policies.OrderBy(x => x.Name)];
        }

        /// <summary>
        /// Returns an approval policy.
        /// </summary>
        /// <param name="policyId">The policy.</param>
        /// <returns>The policy, or <see langword="null"/>.</returns>
        public static ApprovalPolicy GetApprovalPolicy(Guid policyId)
        {
            using var db = CreateDbContext();

            return db.ApprovalPolicies.AsNoTracking().FirstOrDefault(x => x.Id == policyId);
        }

        /// <summary>
        /// Adds an approval policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        public static void AddApprovalPolicy(ApprovalPolicy policy)
        {
            ArgumentNullException.ThrowIfNull(policy);

            using var db = CreateDbContext();

            db.ApprovalPolicies.Add(policy);
            db.SaveChanges();
        }

        /// <summary>
        /// Writes the changes of an approval policy.
        /// </summary>
        /// <param name="policy">The policy carrying the new values.</param>
        public static void UpdateApprovalPolicy(ApprovalPolicy policy)
        {
            ArgumentNullException.ThrowIfNull(policy);

            using var db = CreateDbContext();

            var stored = db.ApprovalPolicies.FirstOrDefault(x => x.Id == policy.Id);

            if (stored is null)
            {
                return;
            }

            stored.ClassId = policy.ClassId;
            stored.Name = policy.Name;
            stored.Description = policy.Description;
            stored.Steps = policy.Steps;
            stored.State = policy.State;
            stored.AllowSelfApproval = policy.AllowSelfApproval;
            stored.RequestStatusId = policy.RequestStatusId;
            stored.ApprovedStatusId = policy.ApprovedStatusId;
            stored.RejectedStatusId = policy.RejectedStatusId;
            stored.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Removes an approval policy; the approvals that followed it stay.
        /// </summary>
        /// <param name="policyId">The policy.</param>
        public static void RemoveApprovalPolicy(Guid policyId)
        {
            using var db = CreateDbContext();

            db.ApprovalPolicies.RemoveRange(db.ApprovalPolicies.Where(x => x.Id == policyId));
            db.SaveChanges();
        }

        /// <summary>
        /// Returns the approvals of an object with their decisions, the newest first.
        /// </summary>
        /// <param name="objectId">The object.</param>
        /// <returns>The approvals.</returns>
        public static IReadOnlyList<Approval> GetApprovals(Guid objectId)
        {
            using var db = CreateDbContext();

            return [.. db.Approvals
                .AsNoTracking()
                .Include(x => x.Decisions)
                .Where(x => x.ObjectId == objectId)
                .OrderByDescending(x => x.Requested)];
        }

        /// <summary>
        /// Returns the approvals in a state, with their decisions.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="limit">The most to return.</param>
        /// <returns>The approvals, the oldest first.</returns>
        public static IReadOnlyList<Approval> GetApprovals(ApprovalState state, int limit)
        {
            using var db = CreateDbContext();

            return [.. db.Approvals
                .AsNoTracking()
                .Include(x => x.Decisions)
                .Where(x => x.State == state)
                .OrderBy(x => x.Requested)
                .Take(limit)];
        }

        /// <summary>
        /// Returns an approval with its decisions.
        /// </summary>
        /// <param name="approvalId">The approval.</param>
        /// <returns>The approval, or <see langword="null"/>.</returns>
        public static Approval GetApproval(Guid approvalId)
        {
            using var db = CreateDbContext();

            return db.Approvals.AsNoTracking().Include(x => x.Decisions).FirstOrDefault(x => x.Id == approvalId);
        }

        /// <summary>
        /// Adds an approval.
        /// </summary>
        /// <param name="approval">The approval.</param>
        public static void AddApproval(Approval approval)
        {
            ArgumentNullException.ThrowIfNull(approval);

            using var db = CreateDbContext();

            db.Approvals.Add(approval);
            db.SaveChanges();
        }

        /// <summary>
        /// Writes the new state of an approval, and a decision with it, provided nobody changed
        /// the approval since it was read.
        /// </summary>
        /// <param name="approval">The approval carrying the new state and the version it was read at.</param>
        /// <param name="decision">The decision to record with it, or <see langword="null"/>.</param>
        /// <returns><see langword="true"/> when written; <see langword="false"/> when somebody was
        /// faster, or the approver had already decided the step.</returns>
        public static bool UpdateApproval(Approval approval, ApprovalDecision decision)
        {
            ArgumentNullException.ThrowIfNull(approval);

            using var db = CreateDbContext();

            var stored = db.Approvals.FirstOrDefault(x => x.Id == approval.Id);

            if (stored is null || stored.Version != approval.Version)
            {
                return false;
            }

            stored.State = approval.State;
            stored.CurrentStep = approval.CurrentStep;
            stored.StepStarted = approval.StepStarted;
            stored.Completed = approval.Completed;
            stored.Outcome = approval.Outcome;
            stored.Version = Guid.NewGuid();

            if (decision is not null)
            {
                db.ApprovalDecisions.Add(decision);
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            approval.Version = stored.Version;

            return true;
        }
    }
}
