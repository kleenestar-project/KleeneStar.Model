using KleeneStar.Model.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model
{
    public static class KleeneStarDbSeeder
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

            if (!db.Workspaces.Any())
            {
                SeedWorkspaces(db);
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Adds a predefined set of category entities to the specified database context.
        /// </summary>
        /// <remarks>
        /// This method is intended to seed the database with a standard set of categories. 
        /// It does not save changes to the database; callers must call SaveChanges on the 
        /// context to persist the additions.
        /// </remarks>
        /// <param name="db">The database context to which the category entities will be added. Cannot be null.</param>
        private static void SeedCategories(KleeneStarDbContext db)
        {
            void add(string id, string name) => db.Categories.Add(new Category
            {
                Id = Guid.Parse(id),
                Name = name
            });

            add("6B90F75F-93C6-4186-965F-C2F974A04E05", "Compliance");
            add("AF7C999B-334F-4745-9799-AD46BCF7D7E0", "Development");
            add("DB91D6FD-1FB3-4F3B-B982-EDFCC733DEAF", "Engineering");
            add("9B08E835-4B55-4748-84BA-14AC1417C52E", "Finance");
            add("8CC28181-5DBC-4F29-8016-41885FF5A323", "HumanResources");
            add("26AC5FB6-CBAB-40A8-9D2F-5DA9E91910C9", "Infrastructure");
            add("9ECA0869-E563-41DB-81A2-50DC7D09478B", "Operations");
            add("F56D0256-4C67-47D7-AEF3-563D05EF9CEB", "Support");
        }

        /// <summary>
        /// Adds a default workspace to the specified database context if it 
        /// does not already exist.
        /// </summary>
        /// <param name="db">
        /// The database context to which the default workspace will be added. Cannot be null.
        /// </param>
        private static void SeedWorkspaces(KleeneStarDbContext db)
        {
            void add(string id, string name, string key, string description, string icon, params string[] categories) =>
                db.Workspaces.Add(new Workspace
                {
                    Id = Guid.Parse(id),
                    Name = name,
                    Key = key,
                    Description = description,
                    Icon = ImageIcon.FromString(icon),
                    Categories = [.. db.Categories.Where(x => categories.Contains(x.Name))]
                });

            add
            (
                "D651799A-690E-4CFF-AE0C-D3341CA3BBB4",
                "Configuration Database",
                "CMDB",
                "Workspace for managing configuration items and asset relationships.",
                "/kleenestar/assets/icons/cmdb.svg",
                "Infrastructure", "Compliance"
            );

            add
            (
                "660E9B11-2D54-4A36-84F9-F3BF5C78B748",
                "Software Development",
                "DEV",
                "Workspace for managing source code, repositories, development tasks, and release pipelines.",
                "/kleenestar/assets/icons/dev.svg",
                "Engineering"
            );

            add
            (
                "9994445E-FDBE-42E2-A3A0-65DF13CB453B",
                "Finance and Controlling",
                "FIN",
                "Workspace for managing budgets, invoices, and financial approvals.",
                "/kleenestar/assets/icons/fin.svg",
                "Finance"
            );

            add
            (
                "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F",
                "Human Resources",
                "HR",
                "Workspace for managing employee records, onboarding, and organizational structure.",
                "/kleenestar/assets/icons/hr.svg",
                "HumanResources"
            );

            add
            (
                "25F04599-C02A-4BEF-80B1-5957FBBB1FED",
                "Product Management",
                "PM",
                "Workspace for planning features, tracking releases, and coordinating product strategy.",
                "/kleenestar/assets/icons/pm.svg",
                "Development"
            );

            add
            (
                "D35FDCD6-5B11-4043-98D6-215DF414D99C",
                "Procurement",
                "PROC",
                "Workspace for managing purchase orders, supplier relations, and procurement workflows.",
                "/kleenestar/assets/icons/proc.svg",
                "Operations"
            );

            add
            (
                "F027A791-4219-4B1D-BA7C-2E7757091AAA",
                "IT Service Desk",
                "SD",
                "Workspace for handling service desk operations.",
                "/kleenestar/assets/icons/sd.svg",
                "Support"
            );
        }
    }
}
