using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the <see cref="Sprint"/> entity.
    /// </summary>
    internal class SprintConfiguration : IEntityTypeConfiguration<Sprint>
    {
        /// <summary>
        /// Configures the entity mapping.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Sprint> builder)
        {
            builder.ToTable("Sprint");

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

            builder.Property(x => x.Goal)
                .HasColumnName("Goal");

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.Start)
                .HasColumnName("Start");

            builder.Property(x => x.End)
                .HasColumnName("End");

            builder.Property(x => x.Capacity)
                .HasColumnName("Capacity")
                .IsRequired();

            builder.Property(x => x.WorkspaceId)
                .HasColumnName("Workspace")
                .IsRequired();

            builder.HasOne(x => x.Workspace)
                .WithMany()
                .HasForeignKey(x => x.WorkspaceId)
                .HasPrincipalKey(w => w.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => new { x.WorkspaceId, x.Name })
                .IsUnique();
        }
    }
}
