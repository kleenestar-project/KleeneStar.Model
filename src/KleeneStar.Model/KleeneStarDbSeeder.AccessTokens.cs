using KleeneStar.Model.Entities;
using System;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Adds a set of personal access tokens to the identity the profile pages are served
        /// for, so the token list shows the states a real account accumulates: tokens in use,
        /// a token that has never been used and one that has run out.
        /// </summary>
        /// <remarks>
        /// The hashes are placeholders. No seeded token is a usable credential — the secret of
        /// a real token exists only in the moment it is handed to its creator.
        /// </remarks>
        /// <param name="db">The database context to which the tokens will be added. Cannot be null.</param>
        private static void SeedAccessTokens(KleeneStarDbContext db)
        {
            var owner = db.Identities.FirstOrDefault(x => x.Email == "admin@kleenestar.org");

            if (owner is null)
            {
                return;
            }

            var now = DateTime.UtcNow;

            void add
            (
                string id,
                string name,
                string prefix,
                string scopes,
                double createdDaysAgo,
                double? lastUsedHoursAgo,
                double? expiresInDays,
                bool revoked = false
            )
            {
                db.AccessTokens.Add(new AccessToken
                {
                    Id = Guid.Parse(id),
                    OwnerId = owner.Id,
                    Name = name,
                    Prefix = prefix,
                    // WARNING: placeholder, non-production hash value for development seeds only.
                    TokenHash = "$seed$v1$" + Guid.Parse(id).ToString("N"),
                    Scopes = scopes,
                    Created = now.AddDays(-createdDaysAgo),
                    LastUsed = lastUsedHoursAgo.HasValue ? now.AddHours(-lastUsedHoursAgo.Value) : null,
                    Expires = expiresInDays.HasValue ? now.AddDays(expiresInDays.Value) : null,
                    Revoked = revoked
                });
            }

            add
            (
                "1F2B7C48-90AE-4D31-A6E5-3C8B01D4F927",
                "CI · GitHub Actions",
                "kls_AB12cd34",
                "read:tickets write:tickets read:workflows",
                createdDaysAgo: 194,
                lastUsedHoursAgo: 0.25,
                expiresInDays: 261
            );

            add
            (
                "5A0D3E71-4C82-49B6-8F35-D71E9A20C4B8",
                "Datadog Reporter",
                "kls_XY98zz12",
                "read:tickets read:forms",
                createdDaysAgo: 277,
                lastUsedHoursAgo: 1,
                expiresInDays: 88
            );

            add
            (
                "9C6E82AF-13D5-4708-B92C-4E0A7F53D186",
                "Lokales CLI · MacBook",
                "kls_LM55qq01",
                "read:tickets write:tickets",
                createdDaysAgo: 205,
                lastUsedHoursAgo: 26,
                expiresInDays: 24
            );

            add
            (
                "7B4A05CD-6E19-4F82-93A0-8D251C6EB70F",
                "Migration Skript",
                "kls_PQ77ww00",
                "admin:tenant",
                createdDaysAgo: 457,
                lastUsedHoursAgo: 4380,
                expiresInDays: -273
            );
        }
    }
}
