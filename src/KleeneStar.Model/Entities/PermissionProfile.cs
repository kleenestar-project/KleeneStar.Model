namespace KleeneStar.Model.Entities;

/// <summary>
/// Represents a permission profile that assigns a specific policy to a group within a workspace.
/// A profile defines which policy a group receives in a specific workspace.
/// A group may have different policies in different workspaces,
/// but a group may appear at most once per workspace.
/// </summary>
public class PermissionProfile
{
    /// <summary>
    /// Gets or sets the unique identifier of the permission profile.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated group.
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// Gets or sets the associated group.
    /// </summary>
    public Group Group { get; set; } = null!;

    /// <summary>
    /// Gets or sets the identifier of the associated policy.
    /// </summary>
    public Guid PolicyId { get; set; }

    /// <summary>
    /// Gets or sets the associated policy.
    /// </summary>
    public Policy Policy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the identifier of the associated workspace.
    /// </summary>
    public Guid WorkspaceId { get; set; }

    /// <summary>
    /// Gets or sets the associated workspace.
    /// </summary>
    public Workspace Workspace { get; set; } = null!;
}
