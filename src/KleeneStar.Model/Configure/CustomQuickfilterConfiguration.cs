using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="CustomQuickfilter"/>.
    /// </summary>
    internal class CustomQuickfilterConfiguration : IEntityTypeConfiguration<CustomQuickfilter>
    {
        /// <summary>
        /// Configures the entity type mapping for the custom quickfilter entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<CustomQuickfilter> builder)
        {
            builder.ToTable("CustomQuickfilter");

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
                .HasMaxLength(256);

            builder.Property(x => x.ViewKey)
                .HasColumnName("ViewKey")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.ContextKey)
                .HasColumnName("ContextKey")
                .HasMaxLength(256);

            builder.Property(x => x.Query)
                .HasColumnName("Query")
                .IsRequired();

            builder.Property(x => x.OwnerId)
                .HasColumnName("Owner")
                .IsRequired();

            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Shared)
                .HasColumnName("Shared")
                .IsRequired();

            builder.Property(x => x.Ordinal)
                .HasColumnName("Ordinal");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            // the bar is rebuilt on every request for one view, so the lookup that fills it is the
            // one worth indexing
            builder.HasIndex(x => new { x.ViewKey, x.ContextKey });

            builder.HasIndex(x => x.Id)
                .IsUnique();
        }
    }
}
