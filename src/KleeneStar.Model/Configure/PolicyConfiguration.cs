using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the policy entity type.
    /// </summary>
    internal class PolicyConfiguration : IEntityTypeConfiguration<Policy>
    {
        /// <summary>
        /// Configuration of the policy entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Policy> builder)
        {
            builder.ToTable("Policy");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            // MANY-TO-MANY: Policy <-> Permission
            builder.HasMany(p => p.Permissions)
                .WithMany(perm => perm.Policies)
                .UsingEntity<Dictionary<string, object>>
                (
                    "PolicyPermission",
                    j => j
                        .HasOne<Permission>()
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Policy>()
                        .WithMany()
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.Cascade)
                );

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
