using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="SavedSearch"/>.
    /// </summary>
    internal class SavedSearchConfiguration : IEntityTypeConfiguration<SavedSearch>
    {
        /// <summary>
        /// Configures the entity type mapping for the SavedSearch entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<SavedSearch> builder)
        {
            builder.ToTable("SavedSearch");

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

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.Query)
                .HasColumnName("Query");

            builder.Property(x => x.OwnerId)
                .HasColumnName("Owner")
                .IsRequired();

            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Starred)
                .HasColumnName("Starred")
                .IsRequired();

            builder.Property(x => x.LastUsed)
                .HasColumnName("LastUsed")
                .IsRequired();

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => x.OwnerId);
        }
    }
}
