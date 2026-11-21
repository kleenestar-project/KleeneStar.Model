using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="TemplateClass"/>.
    /// </summary>
    internal class TemplateClassConfiguration : IEntityTypeConfiguration<TemplateClass>
    {
        /// <summary>
        /// Configures the entity type mapping for the TemplateClass entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<TemplateClass> builder)
        {
            builder.ToTable("TemplateClass");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.TemplateId)
                .HasColumnName("Template")
                .IsRequired();

            // the assignment belongs to the template and leaves with it
            builder.HasOne(x => x.Template)
                .WithMany()
                .HasForeignKey(x => x.TemplateId)
                .HasPrincipalKey(t => t.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ClassId)
                .HasColumnName("Class")
                .IsRequired();

            // a removed class is no longer served; the template stays with its other classes
            builder.HasOne(x => x.Class)
                .WithMany()
                .HasForeignKey(x => x.ClassId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.TemplateId, x.ClassId }).IsUnique();
        }
    }
}
