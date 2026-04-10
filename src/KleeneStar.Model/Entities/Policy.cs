namespace KleeneStar.Model.Entities;

/// <summary>
/// Represents a global policy that bundles one or more permissions.
/// Policies are assigned to groups within workspaces via permission profiles.
/// </summary>
public class Policy
{
    /// <summary>
    /// Gets or sets the unique identifier of the policy.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the policy (e.g., "workspace_admin_policy").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the policy.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of permissions included in this policy.
    /// </summary>
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    /// <summary>
    /// Gets or sets the collection of permission profiles that reference this policy.
    /// </summary>
    public ICollection<PermissionProfile> PermissionProfiles { get; set; } = new List<PermissionProfile>();
}
