using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the <see cref="SlaTarget"/> entity.
    /// </summary>
    internal class SlaTargetConfiguration : IEntityTypeConfiguration<SlaTarget>
    {
        /// <summary>
        /// Configures the target entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<SlaTarget> builder)
        {
            builder.ToTable("SlaTarget");

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
                .HasMaxLength(64);

            builder.Property(x => x.Kind)
                .HasColumnName("Kind");

            builder.Property(x => x.TargetValue)
                .HasColumnName("TargetValue")
                .IsRequired();

            builder.Property(x => x.Unit)
                .HasColumnName("Unit");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.Property(x => x.PolicyId)
                .HasColumnName("Policy")
                .IsRequired();

            builder.HasIndex(x => new { x.PolicyId, x.Kind });
        }
    }
}
