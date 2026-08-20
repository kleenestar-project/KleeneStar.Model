using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the branding entity type.
    /// </summary>
    internal class BrandingConfiguration : IEntityTypeConfiguration<Branding>
    {
        /// <summary>
        /// Configuration of the branding entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Branding> builder)
        {
            builder.ToTable("Branding");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Title)
                .HasColumnName("Title")
                .HasMaxLength(64);

            builder.Property(x => x.Icon)
                .HasColumnName("Icon")
                .HasMaxLength(256)
                .HasConversion
                (
                    icon => icon != null && icon.Uri != null ? icon.Uri.ToString() : null,
                    uri => string.IsNullOrEmpty(uri) ? null : ImageIcon.FromString(uri)
                );

            // the identity is a singleton, so the guid carries a unique index rather than merely
            // identifying a row among many
            builder.HasIndex(x => x.Id)
                .IsUnique();
        }
    }
}
