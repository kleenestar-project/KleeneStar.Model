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
        /// Returns the automation rules of a workspace, or of every workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace, or <c>null</c> for all.</param>
        /// <returns>The rules, ordered by name.</returns>
        public static IReadOnlyList<AutomationRule> GetAutomationRules(Guid? workspaceId = null)
        {
            using var db = CreateDbContext();

            return [.. db.AutomationRules
                .AsNoTracking()
                .Where(x => workspaceId == null || x.WorkspaceId == workspaceId)
                .OrderBy(x => x.Name)];
        }

        /// <summary>
        /// Returns an automation rule.
        /// </summary>
        /// <param name="ruleId">The rule.</param>
        /// <returns>The rule, or <see langword="null"/>.</returns>
        public static AutomationRule GetAutomationRule(Guid ruleId)
        {
            using var db = CreateDbContext();

            return db.AutomationRules.AsNoTracking().FirstOrDefault(x => x.Id == ruleId);
        }

        /// <summary>
        /// Adds an automation rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        public static void AddAutomationRule(AutomationRule rule)
        {
            ArgumentNullException.ThrowIfNull(rule);

            using var db = CreateDbContext();

            db.AutomationRules.Add(rule);
            db.SaveChanges();
        }

        /// <summary>
        /// Writes the changes of an automation rule.
        /// </summary>
        /// <param name="rule">The rule carrying the new values.</param>
        public static void UpdateAutomationRule(AutomationRule rule)
        {
            ArgumentNullException.ThrowIfNull(rule);

            using var db = CreateDbContext();

            db.UpdateEntity(rule);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes an automation rule and the jobs it left.
        /// </summary>
        /// <param name="ruleId">The rule.</param>
        public static void RemoveAutomationRule(Guid ruleId)
        {
            using var db = CreateDbContext();

            db.AutomationJobs.RemoveRange(db.AutomationJobs.Where(x => x.RuleId == ruleId));
            db.AutomationRules.RemoveRange(db.AutomationRules.Where(x => x.Id == ruleId));
            db.SaveChanges();
        }

        /// <summary>
        /// Queues a job unless the event it answers was queued before.
        /// </summary>
        /// <param name="job">The job to queue.</param>
        /// <returns><see langword="true"/> when the job was queued; <see langword="false"/> when
        /// a job with its deduplication key exists.</returns>
        public static bool TryEnqueueAutomationJob(AutomationJob job)
        {
            ArgumentNullException.ThrowIfNull(job);

            using var db = CreateDbContext();

            if (db.AutomationJobs.Any(x => x.Deduplication == job.Deduplication))
            {
                return false;
            }

            db.AutomationJobs.Add(job);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // the same event was queued in parallel between the check and the write; the
                // unique index decided, and the other one stands
                return false;
            }

            return true;
        }

        /// <summary>
        /// Claims the jobs due for a worker, each for the length of a lease.
        /// </summary>
        /// <remarks>
        /// A job is due when it is pending and its next attempt has come, or when it is running
        /// under a lease that ran out - its worker died. Each claim renews the job's concurrency
        /// stamp; a claim that loses the race to another worker fails with a concurrency
        /// conflict and the job is simply not this worker's.
        /// </remarks>
        /// <param name="worker">The worker claiming.</param>
        /// <param name="now">The current time.</param>
        /// <param name="lease">How long a claim holds.</param>
        /// <param name="limit">How many jobs to claim at most.</param>
        /// <returns>The claimed jobs.</returns>
        public static IReadOnlyList<AutomationJob> ClaimAutomationJobs(string worker, DateTime now, TimeSpan lease, int limit)
        {
            var candidates = new List<Guid>();

            using (var db = CreateDbContext())
            {
                candidates.AddRange(db.AutomationJobs
                    .AsNoTracking()
                    .Where(x => (x.State == AutomationJobState.Pending && x.NextAttempt <= now)
                        || (x.State == AutomationJobState.Running && x.LockedUntil < now))
                    .OrderBy(x => x.NextAttempt)
                    .ThenBy(x => x.RawId)
                    .Select(x => x.Id)
                    .Take(limit));
            }

            var claimed = new List<AutomationJob>();

            foreach (var id in candidates)
            {
                using var db = CreateDbContext();

                var job = db.AutomationJobs.FirstOrDefault(x => x.Id == id);

                if (job is null
                    || !((job.State == AutomationJobState.Pending && job.NextAttempt <= now)
                        || (job.State == AutomationJobState.Running && job.LockedUntil < now)))
                {
                    continue;
                }

                job.State = AutomationJobState.Running;
                job.LockedBy = worker;
                job.LockedUntil = now + lease;
                job.Attempts++;
                job.Version = Guid.NewGuid();
                job.Updated = now;

                try
                {
                    db.SaveChanges();
                    claimed.Add(job);
                }
                catch (DbUpdateConcurrencyException)
                {
                    // another worker claimed it first
                }
            }

            return claimed;
        }

        /// <summary>
        /// Writes the progress or outcome of a job the caller holds.
        /// </summary>
        /// <param name="job">The job carrying the new values.</param>
        public static void UpdateAutomationJob(AutomationJob job)
        {
            ArgumentNullException.ThrowIfNull(job);

            using var db = CreateDbContext();

            var stored = db.AutomationJobs.FirstOrDefault(x => x.Id == job.Id);

            if (stored is null)
            {
                return;
            }

            stored.State = job.State;
            stored.CompletedActions = job.CompletedActions;
            stored.NextAttempt = job.NextAttempt;
            stored.LockedBy = job.LockedBy;
            stored.LockedUntil = job.LockedUntil;
            stored.LastError = job.LastError;
            stored.Attempts = job.Attempts;
            stored.Version = Guid.NewGuid();
            stored.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Returns automation jobs, newest first.
        /// </summary>
        /// <param name="ruleIds">The rules whose jobs are returned, or <c>null</c> for all.</param>
        /// <param name="state">The state the jobs are in, or <c>null</c> for any.</param>
        /// <param name="limit">How many jobs to return at most.</param>
        /// <returns>The jobs.</returns>
        public static IReadOnlyList<AutomationJob> GetAutomationJobs(IReadOnlyCollection<Guid> ruleIds, AutomationJobState? state, int limit)
        {
            using var db = CreateDbContext();

            var query = db.AutomationJobs.AsNoTracking();

            if (ruleIds is not null)
            {
                query = query.Where(x => ruleIds.Contains(x.RuleId));
            }

            if (state is AutomationJobState wanted)
            {
                query = query.Where(x => x.State == wanted);
            }

            return [.. query.OrderByDescending(x => x.RawId).Take(limit)];
        }

        /// <summary>
        /// Returns an automation job.
        /// </summary>
        /// <param name="jobId">The job.</param>
        /// <returns>The job, or <see langword="null"/>.</returns>
        public static AutomationJob GetAutomationJob(Guid jobId)
        {
            using var db = CreateDbContext();

            return db.AutomationJobs.AsNoTracking().FirstOrDefault(x => x.Id == jobId);
        }

        /// <summary>
        /// Removes the finished jobs older than a point in time.
        /// </summary>
        /// <param name="before">The point in time.</param>
        /// <returns>The number of jobs removed.</returns>
        public static int PruneAutomationJobs(DateTime before)
        {
            using var db = CreateDbContext();

            var stale = db.AutomationJobs
                .Where(x => (x.State == AutomationJobState.Succeeded || x.State == AutomationJobState.Skipped) && x.Updated < before)
                .ToList();

            db.AutomationJobs.RemoveRange(stale);
            db.SaveChanges();

            return stale.Count;
        }
    }
}
