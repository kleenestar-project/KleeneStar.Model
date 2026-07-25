using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the workflow entity type.
    /// </summary>
    internal class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
    {
        /// <summary>
        /// Configuration of the workflow entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Workflow> builder)
        {
            builder.ToTable("Workflow");

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

            builder.Property(x => x.Icon)
                .HasColumnName("Icon")
                .HasMaxLength(256)
                .HasConversion
                (
                    icon => icon != null && icon.Uri != null ? icon.Uri.ToString() : null,
                    uri => string.IsNullOrEmpty(uri) ? null : ImageIcon.FromString(uri)
                );

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

            // MANY - TO - 1: Workflow <-> Transition
            builder.HasMany(w => w.Transitions)
                .WithOne(s => s.Workflow)
                .HasForeignKey(s => s.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            // MANY - TO - MANY: Workflow <-> Status, through a join entity that carries the
            // canvas position and the entry/end marks of the pairing. Statuses stays a skip
            // navigation, so the read sites that only need the states are unaffected; the
            // designer reaches the payload through WorkflowStatuses.
            builder.HasMany(w => w.Statuses)
                .WithMany()
                .UsingEntity<WorkflowStatus>
                (
                    r => r.HasOne(x => x.Status)
                        .WithMany()
                        .HasForeignKey(x => x.StatusId)
                        .HasPrincipalKey(s => s.Id)
                        .OnDelete(DeleteBehavior.Cascade),
                    l => l.HasOne(x => x.Workflow)
                        .WithMany(w => w.WorkflowStatuses)
                        .HasForeignKey(x => x.WorkflowId)
                        .HasPrincipalKey(w => w.Id)
                        .OnDelete(DeleteBehavior.Cascade)
                );

            builder.Property(x => x.ClassId)
                .HasColumnName("Class")
                .IsRequired();

            builder.HasOne(x => x.Class)
                .WithMany()
                .HasForeignKey(x => x.ClassId)
                .HasPrincipalKey(w => w.Id);

            builder.HasIndex(x => new { x.ClassId, x.Name })
                .IsUnique();
        }
    }
}
