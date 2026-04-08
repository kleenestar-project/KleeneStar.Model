using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the dashboard column entity type.
    /// </summary>
    internal class DashboardColumnConfiguration : IEntityTypeConfiguration<DashboardColumn>
    {
        /// <summary>
        /// Configuration of the dashboard column entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<DashboardColumn> builder)
        {
            builder.ToTable("DashboardColumn");

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

            builder.Property(x => x.Size)
                .HasColumnName("Size")
                .HasMaxLength(64);

            builder.Property(x => x.DashboardId)
                .HasColumnName("Dashboard")
                .IsRequired();

            builder.HasOne(x => x.Dashboard)
                .WithMany(d => d.Columns)
                .HasForeignKey(x => x.DashboardId)
                .HasPrincipalKey(d => d.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // ONE-TO-MANY: DashboardColumn -> Widget
            builder.HasMany(c => c.Widgets)
                .WithOne(w => w.Column)
                .HasForeignKey(w => w.ColumnId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.DashboardId, x.Name })
                .IsUnique();
        }
    }
}
