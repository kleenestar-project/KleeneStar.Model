using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the KleeneStar.
    /// </summary>
    internal static partial class ModelHub
    {
        // The navigation properties on SlaPolicy that should always be hydrated.
        private static readonly string[] _slaPolicyIncludes =
        [
            nameof(SlaPolicy.Class),
            nameof(SlaPolicy.Owner),
            nameof(SlaPolicy.Calendar),
            nameof(SlaPolicy.Targets),
            nameof(SlaPolicy.Scope),
            nameof(SlaPolicy.Escalations)
        ];

        /// <summary>
        /// Returns a queryable collection of SLA policies from the database, optionally filtered.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The materialized collection of policies.</returns>
        public static IEnumerable<SlaPolicy> GetSlaPolicies(IQuery<SlaPolicy> query)
        {
            using var db = CreateDbContext();

            return [.. GetSlaPolicies(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of SLA policies using the supplied DbContext.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The DbContext.</param>
        /// <returns>The filtered collection of policies.</returns>
        public static IEnumerable<SlaPolicy> GetSlaPolicies(IQuery<SlaPolicy> query, KleeneStarDbContext context)
        {
            var data = context.SlaPolicies
                .Include(x => x.Class)
                .Include(x => x.Owner)
                .Include(x => x.Calendar)
                .Include(x => x.Targets)
                .Include(x => x.Scope)
                .Include(x => x.Escalations)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Adds the specified SLA policy to the database when no policy with the same id exists.
        /// </summary>
        /// <param name="policyEntry">The policy to add. Cannot be null.</param>
        public static void Add(SlaPolicy policyEntry)
        {
            ArgumentNullException.ThrowIfNull(policyEntry);

            using var db = CreateDbContext();

            var query = new Query<SlaPolicy>()
                .WhereEquals(x => x.Id, policyEntry.Id);

            if (query.Apply(db.SlaPolicies).Any())
            {
                return;
            }

            // detach the child collections so we can attach/insert them explicitly without
            // relying on the generic AddEntity reflection path tripping over multiple
            // child collections at once.
            var targets = policyEntry.Targets?.ToList() ?? [];
            var scope = policyEntry.Scope?.ToList() ?? [];
            var escalations = policyEntry.Escalations?.ToList() ?? [];

            policyEntry.Targets = [];
            policyEntry.Scope = [];
            policyEntry.Escalations = [];

            if (policyEntry.Created == default)
            {
                policyEntry.Created = DateTime.UtcNow;
            }

            policyEntry.Updated = DateTime.UtcNow;

            db.SlaPolicies.Add(policyEntry);

            foreach (var t in targets)
            {
                t.PolicyId = policyEntry.Id;
                if (t.Created == default)
                {
                    t.Created = DateTime.UtcNow;
                }

                t.Updated = DateTime.UtcNow;
                db.SlaTargets.Add(t);
            }

            foreach (var s in scope)
            {
                s.PolicyId = policyEntry.Id;
                db.SlaScopeRules.Add(s);
            }

            foreach (var e in escalations)
            {
                e.PolicyId = policyEntry.Id;
                db.SlaEscalationLevels.Add(e);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Updates an existing SLA policy in the database, replacing its target/scope/escalation
        /// collections with the supplied ones.
        /// </summary>
        /// <param name="policyEntry">The policy to update.</param>
        public static void Update(SlaPolicy policyEntry)
        {
            ArgumentNullException.ThrowIfNull(policyEntry);

            using var db = CreateDbContext();

            var existing = db.SlaPolicies
                .Include(x => x.Targets)
                .Include(x => x.Scope)
                .Include(x => x.Escalations)
                .FirstOrDefault(x => x.Id == policyEntry.Id);

            if (existing is null)
            {
                return;
            }

            // overwrite scalar properties
            existing.Name = policyEntry.Name;
            existing.Description = policyEntry.Description;
            existing.State = policyEntry.State;
            existing.Priority = policyEntry.Priority;
            existing.CalendarId = policyEntry.CalendarId;
            existing.Notifications = policyEntry.Notifications;
            existing.PauseOn = policyEntry.PauseOn;
            existing.Icon = policyEntry.Icon;
            existing.OwnerId = policyEntry.OwnerId;
            existing.ClassId = policyEntry.ClassId;
            existing.Updated = DateTime.UtcNow;

            // replace child collections
            db.SlaTargets.RemoveRange(existing.Targets);
            db.SlaScopeRules.RemoveRange(existing.Scope);
            db.SlaEscalationLevels.RemoveRange(existing.Escalations);

            foreach (var t in policyEntry.Targets ?? [])
            {
                db.SlaTargets.Add(new SlaTarget
                {
                    Id = t.Id == Guid.Empty ? Guid.NewGuid() : t.Id,
                    Name = t.Name,
                    Kind = t.Kind,
                    TargetValue = t.TargetValue,
                    Unit = t.Unit,
                    PolicyId = existing.Id,
                    Created = t.Created == default ? DateTime.UtcNow : t.Created,
                    Updated = DateTime.UtcNow
                });
            }

            foreach (var s in policyEntry.Scope ?? [])
            {
                db.SlaScopeRules.Add(new SlaScopeRule
                {
                    Id = s.Id == Guid.Empty ? Guid.NewGuid() : s.Id,
                    RuleType = s.RuleType,
                    Value = s.Value,
                    PolicyId = existing.Id
                });
            }

            var level = 1;
            foreach (var e in policyEntry.Escalations ?? [])
            {
                db.SlaEscalationLevels.Add(new SlaEscalationLevel
                {
                    Id = e.Id == Guid.Empty ? Guid.NewGuid() : e.Id,
                    Level = level++,
                    AfterValue = e.AfterValue,
                    Unit = e.Unit,
                    Notify = e.Notify,
                    PolicyId = existing.Id
                });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified SLA policy and its child collections from the database.
        /// </summary>
        /// <param name="policyEntry">The policy to remove.</param>
        public static void Remove(SlaPolicy policyEntry)
        {
            ArgumentNullException.ThrowIfNull(policyEntry);

            using var db = CreateDbContext();

            var existing = db.SlaPolicies
                .Include(x => x.Targets)
                .Include(x => x.Scope)
                .Include(x => x.Escalations)
                .FirstOrDefault(x => x.Id == policyEntry.Id);

            if (existing is null)
            {
                return;
            }

            db.SlaTargets.RemoveRange(existing.Targets);
            db.SlaScopeRules.RemoveRange(existing.Scope);
            db.SlaEscalationLevels.RemoveRange(existing.Escalations);
            db.SlaPolicies.Remove(existing);

            db.SaveChanges();
        }
    }
}
