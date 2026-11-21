using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the object creation record.
    /// </summary>
    internal class ObjectCreationConfiguration : IEntityTypeConfiguration<ObjectCreation>
    {
        /// <summary>
        /// Configures the entity type mapping for the ObjectCreation entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ObjectCreation> builder)
        {
            builder.ToTable("ObjectCreation");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Key)
                .HasColumnName("Key")
                .IsRequired()
                .HasMaxLength(64);

            // the uniqueness is what makes a second submission find the first instead of
            // running beside it - two requests racing for the same key cannot both insert
            builder.HasIndex(x => x.Key)
                .IsUnique();

            builder.Property(x => x.TemplateId)
                .HasColumnName("Template")
                .HasMaxLength(36);

            builder.Property(x => x.RootObjectId)
                .HasColumnName("RootObject")
                .HasMaxLength(36);

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.ObjectIds)
                .HasColumnName("Objects");

            builder.Property(x => x.RelationIds)
                .HasColumnName("Relations");

            builder.Property(x => x.Error)
                .HasColumnName("Error");

            builder.Property(x => x.IdentityId)
                .HasColumnName("Identity")
                .HasMaxLength(36);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Finished)
                .HasColumnName("Finished");
        }
    }
}
