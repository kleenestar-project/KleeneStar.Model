using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Seeds demo <see cref="Sprint"/> iterations for every workspace and commits a
    /// share of each workspace's objects to them, so the Scrum backlog and sprint
    /// views are populated from the very first launch.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Story-point scale used to estimate the seeded objects, applied round-robin.
        /// </summary>
        private static readonly int[] _storyPointScale = [1, 2, 3, 5, 8, 13];

        /// <summary>
        /// Creates one completed, one active and one planned sprint per workspace and
        /// distributes the workspace's objects across the completed sprint, the active
        /// sprint and the product backlog. Ranks are dense and 1-based per group;
        /// most objects receive a story-point estimate, every fifth is left
        /// unestimated so the UI also shows the empty state.
        /// </summary>
        /// <param name="db">The database context. Cannot be null.</param>
        private static void SeedSprints(KleeneStarDbContext db)
        {
            var today = DateTime.UtcNow.Date;

            foreach (var workspace in db.Workspaces.AsNoTracking().ToList())
            {
                var completed = new Sprint
                {
                    Id = Guid.NewGuid(),
                    Name = "Sprint 1",
                    Goal = $"Foundation work for {workspace.Name}.",
                    State = SprintState.Completed,
                    Start = today.AddDays(-28),
                    End = today.AddDays(-14),
                    Capacity = 40,
                    WorkspaceId = workspace.Id,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                var active = new Sprint
                {
                    Id = Guid.NewGuid(),
                    Name = "Sprint 2",
                    Goal = $"Deliver the current iteration of {workspace.Name}.",
                    State = SprintState.Active,
                    Start = today.AddDays(-7),
                    End = today.AddDays(7),
                    Capacity = 40,
                    WorkspaceId = workspace.Id,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                var planned = new Sprint
                {
                    Id = Guid.NewGuid(),
                    Name = "Sprint 3",
                    Goal = $"Prepare the next increment of {workspace.Name}.",
                    State = SprintState.Planned,
                    Start = today.AddDays(7),
                    End = today.AddDays(21),
                    Capacity = 40,
                    WorkspaceId = workspace.Id,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                db.Sprints.Add(completed);
                db.Sprints.Add(active);
                db.Sprints.Add(planned);

                // deterministic distribution: the first fifth of the objects went into
                // the completed sprint, the next third runs in the active sprint, the
                // rest waits in the product backlog
                var objects = db.Objects
                    .Where(x => x.WorkspaceId == workspace.Id)
                    .OrderBy(x => x.Key)
                    .ToList();

                var completedCount = objects.Count / 5;
                var activeCount = objects.Count / 3;

                var completedRank = 0;
                var activeRank = 0;
                var backlogRank = 0;

                for (var i = 0; i < objects.Count; i++)
                {
                    var entity = objects[i];

                    if (i < completedCount)
                    {
                        entity.SprintId = completed.Id;
                        entity.SprintRank = ++completedRank;
                    }
                    else if (i < completedCount + activeCount)
                    {
                        entity.SprintId = active.Id;
                        entity.SprintRank = ++activeRank;
                    }
                    else
                    {
                        entity.SprintId = null;
                        entity.SprintRank = ++backlogRank;
                    }

                    entity.StoryPoints = i % 5 == 4
                        ? null
                        : _storyPointScale[i % _storyPointScale.Length];
                }
            }
        }
    }
}
