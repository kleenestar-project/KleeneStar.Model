using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="Holiday"/>.
    /// </summary>
    internal class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
    {
        /// <summary>
        /// Configures the entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Holiday> builder)
        {
            builder.ToTable("Holiday");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Date)
                .HasColumnName("Date")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Region)
                .HasColumnName("Region")
                .HasMaxLength(16);

            builder.Property(x => x.Enabled)
                .HasColumnName("Enabled");

            builder.Property(x => x.CalendarId)
                .HasColumnName("Calendar")
                .IsRequired();

            builder.HasIndex(x => new { x.CalendarId, x.Date });
        }
    }
}
