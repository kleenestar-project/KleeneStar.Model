using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the object-kind dashboard widget
    /// entity type.
    /// </summary>
    internal class KindDashboardWidgetConfiguration : IEntityTypeConfiguration<KindDashboardWidget>
    {
        /// <summary>
        /// Configuration of the object-kind dashboard widget entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<KindDashboardWidget> builder)
        {
            builder.ToTable("KindDashboardWidget");

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
                .HasMaxLength(64);

            builder.Property(x => x.Type)
                .HasColumnName("Type")
                .HasMaxLength(64);

            builder.Property(x => x.Color)
                .HasColumnName("Color")
                .HasMaxLength(32);

            builder.Property(x => x.Params)
                .HasColumnName("Params");

            builder.Property(x => x.Position)
                .HasColumnName("Position")
                .IsRequired();

            builder.Property(x => x.ColumnId)
                .HasColumnName("Column")
                .IsRequired();

            builder.HasOne(x => x.Column)
                .WithMany(c => c.Widgets)
                .HasForeignKey(x => x.ColumnId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ColumnId);
        }
    }
}
