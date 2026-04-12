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
            void addGroup(string id, string name, string description) => db.Groups.Add(new Group
            {
                Id = Guid.Parse(id),
                Name = name,
                Description = description
            });

            addGroup("C0000001-0000-0000-0000-000000000001", "Admin", "Administrators with full system access.");
            addGroup("C0000001-0000-0000-0000-000000000002", "Engineering", "Software engineering team members.");
            addGroup("C0000001-0000-0000-0000-000000000003", "Marketing", "Marketing department members.");
            addGroup("C0000001-0000-0000-0000-000000000004", "Support", "Customer support team members.");
        }

        /// <summary>
        /// Adds a predefined set of identity entities and group memberships to the specified database context.
        /// </summary>
        /// <param name="db">The database context to which the identity entities will be added. Cannot be null.</param>
        private static void SeedIdentities(KleeneStarDbContext db)
        {
            // WARNING: Placeholder non-production hash values for development/test seed identities only.
            // These values must never be used as real credential hashes in production environments.
            const string adminHash = "$seed$v1$fb4e111dbf8b4c1cb95e0f6579f7f72f";
            const string aliceHash = "$seed$v1$7d47a268f7df4d31bc8a32f8f60f8124";
            const string marketerHash = "$seed$v1$903d043655ff45119a3d1ec0f7bc6f16";
            const string supportHash = "$seed$v1$9b5ddb23be9945039f8d2bf8ff5b81c5";

            void addIdentity(string id, string name, string email, string passwordHash, params string[] groups)
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

            addIdentity(
                "E0000001-0000-0000-0000-000000000001",
                "Admin User",
                "admin@kleenestar.local",
                adminHash,
                "Admin"
            );

            addIdentity(
                "E0000001-0000-0000-0000-000000000002",
                "Alice Engineer",
                "alice.engineer@kleenestar.local",
                aliceHash,
                "Engineering"
            );

            addIdentity(
                "E0000001-0000-0000-0000-000000000003",
                "Marketing User",
                "marketer@kleenestar.local",
                marketerHash,
                "Marketing"
            );

            addIdentity(
                "E0000001-0000-0000-0000-000000000004",
                "Support User",
                "support@kleenestar.local",
                supportHash,
                "Support"
            );
        }
    }
}
