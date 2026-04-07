using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the dashboard entity type.
    /// </summary>
    internal class DashboardConfiguration : IEntityTypeConfiguration<Dashboard>
    {
        /// <summary>
        /// Configuration of the dashboard entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Dashboard> builder)
        {
            builder.ToTable("Dashboard");

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

            // MANY-TO-MANY: Dashboard <-> Category
            builder.HasMany(d => d.Categories)
                .WithMany(c => c.Dashboards)
                .UsingEntity<Dictionary<string, object>>
                (
                    "DashboardCategory",
                    j => j
                        .HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Dashboard>()
                        .WithMany()
                        .HasForeignKey("DashboardId")
                        .OnDelete(DeleteBehavior.Cascade)
                );

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

            builder.HasIndex(x => x.Name)
                .IsUnique();

            // ONE-TO-MANY: Dashboard -> Widget
            builder.HasMany(d => d.Widgets)
                .WithOne(w => w.Dashboard)
                .HasForeignKey(w => w.DashboardId)
                .HasPrincipalKey(d => d.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
