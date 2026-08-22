using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="AuditEvent"/>.
    /// </summary>
    internal class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
    {
        /// <summary>
        /// Configures the audit event entity. Establishes:
        /// <list type="bullet">
        /// <item>The one-to-many relationship to <see cref="AuditDelta"/> (cascade - a delta has
        /// no meaning without the event it belongs to).</item>
        /// <item>A unique index on the sequence, so the log cannot grow two events claiming the
        /// same position. This is the backstop that turns a race between two concurrent
        /// appenders into a failed transaction instead of an ambiguous order.</item>
        /// <item>Indexes on (Timestamp), (Category, Timestamp), (Origin, Timestamp),
        /// (TargetType, TargetId, Sequence) and (CorrelationId) - the five ways the log is
        /// actually read: chronologically, filtered by area or origin, as the trail of one
        /// record, and as the events of one activity.</item>
        /// </list>
        /// No foreign key is declared for <see cref="AuditEvent.ActorId"/> or
        /// <see cref="AuditEvent.TargetId"/>: the log is an append-only trail that has to
        /// survive the deletion of both the identity and the record it names (see the remarks on
        /// <see cref="AuditEvent"/>). The <see cref="AuditEvent.Actor"/> navigation property is
        /// consequently ignored by the model and resolved on read instead.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<AuditEvent> builder)
        {
            builder.ToTable("AuditEvent");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Sequence)
                .HasColumnName("Sequence")
                .IsRequired();

            builder.Property(x => x.Timestamp)
                .HasColumnName("Timestamp")
                .IsRequired();

            builder.Property(x => x.Origin)
                .HasColumnName("Origin")
                .IsRequired();

            builder.Property(x => x.Category)
                .HasColumnName("Category")
                .IsRequired();

            builder.Property(x => x.Action)
                .HasColumnName("Action")
                .IsRequired();

            builder.Property(x => x.Outcome)
                .HasColumnName("Outcome")
                .IsRequired();

            builder.Property(x => x.Severity)
                .HasColumnName("Severity")
                .IsRequired();

            builder.Property(x => x.ActorId)
                .HasColumnName("Actor");

            builder.Property(x => x.ActorName)
                .HasColumnName("ActorName");

            builder.Property(x => x.Agent)
                .HasColumnName("Agent");

            builder.Property(x => x.ClientAddress)
                .HasColumnName("ClientAddress");

            builder.Property(x => x.TargetType)
                .HasColumnName("TargetType")
                .IsRequired();

            builder.Property(x => x.TargetId)
                .HasColumnName("Target");

            builder.Property(x => x.TargetKey)
                .HasColumnName("TargetKey");

            builder.Property(x => x.TargetRevision)
                .HasColumnName("TargetRevision");

            builder.Property(x => x.CorrelationId)
                .HasColumnName("Correlation")
                .IsRequired();

            builder.Property(x => x.CausationId)
                .HasColumnName("Causation");

            builder.Property(x => x.PreviousHash)
                .HasColumnName("PreviousHash")
                .HasMaxLength(64);

            builder.Property(x => x.Hash)
                .HasColumnName("Hash")
                .IsRequired()
                .HasMaxLength(64);

            // the trail outlives the rows it describes, so neither reference is a foreign key
            builder.Ignore(x => x.Actor);

            builder.HasMany(x => x.Deltas)
                .WithOne(x => x.Event)
                .HasForeignKey(x => x.EventId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Sequence)
                .IsUnique();

            builder.HasIndex(x => x.Timestamp);
            builder.HasIndex(x => new { x.Category, x.Timestamp });
            builder.HasIndex(x => new { x.Origin, x.Timestamp });
            builder.HasIndex(x => new { x.TargetType, x.TargetId, x.Sequence });
            builder.HasIndex(x => x.CorrelationId);
        }
    }
}
