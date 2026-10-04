using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the insight entity type.
    /// </summary>
    internal class InsightConfiguration : IEntityTypeConfiguration<Insight>
    {
        /// <summary>
        /// Configuration of the insight entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Insight> builder)
        {
            builder.ToTable("Insight");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(64);

            // the key of the open type catalog; the default is what every insight that was a
            // dashboard carries, so the migration fills existing rows with it
            builder.Property(x => x.Type)
                .HasColumnName("Type")
                .IsRequired()
                .HasMaxLength(64)
                .HasDefaultValue(Insight.DashboardType);

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            // MANY-TO-MANY: Insight <-> Category
            builder.HasMany(d => d.Categories)
                .WithMany(c => c.Insights)
                .UsingEntity<Dictionary<string, object>>
                (
                    "InsightCategory",
                    j => j
                        .HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Insight>()
                        .WithMany()
                        .HasForeignKey("InsightId")
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

            // ONE-TO-MANY: Insight -> DashboardColumn (the content of the dashboard type)
            builder.HasMany(d => d.Columns)
                .WithOne(c => c.Insight)
                .HasForeignKey(c => c.InsightId)
                .HasPrincipalKey(d => d.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
