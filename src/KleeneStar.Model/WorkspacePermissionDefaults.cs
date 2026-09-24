using KleeneStar.Model.Entities;
using System;
using System.Collections.Generic;

namespace KleeneStar.Model
{
    /// <summary>
    /// The grants a workspace starts with: the seeded demo workspaces and every workspace a
    /// template sets up.
    /// </summary>
    /// <remarks>
    /// The installation's administrators administer it, every signed-in account works in it,
    /// and a visitor who is not signed in is left out - by granting nothing to
    /// <see cref="Group.AnonymousId"/>, which is how the permission model says "not you" once a
    /// workspace is administered at all. One list, read by the seeder and by the template
    /// setup, so the two cannot drift apart.
    /// </remarks>
    public static class WorkspacePermissionDefaults
    {
        /// <summary>
        /// The scope the grants are stored under - the name the core's <c>PermissionScope</c>
        /// gives a workspace.
        /// </summary>
        public const string Scope = "workspace";

        /// <summary>
        /// The registered name of the policy that administers a workspace.
        /// </summary>
        public const string AdminPolicy = "workspace_admin_policy";

        /// <summary>
        /// The registered name of the policy that reads and writes a workspace's content.
        /// </summary>
        public const string EditPolicy = "workspace_edit_policy";

        /// <summary>
        /// Returns the default grants as (group, policy) pairs.
        /// </summary>
        public static IReadOnlyList<(Guid GroupId, string Policy)> Grants { get; } =
        [
            (Group.AdministratorsId, AdminPolicy),
            (Group.AuthenticatedId, EditPolicy)
        ];
    }
}
