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
        /// The id of the administrator account a fresh database starts with.
        /// </summary>
        public static readonly Guid AdministratorId = Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD");

        /// <summary>
        /// The user name of the administrator account a fresh database starts with.
        /// </summary>
        public const string AdministratorUserName = "admin";

        /// <summary>
        /// The display name of the administrator account a fresh database starts with.
        /// </summary>
        public const string AdministratorName = "Admin User";

        /// <summary>
        /// The password the administrator account of a fresh database signs in with.
        /// </summary>
        /// <remarks>
        /// Published with the source. An installation changes it on the first sign in
        /// (<c>Profile/Security</c>).
        /// </remarks>
        public const string InitialPassword = "kleenestar";

        /// <summary>
        /// Adds the administrators group (<see cref="Group.AdministratorsId"/>). Account
        /// administration and the administration of unadministered resources are granted to its
        /// members only.
        /// </summary>
        /// <param name="db">The database context to which the group will be added. Cannot be null.</param>
        private static void SeedAdministratorsGroup(KleeneStarDbContext db)
        {
            db.Groups.Add(new Group(Group.AdministratorsId)
            {
                Name = "Admin",
                Description = "Administrators with full system access."
            });
        }

        /// <summary>
        /// Adds the built-in groups with implicit membership (<see cref="Group.AuthenticatedId"/>,
        /// <see cref="Group.AnonymousId"/>) where the store does not carry them.
        /// </summary>
        /// <remarks>
        /// Runs on every start. A permission grant references a stored group, so both groups have
        /// to exist in every store a workspace template grants to.
        /// </remarks>
        /// <param name="db">The database context. Cannot be null.</param>
        /// <returns><see langword="true"/> when a group was added and the context needs saving.</returns>
        private static bool EnsureBuiltInGroups(KleeneStarDbContext db)
        {
            var added = false;

            // both groups are looked up in one statement, since this runs on every start
            var existing = db.Groups
                .Where(x => x.Id == Group.AuthenticatedId || x.Id == Group.AnonymousId)
                .Select(x => x.Id)
                .ToHashSet();

            void ensure(Guid id, string name, string description)
            {
                if (existing.Contains(id))
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
        /// Adds the administrator account (<see cref="AdministratorId"/>): internal, without a
        /// tenant, member of the administrators group, signing in as
        /// <see cref="AdministratorUserName"/> with <see cref="InitialPassword"/>.
        /// </summary>
        /// <remarks>
        /// Account administration is restricted to the members of the administrators group, so a
        /// database without this account has nobody who can create a second one.
        /// </remarks>
        /// <param name="db">The database context to which the account will be added. Cannot be null.</param>
        private static void SeedAdministrator(KleeneStarDbContext db)
        {
            var administrator = new Identity(AdministratorId)
            {
                Name = AdministratorName,
                UserName = AdministratorUserName,
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
                PersonnelNumber = "A-00482",
                PasswordChanged = DateTime.UtcNow
            };

            administrator.PasswordHash = IdentityPassword.Hash(administrator, InitialPassword);

            administrator.GroupMemberships =
            [
                .. db.Groups
                    .Where(x => x.Id == Group.AdministratorsId)
                    .Select(x => new IdentityGroupMembership { Group = x })
            ];

            db.Identities.Add(administrator);
        }

        /// <summary>
        /// Gives <see cref="InitialPassword"/> to every account whose password hash is a
        /// <c>$seed$</c> placeholder (<see cref="IdentityPassword.SeedPlaceholderPrefix"/>).
        /// </summary>
        /// <remarks>
        /// No password verifies against a placeholder, so such an account cannot sign in. An
        /// account with any other hash is left unchanged; the step runs on every start.
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
                identity.PasswordHash = IdentityPassword.Hash(identity, InitialPassword);
                identity.PasswordChanged = DateTime.UtcNow;
            }

            return placeholders.Count > 0;
        }
    }
}
