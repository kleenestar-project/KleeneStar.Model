using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the form tab entity type.
    /// </summary>
    internal class FormTabConfiguration : IEntityTypeConfiguration<FormTab>
    {
        /// <summary>
        /// Configuration of the form tab entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<FormTab> builder)
        {
            builder.ToTable("FormTab");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.FormId)
                .HasColumnName("Form")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Position)
                .HasColumnName("Position")
                .IsRequired();

            builder.HasMany(x => x.Elements)
                .WithOne()
                .HasForeignKey(e => e.FormTabId)
                .HasPrincipalKey(t => t.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.FormId, x.Position });
        }
    }
}
