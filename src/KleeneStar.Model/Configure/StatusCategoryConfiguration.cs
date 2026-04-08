using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration 
    /// for the status category entity type.
    /// </summary>
    internal class StatusCategoryConfiguration : IEntityTypeConfiguration<StatusCategory>
    {
        public void Configure(EntityTypeBuilder<StatusCategory> builder)
        {
            builder.ToTable("StatusCategory");

            // primary key
            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            // guid
            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            // name
            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(64);

            // description
            builder.Property(x => x.Description)
                .HasColumnName("Description");

            // icon
            builder.Property(x => x.Icon)
                .HasColumnName("Icon")
                .HasMaxLength(256)
                .HasConversion(
                    icon => icon != null && icon.Uri != null ? icon.Uri.ToString() : null,
                    uri => string.IsNullOrEmpty(uri) ? null : ImageIcon.FromString(uri)
                );

            // color (e.g., #ffffffff)
            builder.Property(x => x.Color)
                .HasMaxLength(9);

            // created / updated
            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();
        }
    }
}