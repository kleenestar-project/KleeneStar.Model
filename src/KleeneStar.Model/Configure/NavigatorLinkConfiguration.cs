using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the navigator link entity type.
    /// </summary>
    internal class NavigatorLinkConfiguration : IEntityTypeConfiguration<NavigatorLink>
    {
        /// <summary>
        /// Configuration of the navigator link entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<NavigatorLink> builder)
        {
            builder.ToTable("NavigatorLink");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Icon)
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
                .HasMaxLength(64);

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.Uri)
                .HasColumnName("Uri")
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(x => x.Ordinal)
                .HasColumnName("Ordinal");

            builder.Property(x => x.State)
                .HasColumnName("State");

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
