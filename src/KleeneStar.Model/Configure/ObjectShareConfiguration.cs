using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="ObjectShare"/>.
    /// </summary>
    internal class ObjectShareConfiguration : IEntityTypeConfiguration<ObjectShare>
    {
        /// <summary>
        /// Configures the entity type mapping for the ObjectShare entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ObjectShare> builder)
        {
            builder.ToTable("ObjectShare");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.ObjectId)
                .HasColumnName("Object")
                .IsRequired();

            builder.HasOne(x => x.Object)
                .WithMany()
                .HasForeignKey(x => x.ObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.IdentityId)
                .HasColumnName("Identity")
                .IsRequired();

            builder.HasOne(x => x.Identity)
                .WithMany()
                .HasForeignKey(x => x.IdentityId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ObjectId, x.IdentityId }).IsUnique();
        }
    }
}
