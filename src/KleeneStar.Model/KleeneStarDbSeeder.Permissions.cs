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
        /// Grants every seeded workspace the defaults a workspace created from a template gets
        /// (<see cref="WorkspacePermissionDefaults"/>).
        /// </summary>
        /// <remarks>
        /// Must run after the groups - built-in ones included - and the workspaces. A default
        /// naming a group the store does not carry is skipped rather than written as a grant
        /// nobody could read back.
        /// </remarks>
        /// <param name="db">The database context. Cannot be null.</param>
        private static void SeedWorkspacePermissions(KleeneStarDbContext db)
        {
            var groups = db.Groups.Select(x => x.Id).ToHashSet();
            var created = DateTime.UtcNow;

            foreach (var workspace in db.Workspaces.ToList())
            {
                foreach (var (groupId, policy) in WorkspacePermissionDefaults.Grants.Where(x => groups.Contains(x.GroupId)))
                {
                    db.PermissionAssignments.Add(new PermissionAssignment(Guid.NewGuid())
                    {
                        Scope = WorkspacePermissionDefaults.Scope,
                        ScopeId = workspace.Id.ToString(),
                        GroupId = groupId,
                        Policy = policy,
                        Created = created
                    });
                }
            }
        }
    }
}
