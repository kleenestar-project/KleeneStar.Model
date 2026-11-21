using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="PasswordReset"/>.
    /// </summary>
    internal class PasswordResetConfiguration : IEntityTypeConfiguration<PasswordReset>
    {
        /// <summary>
        /// Configures the entity type mapping for the PasswordReset entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PasswordReset> builder)
        {
            builder.ToTable("PasswordReset");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.IdentityId)
                .HasColumnName("Identity")
                .IsRequired();

            // the link dies with the account it was issued for
            builder.HasOne(x => x.Identity)
                .WithMany()
                .HasForeignKey(x => x.IdentityId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.IssuedById)
                .HasColumnName("IssuedBy")
                .HasMaxLength(36);

            builder.Property(x => x.TokenHash)
                .HasColumnName("TokenHash")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Expires)
                .HasColumnName("Expires")
                .IsRequired();

            builder.Property(x => x.Used)
                .HasColumnName("Used");

            builder.HasIndex(x => x.TokenHash)
                .IsUnique();

            builder.HasIndex(x => x.IdentityId);
        }
    }
}
