using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the Kanban board entity type.
    /// </summary>
    internal class KanbanBoardConfiguration : IEntityTypeConfiguration<KanbanBoard>
    {
        /// <summary>
        /// Configuration of the Kanban board entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<KanbanBoard> builder)
        {
            builder.ToTable("KanbanBoard");

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

            builder.Property(x => x.Filter)
                .HasColumnName("Filter");

            // one board per workspace/kind pair
            builder.HasIndex(x => new { x.WorkspaceId, x.Kind })
                .IsUnique();

            // ONE-TO-MANY: KanbanBoard -> KanbanBoardColumn
            builder.HasMany(b => b.Columns)
                .WithOne(c => c.Board)
                .HasForeignKey(c => c.BoardId)
                .HasPrincipalKey(b => b.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // ONE-TO-MANY: KanbanBoard -> KanbanBoardSwimlane
            builder.HasMany(b => b.Swimlanes)
                .WithOne(s => s.Board)
                .HasForeignKey(s => s.BoardId)
                .HasPrincipalKey(b => b.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
