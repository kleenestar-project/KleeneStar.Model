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
             IEnumerable<DashboardColumn> columns, params string[] categories)
            {
                AttachWidgetsToColumns(columns);

                db.Dashboards.Add(new Dashboard
                {
                    Id = Guid.Parse(id),
                    Name = name,
                    Description = description,
                    Icon = ImageIcon.FromString(icon),
                    State = DashboardState.Active,
                    Categories = [.. db.Categories.Where(x => categories.Contains(x.Name))],
                    Columns = [.. columns],
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });
            }

            add
            (
                "A1B2C3D4-E5F6-7890-ABCD-EF1234567890",
                "Operations Overview",
                "Dashboard for monitoring key operational metrics and service health.",
                "/kleenestar/assets/icons/dashboard-ops.svg",
                [
                    new DashboardColumn
                    {
                        Id = Guid.Parse("6571C4F5-242C-42AD-B113-8315C5E2ABBC"),
                        Name = "Service Health",
                        Size = "33%"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("E718AB7C-D08E-47F1-B752-B6B656D97629"),
                        Name = "Incident Overview",
                        Size = "33%"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("7C0CC089-B28F-441A-97A0-0086487BA790"),
                        Name = "Support Queue",
                        Size = "33%"
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
                        Id = Guid.Parse("8C36D96D-ED6C-40E4-85B8-86493429C7F9"),
                        Name = "Build Pipelines",
                        Size = "33%"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("5CC6F5FF-0284-48C4-AACA-D0D704B4DBB2"),
                        Name = "Code Quality",
                        Size = "33%"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("2A039595-787D-429C-B4AE-126603FBDB55"),
                        Name = "Open Issues",
                        Size = "33%"
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
                        Id = Guid.Parse("795F7E25-D6CE-4309-A6B7-2B31A36163A5"),
                        Name = "Budget Overview",
                        Size = "33%"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("ABF4E0E7-F4E6-4FFE-9713-7F6F4F47B250"),
                        Name = "Cost Centers",
                        Size = "33%"
                    },
                    new DashboardColumn
                    {
                        Id = Guid.Parse("E9D9634B-A2CD-4650-BA07-8428EA64333C"),
                        Name = "KPIs",
                        Size = "33%"
                    }
                ],
                "Finance"
            );
        }

        /// <summary>
        /// Creates a default set of widgets for a given dashboard column.
        /// </summary>
        private static List<Widget> CreateWidgetsForColumn(DashboardColumn column)
        {
            var list = new List<Widget>();

            string key = column.Name.ToLowerInvariant();

            // operations/ support
            if (key.Contains("service health"))
            {
                list.AddRange(
                [
                    new Widget
                    {
                        Id= Guid.Parse("606650E4-66E7-48F2-98C3-25443D719C89"),
                        Name = "Uptime Overview",
                        Wql = "SELECT uptime FROM services",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("53606052-FC02-404C-AB0B-852A4CFD3D99"),
                        Name = "Service Status Map",
                        Wql = "SELECT status FROM services GROUP BY region",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("CF46A475-ACF7-4695-9A3C-34D8FFD5E78A"),
                        Name = "Error Rate Trend",
                        Wql = "SELECT errors FROM metrics TREND 24h",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("A4C2D1A8-DABE-419B-B30C-B306DA4D7F75"),
                        Name = "Alert Summary",
                        Wql = "SELECT alerts FROM incidents WHERE active = true",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("8F14E0DD-553A-495D-8B6E-2B6394EFBB79"),
                        Name = "Latency Distribution",
                        Wql = "SELECT latency FROM metrics HISTOGRAM",
                        ColumnId = column.Id
                    }
                ]);
            }
            else if (key.Contains("incident"))
            {
                list.AddRange(
                [
                    new Widget
                    {
                        Id= Guid.Parse("59200C46-FFC1-47DC-A640-B38F32479443"),
                        Name = "Open Incidents",
                        Wql = "SELECT * FROM incidents WHERE status = 'open'",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("D06F2026-7AB8-4B5A-9CE8-3A414066A076"),
                        Name = "Severity Breakdown",
                        Wql = "SELECT severity, COUNT(*) FROM incidents GROUP BY severity",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("435165CE-573A-4E40-A300-F148E6512D3A"),
                        Name = "MTTR Trend",
                        Wql = "SELECT mttr FROM incident_metrics TREND 30d",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("BE548047-D5AE-4517-AA8E-23FB5A385F73"),
                        Name = "Incident Timeline",
                        Wql = "SELECT timestamp, event FROM incident_events ORDER BY timestamp",
                        ColumnId = column.Id
                    }
                ]);
            }
            else if (key.Contains("support queue"))
            {
                list.AddRange(
                [
                    new Widget
                    {
                        Id= Guid.Parse("D67954B5-96E0-479E-9693-3C522E6264FB"),
                        Name = "Queue Length",
                        Wql = "SELECT COUNT(*) FROM tickets WHERE status = 'open'",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("C3AB6783-EC53-4259-A96D-A7BEA9EC6AAD"),
                        Name = "SLA Breaches",
                        Wql = "SELECT * FROM tickets WHERE sla_breached = true",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("FA36CA21-B87A-44C0-A6A5-19DB64DDE7AF"),
                        Name = "Ticket Aging",
                        Wql = "SELECT age FROM tickets DISTRIBUTION",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("AF498B3A-8009-46F8-A3D7-95189F85E7FC"),
                        Name = "Agent Load",
                        Wql = "SELECT agent, COUNT(*) FROM tickets GROUP BY agent",
                        ColumnId = column.Id
                    }
                ]);
            }

            // engineering/ development
            else if (key.Contains("build pipeline"))
            {
                list.AddRange(
                [
                    new Widget
                    {
                        Id= Guid.Parse("8C091263-3CA8-46B2-823C-41CA7AC9037F"),
                        Name = "Pipeline Status",
                        Wql = "SELECT status FROM pipelines",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("F1294089-97DD-4B39-B130-6A926B41AF0A"),
                        Name = "Build Duration Trend",
                        Wql = "SELECT duration FROM builds TREND 14d",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("601F0671-0CD1-4760-8BB3-18A99829E5A2"),
                        Name = "Failed Builds",
                        Wql = "SELECT * FROM builds WHERE result = 'failed'",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("1DD87EB8-28EC-4962-98AC-ABAEF0957C38"),
                        Name = "Deployment Frequency",
                        Wql = "SELECT deployments FROM devops_metrics TREND 30d",
                        ColumnId = column.Id
                    }
                ]);
            }
            else if (key.Contains("code quality"))
            {
                list.AddRange(
                [
                    new Widget
                    {
                        Id= Guid.Parse("DAB5F044-7939-488A-90DE-E446BAF0DA1E"),
                        Name = "Static Analysis Score",
                        Wql = "SELECT score FROM code_quality",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("7BD702F0-36AA-45F9-AB7D-16F1E68E6B79"),
                        Name = "Hotspots",
                        Wql = "SELECT file, issues FROM code_hotspots",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("30517205-BD0D-478B-99DD-4C49BBA50374"),
                        Name = "Test Coverage",
                        Wql = "SELECT coverage FROM test_metrics",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("E83773BB-DA72-41B5-9B7C-4642DAD61C3A"),
                        Name = "Technical Debt",
                        Wql = "SELECT debt FROM quality_metrics",
                        ColumnId = column.Id
                    }
                ]);
            }
            else if (key.Contains("open issues"))
            {
                list.AddRange(
                [
                    new Widget
                    {
                        Id= Guid.Parse("CB001D54-2127-4E6F-856D-7A58D2C844A7"),
                        Name = "Issue List",
                        Wql = "SELECT * FROM issues WHERE status = 'open'",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("1F98A9B2-E884-4D21-A20C-3D05FE37DFFA"),
                        Name = "Issue Aging",
                        Wql = "SELECT age FROM issues DISTRIBUTION",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("98532183-C741-4EC5-B97E-05BF570DB87B"),
                        Name = "Labels Breakdown",
                        Wql = "SELECT label, COUNT(*) FROM issues GROUP BY label",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("18053247-7D3F-4355-972C-F314B8A9F852"),
                        Name = "Assignee Load",
                        Wql = "SELECT assignee, COUNT(*) FROM issues GROUP BY assignee",
                        ColumnId = column.Id
                    }
                ]);
            }

            // finance
            else if (key.Contains("budget"))
            {
                list.AddRange(
                [
                    new Widget
                    {
                        Id= Guid.Parse("18053247-7D3F-4355-972C-F314B8A9F852"),
                        Name = "Budget vs Actual",
                        Wql = "SELECT budget, actual FROM finance_overview",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("{4F9F1ABC-16DE-4976-9727-35C1D7082116}"),
                        Name = "Quarterly Spend",
                        Wql = "SELECT quarter, spend FROM finance_quarterly",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("AC2F4391-4E72-41F2-B322-DB2090139806"),
                        Name = "Forecast",
                        Wql = "SELECT forecast FROM finance_forecast",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("97F8BA56-75A7-420A-BD04-9628F02AD55E"),
                        Name = "Variance Analysis",
                        Wql = "SELECT variance FROM finance_variance",
                        ColumnId = column.Id
                    }
                ]);
            }
            else if (key.Contains("cost center"))
            {
                list.AddRange(
                [
                    new Widget
                    {
                        Id= Guid.Parse("867905F4-1CBB-41B7-84B2-DCD9FD9E34F8"),
                        Name = "Cost Center Breakdown",
                        Wql = "SELECT center, cost FROM cost_centers",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("359D5069-DB6C-4D74-9E6A-FB95CD5383CB"),
                        Name = "Top Spenders",
                        Wql = "SELECT center, cost FROM cost_centers ORDER BY cost DESC LIMIT 5",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("DA58423D-0A11-40CE-A6CD-B845756B1726"),
                        Name = "Monthly Cost Trend",
                        Wql = "SELECT month, cost FROM cost_trend",
                        ColumnId = column.Id
                    }
                ]);
            }
            else if (key.Contains("kpi"))
            {
                list.AddRange(
                [
                    new Widget
                    {
                        Id= Guid.Parse("62689994-167E-43FD-9764-E394C0C163F1"),
                        Name = "Revenue",
                        Wql = "SELECT revenue FROM finance_kpi",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("1223AFB8-320C-4835-8F38-C2CF4C41F393"),
                        Name = "Profit Margin",
                        Wql = "SELECT margin FROM finance_kpi",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("C77F1660-5EE0-4685-983A-07C29EC4A8D9"),
                        Name = "Cash Flow",
                        Wql = "SELECT cashflow FROM finance_kpi",
                        ColumnId = column.Id
                    },
                    new Widget
                    {
                        Id= Guid.Parse("AB0AD838-ED02-43B7-8174-CC5F45021055"),
                        Name = "Runway",
                        Wql = "SELECT runway FROM finance_kpi",
                        ColumnId = column.Id
                    }
                ]);
            }

            // fallback: always add at least one widget
            if (list.Count == 0)
            {
                list.Add(new Widget
                {
                    Id = Guid.Parse("230A007F-BE2A-4EFE-BA65-DCCAC68B63EA"),
                    Name = $"{column.Name} – Overview",
                    Wql = $"SELECT * FROM {column.Name.Replace(" ", "")}",
                    ColumnId = column.Id
                });
            }

            return list;
        }

        /// <summary>
        /// Attaches widgets to each dashboard column.
        /// </summary>
        private static void AttachWidgetsToColumns(IEnumerable<DashboardColumn> columns)
        {
            foreach (var col in columns)
            {
                col.Widgets = CreateWidgetsForColumn(col);
            }
        }

    }
}
