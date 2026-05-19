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
        /// Per-workspace deterministic ids for the six default tabs, in display order
        /// Table, List, Dashboard, Kanban, ScrumSprint, ScrumBacklog.
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
                Guid.Parse("0A1B2C30-CDB0-0006-0000-000000000006")
            ],

            // DEV
            [Guid.Parse("660E9B11-2D54-4A36-84F9-F3BF5C78B748")] =
            [
                Guid.Parse("0A1B2C30-DDE0-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-DDE0-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-DDE0-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-DDE0-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-DDE0-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-DDE0-0006-0000-000000000006")
            ],

            // FIN
            [Guid.Parse("9994445E-FDBE-42E2-A3A0-65DF13CB453B")] =
            [
                Guid.Parse("0A1B2C30-DF10-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-DF10-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-DF10-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-DF10-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-DF10-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-DF10-0006-0000-000000000006")
            ],

            // HR
            [Guid.Parse("8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F")] =
            [
                Guid.Parse("0A1B2C30-DD40-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-DD40-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-DD40-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-DD40-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-DD40-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-DD40-0006-0000-000000000006")
            ],

            // PM
            [Guid.Parse("25F04599-C02A-4BEF-80B1-5957FBBB1FED")] =
            [
                Guid.Parse("0A1B2C30-D0A0-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-D0A0-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-D0A0-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-D0A0-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-D0A0-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-D0A0-0006-0000-000000000006")
            ],

            // PROC
            [Guid.Parse("D35FDCD6-5B11-4043-98D6-215DF414D99C")] =
            [
                Guid.Parse("0A1B2C30-AC00-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-AC00-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-AC00-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-AC00-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-AC00-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-AC00-0006-0000-000000000006")
            ],

            // SD
            [Guid.Parse("F027A791-4219-4B1D-BA7C-2E7757091AAA")] =
            [
                Guid.Parse("0A1B2C30-DD50-0001-0000-000000000001"),
                Guid.Parse("0A1B2C30-DD50-0002-0000-000000000002"),
                Guid.Parse("0A1B2C30-DD50-0003-0000-000000000003"),
                Guid.Parse("0A1B2C30-DD50-0004-0000-000000000004"),
                Guid.Parse("0A1B2C30-DD50-0005-0000-000000000005"),
                Guid.Parse("0A1B2C30-DD50-0006-0000-000000000006")
            ]
        };

        /// <summary>
        /// Creates one <see cref="ObjectView"/> of each <see cref="ObjectViewType"/> for every
        /// seeded workspace, in display order Table, List, Dashboard, Kanban, ScrumSprint,
        /// ScrumBacklog.
        /// </summary>
        /// <param name="db">The database context. Cannot be null.</param>
        private static void SeedObjectViews(KleeneStarDbContext db)
        {
            void add(Guid id, Guid workspaceId, string name, string description,
                ObjectViewType type, int order)
            {
                db.ObjectViews.Add(new ObjectView
                {
                    Id = id,
                    Name = name,
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
                if (!_objectViewIds.TryGetValue(workspace.Id, out var ids))
                {
                    continue;
                }

                add(ids[0], workspace.Id, "Table",
                    $"Tabular view of {workspace.Name} objects.",
                    ObjectViewType.Table, 0);

                add(ids[1], workspace.Id, "List",
                    $"Compact list view of {workspace.Name} objects.",
                    ObjectViewType.List, 1);

                add(ids[2], workspace.Id, "Dashboard",
                    $"Aggregated dashboard for {workspace.Name}.",
                    ObjectViewType.Dashboard, 2);

                add(ids[3], workspace.Id, "Kanban",
                    $"Kanban board of {workspace.Name} objects grouped by status.",
                    ObjectViewType.Kanban, 3);

                add(ids[4], workspace.Id, "Sprint",
                    $"Active Scrum sprint board for {workspace.Name}.",
                    ObjectViewType.ScrumSprint, 4);

                add(ids[5], workspace.Id, "Backlog",
                    $"Scrum product backlog for {workspace.Name}.",
                    ObjectViewType.ScrumBacklog, 5);
            }
        }
    }
}
