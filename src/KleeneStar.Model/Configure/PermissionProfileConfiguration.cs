using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure;

/// <summary>
/// Configures the EF Core entity mapping for <see cref="PermissionProfile"/>.
/// Defines the relationships between Group, Policy, and Workspace,
/// and enforces the unique constraint on (WorkspaceId, GroupId).
/// </summary>
public class PermissionProfileConfiguration : IEntityTypeConfiguration<PermissionProfile>
{
    /// <summary>
    /// Configures the entity properties, keys, relationships, and constraints
    /// for the PermissionProfile entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<PermissionProfile> builder)
    {
        builder.HasKey(pp => pp.Id);

        // configure the relationship between permission profile and group
        builder.HasOne(pp => pp.Group)
            .WithMany(g => g.PermissionProfiles)
            .HasForeignKey(pp => pp.GroupId)
            .HasPrincipalKey(g => g.Id)
            .OnDelete(DeleteBehavior.Restrict);

        // configure the relationship between permission profile and policy
        builder.HasOne(pp => pp.Policy)
            .WithMany(p => p.PermissionProfiles)
            .HasForeignKey(pp => pp.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        // configure the relationship between permission profile and workspace
        builder.HasOne(pp => pp.Workspace)
            .WithMany(w => w.PermissionProfiles)
            .HasForeignKey(pp => pp.WorkspaceId)
            .HasPrincipalKey(w => w.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // enforce unique constraint: a group may appear at most once per workspace
        builder.HasIndex(pp => new { pp.WorkspaceId, pp.GroupId })
            .IsUnique();
    }
}
