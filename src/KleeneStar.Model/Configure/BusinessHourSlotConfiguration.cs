using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="BusinessHourSlot"/>.
    /// </summary>
    internal class BusinessHourSlotConfiguration : IEntityTypeConfiguration<BusinessHourSlot>
    {
        /// <summary>
        /// Configures the entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<BusinessHourSlot> builder)
        {
            builder.ToTable("BusinessHourSlot");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.DayOfWeek)
                .HasColumnName("DayOfWeek");

            builder.Property(x => x.Enabled)
                .HasColumnName("Enabled");

            builder.Property(x => x.StartTime)
                .HasColumnName("StartTime");

            builder.Property(x => x.EndTime)
                .HasColumnName("EndTime");

            builder.Property(x => x.CalendarId)
                .HasColumnName("Calendar")
                .IsRequired();

            builder.HasIndex(x => new { x.CalendarId, x.DayOfWeek })
                .IsUnique();
        }
    }
}
