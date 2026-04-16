using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the widget entity type.
    /// </summary>
    internal class WidgetConfiguration : IEntityTypeConfiguration<Widget>
    {
        /// <summary>
        /// Configuration of the widget entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Widget> builder)
        {
            builder.ToTable("Widget");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Wql)
                .HasColumnName("Wql");

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.ColumnId)
                .HasColumnName("Column")
                .IsRequired();

            builder.HasOne(x => x.Column)
                .WithMany(c => c.Widgets)
                .HasForeignKey(x => x.ColumnId)
                .HasPrincipalKey(c => c.Id);

            builder.HasIndex(x => new { x.ColumnId, x.Name })
                .IsUnique();
        }
    }
}
