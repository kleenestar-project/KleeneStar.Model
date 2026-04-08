using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;
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
        /// Adds default dashboards to the specified database context if none already exist.
        /// </summary>
        /// <param name="db">
        /// The database context to which the default dashboards will be added. Cannot be null.
        /// </param>
        private static void SeedDashboards(KleeneStarDbContext db)
        {
            void add(string id, string name, string description, string icon,
                     IEnumerable<DashboardColumn> columns, params string[] categories) =>
                db.Dashboards.Add(new Dashboard
                {
                    Id = Guid.Parse(id),
                    Name = name,
                    Description = description,
                    Icon = ImageIcon.FromString(icon),
                    State = TypeDashboardState.Active,
                    Categories = [.. db.Categories.Where(x => categories.Contains(x.Name))],
                    Columns = [.. columns],
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });

            add
            (
                "A1B2C3D4-E5F6-7890-ABCD-EF1234567890",
                "Operations Overview",
                "Dashboard for monitoring key operational metrics and service health.",
                "/kleenestar/assets/icons/dashboard-ops.svg",
                [
                    new DashboardColumn
                    {
                        Id = Guid.Parse("A1000001-0000-0000-0000-000000000001"),
                        Name = "Service Health",
                        Size = "large"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("A1000001-0000-0000-0000-000000000002"),
                        Name = "Incident Overview",
                        Size = "medium"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("A1000001-0000-0000-0000-000000000003"),
                        Name = "Support Queue",
                        Size = "small"
                    }
                ],
                "Operations", "Support"
            );

            add
            (
                "B2C3D4E5-F6A7-8901-BCDE-F12345678901",
                "Engineering Insights",
                "Dashboard for tracking development progress, build pipelines, and code quality.",
                "/kleenestar/assets/icons/dashboard-dev.svg",
                [
                    new DashboardColumn
                    {
                        Id = Guid.Parse("B2000002-0000-0000-0000-000000000001"),
                        Name = "Build Pipelines",
                        Size = "large"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("B2000002-0000-0000-0000-000000000002"),
                        Name = "Code Quality",
                        Size = "medium"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("B2000002-0000-0000-0000-000000000003"),
                        Name = "Open Issues",
                        Size = "medium"
                    }
                ],
                "Engineering", "Development"
            );

            add
            (
                "C3D4E5F6-A7B8-9012-CDEF-123456789012",
                "Finance Summary",
                "Dashboard for visualizing budget, costs, and financial KPIs.",
                "/kleenestar/assets/icons/dashboard-fin.svg",
                [
                    new DashboardColumn
                    {
                        Id = Guid.Parse("C3000003-0000-0000-0000-000000000001"),
                        Name = "Budget Overview",
                        Size = "large"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("C3000003-0000-0000-0000-000000000002"),
                        Name = "Cost Centers",
                        Size = "medium"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("C3000003-0000-0000-0000-000000000003"),
                        Name = "KPIs",
                        Size = "small"
                    }
                ],
                "Finance"
            );
        }
    }
}
