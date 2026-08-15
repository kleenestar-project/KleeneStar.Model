using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="AccessToken"/>.
    /// </summary>
    internal class AccessTokenConfiguration : IEntityTypeConfiguration<AccessToken>
    {
        /// <summary>
        /// Configures the entity type mapping for the AccessToken entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AccessToken> builder)
        {
            builder.ToTable("AccessToken");

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

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Prefix)
                .HasColumnName("Prefix")
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(x => x.TokenHash)
                .HasColumnName("TokenHash")
                .HasMaxLength(512);

            builder.Property(x => x.Scopes)
                .HasColumnName("Scopes")
                .HasMaxLength(512);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.LastUsed)
                .HasColumnName("LastUsed");

            builder.Property(x => x.Expires)
                .HasColumnName("Expires");

            builder.Property(x => x.Revoked)
                .HasColumnName("Revoked")
                .IsRequired();

            // derived from Revoked and Expires — never stored
            builder.Ignore(x => x.State);

            builder.HasIndex(x => x.OwnerId);
        }
    }
}
