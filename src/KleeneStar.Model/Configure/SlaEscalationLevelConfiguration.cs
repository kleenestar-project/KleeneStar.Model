using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the <see cref="SlaEscalationLevel"/> entity.
    /// </summary>
    internal class SlaEscalationLevelConfiguration : IEntityTypeConfiguration<SlaEscalationLevel>
    {
        /// <summary>
        /// Configures the escalation-level entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<SlaEscalationLevel> builder)
        {
            builder.ToTable("SlaEscalationLevel");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Level)
                .HasColumnName("Level")
                .IsRequired();

            builder.Property(x => x.AfterValue)
                .HasColumnName("AfterValue")
                .IsRequired();

            builder.Property(x => x.Unit)
                .HasColumnName("Unit");

            builder.Property(x => x.Notify)
                .HasColumnName("Notify")
                .HasMaxLength(512);

            builder.Property(x => x.PolicyId)
                .HasColumnName("Policy")
                .IsRequired();

            builder.HasIndex(x => new { x.PolicyId, x.Level })
                .IsUnique();
        }
    }
}
