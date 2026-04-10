namespace KleeneStar.Model.Entities;

/// <summary>
/// Represents a workspace that serves as the organizational scope for permission profiles.
/// Each workspace may contain multiple profiles defining group-policy assignments.
/// </summary>
public class Workspace
{
    /// <summary>
    /// Gets or sets the unique identifier of the workspace.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the workspace.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the workspace.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the collection of permission profiles within this workspace.
    /// </summary>
    public ICollection<PermissionProfile> PermissionProfiles { get; set; } = new List<PermissionProfile>();
}
