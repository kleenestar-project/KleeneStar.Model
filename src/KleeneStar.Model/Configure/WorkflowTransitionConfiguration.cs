using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the workflow transition entity type.
    /// </summary>
    internal class WorkflowTransitionConfiguration : IEntityTypeConfiguration<WorkflowTransition>
    {
        /// <summary>
        /// Configuration of the workflow transition entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<WorkflowTransition> builder)
        {
            builder.ToTable("WorkflowTransition");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.State)
                .HasColumnName("State");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.WorkflowId)
                .HasColumnName("Workflow")
                .IsRequired();

            // Workflow (many transitions → one workflow)
            builder.HasOne(x => x.Workflow)
                .WithMany(w => w.Transitions)
                .HasForeignKey(x => x.WorkflowId)
                .HasPrincipalKey(w => w.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Source (many transitions → one state)
            builder.HasOne(x => x.Source)
                .WithMany()
                .HasForeignKey(x => x.SourceId)
                .HasPrincipalKey(s => s.Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Target (many transitions → one state)
            builder.HasOne(x => x.Target)
                .WithMany()
                .HasForeignKey(x => x.TargetId)
                .HasPrincipalKey(s => s.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.WorkflowId, x.Name })
                .IsUnique();
        }
    }
}
