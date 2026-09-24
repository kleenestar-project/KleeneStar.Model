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
        /// The password every seeded account signs in with.
        /// </summary>
        /// <remarks>
        /// Demo data, published with the source - see <see cref="SeedIdentities"/>.
        /// </remarks>
        public const string DemoPassword = "kleenestar";

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

            addGroup(Group.AdministratorsId.ToString(), "Admin", "Administrators with full system access.");
            addGroup("7420A9F7-F23A-4EC2-91E4-EDDB2E3172BD", "Engineering", "Software engineering team members.");
            addGroup("4D3739DF-EBB0-4113-A40D-EEEBF9B26182", "Marketing", "Marketing department members.");
            addGroup("7EEB5E9D-87E6-4017-A94A-884F03DF129A", "Support", "Customer support team members.");
        }

        /// <summary>
        /// Adds the built-in groups whose membership is implicit - every signed-in account, every
        /// caller - where the store does not carry them yet.
        /// </summary>
        /// <remarks>
        /// Runs on every start, not only on a fresh store: a grant has to name a stored group, and
        /// a workspace created from a template on a database seeded before these groups existed
        /// would otherwise have nobody to grant its everyday access to.
        /// </remarks>
        /// <param name="db">The database context. Cannot be null.</param>
        /// <returns><see langword="true"/> when a group was added and the context needs saving.</returns>
        private static bool EnsureBuiltInGroups(KleeneStarDbContext db)
        {
            var added = false;

            void ensure(Guid id, string name, string description)
            {
                if (db.Groups.Any(x => x.Id == id))
                {
                    return;
                }

                db.Groups.Add(new Group(id) { Name = name, Description = description });
                added = true;
            }

            ensure(Group.AuthenticatedId, "Signed-in users", "Every signed-in account. Membership is implicit.");
            ensure(Group.AnonymousId, "Anonymous", "Every caller, including visitors who are not signed in. Membership is implicit.");

            return added;
        }

        /// <summary>
        /// Adds a predefined set of identity entities and group memberships to the specified database context.
        /// </summary>
        /// <remarks>
        /// Every identity is seeded with a filled-in profile — display name, bio, contact
        /// channels, regional formats and the business data of its tenant — so the profile
        /// settings pages show a realistic account from the first run rather than a set of
        /// empty inputs.
        /// <para>
        /// Every seeded account is internal and signs in with <see cref="DemoPassword"/>. The
        /// seeded identities are demo data, like the workspaces beside them; an installation
        /// that keeps them past a demonstration has to give them passwords of their own (or
        /// retire them), because the demo password is published with the source.
        /// </para>
        /// </remarks>
        /// <param name="db">The database context to which the identity entities will be added. Cannot be null.</param>
        private static void SeedIdentities(KleeneStarDbContext db)
        {
            void addIdentity(Identity identity, string tenantName, params string[] groups)
            {
                identity.PasswordHash = IdentityPassword.Hash(identity, DemoPassword);
                identity.PasswordChanged = DateTime.UtcNow;

                identity.Tenant = tenantName is null
                    ? null
                    : db.Tenants.FirstOrDefault(x => x.Name == tenantName);

                identity.GroupMemberships =
                [
                    .. db.Groups
                        .Where(x => groups.Contains(x.Name))
                        .Select(x => new IdentityGroupMembership { Group = x })
                ];

                db.Identities.Add(identity);
            }

            // operator-side admin has no tenant (the portal excludes it from
            // IssueScope.Organization entirely).
            addIdentity(new Identity
            {
                Id = Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD"),
                Name = "Admin User",
                UserName = "admin",
                Email = "admin@kleenestar.org",
                EmailVerified = true,
                Bio = "Senior Product Designerin · arbeitet an Workflow-Tools im Bereich SaaS. Berlin → Lissabon → Wien.",
                PhoneCountry = "+49",
                Phone = "151 23456789",
                Website = "kleenestar.org",
                Location = "Berlin, Deutschland",
                Position = "Senior Product Designerin",
                Language = "de",
                TimeZone = null,
                DateFormat = "dd.MM.yyyy",
                WeekStart = WeekStart.Monday,
                Role = "Workspace-Admin · Klasse Bug",
                RoleSince = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                Department = "Engineering · QA",
                CostCenter = "CC-4711",
                PersonnelNumber = "A-00482"
            }, null, "Admin");

            // Alice is an Acme tenant member (typical end-user identity).
            addIdentity(new Identity
            {
                Id = Guid.Parse("BBF45E5D-AA35-4382-9B84-6055193CE544"),
                Name = "Alice Engineer",
                UserName = "alice.engineer",
                Email = "alice.engineer@kleenestar.org",
                EmailVerified = true,
                Bio = "Backend-Entwicklerin · Plattform und Integrationen.",
                PhoneCountry = "+49",
                Phone = "170 9876543",
                Location = "Hamburg, Deutschland",
                Position = "Software Engineer",
                Language = "de",
                DateFormat = "dd.MM.yyyy",
                WeekStart = WeekStart.Monday,
                Role = "Mitglied · Engineering",
                RoleSince = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Department = "Engineering · Platform",
                CostCenter = "CC-2200",
                PersonnelNumber = "A-01137"
            }, "Acme Corp", "Engineering");

            // Marketer stays tenant-less — operator-side account.
            addIdentity(new Identity
            {
                Id = Guid.Parse("1AA3B0E0-5C40-46D8-8ACF-ED12740FD239"),
                Name = "Marketing User",
                UserName = "marketing.user",
                Email = "marketer@kleenestar.org",
                Location = "München, Deutschland",
                Position = "Marketing Manager",
                Language = "de",
                DateFormat = "dd.MM.yyyy",
                WeekStart = WeekStart.Monday,
                Role = "Mitglied · Marketing",
                Department = "Marketing"
            }, null, "Marketing");

            // Support stays tenant-less — operator-side account.
            addIdentity(new Identity
            {
                Id = Guid.Parse("D1C5AED2-78D3-45F7-BB19-E87B8F134301"),
                Name = "Support User",
                UserName = "support.user",
                Email = "support@kleenestar.org",
                Location = "Wien, Österreich",
                Position = "Service Desk Agent",
                Language = "de",
                DateFormat = "dd.MM.yyyy",
                WeekStart = WeekStart.Monday,
                Role = "Mitglied · Support",
                Department = "Customer Support"
            }, null, "Support");
        }

        /// <summary>
        /// Names the deputy of the seeded profile identity. Runs after
        /// <see cref="SeedIdentities"/> has been committed, because the deputy is another row
        /// of the very table being written and can only be referenced once it exists.
        /// </summary>
        /// <param name="db">The database context holding the seeded identities. Cannot be null.</param>
        private static void SeedIdentityDeputies(KleeneStarDbContext db)
        {
            var profile = db.Identities.FirstOrDefault(x => x.Id == Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD"));
            var deputy = db.Identities.FirstOrDefault(x => x.Id == Guid.Parse("BBF45E5D-AA35-4382-9B84-6055193CE544"));

            if (profile is null || deputy is null || profile.DeputyId.HasValue)
            {
                return;
            }

            profile.DeputyId = deputy.Id;
        }

        /// <summary>
        /// Gives the demo password to every account an earlier seeder left with a
        /// <c>$seed$</c> placeholder.
        /// </summary>
        /// <remarks>
        /// Before passwords were checked, the seeder wrote placeholders that no password
        /// produces; with the check in place those accounts could never sign in again, and
        /// nothing in the application could let them. Only the placeholders are touched - an
        /// account whose password somebody set keeps it - so the step is safe to run on every
        /// start, and does nothing once the placeholders are gone.
        /// </remarks>
        /// <param name="db">The database context holding the identities. Cannot be null.</param>
        /// <returns><see langword="true"/> when an account was changed.</returns>
        private static bool UpgradeSeedPasswords(KleeneStarDbContext db)
        {
            var placeholders = db.Identities
                .Where(x => x.PasswordHash != null && x.PasswordHash.StartsWith(IdentityPassword.SeedPlaceholderPrefix))
                .ToList();

            foreach (var identity in placeholders)
            {
                identity.PasswordHash = IdentityPassword.Hash(identity, DemoPassword);
                identity.PasswordChanged = DateTime.UtcNow;
            }

            return placeholders.Count > 0;
        }
    }
}
