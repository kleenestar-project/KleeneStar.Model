using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the <see cref="SlaScopeRule"/> entity.
    /// </summary>
    internal class SlaScopeRuleConfiguration : IEntityTypeConfiguration<SlaScopeRule>
    {
        /// <summary>
        /// Configures the scope-rule entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<SlaScopeRule> builder)
        {
            builder.ToTable("SlaScopeRule");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.RuleType)
                .HasColumnName("RuleType");

            builder.Property(x => x.Value)
                .HasColumnName("Value")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.PolicyId)
                .HasColumnName("Policy")
                .IsRequired();

            builder.HasIndex(x => x.PolicyId);
        }
    }
}
