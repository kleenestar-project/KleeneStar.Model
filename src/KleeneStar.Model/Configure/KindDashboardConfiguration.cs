using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the object-kind dashboard entity type.
    /// </summary>
    internal class KindDashboardConfiguration : IEntityTypeConfiguration<KindDashboard>
    {
        /// <summary>
        /// Configuration of the object-kind dashboard entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<KindDashboard> builder)
        {
            builder.ToTable("KindDashboard");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.WorkspaceId)
                .HasColumnName("Workspace")
                .IsRequired();

            builder.Property(x => x.Kind)
                .HasColumnName("Kind")
                .IsRequired()
                .HasMaxLength(64);

            // one board per workspace/kind pair
            builder.HasIndex(x => new { x.WorkspaceId, x.Kind })
                .IsUnique();

            // ONE-TO-MANY: KindDashboard -> KindDashboardColumn
            builder.HasMany(b => b.Columns)
                .WithOne(c => c.Board)
                .HasForeignKey(c => c.BoardId)
                .HasPrincipalKey(b => b.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
