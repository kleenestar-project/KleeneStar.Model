using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the workflow status entity type.
    /// </summary>
    internal class WorkflowStatusConfiguration : IEntityTypeConfiguration<WorkflowStatus>
    {
        /// <summary>
        /// Configuration of the workflow status entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<WorkflowStatus> builder)
        {
            builder.ToTable("WorkflowStatus");

            // composite primary key (WorkflowId + StatusId): a status takes part in a workflow
            // at most once
            builder.HasKey(x => new { x.WorkflowId, x.StatusId });

            builder.Property(x => x.WorkflowId)
                .HasColumnName("Workflow")
                .IsRequired();

            builder.Property(x => x.StatusId)
                .HasColumnName("Status")
                .IsRequired();

            builder.Property(x => x.X)
                .HasColumnName("X");

            builder.Property(x => x.Y)
                .HasColumnName("Y");

            builder.Property(x => x.IsStart)
                .HasColumnName("IsStart");

            builder.Property(x => x.IsEnd)
                .HasColumnName("IsEnd");
        }
    }
}
