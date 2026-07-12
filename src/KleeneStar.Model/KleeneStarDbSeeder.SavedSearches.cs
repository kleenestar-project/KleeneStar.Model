using KleeneStar.Model.Entities;
using System;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// The seeded admin identity that owns the example saved searches. Mirrors the id
        /// assigned in <see cref="SeedIdentities"/>.
        /// </summary>
        private static readonly Guid SavedSearchOwnerId = Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD");

        /// <summary>
        /// Adds a default set of example saved searches owned by the seeded admin identity, if
        /// none already exist. The names and descriptions are product text (not i18n keys) and
        /// are therefore English-only.
        /// </summary>
        /// <param name="db">The database context to seed. Cannot be null.</param>
        private static void SeedSavedSearches(KleeneStarDbContext db)
        {
            void add(string id, string name, string description, string query, bool starred, int recencyHours)
            {
                db.SavedSearches.Add(new SavedSearch
                {
                    Id = Guid.Parse(id),
                    Name = name,
                    Description = description,
                    Query = query,
                    OwnerId = SavedSearchOwnerId,
                    Starred = starred,
                    State = SavedSearchState.Active,
                    LastUsed = DateTime.UtcNow.AddHours(-recencyHours),
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });
            }

            add
            (
                "1A0B9C8D-7E6F-4051-8243-657483920A1B",
                "My open incidents",
                "All open incidents I am working on across every workspace.",
                "Summary ~ \"incident\"",
                starred: true,
                recencyHours: 1
            );

            add
            (
                "2B1C0D9E-8F70-4162-9354-768594031B2C",
                "High priority this week",
                "Recently updated high-priority items across all workspaces.",
                "Key ~ \"INC\"",
                starred: true,
                recencyHours: 5
            );

            add
            (
                "3C2D1E0F-9081-4273-A465-879605142C3D",
                "Login flow tickets",
                "Anything mentioning the login flow.",
                "Summary ~ \"Login Flow\"",
                starred: false,
                recencyHours: 26
            );

            add
            (
                "4D3E2F10-A192-4384-B576-98A716253D4E",
                "Service desk backlog",
                "Service desk items awaiting triage.",
                "Key ~ \"SD\"",
                starred: false,
                recencyHours: 72
            );
        }
    }
}
