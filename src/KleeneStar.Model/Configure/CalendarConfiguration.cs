using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="Calendar"/>.
    /// </summary>
    internal class CalendarConfiguration : IEntityTypeConfiguration<Calendar>
    {
        /// <summary>
        /// Configures the entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Calendar> builder)
        {
            builder.ToTable("Calendar");

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
                .HasMaxLength(128);

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.TimeZone)
                .HasColumnName("TimeZone")
                .HasMaxLength(64);

            builder.Property(x => x.Region)
                .HasColumnName("Region")
                .HasMaxLength(16);

            builder.Property(x => x.State)
                .HasColumnName("State");

            builder.Property(x => x.IsDefault)
                .HasColumnName("IsDefault");

            builder.Property(x => x.Icon)
                .HasColumnName("Icon")
                .HasMaxLength(256)
                .HasConversion
                (
                    icon => icon != null && icon.Uri != null ? icon.Uri.ToString() : null,
                    uri => string.IsNullOrEmpty(uri) ? null : ImageIcon.FromString(uri)
                );

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.Property(x => x.ClassId)
                .HasColumnName("Class")
                .IsRequired();

            builder.HasOne(x => x.Class)
                .WithMany()
                .HasForeignKey(x => x.ClassId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.BusinessHours)
                .WithOne(s => s.Calendar)
                .HasForeignKey(s => s.CalendarId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Holidays)
                .WithOne(h => h.Calendar)
                .HasForeignKey(h => h.CalendarId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ClassId, x.Name })
                .IsUnique();
        }
    }
}
