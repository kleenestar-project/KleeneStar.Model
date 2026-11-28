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
        /// Returns the settings of a poll object.
        /// </summary>
        /// <param name="objectId">The poll object.</param>
        /// <returns>The settings, or <see langword="null"/>.</returns>
        public static Poll GetPoll(Guid objectId)
        {
            using var db = CreateDbContext();

            return db.Polls.AsNoTracking().FirstOrDefault(x => x.ObjectId == objectId);
        }

        /// <summary>
        /// Returns the settings of several poll objects in one read.
        /// </summary>
        /// <param name="objectIds">The poll objects.</param>
        /// <returns>The settings found, keyed by object.</returns>
        public static IReadOnlyDictionary<Guid, Poll> GetPolls(IEnumerable<Guid> objectIds)
        {
            var ids = (objectIds ?? []).Distinct().ToList();

            if (ids.Count == 0)
            {
                return new Dictionary<Guid, Poll>();
            }

            using var db = CreateDbContext();

            return db.Polls
                .AsNoTracking()
                .Where(x => ids.Contains(x.ObjectId))
                .ToList()
                .GroupBy(x => x.ObjectId)
                .ToDictionary(x => x.Key, x => x.First());
        }

        /// <summary>
        /// Returns the settings of the polls of some workspaces that still run at a point in
        /// time: not closed, and without deadline or with a deadline after it.
        /// </summary>
        /// <param name="workspaceIds">The workspaces, or <see langword="null"/> for all.</param>
        /// <param name="now">The point in time (UTC).</param>
        /// <returns>The settings; the objects are for the caller to read with its visibility.</returns>
        public static IReadOnlyList<Poll> GetRunningPolls(IReadOnlyCollection<Guid> workspaceIds, DateTime now)
        {
            using var db = CreateDbContext();

            var objects = db.Objects.AsNoTracking().Where(o => o.Kind == ObjectKind.Poll);

            if (workspaceIds is not null)
            {
                var ids = workspaceIds.ToList();
                objects = objects.Where(o => ids.Contains(o.WorkspaceId));
            }

            return [.. db.Polls
                .AsNoTracking()
                .Where(x => x.Closed == null && (x.Deadline == null || x.Deadline > now))
                .Where(x => objects.Any(o => o.Id == x.ObjectId))];
        }

        /// <summary>
        /// Returns the settings of the polls whose deadline ran out before a point in time but
        /// that were not closed yet.
        /// </summary>
        /// <param name="before">The point in time (UTC).</param>
        /// <param name="limit">The most polls answered in one call.</param>
        /// <returns>The settings, the longest overdue first.</returns>
        public static IReadOnlyList<Poll> GetPollsToClose(DateTime before, int limit)
        {
            using var db = CreateDbContext();

            return [.. db.Polls
                .AsNoTracking()
                .Where(x => x.Closed == null && x.Deadline != null && x.Deadline <= before)
                .OrderBy(x => x.Deadline)
                .Take(Math.Max(1, limit))];
        }

        /// <summary>
        /// Writes the settings of a poll: adds them when the object has none, replaces the
        /// stored ones otherwise.
        /// </summary>
        /// <param name="poll">The settings.</param>
        public static void SavePoll(Poll poll)
        {
            ArgumentNullException.ThrowIfNull(poll);

            using var db = CreateDbContext();

            var stored = db.Polls.FirstOrDefault(x => x.ObjectId == poll.ObjectId);

            if (stored is null)
            {
                poll.Created = poll.Created == default ? DateTime.UtcNow : poll.Created;
                poll.Updated = DateTime.UtcNow;
                db.Polls.Add(poll);
            }
            else
            {
                stored.Type = poll.Type;
                stored.MultipleChoice = poll.MultipleChoice;
                stored.AllowMaybe = poll.AllowMaybe;
                stored.Anonymous = poll.Anonymous;
                stored.HideResults = poll.HideResults;
                stored.Deadline = poll.Deadline;
                stored.Closed = poll.Closed;
                stored.ClosedById = poll.ClosedById;
                stored.ChosenOptionId = poll.ChosenOptionId;
                stored.TimeZone = poll.TimeZone;
                stored.Updated = DateTime.UtcNow;

                poll.Id = stored.Id;
                poll.RawId = stored.RawId;
                poll.Created = stored.Created;
                poll.Updated = stored.Updated;
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Returns the options of a poll in their order.
        /// </summary>
        /// <param name="objectId">The poll object.</param>
        /// <returns>The options.</returns>
        public static IReadOnlyList<PollOption> GetPollOptions(Guid objectId)
        {
            using var db = CreateDbContext();

            return [.. db.PollOptions
                .AsNoTracking()
                .Where(x => x.ObjectId == objectId)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.RawId)];
        }

        /// <summary>
        /// Replaces the options of a poll. An option whose id is stored keeps its votes and takes
        /// the new text, time and position; an option no longer named is removed with its votes.
        /// </summary>
        /// <param name="objectId">The poll object.</param>
        /// <param name="options">The options the poll is to have, in their order.</param>
        /// <returns>The options as stored.</returns>
        public static IReadOnlyList<PollOption> SetPollOptions(Guid objectId, IEnumerable<PollOption> options)
        {
            using var db = CreateDbContext();

            var stored = db.PollOptions.Where(x => x.ObjectId == objectId).ToList();
            var wanted = (options ?? []).Where(x => x is not null).ToList();
            var kept = new HashSet<Guid>();
            var order = 0;

            foreach (var option in wanted)
            {
                var match = stored.FirstOrDefault(x => x.Id == option.Id && !kept.Contains(x.Id));

                if (match is null)
                {
                    option.ObjectId = objectId;
                    option.Order = order++;
                    option.Created = option.Created == default ? DateTime.UtcNow : option.Created;
                    option.Updated = DateTime.UtcNow;

                    if (stored.Any(x => x.Id == option.Id))
                    {
                        option.Id = Guid.NewGuid();
                    }

                    db.PollOptions.Add(option);
                    kept.Add(option.Id);

                    continue;
                }

                kept.Add(match.Id);

                match.Order = order++;
                match.Text = option.Text;
                match.Start = option.Start;
                match.End = option.End;
                match.AllDay = option.AllDay;
                match.Updated = DateTime.UtcNow;
            }

            var removed = stored.Where(x => !kept.Contains(x.Id)).ToList();

            if (removed.Count > 0)
            {
                var removedIds = removed.Select(x => x.Id).ToList();

                // the foreign key cascades on a relational store; the votes are removed here as
                // well so that every store forgets them
                db.PollVotes.RemoveRange(db.PollVotes.Where(x => removedIds.Contains(x.OptionId)));
                db.PollOptions.RemoveRange(removed);
            }

            db.SaveChanges();

            return [.. db.PollOptions
                .AsNoTracking()
                .Where(x => x.ObjectId == objectId)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.RawId)];
        }

        /// <summary>
        /// Returns the votes of a poll.
        /// </summary>
        /// <param name="objectId">The poll object.</param>
        /// <returns>The votes, in the order they were first given.</returns>
        public static IReadOnlyList<PollVote> GetPollVotes(Guid objectId)
        {
            using var db = CreateDbContext();

            return [.. db.PollVotes
                .AsNoTracking()
                .Where(x => x.ObjectId == objectId)
                .OrderBy(x => x.Created)
                .ThenBy(x => x.RawId)];
        }

        /// <summary>
        /// Returns how many accounts voted in each of several polls, in one read.
        /// </summary>
        /// <param name="objectIds">The poll objects.</param>
        /// <returns>The numbers of voters, keyed by object; a poll without votes is missing.</returns>
        public static IReadOnlyDictionary<Guid, int> GetPollVoterCounts(IEnumerable<Guid> objectIds)
        {
            var ids = (objectIds ?? []).Distinct().ToList();

            if (ids.Count == 0)
            {
                return new Dictionary<Guid, int>();
            }

            using var db = CreateDbContext();

            return db.PollVotes
                .AsNoTracking()
                .Where(x => ids.Contains(x.ObjectId))
                .Select(x => new { x.ObjectId, x.IdentityId })
                .Distinct()
                .ToList()
                .GroupBy(x => x.ObjectId)
                .ToDictionary(x => x.Key, x => x.Count());
        }

        /// <summary>
        /// Returns the polls among some poll objects an account has voted in.
        /// </summary>
        /// <param name="objectIds">The poll objects.</param>
        /// <param name="identityId">The account.</param>
        /// <returns>The poll objects the account voted in.</returns>
        public static IReadOnlySet<Guid> GetPollsVotedBy(IEnumerable<Guid> objectIds, Guid identityId)
        {
            var ids = (objectIds ?? []).Distinct().ToList();

            if (ids.Count == 0 || identityId == Guid.Empty)
            {
                return new HashSet<Guid>();
            }

            using var db = CreateDbContext();

            return db.PollVotes
                .AsNoTracking()
                .Where(x => x.IdentityId == identityId && ids.Contains(x.ObjectId))
                .Select(x => x.ObjectId)
                .Distinct()
                .ToHashSet();
        }

        /// <summary>
        /// Replaces the answers of one account to a poll. Answers naming an option of another
        /// poll are ignored; an empty set withdraws the account's vote.
        /// </summary>
        /// <param name="objectId">The poll object.</param>
        /// <param name="identityId">The account.</param>
        /// <param name="answers">The answers, keyed by option.</param>
        /// <returns>The votes as stored.</returns>
        public static IReadOnlyList<PollVote> SetPollVotes(Guid objectId, Guid identityId, IReadOnlyDictionary<Guid, PollAnswer> answers)
        {
            using var db = CreateDbContext();

            var options = db.PollOptions
                .AsNoTracking()
                .Where(x => x.ObjectId == objectId)
                .Select(x => x.Id)
                .ToHashSet();

            var stored = db.PollVotes.Where(x => x.ObjectId == objectId && x.IdentityId == identityId).ToList();
            var wanted = (answers ?? new Dictionary<Guid, PollAnswer>())
                .Where(x => options.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var vote in stored)
            {
                if (wanted.TryGetValue(vote.OptionId, out var answer))
                {
                    if (vote.Answer != answer)
                    {
                        vote.Answer = answer;
                        vote.Updated = DateTime.UtcNow;
                    }

                    wanted.Remove(vote.OptionId);
                }
                else
                {
                    db.PollVotes.Remove(vote);
                }
            }

            foreach (var (optionId, answer) in wanted)
            {
                db.PollVotes.Add(new PollVote
                {
                    ObjectId = objectId,
                    OptionId = optionId,
                    IdentityId = identityId,
                    Answer = answer,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });
            }

            db.SaveChanges();

            return [.. db.PollVotes
                .AsNoTracking()
                .Where(x => x.ObjectId == objectId && x.IdentityId == identityId)];
        }

        /// <summary>
        /// Removes every vote of a poll, because what was voted on changed in kind.
        /// </summary>
        /// <param name="objectId">The poll object.</param>
        /// <returns>The number of votes removed.</returns>
        public static int ClearPollVotes(Guid objectId)
        {
            using var db = CreateDbContext();

            var votes = db.PollVotes.Where(x => x.ObjectId == objectId).ToList();

            db.PollVotes.RemoveRange(votes);
            db.SaveChanges();

            return votes.Count;
        }
    }
}
