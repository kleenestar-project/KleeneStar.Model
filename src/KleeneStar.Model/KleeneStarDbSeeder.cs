using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace KleeneStar.Model;

/// <summary>
/// Seeds the database with initial permissions, policies, groups, workspaces,
/// and permission profiles required by the permissions subsystem.
/// </summary>
public static class KleeneStarDbSeeder
{
    /// <summary>
    /// Seeds all permission-related data into the provided database context.
    /// </summary>
    /// <param name="context">The database context to seed.</param>
    public static async Task SeedAsync(KleeneStarDbContext context)
    {
        // ensure the database schema is created (intended for development/testing only;
        // production environments should use context.Database.MigrateAsync() instead)
        await context.Database.EnsureCreatedAsync();

        // seed permissions first as policies depend on them
        var permissions = await SeedPermissionsAsync(context);

        // seed policies with their permission assignments
        var policies = await SeedPoliciesAsync(context, permissions);

        // seed global groups
        var groups = await SeedGroupsAsync(context);

        // seed sample workspaces
        var workspaces = await SeedWorkspacesAsync(context);

        // seed permission profiles linking groups to policies per workspace
        await SeedPermissionProfilesAsync(context, groups, policies, workspaces);
    }

    /// <summary>
    /// Seeds all defined permissions into the database.
    /// </summary>
    /// <param name="context">The database context to seed.</param>
    /// <returns>A dictionary mapping permission names to their entities.</returns>
    private static async Task<Dictionary<string, Permission>> SeedPermissionsAsync(KleeneStarDbContext context)
    {
        // skip if permissions already exist
        if (await context.Permissions.AnyAsync())
            return await context.Permissions.ToDictionaryAsync(p => p.Name);

        // define all atomic permissions
        var permissionDefinitions = new (string Name, string Description)[]
        {
            ("workspace_create", "Allows creating new workspaces."),
            ("workspace_read", "Allows reading workspace metadata."),
            ("workspace_update", "Allows updating workspace settings."),
            ("workspace_delete", "Allows deleting workspaces."),
            ("workspace_archive", "Allows archiving workspaces."),
            ("workspace_restore", "Allows restoring archived workspaces."),
            ("workspace_clone", "Allows cloning workspaces."),
            ("workspace_manage_profiles", "Allows managing permission profiles within a workspace."),
            ("workspace_read_content", "Allows reading content within a workspace."),
            ("workspace_write_content", "Allows writing content within a workspace."),
        };

        var permissions = new Dictionary<string, Permission>();

        foreach (var (name, description) in permissionDefinitions)
        {
            // create each permission entity
            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
            };
            permissions[name] = permission;
            context.Permissions.Add(permission);
        }

        // persist all permissions before creating policies
        await context.SaveChangesAsync();

        return permissions;
    }

    /// <summary>
    /// Seeds all defined policies with their permission assignments.
    /// </summary>
    /// <param name="context">The database context to seed.</param>
    /// <param name="permissions">The dictionary of available permissions.</param>
    /// <returns>A dictionary mapping policy names to their entities.</returns>
    private static async Task<Dictionary<string, Policy>> SeedPoliciesAsync(
        KleeneStarDbContext context,
        Dictionary<string, Permission> permissions)
    {
        // skip if policies already exist
        if (await context.Policies.AnyAsync())
            return await context.Policies.ToDictionaryAsync(p => p.Name);

        // define policies with their included permissions
        var policyDefinitions = new (string Name, string Description, string[] PermissionNames)[]
        {
            (
                "workspace_admin_policy",
                "Grants all workspace permissions.",
                permissions.Keys.Where(k => k.StartsWith("workspace_")).ToArray()
            ),
            (
                "workspace_edit_policy",
                "Grants read and write access to workspace content.",
                new[] { "workspace_read", "workspace_read_content", "workspace_write_content" }
            ),
            (
                "workspace_view_policy",
                "Grants read-only access to workspace content.",
                new[] { "workspace_read", "workspace_read_content" }
            ),
            (
                "workspace_creator_policy",
                "Grants permission to create new workspaces.",
                new[] { "workspace_create" }
            ),
        };

        var policies = new Dictionary<string, Policy>();

        foreach (var (name, description, permissionNames) in policyDefinitions)
        {
            // create each policy and assign its permissions
            var policy = new Policy
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
            };

            // link the specified permissions to this policy
            foreach (var permName in permissionNames)
            {
                if (permissions.TryGetValue(permName, out var perm))
                {
                    policy.Permissions.Add(perm);
                }
            }

            policies[name] = policy;
            context.Policies.Add(policy);
        }

        // persist all policies with their permission assignments
        await context.SaveChangesAsync();

        return policies;
    }

    /// <summary>
    /// Seeds global user groups into the database.
    /// </summary>
    /// <param name="context">The database context to seed.</param>
    /// <returns>A dictionary mapping group names to their entities.</returns>
    private static async Task<Dictionary<string, Group>> SeedGroupsAsync(KleeneStarDbContext context)
    {
        // skip if groups already exist
        if (await context.Groups.AnyAsync())
            return await context.Groups.ToDictionaryAsync(g => g.Name);

        // define sample global groups
        var groupDefinitions = new (string Name, string? Description)[]
        {
            ("Admin", "Administrators with full access."),
            ("Editor", "Users who can read and write content."),
            ("Viewer", "Users with read-only access."),
            ("Creator", "Users who can create new workspaces."),
        };

        var groups = new Dictionary<string, Group>();

        foreach (var (name, description) in groupDefinitions)
        {
            // create each group entity
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
            };
            groups[name] = group;
            context.Groups.Add(group);
        }

        // persist all groups
        await context.SaveChangesAsync();

        return groups;
    }

    /// <summary>
    /// Seeds sample workspaces into the database.
    /// </summary>
    /// <param name="context">The database context to seed.</param>
    /// <returns>A dictionary mapping workspace names to their entities.</returns>
    private static async Task<Dictionary<string, Workspace>> SeedWorkspacesAsync(KleeneStarDbContext context)
    {
        // skip if workspaces already exist
        if (await context.Workspaces.AnyAsync())
            return await context.Workspaces.ToDictionaryAsync(w => w.Name);

        // define sample workspaces
        var workspaceDefinitions = new (string Name, string? Description)[]
        {
            ("Default Workspace", "The default workspace for general use."),
            ("Marketing Workspace", "Workspace for the marketing team."),
            ("Engineering Workspace", "Workspace for the engineering team."),
        };

        var workspaces = new Dictionary<string, Workspace>();

        foreach (var (name, description) in workspaceDefinitions)
        {
            // create each workspace entity
            var workspace = new Workspace
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
            };
            workspaces[name] = workspace;
            context.Workspaces.Add(workspace);
        }

        // persist all workspaces
        await context.SaveChangesAsync();

        return workspaces;
    }

    /// <summary>
    /// Seeds sample permission profiles linking groups to policies within workspaces.
    /// </summary>
    /// <param name="context">The database context to seed.</param>
    /// <param name="groups">The dictionary of available groups.</param>
    /// <param name="policies">The dictionary of available policies.</param>
    /// <param name="workspaces">The dictionary of available workspaces.</param>
    private static async Task SeedPermissionProfilesAsync(
        KleeneStarDbContext context,
        Dictionary<string, Group> groups,
        Dictionary<string, Policy> policies,
        Dictionary<string, Workspace> workspaces)
    {
        // skip if permission profiles already exist
        if (await context.PermissionProfiles.AnyAsync())
            return;

        // define sample profiles: (workspace, group, policy)
        var profileDefinitions = new (string WorkspaceName, string GroupName, string PolicyName)[]
        {
            ("Default Workspace", "Admin", "workspace_admin_policy"),
            ("Default Workspace", "Editor", "workspace_edit_policy"),
            ("Default Workspace", "Viewer", "workspace_view_policy"),
            ("Marketing Workspace", "Admin", "workspace_admin_policy"),
            ("Marketing Workspace", "Editor", "workspace_edit_policy"),
            ("Marketing Workspace", "Viewer", "workspace_view_policy"),
            ("Engineering Workspace", "Admin", "workspace_admin_policy"),
            ("Engineering Workspace", "Editor", "workspace_edit_policy"),
            ("Engineering Workspace", "Viewer", "workspace_view_policy"),
        };

        foreach (var (workspaceName, groupName, policyName) in profileDefinitions)
        {
            // resolve the referenced entities with validation
            if (!workspaces.TryGetValue(workspaceName, out var workspace))
                throw new InvalidOperationException($"workspace '{workspaceName}' not found during seeding.");
            if (!groups.TryGetValue(groupName, out var group))
                throw new InvalidOperationException($"group '{groupName}' not found during seeding.");
            if (!policies.TryGetValue(policyName, out var policy))
                throw new InvalidOperationException($"policy '{policyName}' not found during seeding.");

            // create a permission profile linking them together
            var profile = new PermissionProfile
            {
                Id = Guid.NewGuid(),
                WorkspaceId = workspace.Id,
                GroupId = group.Id,
                PolicyId = policy.Id,
            };
            context.PermissionProfiles.Add(profile);
        }

        // persist all permission profiles
        await context.SaveChangesAsync();
    }
}
