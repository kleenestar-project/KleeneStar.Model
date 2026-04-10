namespace KleeneStar.Model.Entities;

/// <summary>
/// Represents a global user group (e.g., "Marketing", "Admin", "Engineering").
/// Groups are assigned policies within specific workspaces via permission profiles.
/// </summary>
public class Group
{
    /// <summary>
    /// Gets or sets the unique identifier of the group.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional description of the group.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the collection of permission profiles assigned to this group.
    /// </summary>
    public ICollection<PermissionProfile> PermissionProfiles { get; set; } = new List<PermissionProfile>();
}
