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
        /// <summary>
        /// Returns a materialized collection of commits from the database.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching commits, each with its changes in recorded order.</returns>
        public static IEnumerable<Commit> GetCommits(IQuery<Commit> query)
        {
            using var db = CreateDbContext();

            return [.. GetCommits(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of commits using the supplied DbContext.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The DbContext.</param>
        /// <returns>The filtered collection.</returns>
        public static IEnumerable<Commit> GetCommits(IQuery<Commit> query, KleeneStarDbContext context)
        {
            var data = context.Commits
                .Include(x => x.Changes)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Returns the whole commit chain of an object, oldest first, with the changes of each
        /// commit in the order they were recorded.
        /// </summary>
        /// <param name="objectId">The id of the object whose chain is read.</param>
        /// <returns>The chain, oldest first. Empty when the object has no history.</returns>
        public static IReadOnlyList<Commit> GetCommitChain(Guid objectId)
        {
            using var db = CreateDbContext();

            return [.. ReadChain(db, objectId)];
        }

        /// <summary>
        /// Returns a single commit of an object by its revision number, with its changes.
        /// </summary>
        /// <param name="objectId">The id of the owning object.</param>
        /// <param name="number">The 1-based revision number.</param>
        /// <returns>The commit, or <c>null</c> when the chain has no such revision.</returns>
        public static Commit GetCommit(Guid objectId, int number)
        {
            using var db = CreateDbContext();

            var commit = db.Commits
                .Include(x => x.Changes)
                .AsNoTracking()
                .FirstOrDefault(x => x.ObjectId == objectId && x.Number == number);

            OrderChanges(commit);

            return commit;
        }

        /// <summary>
        /// Returns a single commit by its unique identifier, with its changes.
        /// </summary>
        /// <param name="commitId">The commit id.</param>
        /// <returns>The commit, or <c>null</c> when no commit matches.</returns>
        public static Commit GetCommit(Guid commitId)
        {
            using var db = CreateDbContext();

            var commit = db.Commits
                .Include(x => x.Changes)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == commitId);

            OrderChanges(commit);

            return commit;
        }

        /// <summary>
        /// Returns the head of an object's chain - the newest commit - or <c>null</c> when the
        /// object has no history yet.
        /// </summary>
        /// <param name="objectId">The id of the owning object.</param>
        /// <returns>The head commit, or <c>null</c>.</returns>
        public static Commit GetHeadCommit(Guid objectId)
        {
            using var db = CreateDbContext();

            var commit = db.Commits
                .Include(x => x.Changes)
                .AsNoTracking()
                .Where(x => x.ObjectId == objectId)
                .OrderByDescending(x => x.Number)
                .FirstOrDefault();

            OrderChanges(commit);

            return commit;
        }

        /// <summary>
        /// Returns the number of commits recorded for an object.
        /// </summary>
        /// <param name="objectId">The id of the owning object.</param>
        /// <returns>The length of the chain.</returns>
        public static int GetCommitCount(Guid objectId)
        {
            using var db = CreateDbContext();

            return db.Commits.Count(x => x.ObjectId == objectId);
        }

        /// <summary>
        /// Appends a commit to the chain of its object and applies the field values the commit
        /// describes, both inside a single transaction.
        /// </summary>
        /// <remarks>
        /// This is the only write path of the versioning store. The revision number and the
        /// predecessor are resolved from the current head <b>inside</b> the transaction rather
        /// than by the caller, so two concurrent writers cannot produce two commits carrying the
        /// same number - the unique index on (Object, Number) is the backstop that turns such a
        /// race into a failed transaction instead of a corrupted chain.
        /// <para>
        /// The <see cref="Value"/> rows travel with the commit rather than being written ahead of
        /// it: an object's current state is defined as the head of its chain, so if the commit
        /// cannot be written the values must not change either.
        /// </para>
        /// </remarks>
        /// <param name="commit">
        /// The commit to append. Its <see cref="Commit.Number"/> and <see cref="Commit.ParentId"/>
        /// are assigned here and overwrite whatever the caller set.
        /// </param>
        /// <param name="upserts">
        /// The value rows to insert or overwrite, identified by their (object, field) pair. May
        /// be <c>null</c>.
        /// </param>
        /// <param name="removals">
        /// The ids of the value rows to delete. May be <c>null</c>.
        /// </param>
        /// <returns>The appended commit, carrying its assigned number and predecessor.</returns>
        public static Commit AddCommit(Commit commit, IEnumerable<Value> upserts, IEnumerable<Guid> removals)
        {
            ArgumentNullException.ThrowIfNull(commit);

            using var db = CreateDbContext();
            using var transaction = db.Database.BeginTransaction();

            var head = db.Commits
                .AsNoTracking()
                .Where(x => x.ObjectId == commit.ObjectId)
                .OrderByDescending(x => x.Number)
                .FirstOrDefault();

            commit.ParentId = head?.Id;
            commit.Number = (head?.Number ?? 0) + 1;

            if (commit.Created == default)
            {
                commit.Created = DateTime.UtcNow;
            }

            commit.Updated = commit.Created;

            var ordinal = 0;

            foreach (var change in commit.Changes ?? [])
            {
                change.CommitId = commit.Id;
                change.Ordinal = ordinal++;
            }

            db.Commits.Add(commit);

            ApplyValues(db, upserts, removals);

            db.SaveChanges();
            transaction.Commit();

            return commit;
        }

        /// <summary>
        /// Inserts or overwrites the supplied value rows and deletes the supplied ones, without
        /// saving. Called from inside the transaction opened by <see cref="AddCommit"/>.
        /// </summary>
        /// <param name="db">The context the write is staged on.</param>
        /// <param name="upserts">The value rows to insert or overwrite. May be <c>null</c>.</param>
        /// <param name="removals">The ids of the value rows to delete. May be <c>null</c>.</param>
        private static void ApplyValues(KleeneStarDbContext db, IEnumerable<Value> upserts, IEnumerable<Guid> removals)
        {
            foreach (var value in upserts ?? [])
            {
                if (value is null)
                {
                    continue;
                }

                var existing = db.Values
                    .FirstOrDefault(x => x.ObjectId == value.ObjectId && x.FieldId == value.FieldId);

                if (existing is null)
                {
                    db.Values.Add(new Value(value.Id == Guid.Empty ? Guid.NewGuid() : value.Id)
                    {
                        ObjectId = value.ObjectId,
                        FieldId = value.FieldId,
                        Data = value.Data,
                        Created = value.Created == default ? DateTime.UtcNow : value.Created,
                        Updated = DateTime.UtcNow
                    });

                    continue;
                }

                existing.Data = value.Data;
                existing.Updated = DateTime.UtcNow;
            }

            foreach (var id in removals ?? [])
            {
                var existing = db.Values.FirstOrDefault(x => x.Id == id);

                if (existing is not null)
                {
                    db.Values.Remove(existing);
                }
            }
        }

        /// <summary>
        /// Reads the commit chain of an object, oldest first, with ordered changes.
        /// </summary>
        /// <param name="db">The context to read from.</param>
        /// <param name="objectId">The id of the owning object.</param>
        /// <returns>The chain, oldest first.</returns>
        private static List<Commit> ReadChain(KleeneStarDbContext db, Guid objectId)
        {
            var chain = db.Commits
                .Include(x => x.Changes)
                .AsNoTracking()
                .Where(x => x.ObjectId == objectId)
                .OrderBy(x => x.Number)
                .ToList();

            foreach (var commit in chain)
            {
                OrderChanges(commit);
            }

            return chain;
        }

        /// <summary>
        /// Restores the recorded order of a commit's changes, which the store does not guarantee
        /// on read.
        /// </summary>
        /// <param name="commit">The commit whose changes are ordered. May be <c>null</c>.</param>
        private static void OrderChanges(Commit commit)
        {
            if (commit?.Changes is { Count: > 1 })
            {
                commit.Changes = [.. commit.Changes.OrderBy(x => x.Ordinal)];
            }
        }
    }
}
