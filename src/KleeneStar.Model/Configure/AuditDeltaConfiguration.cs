using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="AuditDelta"/>.
    /// </summary>
    /// <remarks>
    /// The relationship to <see cref="AuditEvent"/> is declared on the principal side (see
    /// <see cref="AuditEventConfiguration"/>); this configuration owns the column mapping and
    /// the indexes the replay reads by. <see cref="AuditDelta.AttributeId"/> carries no foreign
    /// key for the same reason <see cref="AuditEvent.TargetId"/> does not - a field that is
    /// deleted must not take the record of what it once held with it.
    /// </remarks>
    internal class AuditDeltaConfiguration : IEntityTypeConfiguration<AuditDelta>
    {
        /// <summary>
        /// Configures the audit delta entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<AuditDelta> builder)
        {
            builder.ToTable("AuditDelta");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.EventId)
                .HasColumnName("Event")
                .IsRequired();

            builder.Property(x => x.Kind)
                .HasColumnName("Kind")
                .IsRequired();

            builder.Property(x => x.Attribute)
                .HasColumnName("Attribute")
                .IsRequired();

            builder.Property(x => x.AttributeId)
                .HasColumnName("AttributeRef");

            builder.Property(x => x.ValueKind)
                .HasColumnName("ValueKind")
                .IsRequired();

            builder.Property(x => x.OldValue)
                .HasColumnName("OldValue");

            builder.Property(x => x.NewValue)
                .HasColumnName("NewValue");

            builder.Property(x => x.Ordinal)
                .HasColumnName("Ordinal")
                .HasDefaultValue(0);

            builder.Ignore(x => x.Field);

            builder.HasIndex(x => new { x.EventId, x.Ordinal });

            // the projection of one record replays the deltas of one attribute across many
            // events, so the attribute is the leading column of its own index
            builder.HasIndex(x => x.Attribute);
        }
    }
}
