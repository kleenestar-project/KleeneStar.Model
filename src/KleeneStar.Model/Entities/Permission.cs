using System;
using System.Collections.Generic;

namespace KleeneStar.Model.Entities;

/// <summary>
/// Represents a single atomic permission (e.g., workspace_read, workspace_update).
/// Permissions are the finest-grained unit of access control and are bundled into policies.
/// </summary>
public class Permission
{
    /// <summary>
    /// Gets or sets the unique identifier of the permission.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the permission (e.g., "workspace_read").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the permission.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of policies that include this permission.
    /// </summary>
    public ICollection<Policy> Policies { get; set; } = new List<Policy>();
}
