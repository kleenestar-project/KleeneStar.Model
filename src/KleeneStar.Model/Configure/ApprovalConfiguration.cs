using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the approval policies.
    /// </summary>
    internal class ApprovalPolicyConfiguration : IEntityTypeConfiguration<ApprovalPolicy>
    {
        /// <summary>
        /// Configures the entity type mapping for the ApprovalPolicy entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ApprovalPolicy> builder)
        {
            builder.ToTable("ApprovalPolicy");

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

            builder.HasOne<Workspace>()
                .WithMany()
                .HasForeignKey(x => x.WorkspaceId)
                .HasPrincipalKey(w => w.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ClassId)
                .HasColumnName("Class")
                .IsRequired();

            builder.HasOne<Class>()
                .WithMany()
                .HasForeignKey(x => x.ClassId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(128);

            builder.HasIndex(x => new { x.WorkspaceId, x.Name })
                .IsUnique();

            builder.Property(x => x.Description)
                .HasColumnName("Description")
                .HasMaxLength(1024);

            builder.Property(x => x.Steps)
                .HasColumnName("Steps")
                .IsRequired()
                .HasMaxLength(8192);

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.AllowSelfApproval)
                .HasColumnName("AllowSelfApproval")
                .IsRequired();

            builder.Property(x => x.RequestStatusId)
                .HasColumnName("RequestStatus");

            builder.Property(x => x.ApprovedStatusId)
                .HasColumnName("ApprovedStatus");

            builder.Property(x => x.RejectedStatusId)
                .HasColumnName("RejectedStatus");

            builder.Property(x => x.CreatedById)
                .HasColumnName("CreatedBy");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the approvals.
    /// </summary>
    internal class ApprovalConfiguration : IEntityTypeConfiguration<Approval>
    {
        /// <summary>
        /// Configures the entity type mapping for the Approval entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Approval> builder)
        {
            builder.ToTable("Approval");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.HasIndex(x => x.Id)
                .IsUnique();

            builder.Property(x => x.ObjectId)
                .HasColumnName("Object")
                .IsRequired();

            // the approvals of an object are part of its record and go with it
            builder.HasOne<Object>()
                .WithMany()
                .HasForeignKey(x => x.ObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ObjectId, x.State });

            builder.HasIndex(x => x.State);

            // no foreign key: the record outlives its policy
            builder.Property(x => x.PolicyId)
                .HasColumnName("Policy")
                .IsRequired();

            builder.Property(x => x.PolicyName)
                .HasColumnName("PolicyName")
                .HasMaxLength(128);

            builder.Property(x => x.Steps)
                .HasColumnName("Steps")
                .IsRequired();

            builder.Property(x => x.AllowSelfApproval)
                .HasColumnName("AllowSelfApproval")
                .IsRequired();

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.CurrentStep)
                .HasColumnName("CurrentStep")
                .IsRequired();

            builder.Property(x => x.RequestedById)
                .HasColumnName("RequestedBy")
                .IsRequired();

            builder.Property(x => x.Note)
                .HasColumnName("Note")
                .HasMaxLength(2048);

            builder.Property(x => x.Requested)
                .HasColumnName("Requested")
                .IsRequired();

            builder.Property(x => x.StepStarted)
                .HasColumnName("StepStarted")
                .IsRequired();

            builder.Property(x => x.Completed)
                .HasColumnName("Completed");

            builder.Property(x => x.Outcome)
                .HasColumnName("Outcome")
                .HasMaxLength(1024);

            builder.Property(x => x.Version)
                .HasColumnName("Version")
                .IsConcurrencyToken()
                .IsRequired();

            builder.HasMany(x => x.Decisions)
                .WithOne()
                .HasForeignKey(x => x.ApprovalId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the approval decisions.
    /// </summary>
    internal class ApprovalDecisionConfiguration : IEntityTypeConfiguration<ApprovalDecision>
    {
        /// <summary>
        /// Configures the entity type mapping for the ApprovalDecision entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ApprovalDecision> builder)
        {
            builder.ToTable("ApprovalDecision");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.ApprovalId)
                .HasColumnName("Approval")
                .IsRequired();

            builder.Property(x => x.Step)
                .HasColumnName("Step")
                .IsRequired();

            builder.Property(x => x.IdentityId)
                .HasColumnName("Identity")
                .IsRequired();

            // one vote per approver and step, whatever reaches the database first
            builder.HasIndex(x => new { x.ApprovalId, x.Step, x.IdentityId })
                .IsUnique();

            builder.Property(x => x.Verdict)
                .HasColumnName("Verdict")
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasColumnName("Reason")
                .HasMaxLength(2048);

            builder.Property(x => x.Decided)
                .HasColumnName("Decided")
                .IsRequired();
        }
    }
}
