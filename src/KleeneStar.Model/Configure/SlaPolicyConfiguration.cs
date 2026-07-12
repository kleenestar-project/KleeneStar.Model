using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the <see cref="SlaPolicy"/> entity.
    /// </summary>
    internal class SlaPolicyConfiguration : IEntityTypeConfiguration<SlaPolicy>
    {
        /// <summary>
        /// Configures the policy entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<SlaPolicy> builder)
        {
            builder.ToTable("SlaPolicy");

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

            builder.Property(x => x.State)
                .HasColumnName("State");

            builder.Property(x => x.Priority)
                .HasColumnName("Priority");

            builder.Property(x => x.CalendarId)
                .HasColumnName("Calendar");

            builder.HasOne(x => x.Calendar)
                .WithMany()
                .HasForeignKey(x => x.CalendarId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.Notifications)
                .HasColumnName("Notifications");

            builder.Property(x => x.PauseOn)
                .HasColumnName("PauseOn");

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

            builder.Property(x => x.OwnerId)
                .HasColumnName("Owner");

            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Targets)
                .WithOne(t => t.Policy)
                .HasForeignKey(t => t.PolicyId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Scope)
                .WithOne(s => s.Policy)
                .HasForeignKey(s => s.PolicyId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Escalations)
                .WithOne(e => e.Policy)
                .HasForeignKey(e => e.PolicyId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ClassId, x.Name })
                .IsUnique();
        }
    }
}
