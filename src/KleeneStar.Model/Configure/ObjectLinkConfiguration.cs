using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the <see cref="ObjectLink"/> entity.
    /// </summary>
    internal class ObjectLinkConfiguration : IEntityTypeConfiguration<ObjectLink>
    {
        /// <summary>
        /// Configures the object-link entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<ObjectLink> builder)
        {
            builder.ToTable("ObjectLink");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.SourceObjectId)
                .HasColumnName("Source")
                .IsRequired();

            builder.HasOne(x => x.SourceObject)
                .WithMany()
                .HasForeignKey(x => x.SourceObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.TargetObjectId)
                .HasColumnName("Target")
                .IsRequired();

            builder.HasOne(x => x.TargetObject)
                .WithMany()
                .HasForeignKey(x => x.TargetObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.RelationType)
                .HasColumnName("RelationType");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => x.SourceObjectId);
            builder.HasIndex(x => x.TargetObjectId);
            builder.HasIndex(x => new { x.SourceObjectId, x.TargetObjectId, x.RelationType })
                .IsUnique();
        }
    }
}
