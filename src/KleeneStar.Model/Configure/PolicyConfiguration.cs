using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace KleeneStar.Model.Configure;

/// <summary>
/// Configures the EF Core entity mapping for <see cref="Policy"/>.
/// Includes the many-to-many relationship between Policy and Permission.
/// </summary>
public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    /// <summary>
    /// Configures the entity properties, keys, and relationships for the Policy entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(512);

        builder.HasIndex(p => p.Name)
            .IsUnique();

        // configure many-to-many relationship between policy and permission
        builder.HasMany(p => p.Permissions)
            .WithMany(p => p.Policies)
            .UsingEntity<Dictionary<string, object>>(
                "PolicyPermission",
                right => right.HasOne<Permission>()
                    .WithMany()
                    .HasForeignKey("PermissionId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<Policy>()
                    .WithMany()
                    .HasForeignKey("PolicyId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("PolicyId", "PermissionId");
                    join.ToTable("PolicyPermission");
                });
    }
}
