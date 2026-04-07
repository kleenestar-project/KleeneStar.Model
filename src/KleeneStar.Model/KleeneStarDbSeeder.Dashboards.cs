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
        /// Adds default dashboards to the specified database context if none already exist.
        /// </summary>
        /// <param name="db">
        /// The database context to which the default dashboards will be added. Cannot be null.
        /// </param>
        private static void SeedDashboards(KleeneStarDbContext db)
        {
            void add(string id, string name, string description, string icon, params string[] categories) =>
                db.Dashboards.Add(new Dashboard
                {
                    Id = Guid.Parse(id),
                    Name = name,
                    Description = description,
                    Icon = ImageIcon.FromString(icon),
                    State = TypeDashboardState.Active,
                    Categories = [.. db.Categories.Where(x => categories.Contains(x.Name))],
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });

            add
            (
                "A1B2C3D4-E5F6-7890-ABCD-EF1234567890",
                "Operations Overview",
                "Dashboard for monitoring key operational metrics and service health.",
                "/kleenestar/assets/icons/dashboard-ops.svg",
                "Operations", "Support"
            );

            add
            (
                "B2C3D4E5-F6A7-8901-BCDE-F12345678901",
                "Engineering Insights",
                "Dashboard for tracking development progress, build pipelines, and code quality.",
                "/kleenestar/assets/icons/dashboard-dev.svg",
                "Engineering", "Development"
            );

            add
            (
                "C3D4E5F6-A7B8-9012-CDEF-123456789012",
                "Finance Summary",
                "Dashboard for visualizing budget, costs, and financial KPIs.",
                "/kleenestar/assets/icons/dashboard-fin.svg",
                "Finance"
            );
        }
    }
}
