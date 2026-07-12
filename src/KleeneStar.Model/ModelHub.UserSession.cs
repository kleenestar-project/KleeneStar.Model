using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with per-identity session/preference data.
    /// Storage is generic key/value: callers pick a <c>scope</c> (e.g. <c>"rest-table"</c>)
    /// plus a <c>key</c> (e.g. the REST table type name) and supply an opaque payload —
    /// typically JSON — that the producer/consumer pair knows how to interpret.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the value associated with the given owner/scope/key tuple, or
        /// <see langword="null"/> if none has been stored yet.
        /// </summary>
        /// <param name="ownerId">The identity that owns the entry.</param>
        /// <param name="scope">The scope namespace.</param>
        /// <param name="key">The key inside the scope.</param>
        /// <returns>The stored value, or <see langword="null"/> when no entry exists.</returns>
        public static string GetUserSessionValue(Guid ownerId, string scope, string key)
        {
            if (ownerId == Guid.Empty || string.IsNullOrWhiteSpace(scope) || string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            using var db = CreateDbContext();

            return db.UserSessions
                .AsNoTracking()
                .Where(x => x.OwnerId == ownerId && x.Scope == scope && x.Key == key)
                .Select(x => x.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns all entries stored under the given owner/scope combination.
        /// </summary>
        /// <param name="ownerId">The identity that owns the entries.</param>
        /// <param name="scope">The scope namespace.</param>
        /// <returns>All matching entries (possibly empty).</returns>
        public static IEnumerable<UserSession> GetUserSessions(Guid ownerId, string scope)
        {
            if (ownerId == Guid.Empty || string.IsNullOrWhiteSpace(scope))
            {
                return [];
            }

            using var db = CreateDbContext();

            return [.. db.UserSessions
                .AsNoTracking()
                .Where(x => x.OwnerId == ownerId && x.Scope == scope)];
        }

        /// <summary>
        /// Inserts or updates the value for the given owner/scope/key tuple.
        /// Setting <paramref name="value"/> to <see langword="null"/> deletes the entry.
        /// </summary>
        /// <param name="ownerId">The identity that owns the entry.</param>
        /// <param name="scope">The scope namespace.</param>
        /// <param name="key">The key inside the scope.</param>
        /// <param name="value">The new value, or <see langword="null"/> to delete.</param>
        public static void SetUserSessionValue(Guid ownerId, string scope, string key, string value)
        {
            if (ownerId == Guid.Empty || string.IsNullOrWhiteSpace(scope) || string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            using var db = CreateDbContext();

            var existing = db.UserSessions
                .FirstOrDefault(x => x.OwnerId == ownerId && x.Scope == scope && x.Key == key);

            if (value is null)
            {
                if (existing is not null)
                {
                    db.UserSessions.Remove(existing);
                    db.SaveChanges();
                }

                return;
            }

            var now = DateTime.UtcNow;

            if (existing is null)
            {
                db.UserSessions.Add(new UserSession
                {
                    OwnerId = ownerId,
                    Scope = scope,
                    Key = key,
                    Value = value,
                    Created = now,
                    Updated = now
                });
            }
            else
            {
                existing.Value = value;
                existing.Updated = now;
            }

            db.SaveChanges();
        }
    }
}
