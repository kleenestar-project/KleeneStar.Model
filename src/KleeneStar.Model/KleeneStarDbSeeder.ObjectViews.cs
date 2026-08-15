using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;

namespace KleeneStar.Model
{
    /// <summary>
    /// Seeds the default <see cref="ObjectView"/> tabs for each workspace.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Per-workspace deterministic ids for the default tabs, in the original order
        /// Table, List, Dashboard, Kanban, ScrumSprint, ScrumBacklog. The seventh id
        /// (index 6) belongs to the issues view, which was appended after the original six
        /// and leads the display order. Index 5 — the former backlog tab — is no longer
        /// seeded since the sprint and the backlog were merged into one scrum view, but the
        /// id stays listed: it is well-known and still present in existing databases.
        /// </summary>
        private static readonly Dictionary<Guid, Guid[]> _objectViewIds = new()
        {
            // CMDB
            [Guid.Parse("D651799A-690E-4CFF-AE0C-D3341CA3BBB4")] =
            [
                Guid.Parse("0A1B2C30-CDB0-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-CDB0-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-CDB0-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-CDB0-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-CDB0-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-CDB0-0006-0000-000000000006"),
                Guid.Parse("0A1B2C30-CDB0-0007-0000-000000000007")
            ],

            // DEV
            [Guid.Parse("660E9B11-2D54-4A36-84F9-F3BF5C78B748")] =
            [
                Guid.Parse("0A1B2C30-DDE0-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-DDE0-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-DDE0-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-DDE0-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-DDE0-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-DDE0-0006-0000-000000000006"),
                Guid.Parse("0A1B2C30-DDE0-0007-0000-000000000007")
            ],

            // FIN
            [Guid.Parse("9994445E-FDBE-42E2-A3A0-65DF13CB453B")] =
            [
                Guid.Parse("0A1B2C30-DF10-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-DF10-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-DF10-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-DF10-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-DF10-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-DF10-0006-0000-000000000006"),
                Guid.Parse("0A1B2C30-DF10-0007-0000-000000000007")
            ],

            // HR
            [Guid.Parse("8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F")] =
            [
                Guid.Parse("0A1B2C30-DD40-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-DD40-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-DD40-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-DD40-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-DD40-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-DD40-0006-0000-000000000006"),
                Guid.Parse("0A1B2C30-DD40-0007-0000-000000000007")
            ],

            // PM
            [Guid.Parse("25F04599-C02A-4BEF-80B1-5957FBBB1FED")] =
            [
                Guid.Parse("0A1B2C30-D0A0-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-D0A0-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-D0A0-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-D0A0-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-D0A0-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-D0A0-0006-0000-000000000006"),
                Guid.Parse("0A1B2C30-D0A0-0007-0000-000000000007")
            ],

            // PROC
            [Guid.Parse("D35FDCD6-5B11-4043-98D6-215DF414D99C")] =
            [
                Guid.Parse("0A1B2C30-AC00-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-AC00-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-AC00-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-AC00-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-AC00-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-AC00-0006-0000-000000000006"),
                Guid.Parse("0A1B2C30-AC00-0007-0000-000000000007")
            ],

            // SD
            [Guid.Parse("F027A791-4219-4B1D-BA7C-2E7757091AAA")] =
            [
                Guid.Parse("0A1B2C30-DD50-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-DD50-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-DD50-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-DD50-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-DD50-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-DD50-0006-0000-000000000006"),
                Guid.Parse("0A1B2C30-DD50-0007-0000-000000000007")
            ]
        };

        /// <summary>
        /// Per-workspace deterministic ids for the five default asset tabs, in display
        /// order Assets, Table, List, Dashboard, Kanban. The asset overview offers the
        /// same layouts as the issue overview except the two Scrum boards, which do not
        /// apply to configuration items. The ids mirror the issue ids of the same
        /// workspace with a distinct leading block so the two tab sets never collide.
        /// </summary>
        private static readonly Dictionary<Guid, Guid[]> _assetObjectViewIds = new()
        {
            // CMDB
            [Guid.Parse("D651799A-690E-4CFF-AE0C-D3341CA3BBB4")] =
            [
                Guid.Parse("0A1B2C31-CDB0-0001-0000-000000000001"),
                Guid.Parse("0A1B2C31-CDB0-0002-0000-000000000002"),
                Guid.Parse("0A1B2C31-CDB0-0003-0000-000000000003"),
                Guid.Parse("0A1B2C31-CDB0-0004-0000-000000000004"),
                Guid.Parse("0A1B2C31-CDB0-0005-0000-000000000005")
            ],

            // DEV
            [Guid.Parse("660E9B11-2D54-4A36-84F9-F3BF5C78B748")] =
            [
                Guid.Parse("0A1B2C31-DDE0-0001-0000-000000000001"),
                Guid.Parse("0A1B2C31-DDE0-0002-0000-000000000002"),
                Guid.Parse("0A1B2C31-DDE0-0003-0000-000000000003"),
                Guid.Parse("0A1B2C31-DDE0-0004-0000-000000000004"),
                Guid.Parse("0A1B2C31-DDE0-0005-0000-000000000005")
            ],

            // FIN
            [Guid.Parse("9994445E-FDBE-42E2-A3A0-65DF13CB453B")] =
            [
                Guid.Parse("0A1B2C31-DF10-0001-0000-000000000001"),
                Guid.Parse("0A1B2C31-DF10-0002-0000-000000000002"),
                Guid.Parse("0A1B2C31-DF10-0003-0000-000000000003"),
                Guid.Parse("0A1B2C31-DF10-0004-0000-000000000004"),
                Guid.Parse("0A1B2C31-DF10-0005-0000-000000000005")
            ],

            // HR
            [Guid.Parse("8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F")] =
            [
                Guid.Parse("0A1B2C31-DD40-0001-0000-000000000001"),
                Guid.Parse("0A1B2C31-DD40-0002-0000-000000000002"),
                Guid.Parse("0A1B2C31-DD40-0003-0000-000000000003"),
                Guid.Parse("0A1B2C31-DD40-0004-0000-000000000004"),
                Guid.Parse("0A1B2C31-DD40-0005-0000-000000000005")
            ],

            // PM
            [Guid.Parse("25F04599-C02A-4BEF-80B1-5957FBBB1FED")] =
            [
                Guid.Parse("0A1B2C31-D0A0-0001-0000-000000000001"),
                Guid.Parse("0A1B2C31-D0A0-0002-0000-000000000002"),
                Guid.Parse("0A1B2C31-D0A0-0003-0000-000000000003"),
                Guid.Parse("0A1B2C31-D0A0-0004-0000-000000000004"),
                Guid.Parse("0A1B2C31-D0A0-0005-0000-000000000005")
            ],

            // PROC
            [Guid.Parse("D35FDCD6-5B11-4043-98D6-215DF414D99C")] =
            [
                Guid.Parse("0A1B2C31-AC00-0001-0000-000000000001"),
                Guid.Parse("0A1B2C31-AC00-0002-0000-000000000002"),
                Guid.Parse("0A1B2C31-AC00-0003-0000-000000000003"),
                Guid.Parse("0A1B2C31-AC00-0004-0000-000000000004"),
                Guid.Parse("0A1B2C31-AC00-0005-0000-000000000005")
            ],

            // SD
            [Guid.Parse("F027A791-4219-4B1D-BA7C-2E7757091AAA")] =
            [
                Guid.Parse("0A1B2C31-DD50-0001-0000-000000000001"),
                Guid.Parse("0A1B2C31-DD50-0002-0000-000000000002"),
                Guid.Parse("0A1B2C31-DD50-0003-0000-000000000003"),
                Guid.Parse("0A1B2C31-DD50-0004-0000-000000000004"),
                Guid.Parse("0A1B2C31-DD50-0005-0000-000000000005")
            ]
        };

        /// <summary>
        /// Creates the default <see cref="ObjectView"/> tabs for every seeded workspace:
        /// the issue tab set (Issues, Table, List, Dashboard, Kanban, Scrum) and the asset
        /// tab set (Assets, Table, List, Dashboard, Kanban).
        /// Each kind keeps its own tab set; the curated view of each kind leads because it
        /// is the default entry of that kind's overview page hosting the tab control.
        /// </summary>
        /// <param name="db">The database context. Cannot be null.</param>
        private static void SeedObjectViews(KleeneStarDbContext db)
        {
            void add(Guid id, Guid workspaceId, string kind, string name, string description,
                ObjectViewType type, int order)
            {
                db.ObjectViews.Add(new ObjectView
                {
                    Id = id,
                    Name = name,
                    Kind = kind,
                    Description = description,
                    ViewType = type,
                    Order = order,
                    State = ObjectViewState.Active,
                    WorkspaceId = workspaceId,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });
            }

            foreach (var workspace in db.Workspaces)
            {
                if (_objectViewIds.TryGetValue(workspace.Id, out var ids))
                {
                    add(ids[6], workspace.Id, ObjectKind.Issue, "Issues",
                        $"Most recently updated issues of {workspace.Name} with personal filters.",
                        ObjectViewType.Issues, 0);

                    add(ids[0], workspace.Id, ObjectKind.Issue, "Table",
                        $"Tabular view of {workspace.Name} issues.",
                        ObjectViewType.Table, 1);

                    add(ids[1], workspace.Id, ObjectKind.Issue, "List",
                        $"Compact list view of {workspace.Name} issues.",
                        ObjectViewType.List, 2);

                    add(ids[2], workspace.Id, ObjectKind.Issue, "Dashboard",
                        $"Aggregated dashboard for {workspace.Name}.",
                        ObjectViewType.Dashboard, 3);

                    add(ids[3], workspace.Id, ObjectKind.Issue, "Kanban",
                        $"Kanban board of {workspace.Name} issues grouped by status.",
                        ObjectViewType.Kanban, 4);

                    // the sprint board and the backlog share one view now, so only one tab
                    // is seeded. ids[5] — the former backlog tab — is deliberately left
                    // unused: the ids are well-known and reusing it for something else
                    // would collide with the row an existing database still carries.
                    add(ids[4], workspace.Id, ObjectKind.Issue, "Scrum",
                        $"Active Scrum sprint and product backlog for {workspace.Name}.",
                        ObjectViewType.ScrumSprint, 5);
                }

                if (_assetObjectViewIds.TryGetValue(workspace.Id, out var assetIds))
                {
                    add(assetIds[0], workspace.Id, ObjectKind.Asset, "Assets",
                        $"Most recently updated assets of {workspace.Name} with personal filters.",
                        ObjectViewType.Assets, 0);

                    add(assetIds[1], workspace.Id, ObjectKind.Asset, "Table",
                        $"Tabular view of {workspace.Name} assets.",
                        ObjectViewType.Table, 1);

                    add(assetIds[2], workspace.Id, ObjectKind.Asset, "List",
                        $"Compact list view of {workspace.Name} assets.",
                        ObjectViewType.List, 2);

                    add(assetIds[3], workspace.Id, ObjectKind.Asset, "Dashboard",
                        $"Aggregated dashboard for {workspace.Name} assets.",
                        ObjectViewType.Dashboard, 3);

                    add(assetIds[4], workspace.Id, ObjectKind.Asset, "Kanban",
                        $"Kanban board of {workspace.Name} assets grouped by status.",
                        ObjectViewType.Kanban, 4);
                }
            }
        }

    }
}
