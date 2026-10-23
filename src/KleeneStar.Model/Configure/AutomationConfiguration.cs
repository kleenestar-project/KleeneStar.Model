using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the automation rules.
    /// </summary>
    internal class AutomationRuleConfiguration : IEntityTypeConfiguration<AutomationRule>
    {
        /// <summary>
        /// Configures the entity type mapping for the AutomationRule entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AutomationRule> builder)
        {
            builder.ToTable("AutomationRule");

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
                .HasMaxLength(128);

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.WorkspaceId)
                .HasColumnName("Workspace")
                .IsRequired();

            // the rules of a workspace go with it
            builder.HasOne<Workspace>()
                .WithMany()
                .HasForeignKey(x => x.WorkspaceId)
                .HasPrincipalKey(w => w.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ClassId)
                .HasColumnName("Class");

            builder.Property(x => x.Trigger)
                .HasColumnName("Trigger")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.TriggerParameter)
                .HasColumnName("TriggerParameter")
                .HasMaxLength(256);

            builder.Property(x => x.Condition)
                .HasColumnName("Condition");

            builder.Property(x => x.Actions)
                .HasColumnName("Actions");

            builder.Property(x => x.RunAsId)
                .HasColumnName("RunAs");

            builder.Property(x => x.CreatedById)
                .HasColumnName("CreatedBy");

            builder.Property(x => x.NextRun)
                .HasColumnName("NextRun");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => new { x.WorkspaceId, x.Trigger });
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the automation jobs.
    /// </summary>
    internal class AutomationJobConfiguration : IEntityTypeConfiguration<AutomationJob>
    {
        /// <summary>
        /// Configures the entity type mapping for the AutomationJob entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AutomationJob> builder)
        {
            builder.ToTable("AutomationJob");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.RuleId)
                .HasColumnName("Rule")
                .IsRequired();

            builder.Property(x => x.ObjectId)
                .HasColumnName("Object")
                .IsRequired();

            builder.Property(x => x.Trigger)
                .HasColumnName("Trigger")
                .HasMaxLength(64);

            builder.Property(x => x.Deduplication)
                .HasColumnName("Deduplication")
                .IsRequired()
                .HasMaxLength(256);

            // a second delivery of the same event finds the first job instead of adding one
            builder.HasIndex(x => x.Deduplication)
                .IsUnique();

            builder.Property(x => x.Depth)
                .HasColumnName("Depth");

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.Attempts)
                .HasColumnName("Attempts");

            builder.Property(x => x.CompletedActions)
                .HasColumnName("CompletedActions");

            builder.Property(x => x.NextAttempt)
                .HasColumnName("NextAttempt")
                .IsRequired();

            builder.Property(x => x.LockedBy)
                .HasColumnName("LockedBy")
                .HasMaxLength(128);

            builder.Property(x => x.LockedUntil)
                .HasColumnName("LockedUntil");

            builder.Property(x => x.LastError)
                .HasColumnName("LastError");

            // a claim compares and renews the stamp, so of two workers claiming one job the
            // second fails instead of running it again
            builder.Property(x => x.Version)
                .HasColumnName("Version")
                .IsConcurrencyToken();

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => new { x.State, x.NextAttempt });
            builder.HasIndex(x => x.RuleId);
        }
    }
}
