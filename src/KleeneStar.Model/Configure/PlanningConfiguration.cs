using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the worklogs.
    /// </summary>
    internal class WorklogConfiguration : IEntityTypeConfiguration<Worklog>
    {
        /// <summary>
        /// Configures the entity type mapping for the Worklog entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Worklog> builder)
        {
            builder.ToTable("Worklog");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.ObjectId).HasColumnName("Object").IsRequired();

            // the time spent on an object is part of its record and goes with it
            builder.HasOne<Object>().WithMany().HasForeignKey(x => x.ObjectId).HasPrincipalKey(o => o.Id).OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.IdentityId).HasColumnName("Identity").IsRequired();
            builder.Property(x => x.Started).HasColumnName("Started").IsRequired();
            builder.Property(x => x.Minutes).HasColumnName("Minutes").IsRequired();
            builder.Property(x => x.Comment).HasColumnName("Comment").HasMaxLength(2048);
            builder.Property(x => x.Created).HasColumnName("Created").IsRequired();
            builder.Property(x => x.Updated).HasColumnName("Updated").IsRequired();
            builder.HasIndex(x => x.ObjectId);
            builder.HasIndex(x => new { x.IdentityId, x.Started });
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the planning figures of objects.
    /// </summary>
    internal class ObjectPlanningConfiguration : IEntityTypeConfiguration<ObjectPlanning>
    {
        /// <summary>
        /// Configures the entity type mapping for the ObjectPlanning entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ObjectPlanning> builder)
        {
            builder.ToTable("ObjectPlanning");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.Property(x => x.ObjectId).HasColumnName("Object").IsRequired();
            builder.HasIndex(x => x.ObjectId).IsUnique();
            builder.HasOne<Object>().WithMany().HasForeignKey(x => x.ObjectId).HasPrincipalKey(o => o.Id).OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.OriginalEstimateMinutes).HasColumnName("OriginalEstimate");
            builder.Property(x => x.RemainingEstimateMinutes).HasColumnName("RemainingEstimate");
            builder.Property(x => x.MilestoneId).HasColumnName("Milestone");

            // an object outlives the milestone it was due with
            builder.HasOne<Milestone>().WithMany().HasForeignKey(x => x.MilestoneId).HasPrincipalKey(m => m.Id).OnDelete(DeleteBehavior.SetNull);
            builder.HasIndex(x => x.MilestoneId);
            builder.Property(x => x.Updated).HasColumnName("Updated").IsRequired();
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the milestones.
    /// </summary>
    internal class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
    {
        /// <summary>
        /// Configures the entity type mapping for the Milestone entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Milestone> builder)
        {
            builder.ToTable("Milestone");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.WorkspaceId).HasColumnName("Workspace").IsRequired();
            builder.HasOne<Workspace>().WithMany().HasForeignKey(x => x.WorkspaceId).HasPrincipalKey(w => w.Id).OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(128);
            builder.HasIndex(x => new { x.WorkspaceId, x.Name }).IsUnique();
            builder.Property(x => x.Description).HasColumnName("Description").HasMaxLength(2048);
            builder.Property(x => x.Due).HasColumnName("Due");
            builder.Property(x => x.State).HasColumnName("State").IsRequired();
            builder.Property(x => x.Created).HasColumnName("Created").IsRequired();
            builder.Property(x => x.Updated).HasColumnName("Updated").IsRequired();
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the planning settings of workspaces.
    /// </summary>
    internal class WorkspacePlanningConfiguration : IEntityTypeConfiguration<WorkspacePlanning>
    {
        /// <summary>
        /// Configures the entity type mapping for the WorkspacePlanning entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<WorkspacePlanning> builder)
        {
            builder.ToTable("WorkspacePlanning");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.Property(x => x.WorkspaceId).HasColumnName("Workspace").IsRequired();
            builder.HasIndex(x => x.WorkspaceId).IsUnique();
            builder.HasOne<Workspace>().WithMany().HasForeignKey(x => x.WorkspaceId).HasPrincipalKey(w => w.Id).OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.TimeTracking).HasColumnName("TimeTracking").IsRequired();
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the capacities of people.
    /// </summary>
    internal class IdentityCapacityConfiguration : IEntityTypeConfiguration<IdentityCapacity>
    {
        /// <summary>
        /// Configures the entity type mapping for the IdentityCapacity entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<IdentityCapacity> builder)
        {
            builder.ToTable("IdentityCapacity");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.Property(x => x.IdentityId).HasColumnName("Identity").IsRequired();
            builder.HasIndex(x => x.IdentityId).IsUnique();
            builder.Property(x => x.WeeklyMinutes).HasColumnName("WeeklyMinutes").IsRequired();
        }
    }
}
