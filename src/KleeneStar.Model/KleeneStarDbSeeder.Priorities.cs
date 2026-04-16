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
        /// Adds a predefined set of priority entities to the specified database context based on the class and workspace.
        /// </summary>
        /// <remarks>
        /// This method is intended to seed the database with a standard set of priorities. 
        /// It does not save changes to the database; callers must call SaveChanges on the 
        /// context to persist the additions.
        /// </remarks>
        /// <param name="db">The database context to which the priority entities will be added. Cannot be null.</param>
        private static void SeedPriorities(KleeneStarDbContext db)
        {
            // retrieve all classes including their associated workspaces
            var classes = db.Classes
                .Include(c => c.Workspace)
                .AsNoTracking()
                .ToList();

            foreach (var cls in classes)
            {
                // get the specific priority templates for the current class and workspace
                var templates = GetPriorityTemplatesForClassAndWorkspace(cls.Name, cls.Workspace.Key);

                foreach (var template in templates)
                {
                    // add the new priority entity to the context
                    db.Priorities.Add(new Priority
                    {
                        Id = Guid.NewGuid(),
                        Name = template.Name,
                        Description = template.Description,
                        State = PriorityState.Active,
                        Icon = ImageIcon.FromString(template.Icon),
                        ClassId = cls.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    });
                }
            }
        }

        /// <summary>
        /// Returns priority templates based on the class name and workspace key.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <param name="workspaceKey">The key of the workspace.</param>
        /// <returns>A list of priority templates.</returns>
        private static IReadOnlyList<(string Name, string Description, string Icon)> GetPriorityTemplatesForClassAndWorkspace(string className, string workspaceKey)
        {
            // default priorities for fallback
            var defaults = new List<(string Name, string Description, string Icon)>
            {
                ("Critical", "Critical priority for urgent and system-blocking issues.", "/kleenestar/assets/icons/priority/critical.svg"),
                ("High", "High priority.", "/kleenestar/assets/icons/priority/high.svg"),
                ("Medium", "Medium priority.", "/kleenestar/assets/icons/priority/medium.svg"),
                ("Low", "Low priority.", "/kleenestar/assets/icons/priority/low.svg")
            };

            if (workspaceKey == "SD")
            {
                if (className == "Incident" || className == "Problem")
                {
                    return
                    [
                        ("P1 - Critical", "Critical system outage.", "/kleenestar/assets/icons/priority/p1.svg"),
                        ("P2 - High", "Severe degradation.", "/kleenestar/assets/icons/priority/p2.svg"),
                        ("P3 - Moderate", "Moderate impact.", "/kleenestar/assets/icons/priority/p3.svg"),
                        ("P4 - Low", "Minor issue.", "/kleenestar/assets/icons/priority/p4.svg")
                    ];
                }
            }

            if (workspaceKey == "DEV")
            {
                if (className == "Bug")
                {
                    return
                    [
                        ("Blocker", "Blocks development or testing.", "/kleenestar/assets/icons/priority/blocker.svg"),
                        ("Critical", "Causes crashes or data loss.", "/kleenestar/assets/icons/priority/critical.svg"),
                        ("Major", "Major loss of function.", "/kleenestar/assets/icons/priority/major.svg"),
                        ("Minor", "Minor loss of function or UI issue.", "/kleenestar/assets/icons/priority/minor.svg"),
                        ("Trivial", "Cosmetic issue.", "/kleenestar/assets/icons/priority/trivial.svg")
                    ];
                }

                if (className == "Feature")
                {
                    return
                    [
                        ("Must Have", "Essential for the release.", "/kleenestar/assets/icons/priority/musthave.svg"),
                        ("Should Have", "Important but not critical.", "/kleenestar/assets/icons/priority/shouldhave.svg"),
                        ("Could Have", "Nice to have.", "/kleenestar/assets/icons/priority/couldhave.svg"),
                        ("Won't Have", "Not planned for this release.", "/kleenestar/assets/icons/priority/wonthave.svg")
                    ];
                }
            }

            if (workspaceKey == "FIN")
            {
                if (className == "Invoice" || className == "Approval")
                {
                    return
                    [
                        ("Urgent", "Requires immediate financial approval.", "/kleenestar/assets/icons/priority/urgent.svg"),
                        ("Standard", "Standard processing time.", "/kleenestar/assets/icons/priority/standard.svg")
                    ];
                }
            }

            // return defaults if no specific rules match
            return defaults;
        }
    }
}