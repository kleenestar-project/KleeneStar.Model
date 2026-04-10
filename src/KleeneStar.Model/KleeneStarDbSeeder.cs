using System;
using System.Linq;
using System.Threading.Tasks;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Ensures that the database is populated with required initial data if it is not 
        /// already present.
        /// </summary>
        /// <remarks>
        /// This method should be invoked during application startup to guarantee that the database
        /// contains the minimum necessary data for the application to operate. If the required data
        /// already exists, no changes are made.
        /// </remarks>
        /// <param name="db">
        /// The database context used for performing the seeding operation. Cannot be null.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous seeding process.
        /// </returns>
        public static async Task SeedAsync(KleeneStarDbContext db)
        {
            if (!db.Categories.Any())
            {
                SeedCategories(db);
                await db.SaveChangesAsync();
            }

            if (!db.Tenants.Any())
            {
                SeedTenants(db);
                await db.SaveChangesAsync();
            }

            if (!db.PermissionProfiles.Any())
            {
                SeedPermissionProfiles(db);
                await db.SaveChangesAsync();
            }

            if (!db.Workspaces.Any())
            {
                SeedWorkspaces(db);
                await db.SaveChangesAsync();
            }

            if (!db.Classes.Any())
            {
                SeedClasses(db);
                await db.SaveChangesAsync();
            }

            if (!db.Fields.Any())
            {
                SeedFields(db);
                await db.SaveChangesAsync();
            }

            if (!db.Forms.Any())
            {
                SeedForms(db);
                await db.SaveChangesAsync();
            }

            if (!db.Priorities.Any())
            {
                SeedPriorities(db);
                await db.SaveChangesAsync();
            }

            if (!db.StatusCategories.Any())
            {
                SeedStatusCategories(db);
                await db.SaveChangesAsync();
            }

            if (!db.Statuses.Any())
            {
                try
                {
                    SeedStatuses(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // log the exception or handle it as needed
                    Console.WriteLine($"Error seeding objects: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }

            if (!db.Workflows.Any())
            {
                try
                {
                    SeedWorkflows(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // log the exception or handle it as needed
                    Console.WriteLine($"Error seeding objects: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }

            if (!db.Objects.Any())
            {
                try
                {
                    SeedObjects(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // log the exception or handle it as needed
                    Console.WriteLine($"Error seeding objects: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }

            if (!db.Dashboards.Any())
            {
                try
                {
                    SeedDashboards(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // log the exception or handle it as needed
                    Console.WriteLine($"Error seeding objects: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }
        }
    }
}
