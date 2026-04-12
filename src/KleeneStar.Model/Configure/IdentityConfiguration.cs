using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebCore.WebIdentity;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the Identity entity type.
    /// </summary>
    internal class IdentityConfiguration : IEntityTypeConfiguration<Identity>
    {
        /// <summary>
        /// Configuration of the field entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Identity> builder)
        {
            builder.ToTable("Identity");

            // Primary key
            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            // business GUID
            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Avatar)
                .HasColumnName("Icon")
                .HasMaxLength(256)
                .HasConversion
                (
                    icon => icon != null && icon.Uri != null ? icon.Uri.ToString() : null,
                    uri => string.IsNullOrEmpty(uri) ? null : ImageIcon.FromString(uri)
                );

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Email)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .HasColumnName("PasswordHash")
                .IsRequired()
                .HasMaxLength(512);

            // m:n relation via IdentityGroupMembership
            builder.HasMany(x => x.GroupMemberships)
                .WithOne(x => x.Identity)
                .HasForeignKey(x => x.IdentityId)
                .OnDelete(DeleteBehavior.Cascade);

            // ignore interface projection
            builder.Ignore(x => ((IIdentity)x).Groups);
        }
    }
}