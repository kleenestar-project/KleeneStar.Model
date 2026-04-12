using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the Identity entity type.
    /// </summary>
    internal class IdentityConfiguration : IEntityTypeConfiguration<Identity>
    {
        /// <summary>
        /// Configuration of the field entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Identity> builder)
        {
            builder.ToTable("Identity");

            // Primary key
            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            // Business GUID
            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Email)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .HasColumnName("PasswordHash")
                .IsRequired()
                .HasMaxLength(512);

            // m:n relation via IdentityGroupMembership
            builder.HasMany(x => x.GroupMemberships)
                .WithOne(x => x.Identity)
                .HasForeignKey(x => x.IdentityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore interface projection
            builder.Ignore(x => ((IIdentity)x).Groups);
        }
    }
}