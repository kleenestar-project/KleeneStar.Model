using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the maintenance entity type.
    /// </summary>
    internal class MaintenanceConfiguration : IEntityTypeConfiguration<Maintenance>
    {
        /// <summary>
        /// Configuration of the maintenance entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Maintenance> builder)
        {
            builder.ToTable("Maintenance");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Enabled)
                .HasColumnName("Enabled");

            builder.Property(x => x.Message)
                .HasColumnName("Message");

            // the notice is a singleton, so the guid carries a unique index rather than merely
            // identifying a row among many
            builder.HasIndex(x => x.Id)
                .IsUnique();
        }
    }
}
