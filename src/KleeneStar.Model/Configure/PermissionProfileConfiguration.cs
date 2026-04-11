using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the permission profile entity type.
    /// </summary>
    internal class PermissionProfileConfiguration : IEntityTypeConfiguration<PermissionProfile>
    {
        /// <summary>
        /// Configuration of the permission profile entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<PermissionProfile> builder)
        {
            builder.ToTable("PermissionProfile");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.GroupId)
                .HasColumnName("GroupId")
                .IsRequired();

            builder.Property(x => x.WorkspaceId)
                .HasColumnName("WorkspaceId")
                .IsRequired();

            // ONE-TO-MANY: Group -> PermissionProfile
            builder.HasOne(x => x.Group)
                .WithMany(g => g.PermissionProfiles)
                .HasForeignKey(x => x.GroupId)
                .HasPrincipalKey(g => g.Id)
                .OnDelete(DeleteBehavior.Restrict);

            // ONE-TO-MANY: Workspace -> PermissionProfile
            builder.HasOne(x => x.Workspace)
                .WithMany(w => w.PermissionProfiles)
                .HasForeignKey(x => x.WorkspaceId)
                .HasPrincipalKey(w => w.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // a group may appear at most once per workspace
            builder.HasIndex(x => new { x.WorkspaceId, x.GroupId })
                .IsUnique();
        }
    }
}
