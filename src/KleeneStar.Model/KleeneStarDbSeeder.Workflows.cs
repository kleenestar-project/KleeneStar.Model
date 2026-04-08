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
        /// Adds the status category to the database if it does not already exist.
        /// </summary>
        /// <param name="db">The database context used to add the status category. Cannot be null.</param>
        private static void SeedStatusCategories(KleeneStarDbContext db)
        {
            db.StatusCategories.Add
            (
                new StatusCategory
                {
                    Id = Guid.Parse("74C668D5-BAED-4DDD-8176-B54C396EA250"),
                    Name = "ToDo",
                    Description = "Items that are planned but not yet started.",
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/status-category-todo.svg"),
                    Color = "#FF5733",
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                }
            );

            db.StatusCategories.Add
            (
                new StatusCategory
                {
                    Id = Guid.Parse("66948427-5501-4E76-92BC-300929DA4A8F"),
                    Name = "InProgress",
                    Description = "Items that are currently being worked on.",
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/status-category-inprogress.svg"),
                    Color = "#33C1FF",
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                }
            );

            db.StatusCategories.Add
            (
                new StatusCategory
                {
                    Id = Guid.Parse("922133B8-A250-495E-87F0-3485290A77CC"),
                    Name = "Waiting",
                    Description = "Items that are blocked or waiting for external input.",
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/status-category-waiting.svg"),
                    Color = "#FFC300",
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                }
            );

            db.StatusCategories.Add
            (
                new StatusCategory
                {
                    Id = Guid.Parse("7407A6D6-205F-4BDD-8BF4-B3EBE4CBD0B9"),
                    Name = "Done",
                    Description = "Items that have been completed successfully.",
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/status-category-done.svg"),
                    Color = "#28A745",
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                }
            );
        }

        /// <summary>
        /// Initializes default status values for all classes in the specified database context.
        /// </summary>
        /// <remarks>
        /// This method adds predefined status values for each existing class, provided they do not already exist.
        /// It is intended for initializing workflow scenarios and is typically invoked during database migration
        /// or as part of the seeding process.
        /// </remarks>
        /// <param name="db">
        /// The database context in which the status values are created. Must not be null.
        /// </param>
        private static void SeedStatuses(KleeneStarDbContext db)
        {
            // retrieve all classes without tracking to improve performance
            var classes = db.Classes
                .AsNoTracking()
                .ToList();

            foreach (var cls in classes)
            {
                // define standard states for the workflow
                db.Statuses.Add
                (
                    new Status()
                    {
                        Id = Guid.NewGuid(),
                        Name = "New",
                        Description = "Initial state.",
                        State = StatusState.Active,
                        Icon = ImageIcon.FromString("/kleenestar/assets/icons/state-new.svg"),
                        CategoryId = Guid.Parse("74C668D5-BAED-4DDD-8176-B54C396EA250"), // ToDo category
                        ClassId = cls.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    }
                );

                db.Statuses.Add
                (
                    new Status
                    {
                        Id = Guid.NewGuid(),
                        Name = "In Progress",
                        Description = "Work is ongoing.",
                        State = StatusState.Active,
                        Icon = ImageIcon.FromString("/kleenestar/assets/icons/state-progress.svg"),
                        CategoryId = Guid.Parse("66948427-5501-4E76-92BC-300929DA4A8F"), // InProgress category
                        ClassId = cls.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    }
                );

                db.Statuses.Add
                (
                    new Status
                    {
                        Id = Guid.NewGuid(),
                        Name = "Resolved",
                        Description = "Work has been completed.",
                        State = StatusState.Active,
                        Icon = ImageIcon.FromString("/kleenestar/assets/icons/state-resolved.svg"),
                        CategoryId = Guid.Parse("7407A6D6-205F-4BDD-8BF4-B3EBE4CBD0B9"), // Done category
                        ClassId = cls.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    }
                );

                db.Statuses.Add
                (
                    new Status
                    {
                        Id = Guid.NewGuid(),
                        Name = "Closed",
                        Description = "The item is closed.",
                        State = StatusState.Active,
                        Icon = ImageIcon.FromString("/kleenestar/assets/icons/state-closed.svg"),
                        CategoryId = Guid.Parse("7407A6D6-205F-4BDD-8BF4-B3EBE4CBD0B9"), // Done category
                        ClassId = cls.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    }
                );
            }
        }

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
                    State = WorkflowState.Active,
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/workflow.svg"),
                    ClassId = cls.Id,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                db.Workflows.Add(workflow);

                var stateNew = db.Statuses.FirstOrDefault(s => s.ClassId == cls.Id && s.Name == "New");
                var stateInProgress = db.Statuses.FirstOrDefault(s => s.ClassId == cls.Id && s.Name == "In Progress");
                var stateResolved = db.Statuses.FirstOrDefault(s => s.ClassId == cls.Id && s.Name == "Resolved");
                var stateClosed = db.Statuses.FirstOrDefault(s => s.ClassId == cls.Id && s.Name == "Closed");

                // add the states to the database context
                workflow.Statuses = [stateNew, stateInProgress, stateResolved, stateClosed];

                // define allowed transitions between the states
                var transitions = new List<Transition>
                {
                    new Transition
                    {
                        Id = Guid.NewGuid(),
                        Name = "Start Work",
                        Description = "Move from new to in progress.",
                        State = TransitionState.Active,
                        WorkflowId = workflow.Id,
                        SourceId = stateNew.Id,
                        TargetId = stateInProgress.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    },
                    new Transition
                    {
                        Id = Guid.NewGuid(),
                        Name = "Resolve",
                        Description = "Move from in progress to resolved.",
                        State = TransitionState.Active,
                        WorkflowId = workflow.Id,
                        SourceId = stateInProgress.Id,
                        TargetId = stateResolved.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    },
                    new Transition
                    {
                        Id = Guid.NewGuid(),
                        Name = "Close",
                        Description = "Move from resolved to closed.",
                        State = TransitionState.Active,
                        WorkflowId = workflow.Id,
                        SourceId = stateResolved.Id,
                        TargetId = stateClosed.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    },
                    new Transition
                    {
                        Id = Guid.NewGuid(),
                        Name = "Cancel",
                        Description = "Close directly from new.",
                        State = TransitionState.Active,
                        WorkflowId = workflow.Id,
                        SourceId = stateNew.Id,
                        TargetId = stateClosed.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    }
                };

                // add the transitions to the database context
                db.Transitions.AddRange(transitions);
            }
        }
    }
}