using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the personal access tokens an identity
    /// created for API access and integrations.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the tokens owned by the given identity, newest first.
        /// </summary>
        /// <param name="ownerId">The identity that owns the tokens.</param>
        /// <returns>A materialized collection of tokens (possibly empty).</returns>
        public static IEnumerable<AccessToken> GetAccessTokens(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
            {
                return [];
            }

            using var db = CreateDbContext();

            return [.. db.AccessTokens
                .AsNoTracking()
                .Where(x => x.OwnerId == ownerId)
                .OrderByDescending(x => x.Created)];
        }

        /// <summary>
        /// Returns a single token by its id, or <see langword="null"/> when no such token exists.
        /// </summary>
        /// <param name="tokenId">The id of the token.</param>
        /// <returns>The token, or <see langword="null"/>.</returns>
        public static AccessToken GetAccessToken(Guid tokenId)
        {
            if (tokenId == Guid.Empty)
            {
                return null;
            }

            using var db = CreateDbContext();

            return db.AccessTokens
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == tokenId);
        }

        /// <summary>
        /// Adds the specified token to the database if it does not already exist.
        /// </summary>
        /// <param name="token">The token to add.</param>
        public static void Add(AccessToken token)
        {
            ArgumentNullException.ThrowIfNull(token);

            using var db = CreateDbContext();

            if (db.AccessTokens.Any(x => x.Id == token.Id))
            {
                return;
            }

            db.AccessTokens.Add(token);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified token in the database.
        /// </summary>
        /// <param name="token">The token to update.</param>
        public static void Update(AccessToken token)
        {
            ArgumentNullException.ThrowIfNull(token);

            using var db = CreateDbContext();

            var existing = db.AccessTokens.FirstOrDefault(x => x.Id == token.Id);

            if (existing is null)
            {
                return;
            }

            existing.Name = token.Name;
            existing.Scopes = token.Scopes;
            existing.Expires = token.Expires;
            existing.LastUsed = token.LastUsed;
            existing.Revoked = token.Revoked;

            db.SaveChanges();
        }

        /// <summary>
        /// Removes the token with the specified id from the database.
        /// </summary>
        /// <param name="tokenId">The id of the token to remove.</param>
        public static void RemoveAccessToken(Guid tokenId)
        {
            using var db = CreateDbContext();

            var existing = db.AccessTokens.FirstOrDefault(x => x.Id == tokenId);

            if (existing is null)
            {
                return;
            }

            db.AccessTokens.Remove(existing);
            db.SaveChanges();
        }
    }
}
