using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the form entity type.
    /// </summary>
    internal class FormConfiguration : IEntityTypeConfiguration<Form>
    {
        /// <summary>
        /// Configuration of the form entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Form> builder)
        {
            builder.ToTable("Form");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.Icon)
                .HasColumnName("Icon")
                .HasMaxLength(256)
                .HasConversion
                (
                    icon => icon != null && icon.Uri != null ? icon.Uri.ToString() : null,
                    uri => string.IsNullOrEmpty(uri) ? null : ImageIcon.FromString(uri)
                );

            builder.Property(x => x.State)
                .HasColumnName("State");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.ClassId)
                .HasColumnName("Class")
                .IsRequired();

            builder.HasOne(x => x.Class)
                .WithMany()
                .HasForeignKey(x => x.ClassId)
                .HasPrincipalKey(w => w.Id);

            builder.HasIndex(x => new { x.ClassId, x.Name })
                .IsUnique();
        }
    }
}
