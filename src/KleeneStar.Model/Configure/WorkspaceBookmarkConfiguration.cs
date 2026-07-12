using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="WorkspaceBookmark"/>.
    /// </summary>
    internal class WorkspaceBookmarkConfiguration : IEntityTypeConfiguration<WorkspaceBookmark>
    {
        /// <summary>
        /// Configures the entity type mapping for the WorkspaceBookmark entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<WorkspaceBookmark> builder)
        {
            builder.ToTable("WorkspaceBookmark");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.OwnerId)
                .HasColumnName("Owner")
                .IsRequired();

            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.WorkspaceId)
                .HasColumnName("Workspace")
                .IsRequired();

            builder.HasOne(x => x.Workspace)
                .WithMany()
                .HasForeignKey(x => x.WorkspaceId)
                .HasPrincipalKey(w => w.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Favorite)
                .HasColumnName("Favorite")
                .IsRequired();

            builder.Property(x => x.LastVisited)
                .HasColumnName("LastVisited")
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => new { x.OwnerId, x.WorkspaceId }).IsUnique();
        }
    }
}
