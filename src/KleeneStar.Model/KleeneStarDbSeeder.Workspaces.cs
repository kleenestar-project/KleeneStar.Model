using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Adds a default workspace to the specified database context if it 
        /// does not already exist.
        /// </summary>
        /// <param name="db">
        /// The database context to which the default workspace will be added. Cannot be null.
        /// </param>
        private static void SeedWorkspaces(KleeneStarDbContext db)
        {
            void add(string id, string name, string key, string description, string icon, WorkspaceState state, params string[] categories) =>
                db.Workspaces.Add(new Workspace
                {
                    Id = Guid.Parse(id),
                    Name = name,
                    Key = key,
                    Description = description,
                    State = state,
                    Icon = ImageIcon.FromString(icon),
                    Categories = [.. db.Categories.Where(x => categories.Contains(x.Name))],
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });

            add
            (
                "D651799A-690E-4CFF-AE0C-D3341CA3BBB4",
                "Configuration Database",
                "CMDB",
                "Workspace for managing configuration items and asset relationships.",
                "/kleenestar/assets/icons/cmdb.svg",
                WorkspaceState.Active,
                "Infrastructure", "Compliance"
            );

            add
            (
                "660E9B11-2D54-4A36-84F9-F3BF5C78B748",
                "Software Development",
                "DEV",
                "Workspace for managing source code, repositories, development tasks, and release pipelines.",
                "/kleenestar/assets/icons/dev.svg",
                WorkspaceState.Active,
                "Engineering"
            );

            add
            (
                "9994445E-FDBE-42E2-A3A0-65DF13CB453B",
                "Finance and Controlling",
                "FIN",
                "Workspace for managing budgets, invoices, and financial approvals.",
                "/kleenestar/assets/icons/fin.svg",
                WorkspaceState.Active,
                "Finance"
            );

            add
            (
                "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F",
                "Human Resources",
                "HR",
                "Workspace for managing employee records, onboarding, and organizational structure.",
                "/kleenestar/assets/icons/hr.svg",
                WorkspaceState.Active,
                "HumanResources"
            );

            add
            (
                "25F04599-C02A-4BEF-80B1-5957FBBB1FED",
                "Product Management",
                "PM",
                "Workspace for planning features, tracking releases, and coordinating product strategy.",
                "/kleenestar/assets/icons/pm.svg",
                WorkspaceState.Archived,
                "Development"
            );

            add
            (
                "D35FDCD6-5B11-4043-98D6-215DF414D99C",
                "Procurement",
                "PROC",
                "Workspace for managing purchase orders, supplier relations, and procurement workflows.",
                "/kleenestar/assets/icons/proc.svg",
                WorkspaceState.Archived,
                "Operations"
            );

            add
            (
                "F027A791-4219-4B1D-BA7C-2E7757091AAA",
                "IT Service Desk",
                "SD",
                "Workspace for handling service desk operations.",
                "/kleenestar/assets/icons/sd.svg",
                WorkspaceState.Active,
                "Support"
            );
        }
    }
}
