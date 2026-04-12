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

            addGroup("7F57823B-8B94-4284-8DA1-39C49E152C8C", "Admin", "Administrators with full system access.");
            addGroup("7420A9F7-F23A-4EC2-91E4-EDDB2E3172BD", "Engineering", "Software engineering team members.");
            addGroup("4D3739DF-EBB0-4113-A40D-EEEBF9B26182", "Marketing", "Marketing department members.");
            addGroup("7EEB5E9D-87E6-4017-A94A-884F03DF129A", "Support", "Customer support team members.");
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
                "77087646-B13A-44B1-9BAC-6E66443CEDFD",
                "Admin User",
                "admin@kleenestar.org",
                adminHash,
                "Admin"
            );

            addIdentity(
                "BBF45E5D-AA35-4382-9B84-6055193CE544",
                "Alice Engineer",
                "alice.engineer@kleenestar.org",
                aliceHash,
                "Engineering"
            );

            addIdentity(
                "1AA3B0E0-5C40-46D8-8ACF-ED12740FD239",
                "Marketing User",
                "marketer@kleenestar.org",
                marketerHash,
                "Marketing"
            );

            addIdentity(
                "D1C5AED2-78D3-45F7-BB19-E87B8F134301",
                "Support User",
                "support@kleenestar.org",
                supportHash,
                "Support"
            );
        }
    }
}
