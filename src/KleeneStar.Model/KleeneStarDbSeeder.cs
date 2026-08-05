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

            if (!db.Maintenances.Any())
            {
                SeedMaintenance(db);
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

            // Tags must be seeded AFTER Objects — SeedTags attaches labels to existing objects.
            if (!db.ObjectTags.Any())
            {
                try
                {
                    SeedTags(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding tags: {ex.InnerException?.Message ?? ex.Message}");
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

            // Sprints must be seeded AFTER Objects — SeedSprints commits a share of each
            // workspace's existing objects to the seeded iterations.
            if (!db.Sprints.Any())
            {
                try
                {
                    SeedSprints(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding sprints: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }

            // SavedSearches must be seeded AFTER Identities — each saved search is owned
            // by the seeded admin identity (SavedSearch.OwnerId references Identity.Id).
            if (!db.SavedSearches.Any())
            {
                try
                {
                    SeedSavedSearches(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding saved searches: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }

            // WorkspaceBookmarks must be seeded AFTER Identities + Workspaces — each bookmark
            // references both the seeded admin identity and an existing workspace.
            if (!db.WorkspaceBookmarks.Any())
            {
                try
                {
                    SeedWorkspaceBookmarks(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding workspace bookmarks: {ex.InnerException?.Message ?? ex.Message}");
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

            // Attachments must be seeded AFTER Objects + Identities — SeedAttachments
            // resolves uploaders by e-mail and attaches each file to an existing object.
            if (!db.Attachments.Any())
            {
                try
                {
                    SeedAttachments(db);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding attachments: {ex.InnerException?.Message ?? ex.Message}");
                    throw;
                }
            }
        }
    }
}
