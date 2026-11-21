using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the Kanban board swimlane entity type.
    /// </summary>
    internal class KanbanBoardSwimlaneConfiguration : IEntityTypeConfiguration<KanbanBoardSwimlane>
    {
        /// <summary>
        /// Configuration of the Kanban board swimlane entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<KanbanBoardSwimlane> builder)
        {
            builder.ToTable("KanbanBoardSwimlane");

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

            builder.Property(x => x.Filter)
                .HasColumnName("Filter");

            builder.Property(x => x.Position)
                .HasColumnName("Position")
                .IsRequired();

            builder.Property(x => x.ClassId)
                .HasColumnName("Class");

            builder.Property(x => x.Key)
                .HasColumnName("Key")
                .HasMaxLength(64);

            builder.Property(x => x.BoardId)
                .HasColumnName("Board")
                .IsRequired();

            builder.HasOne(x => x.Board)
                .WithMany(b => b.Swimlanes)
                .HasForeignKey(x => x.BoardId)
                .HasPrincipalKey(b => b.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Swimlane names are not unique: the board "…" menu can add several swimlanes that
            // share the default name, so only a non-unique lookup index is kept.
            builder.HasIndex(x => x.BoardId);
        }
    }
}
