using KleeneStar.Model.Entities;
using System;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Adds sample permission profile entities linking groups to policies per workspace.
        /// </summary>
        /// <param name="db">The database context to which the permission profile entities will be added. Cannot be null.</param>
        private static void SeedPermissionProfiles(KleeneStarDbContext db)
        {
            // helper to add a profile linking a group to a policy in a workspace
            void add(string id, string groupName, string policyName, string workspaceName)
            {
                // resolve the referenced entities by name
                var group = db.Groups.Local.FirstOrDefault(g => g.Name == groupName)
                    ?? db.Groups.First(g => g.Name == groupName);
                //var policy = db.Policies.Local.FirstOrDefault(p => p.Name == policyName)
                //    ?? db.Policies.First(p => p.Name == policyName);
                var workspace = db.Workspaces.Local.FirstOrDefault(w => w.Name == workspaceName)
                    ?? db.Workspaces.First(w => w.Name == workspaceName);
            }

            // admin group gets admin policy in cmdb and service desk workspaces
            add("D0000001-0000-0000-0000-000000000001", "Admin", "workspace_admin_policy", "Configuration Database");
            add("D0000001-0000-0000-0000-000000000002", "Admin", "workspace_admin_policy", "IT Service Desk");

            // engineering group gets edit policy in dev, view policy in cmdb
            add("D0000001-0000-0000-0000-000000000003", "Engineering", "workspace_edit_policy", "Software Development");
            add("D0000001-0000-0000-0000-000000000004", "Engineering", "workspace_view_policy", "Configuration Database");

            // marketing group gets view policy in service desk
            add("D0000001-0000-0000-0000-000000000005", "Marketing", "workspace_view_policy", "IT Service Desk");

            // support group gets edit policy in service desk
            add("D0000001-0000-0000-0000-000000000006", "Support", "workspace_edit_policy", "IT Service Desk");
        }
    }
}
