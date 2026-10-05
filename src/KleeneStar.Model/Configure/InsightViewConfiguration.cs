using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the tabs of an insight.
    /// </summary>
    internal class InsightViewConfiguration : IEntityTypeConfiguration<InsightView>
    {
        /// <summary>
        /// Configuration of the insight view entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<InsightView> builder)
        {
            builder.ToTable("InsightView");

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
                .HasMaxLength(64);

            builder.Property(x => x.ViewType)
                .HasColumnName("ViewType")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Configuration)
                .HasColumnName("Configuration");

            builder.Property(x => x.Order)
                .HasColumnName("Order")
                .IsRequired();

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.InsightId)
                .HasColumnName("Insight")
                .IsRequired();

            // the tabs go with their insight; the insight carries no collection of them, so the
            // generic update of an insight never touches its tabs
            builder.HasOne(x => x.Insight)
                .WithMany()
                .HasForeignKey(x => x.InsightId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => x.Id)
                .IsUnique();

            builder.HasIndex(x => new { x.InsightId, x.Name })
                .IsUnique();
        }
    }
}
