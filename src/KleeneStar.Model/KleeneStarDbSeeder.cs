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

            if (!db.Groups.Any())
            {
                SeedGroups(db);
                await db.SaveChangesAsync();
            }

            if (!db.Identities.Any())
            {
                SeedIdentities(db);
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

            if (!db.Values.Any())
            {
                try
                {
                    SeedValues(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // log the exception or handle it as needed
                    Console.WriteLine($"Error seeding values: {ex.InnerException?.Message ?? ex.Message}");
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

            if (!db.ObjectViews.Any())
            {
                try
                {
                    SeedObjectViews(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding object views: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }

            // Calendars must be seeded BEFORE SLA policies — SeedSlas resolves the
            // per-class calendar by name to populate SlaPolicy.CalendarId.
            if (!db.Calendars.Any())
            {
                try
                {
                    SeedCalendars(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding calendars: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }

            if (!db.SlaPolicies.Any())
            {
                try
                {
                    SeedSlas(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding SLA policies: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }

            // Comments must be seeded AFTER Objects + Identities — SeedComments
            // resolves authors by e-mail and attaches each thread to an existing object.
            if (!db.Comments.Any())
            {
                try
                {
                    SeedComments(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding comments: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }
        }
    }
}
