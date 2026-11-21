using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    internal static partial class ModelHub
    {
        /// <summary>
        /// Brings the graph of a follower's workflow in step with the workflow of its blueprint:
        /// the statuses taking part and the transitions between them.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The graph is written in one context and one save, so a follower's workflow is never
        /// left with half of the blueprint's transitions. The statuses themselves are the
        /// follower's own rows, already synchronized by the caller; <paramref name="statusMap"/>
        /// translates a blueprint status into the follower's copy of it.
        /// </para>
        /// <para>
        /// Only what the blueprint provides is touched. A participation of a managed status the
        /// blueprint dropped is removed, a managed transition the blueprint dropped is removed,
        /// everything else of the blueprint is added or updated; a status or transition the
        /// follower added locally stays as it is. The screen form of a transition is a form of
        /// the follower's, which the blueprint knows nothing of, so it is left alone.
        /// </para>
        /// </remarks>
        /// <param name="followerWorkflowId">The follower's workflow.</param>
        /// <param name="blueprint">The blueprint's workflow, with its structure.</param>
        /// <param name="statusMap">The follower's status per blueprint status.</param>
        /// <param name="managedStatusIds">The follower's statuses that are managed copies.</param>
        public static void SynchronizeWorkflowGraph
        (
            Guid followerWorkflowId,
            Workflow blueprint,
            IReadOnlyDictionary<Guid, Guid> statusMap,
            IReadOnlySet<Guid> managedStatusIds
        )
        {
            ArgumentNullException.ThrowIfNull(blueprint);
            ArgumentNullException.ThrowIfNull(statusMap);

            using var db = CreateDbContext();

            var workflow = db.Workflows
                .Include(x => x.WorkflowStatuses)
                .Include(x => x.Transitions)
                .FirstOrDefault(x => x.Id == followerWorkflowId);

            if (workflow is null)
            {
                return;
            }

            workflow.WorkflowStatuses ??= [];
            workflow.Transitions ??= [];

            // participations: the blueprint's, translated; a managed status the blueprint no
            // longer has leaves the graph, a local one stays
            var wanted = new Dictionary<Guid, WorkflowStatus>();

            foreach (var participation in blueprint.WorkflowStatuses ?? [])
            {
                if (statusMap.TryGetValue(participation.StatusId, out var statusId))
                {
                    wanted[statusId] = participation;
                }
            }

            foreach (var participation in workflow.WorkflowStatuses.ToList())
            {
                if (wanted.TryGetValue(participation.StatusId, out var source))
                {
                    participation.X = source.X;
                    participation.Y = source.Y;
                    participation.IsStart = source.IsStart;
                    participation.IsEnd = source.IsEnd;
                    wanted.Remove(participation.StatusId);
                }
                else if (managedStatusIds.Contains(participation.StatusId))
                {
                    workflow.WorkflowStatuses.Remove(participation);
                }
            }

            foreach (var (statusId, source) in wanted)
            {
                workflow.WorkflowStatuses.Add(new WorkflowStatus
                {
                    WorkflowId = workflow.Id,
                    StatusId = statusId,
                    X = source.X,
                    Y = source.Y,
                    IsStart = source.IsStart,
                    IsEnd = source.IsEnd
                });
            }

            // transitions: matched by the blueprint transition they copy
            var bySource = workflow.Transitions
                .Where(x => x.BlueprintSourceId.HasValue)
                .GroupBy(x => x.BlueprintSourceId.Value)
                .ToDictionary(x => x.Key, x => x.First());
            var retained = new HashSet<Guid>();
            var now = DateTime.UtcNow;

            foreach (var transition in blueprint.Transitions ?? [])
            {
                if (!statusMap.TryGetValue(transition.SourceId, out var sourceId)
                    || !statusMap.TryGetValue(transition.TargetId, out var targetId))
                {
                    continue;
                }

                if (!bySource.TryGetValue(transition.Id, out var copy))
                {
                    copy = new Transition
                    {
                        Id = Guid.NewGuid(),
                        WorkflowId = workflow.Id,
                        BlueprintSourceId = transition.Id,
                        Created = now
                    };

                    db.Transitions.Add(copy);
                }

                copy.Name = transition.Name;
                copy.Description = transition.Description;
                copy.State = transition.State;
                copy.Color = transition.Color;
                copy.DashArray = transition.DashArray;
                copy.Waypoints = transition.Waypoints is null ? null : [.. transition.Waypoints.Select(x => new TransitionWaypoint { X = x.X, Y = x.Y })];
                copy.SourceId = sourceId;
                copy.TargetId = targetId;
                copy.GuardExpression = transition.GuardExpression;
                copy.ValidatorExpression = transition.ValidatorExpression;
                copy.PostFunctionKeys = transition.PostFunctionKeys is null ? null : [.. transition.PostFunctionKeys];
                copy.Updated = now;

                retained.Add(copy.Id);
            }

            db.Transitions.RemoveRange(bySource.Values.Where(x => !retained.Contains(x.Id)));

            workflow.Updated = now;
            db.SaveChanges();
        }
    }
}
