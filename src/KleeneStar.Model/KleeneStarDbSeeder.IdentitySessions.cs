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
        /// Adds the devices and browsers signed in with the identity the profile pages are
        /// served for, so the "active sessions" page has the current device plus the other
        /// logins it offers to end.
        /// </summary>
        /// <param name="db">The database context to which the sessions will be added. Cannot be null.</param>
        private static void SeedIdentitySessions(KleeneStarDbContext db)
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
                string device,
                string client,
                bool mobile,
                string location,
                string ipAddress,
                double lastActiveHoursAgo,
                double createdDaysAgo,
                bool current = false
            )
            {
                db.IdentitySessions.Add(new IdentitySession
                {
                    Id = Guid.Parse(id),
                    OwnerId = owner.Id,
                    Device = device,
                    Client = client,
                    Mobile = mobile,
                    Location = location,
                    IpAddress = ipAddress,
                    Created = now.AddDays(-createdDaysAgo),
                    LastActive = now.AddHours(-lastActiveHoursAgo),
                    Current = current
                });
            }

            add
            (
                "2E9C41B7-58D0-4A63-9F17-C0B8543E7D26",
                "MacBook Pro 14\"",
                "Chrome 125",
                mobile: false,
                "Berlin, DE",
                "85.214.···.42",
                lastActiveHoursAgo: 0,
                createdDaysAgo: 12,
                current: true
            );

            add
            (
                "8D537FA2-6B14-49C8-A05E-3F91C7B620D4",
                "iPhone 15",
                "KleeneStar iOS 4.12",
                mobile: true,
                "Berlin, DE",
                "88.130.···.18",
                lastActiveHoursAgo: 2,
                createdDaysAgo: 30
            );

            add
            (
                "4A1B96E3-0C75-42DF-8B69-E5D307A14FC8",
                "ThinkPad X1",
                "Firefox 124",
                mobile: false,
                "Wien, AT",
                "193.81.···.7",
                lastActiveHoursAgo: 72,
                createdDaysAgo: 45
            );

            add
            (
                "C7402D68-9EB3-4517-A2F4-6081B5D93E7A",
                "Linux Workstation",
                "Safari 17.4",
                mobile: false,
                "Lissabon, PT",
                "212.55.···.91",
                lastActiveHoursAgo: 192,
                createdDaysAgo: 60
            );
        }
    }
}
