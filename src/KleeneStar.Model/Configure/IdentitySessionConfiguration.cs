using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="IdentitySession"/>.
    /// </summary>
    internal class IdentitySessionConfiguration : IEntityTypeConfiguration<IdentitySession>
    {
        /// <summary>
        /// Configures the entity type mapping for the IdentitySession entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<IdentitySession> builder)
        {
            builder.ToTable("IdentitySession");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.OwnerId)
                .HasColumnName("Owner")
                .IsRequired();

            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Device)
                .HasColumnName("Device")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Client)
                .HasColumnName("Client")
                .HasMaxLength(128);

            builder.Property(x => x.Mobile)
                .HasColumnName("Mobile")
                .IsRequired();

            builder.Property(x => x.Location)
                .HasColumnName("Location")
                .HasMaxLength(128);

            builder.Property(x => x.IpAddress)
                .HasColumnName("IpAddress")
                .HasMaxLength(64);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.LastActive)
                .HasColumnName("LastActive")
                .IsRequired();

            builder.Property(x => x.Current)
                .HasColumnName("Current")
                .IsRequired();

            builder.HasIndex(x => x.OwnerId);
        }
    }
}
