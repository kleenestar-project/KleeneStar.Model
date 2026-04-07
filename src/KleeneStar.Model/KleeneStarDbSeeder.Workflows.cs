using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
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
        /// Adds a predefined set of workflows, states, and transitions to the specified database context.
        /// </summary>
        /// <remarks>
        /// This method seeds a default lifecycle workflow for each class in the database.
        /// It does not save changes to the database; callers must call SaveChanges on the 
        /// context to persist the additions.
        /// </remarks>
        /// <param name="db">The database context to which the workflow entities will be added. Cannot be null.</param>
        private static void SeedWorkflows(KleeneStarDbContext db)
        {
            // retrieve all classes without tracking to improve performance
            var classes = db.Classes
                .AsNoTracking()
                .ToList();

            foreach (var cls in classes)
            {
                // create the default workflow for the current class
                var workflow = new Workflow
                {
                    Id = Guid.NewGuid(),
                    Name = "Standard Lifecycle",
                    Description = "The default lifecycle workflow for this class.",
                    State = TypeWorkspaceState.Active,
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/workflow.svg"),
                    ClassId = cls.Id,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                db.Workflows.Add(workflow);

                // define standard states for the workflow
                var stateNew = new WorkflowState
                {
                    Id = Guid.NewGuid(),
                    Name = "New",
                    Description = "Initial state.",
                    State = TypeWorkspaceState.Active,
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/state-new.svg"),
                    ClassId = cls.Id,
                    WorkflowId = workflow.Id,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                var stateInProgress = new WorkflowState
                {
                    Id = Guid.NewGuid(),
                    Name = "In Progress",
                    Description = "Work is ongoing.",
                    State = TypeWorkspaceState.Active,
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/state-progress.svg"),
                    ClassId = cls.Id,
                    WorkflowId = workflow.Id,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                var stateResolved = new WorkflowState
                {
                    Id = Guid.NewGuid(),
                    Name = "Resolved",
                    Description = "Work has been completed.",
                    State = TypeWorkspaceState.Active,
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/state-resolved.svg"),
                    ClassId = cls.Id,
                    WorkflowId = workflow.Id,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                var stateClosed = new WorkflowState
                {
                    Id = Guid.NewGuid(),
                    Name = "Closed",
                    Description = "The item is closed.",
                    State = TypeWorkspaceState.Active,
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/state-closed.svg"),
                    ClassId = cls.Id,
                    WorkflowId = workflow.Id,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                // add the states to the database context
                db.WorkflowStates.AddRange(stateNew, stateInProgress, stateResolved, stateClosed);

                // define allowed transitions between the states
                var transitions = new List<WorkflowTransition>
                {
                    new WorkflowTransition
                    {
                        Id = Guid.NewGuid(),
                        Name = "Start Work",
                        Description = "Move from new to in progress.",
                        State = TypeWorkspaceState.Active,
                        WorkflowId = workflow.Id,
                        SourceId = stateNew.Id,
                        TargetId = stateInProgress.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    },
                    new WorkflowTransition
                    {
                        Id = Guid.NewGuid(),
                        Name = "Resolve",
                        Description = "Move from in progress to resolved.",
                        State = TypeWorkspaceState.Active,
                        WorkflowId = workflow.Id,
                        SourceId = stateInProgress.Id,
                        TargetId = stateResolved.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    },
                    new WorkflowTransition
                    {
                        Id = Guid.NewGuid(),
                        Name = "Close",
                        Description = "Move from resolved to closed.",
                        State = TypeWorkspaceState.Active,
                        WorkflowId = workflow.Id,
                        SourceId = stateResolved.Id,
                        TargetId = stateClosed.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    },
                    new WorkflowTransition
                    {
                        Id = Guid.NewGuid(),
                        Name = "Cancel",
                        Description = "Close directly from new.",
                        State = TypeWorkspaceState.Active,
                        WorkflowId = workflow.Id,
                        SourceId = stateNew.Id,
                        TargetId = stateClosed.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    }
                };

                // add the transitions to the database context
                db.WorkflowTransitions.AddRange(transitions);
            }
        }
    }
}