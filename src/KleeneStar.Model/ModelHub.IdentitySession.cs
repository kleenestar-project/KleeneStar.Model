using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the devices and browsers that are currently
    /// signed in with an identity.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the sessions of the given identity, the current device first and the
        /// remaining ones ordered by their last activity.
        /// </summary>
        /// <param name="ownerId">The signed-in identity.</param>
        /// <returns>A materialized collection of sessions (possibly empty).</returns>
        public static IEnumerable<IdentitySession> GetIdentitySessions(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
            {
                return [];
            }

            using var db = CreateDbContext();

            return [.. db.IdentitySessions
                .AsNoTracking()
                .Where(x => x.OwnerId == ownerId)
                .OrderByDescending(x => x.Current)
                .ThenByDescending(x => x.LastActive)];
        }

        /// <summary>
        /// Returns a single session by its id, or <see langword="null"/> when no such session exists.
        /// </summary>
        /// <param name="sessionId">The id of the session.</param>
        /// <returns>The session, or <see langword="null"/>.</returns>
        public static IdentitySession GetIdentitySession(Guid sessionId)
        {
            if (sessionId == Guid.Empty)
            {
                return null;
            }

            using var db = CreateDbContext();

            return db.IdentitySessions
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == sessionId);
        }

        /// <summary>
        /// Adds the specified session to the database if it does not already exist.
        /// </summary>
        /// <param name="session">The session to add.</param>
        public static void Add(IdentitySession session)
        {
            ArgumentNullException.ThrowIfNull(session);

            using var db = CreateDbContext();

            if (db.IdentitySessions.Any(x => x.Id == session.Id))
            {
                return;
            }

            db.IdentitySessions.Add(session);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the session with the specified id, ending the login on that device.
        /// </summary>
        /// <param name="sessionId">The id of the session to end.</param>
        public static void RemoveIdentitySession(Guid sessionId)
        {
            using var db = CreateDbContext();

            var existing = db.IdentitySessions.FirstOrDefault(x => x.Id == sessionId);

            if (existing is null)
            {
                return;
            }

            db.IdentitySessions.Remove(existing);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes every session of the given identity except the one the request is served
        /// to, signing the account out everywhere else.
        /// </summary>
        /// <param name="ownerId">The signed-in identity.</param>
        /// <returns>The number of sessions that were ended.</returns>
        public static int RemoveOtherIdentitySessions(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
            {
                return 0;
            }

            using var db = CreateDbContext();

            var others = db.IdentitySessions
                .Where(x => x.OwnerId == ownerId && !x.Current)
                .ToList();

            if (others.Count == 0)
            {
                return 0;
            }

            db.IdentitySessions.RemoveRange(others);
            db.SaveChanges();

            return others.Count;
        }
    }
}
