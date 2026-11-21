using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides the data access for the WQL query history.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// How many queries are kept per identity and subject.
        /// </summary>
        /// <remarks>
        /// The prompt is navigated one step at a time with PageUp/PageDown, so a history
        /// longer than this is not reachable in practice - it is only rows nobody deletes.
        /// </remarks>
        public const int WqlHistoryLimit = WqlHistory.Limit;

        /// <summary>
        /// Returns the queries an identity has run against a subject, most recent first.
        /// </summary>
        /// <param name="ownerId">The identity whose history is read.</param>
        /// <param name="subject">What was queried - the full name of the indexed type.</param>
        /// <param name="count">How many entries to return at most.</param>
        /// <returns>The queries, most recent first; empty when there are none.</returns>
        public static IReadOnlyList<string> GetWqlHistory(Guid ownerId, string subject, int count = WqlHistoryLimit)
        {
            if (ownerId == Guid.Empty || string.IsNullOrWhiteSpace(subject) || count <= 0)
            {
                return [];
            }

            using var db = CreateDbContext();

            return db.WqlHistories
                .AsNoTracking()
                .Where(x => x.OwnerId == ownerId && x.Subject == subject)
                .OrderByDescending(x => x.LastUsed)
                // two queries run in the same millisecond carry the same timestamp, and the
                // order of a history must not depend on which row the provider happens to
                // return first. The insert id is monotonic, so it settles the tie the way the
                // clock would have if it were finer
                .ThenByDescending(x => x.RawId)
                .Take(count)
                .Select(x => x.Query)
                .ToList();
        }

        /// <summary>
        /// Records that an identity ran a query against a subject.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A query that is already in the history is <em>moved</em> to the front rather than
        /// added a second time: a history of the same expression twenty times over is not a
        /// history, and re-running something must not push everything else out of the list.
        /// </para>
        /// <para>
        /// The trim keeps the newest <see cref="WqlHistoryLimit"/> entries. It runs on the set
        /// this method has already loaded to recognize the repeat, so it costs no extra query.
        /// </para>
        /// </remarks>
        /// <param name="ownerId">The identity that ran the query.</param>
        /// <param name="subject">What was queried - the full name of the indexed type.</param>
        /// <param name="query">The query as it was submitted.</param>
        /// <returns>The recorded entry, or <see langword="null"/> when nothing was recorded.</returns>
        public static WqlHistory RecordWqlQuery(Guid ownerId, string subject, string query)
        {
            var text = query?.Trim();

            if (ownerId == Guid.Empty || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            using var db = CreateDbContext();

            // an entry belongs to an identity, so an unknown owner has no history to write to.
            // Without this the cascade would have nothing to hang the row off and the insert
            // would fail on the foreign key instead of being quietly skipped
            if (!db.Identities.AsNoTracking().Any(x => x.Id == ownerId))
            {
                return null;
            }

            var now = DateTime.UtcNow;

            var existing = db.WqlHistories
                .Where(x => x.OwnerId == ownerId && x.Subject == subject)
                .OrderByDescending(x => x.LastUsed)
                .ThenByDescending(x => x.RawId)
                .ToList();

            var entry = existing.FirstOrDefault(x => string.Equals(x.Query, text, StringComparison.Ordinal));

            if (entry is not null)
            {
                entry.UseCount++;
                entry.LastUsed = now;
                entry.Updated = now;
            }
            else
            {
                entry = new WqlHistory
                {
                    OwnerId = ownerId,
                    Subject = subject,
                    Query = text,
                    UseCount = 1,
                    LastUsed = now,
                    Created = now,
                    Updated = now
                };

                db.WqlHistories.Add(entry);

                // the new entry is the newest, so it is the oldest of the existing ones that
                // go - not the oldest of the whole set, which would include the one just added
                var surplus = existing
                    .Skip(WqlHistoryLimit - 1)
                    .ToList();

                if (surplus.Count > 0)
                {
                    db.WqlHistories.RemoveRange(surplus);
                }
            }

            db.SaveChanges();

            return entry;
        }

        /// <summary>
        /// Removes the query history of an identity, either wholly or for one subject.
        /// </summary>
        /// <param name="ownerId">The identity whose history is cleared.</param>
        /// <param name="subject">
        /// The subject to clear, or <see langword="null"/> to clear every subject.
        /// </param>
        /// <returns>The number of entries removed.</returns>
        public static int ClearWqlHistory(Guid ownerId, string subject = null)
        {
            if (ownerId == Guid.Empty)
            {
                return 0;
            }

            using var db = CreateDbContext();

            var entries = db.WqlHistories
                .Where(x => x.OwnerId == ownerId && (subject == null || x.Subject == subject))
                .ToList();

            if (entries.Count == 0)
            {
                return 0;
            }

            db.WqlHistories.RemoveRange(entries);
            db.SaveChanges();

            return entries.Count;
        }
    }
}
