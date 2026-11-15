using System.Linq;
using System.Threading.Tasks;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    /// <remarks>
    /// The base seed writes what every installation needs: the maintenance and branding records,
    /// the administrators group and the built-in groups, the administrator account, the status
    /// categories, the relation catalog and the genesis event of the audit log. Demonstration and
    /// test data (workspaces, classes, objects, further accounts) is the plugin
    /// <c>KleeneStar.Seeder</c>, which runs after this seed as an <c>IDatabaseSeed</c> of the core.
    /// </remarks>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Adds every part of the base data the store does not carry yet. Each part is guarded by
        /// a check on its table, so the call is idempotent and runs on every start.
        /// </summary>
        /// <param name="db">
        /// The database context used for performing the seeding operation. Cannot be null.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous seeding process.
        /// </returns>
        public static async Task SeedAsync(KleeneStarDbContext db)
        {
            if (!db.Maintenances.Any())
            {
                SeedMaintenance(db);
                await db.SaveChangesAsync();
            }

            if (!db.Brandings.Any())
            {
                SeedBranding(db);
                await db.SaveChangesAsync();
            }

            if (!db.Groups.Any())
            {
                SeedAdministratorsGroup(db);
                await db.SaveChangesAsync();
            }

            if (EnsureBuiltInGroups(db))
            {
                await db.SaveChangesAsync();
            }

            if (!db.Identities.Any())
            {
                SeedAdministrator(db);
                await db.SaveChangesAsync();
            }

            if (UpgradeSeedPasswords(db))
            {
                await db.SaveChangesAsync();
            }

            // no sessions and no access tokens are seeded: both are credentials of the WebExpress
            // token store (a sign-in grant, a personal access token), and a row without its
            // credential describes nothing that can be used

            if (!db.StatusCategories.Any())
            {
                SeedStatusCategories(db);
                await db.SaveChangesAsync();
            }

            if (!db.ObjectRelationTypes.Any())
            {
                SeedObjectRelationTypes(db);
                await db.SaveChangesAsync();
            }

            // last: the genesis event counts the rows written above
            if (!db.AuditEvents.Any())
            {
                SeedAudit(db);
                await db.SaveChangesAsync();
            }
        }
    }
}
