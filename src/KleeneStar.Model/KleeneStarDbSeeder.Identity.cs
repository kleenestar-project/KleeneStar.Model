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
        /// Adds a predefined set of group entities to the specified database context.
        /// </summary>
        /// <param name="db">The database context to which the group entities will be added. Cannot be null.</param>
        private static void SeedGroups(KleeneStarDbContext db)
        {
            void add(string id, string name, string description) => db.Groups.Add(new Group
            {
                Id = Guid.Parse(id),
                Name = name,
                Description = description
            });

            add("C0000001-0000-0000-0000-000000000001", "Admin", "Administrators with full system access.");
            add("C0000001-0000-0000-0000-000000000002", "Engineering", "Software engineering team members.");
            add("C0000001-0000-0000-0000-000000000003", "Marketing", "Marketing department members.");
            add("C0000001-0000-0000-0000-000000000004", "Support", "Customer support team members.");
        }

        /// <summary>
        /// Adds a predefined set of identity entities and group memberships to the specified database context.
        /// </summary>
        /// <param name="db">The database context to which the identity entities will be added. Cannot be null.</param>
        private static void SeedIdentities(KleeneStarDbContext db)
        {
            void add(string id, string name, string email, string passwordHash, params string[] groups)
            {
                db.Identities.Add(new Identity
                {
                    Id = Guid.Parse(id),
                    Name = name,
                    Email = email,
                    PasswordHash = passwordHash,
                    GroupMemberships =
                    [
                        .. db.Groups
                            .Where(x => groups.Contains(x.Name))
                            .Select(x => new IdentityGroupMembership { Group = x })
                    ]
                });
            }

            add(
                "E0000001-0000-0000-0000-000000000001",
                "Admin User",
                "admin@kleenestar.local",
                "seed:admin",
                "Admin"
            );

            add(
                "E0000001-0000-0000-0000-000000000002",
                "Alice Engineer",
                "alice.engineer@kleenestar.local",
                "seed:alice",
                "Engineering"
            );

            add(
                "E0000001-0000-0000-0000-000000000003",
                "Marketer User",
                "marketer@kleenestar.local",
                "seed:marketer",
                "Marketing"
            );

            add(
                "E0000001-0000-0000-0000-000000000004",
                "Support User",
                "support@kleenestar.local",
                "seed:support",
                "Support"
            );
        }
    }
}
