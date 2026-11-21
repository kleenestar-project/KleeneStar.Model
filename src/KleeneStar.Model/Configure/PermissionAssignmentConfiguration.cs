using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="PermissionAssignment"/>.
    /// </summary>
    internal class PermissionAssignmentConfiguration : IEntityTypeConfiguration<PermissionAssignment>
    {
        /// <summary>
        /// Configures the entity type mapping for the permission assignment entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PermissionAssignment> builder)
        {
            builder.ToTable("PermissionAssignment");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.GroupId)
                .HasColumnName("Group")
                .IsRequired();

            builder.HasOne(x => x.Group)
                .WithMany()
                .HasForeignKey(x => x.GroupId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Policy)
                .HasColumnName("Policy")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Scope)
                .HasColumnName("Scope")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.ScopeId)
                .HasColumnName("ScopeId")
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            // the dialog fills itself from exactly this lookup
            builder.HasIndex(x => new { x.Scope, x.ScopeId });

            // granting the same policy to the same group twice on one resource says nothing more
            // than granting it once, and would show up as a duplicate row in the dialog
            builder.HasIndex(x => new { x.Scope, x.ScopeId, x.GroupId, x.Policy })
                .IsUnique();

            builder.HasIndex(x => x.Id)
                .IsUnique();
        }
    }
}
