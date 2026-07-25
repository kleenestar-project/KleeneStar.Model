using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the Kanban board column entity type.
    /// </summary>
    internal class KanbanBoardColumnConfiguration : IEntityTypeConfiguration<KanbanBoardColumn>
    {
        /// <summary>
        /// Configuration of the Kanban board column entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<KanbanBoardColumn> builder)
        {
            builder.ToTable("KanbanBoardColumn");

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

            builder.Property(x => x.Color)
                .HasColumnName("Color")
                .HasMaxLength(32);

            builder.Property(x => x.Position)
                .HasColumnName("Position")
                .IsRequired();

            builder.Property(x => x.CategoryId)
                .HasColumnName("Category");

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

            // Column names are not unique: the board "…" menu can add several columns that share
            // the default name, so only a non-unique lookup index is kept.
            builder.HasIndex(x => x.BoardId);
        }
    }
}
