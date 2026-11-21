using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the object-kind dashboard column
    /// entity type.
    /// </summary>
    internal class KindDashboardColumnConfiguration : IEntityTypeConfiguration<KindDashboardColumn>
    {
        /// <summary>
        /// Configuration of the object-kind dashboard column entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<KindDashboardColumn> builder)
        {
            builder.ToTable("KindDashboardColumn");

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

            builder.Property(x => x.Color)
                .HasColumnName("Color")
                .HasMaxLength(32);

            builder.Property(x => x.Position)
                .HasColumnName("Position")
                .IsRequired();

            builder.Property(x => x.Key)
                .HasColumnName("Key")
                .HasMaxLength(64);

            builder.Property(x => x.BoardId)
                .HasColumnName("Board")
                .IsRequired();

            builder.HasOne(x => x.Board)
                .WithMany(b => b.Columns)
                .HasForeignKey(x => x.BoardId)
                .HasPrincipalKey(b => b.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // ONE-TO-MANY: KindDashboardColumn -> KindDashboardWidget
            builder.HasMany(c => c.Widgets)
                .WithOne(w => w.Column)
                .HasForeignKey(w => w.ColumnId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Column names are not unique: the board "…" menu can add several columns that share
            // the default name, so only a non-unique lookup index is kept.
            builder.HasIndex(x => x.BoardId);
        }
    }
}
