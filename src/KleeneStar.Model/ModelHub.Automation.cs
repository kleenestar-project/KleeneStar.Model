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
        /// How often a conditional write of a job is tried again after its concurrency stamp
        /// moved between the read and the write - the worker's own lease renewal does that.
        /// </summary>
        private const int JobWriteAttempts = 3;

        /// <summary>
        /// Returns the ids of the jobs due, oldest first, without claiming any.
        /// </summary>
        /// <remarks>
        /// A job is due when it is pending and its next attempt has come, or when it is running
        /// under a lease that ran out - its worker died or stalled.
        /// </remarks>
        /// <param name="now">The current time.</param>
        /// <param name="limit">How many ids to return at most.</param>
        /// <returns>The ids.</returns>
        public static IReadOnlyList<Guid> GetDueAutomationJobIds(DateTime now, int limit)
        {
            using var db = CreateDbContext();

            return [.. db.AutomationJobs
                .AsNoTracking()
                .Where(x => (x.State == AutomationJobState.Pending && x.NextAttempt <= now)
                    || (x.State == AutomationJobState.Running && x.LockedUntil < now))
                .OrderBy(x => x.NextAttempt)
                .ThenBy(x => x.RawId)
                .Select(x => x.Id)
                .Take(limit)];
        }

        /// <summary>
        /// Claims one job for a worker for the length of a lease, if it is (still) due.
        /// </summary>
        /// <remarks>
        /// The claim renews the job's concurrency stamp; a claim that loses the race to another
        /// worker fails with a concurrency conflict and the job is simply not this worker's. A
        /// worker claims a job right before it runs it (<see cref="GetDueAutomationJobIds"/>
        /// only lists candidates), so a lease starts when the work does - claiming a batch up
        /// front let the leases of the last jobs of a slow batch run out before they started.
        /// </remarks>
        /// <param name="jobId">The job.</param>
        /// <param name="worker">The worker claiming.</param>
        /// <param name="now">The current time.</param>
        /// <param name="lease">How long the claim holds.</param>
        /// <returns>The claimed job, or <see langword="null"/>.</returns>
        public static AutomationJob ClaimAutomationJob(Guid jobId, string worker, DateTime now, TimeSpan lease)
        {
            using var db = CreateDbContext();

            var job = db.AutomationJobs.FirstOrDefault(x => x.Id == jobId);

            if (job is null
                || !((job.State == AutomationJobState.Pending && job.NextAttempt <= now)
                    || (job.State == AutomationJobState.Running && job.LockedUntil < now)))
            {
                return null;
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

                return job;
            }
            catch (DbUpdateConcurrencyException)
            {
                // another worker claimed it first
                return null;
            }
        }

        /// <summary>
        /// Claims the jobs due for a worker, each for the length of a lease.
        /// </summary>
        /// <remarks>
        /// For callers that hold the jobs only briefly (tests, inspection). A worker that runs
        /// the jobs claims them one by one with <see cref="ClaimAutomationJob"/> instead.
        /// </remarks>
        /// <param name="worker">The worker claiming.</param>
        /// <param name="now">The current time.</param>
        /// <param name="lease">How long a claim holds.</param>
        /// <param name="limit">How many jobs to claim at most.</param>
        /// <returns>The claimed jobs.</returns>
        public static IReadOnlyList<AutomationJob> ClaimAutomationJobs(string worker, DateTime now, TimeSpan lease, int limit)
        {
            return [.. GetDueAutomationJobIds(now, limit)
                .Select(id => ClaimAutomationJob(id, worker, now, lease))
                .Where(x => x is not null)];
        }

        /// <summary>
        /// Extends the lease of a job the worker still holds.
        /// </summary>
        /// <param name="jobId">The job.</param>
        /// <param name="worker">The worker holding it.</param>
        /// <param name="now">The current time.</param>
        /// <param name="lease">How long the lease holds from now.</param>
        /// <returns>The new end of the lease, or <see langword="null"/> when the worker no longer
        /// holds the job - another worker took it over after the lease ran out.</returns>
        public static DateTime? RenewAutomationJob(Guid jobId, string worker, DateTime now, TimeSpan lease)
        {
            for (var attempt = 0; attempt < JobWriteAttempts; attempt++)
            {
                using var db = CreateDbContext();

                var stored = db.AutomationJobs.FirstOrDefault(x => x.Id == jobId);

                if (!IsHeldBy(stored, worker))
                {
                    return null;
                }

                stored.LockedUntil = now + lease;
                stored.Version = Guid.NewGuid();
                stored.Updated = now;

                try
                {
                    db.SaveChanges();

                    return stored.LockedUntil;
                }
                catch (DbUpdateConcurrencyException)
                {
                    // the stamp moved between read and write: read again and decide anew
                }
            }

            return null;
        }

        /// <summary>
        /// Writes the state of a job without asking who holds it - for an administrator's retry
        /// and for tests. A worker writes through the overload that names it.
        /// </summary>
        /// <param name="job">The job carrying the new values.</param>
        /// <returns><see langword="true"/> when the job was written.</returns>
        public static bool UpdateAutomationJob(AutomationJob job)
        {
            return UpdateAutomationJob(job, null);
        }

        /// <summary>
        /// Writes the progress or outcome of a job - when <paramref name="worker"/> is named, only
        /// while that worker still holds it.
        /// </summary>
        /// <remarks>
        /// A worker whose lease ran out may find its job taken over by another; its writes then
        /// fail instead of overwriting the other worker's progress, and it stops. While the job
        /// stays running, the lease columns are left as stored: the worker's renewal moves them
        /// on its own, and the copy the worker carries may be older.
        /// </remarks>
        /// <param name="job">The job carrying the new values.</param>
        /// <param name="worker">The worker that must hold the job, or <see langword="null"/> for no check.</param>
        /// <returns><see langword="true"/> when the job was written.</returns>
        public static bool UpdateAutomationJob(AutomationJob job, string worker)
        {
            ArgumentNullException.ThrowIfNull(job);

            for (var attempt = 0; attempt < JobWriteAttempts; attempt++)
            {
                using var db = CreateDbContext();

                var stored = db.AutomationJobs.FirstOrDefault(x => x.Id == job.Id);

                if (stored is null || (worker is not null && !IsHeldBy(stored, worker)))
                {
                    return false;
                }

                stored.State = job.State;
                stored.CompletedActions = job.CompletedActions;
                stored.Actions = job.Actions ?? stored.Actions;
                stored.NextAttempt = job.NextAttempt;
                stored.LastError = job.LastError;
                stored.Attempts = job.Attempts;

                if (worker is null || job.State != AutomationJobState.Running)
                {
                    stored.LockedBy = job.LockedBy;
                    stored.LockedUntil = job.LockedUntil;
                }

                stored.Version = Guid.NewGuid();
                stored.Updated = DateTime.UtcNow;

                try
                {
                    db.SaveChanges();

                    job.Version = stored.Version;

                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    // the stamp moved between read and write: read again and decide anew
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether a worker holds a job: it is running under that worker's name.
        /// </summary>
        /// <param name="job">The stored job. May be null.</param>
        /// <param name="worker">The worker.</param>
        /// <returns><see langword="true"/> when the worker holds the job.</returns>
        private static bool IsHeldBy(AutomationJob job, string worker)
        {
            return job is not null
                && job.State == AutomationJobState.Running
                && string.Equals(job.LockedBy, worker, StringComparison.Ordinal);
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
