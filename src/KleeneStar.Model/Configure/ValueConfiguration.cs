using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the value entity type.
    /// </summary>
    internal class ValueConfiguration : IEntityTypeConfiguration<Value>
    {
        /// <summary>
        /// Configuration of the value entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Value> builder)
        {
            builder.ToTable("Value");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.ObjectId)
                .HasColumnName("Object")
                .IsRequired();

            builder.Property(x => x.FieldId)
                .HasColumnName("Field")
                .IsRequired();

            builder.Property(x => x.Data)
                .HasColumnName("Data");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasOne(x => x.Object)
                .WithMany()
                .HasForeignKey(x => x.ObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Field)
                .WithMany()
                .HasForeignKey(x => x.FieldId)
                .HasPrincipalKey(f => f.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ObjectId, x.FieldId })
                .IsUnique();
        }
    }
}
