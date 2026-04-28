using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the form element entity hierarchy.
    /// </summary>
    /// <remarks>
    /// Maps the abstract <see cref="FormElement"/> base type and its concrete subtypes
    /// <see cref="FormGroupElement"/> and <see cref="FormFieldRefElement"/> to a single
    /// table using table-per-hierarchy (TPH) inheritance with a discriminator column.
    /// </remarks>
    internal class FormElementConfiguration : IEntityTypeConfiguration<FormElement>
    {
        /// <summary>
        /// Configuration of the form element hierarchy.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<FormElement> builder)
        {
            builder.ToTable("FormElement");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.FormTabId)
                .HasColumnName("Tab")
                .IsRequired();

            builder.Property(x => x.ParentElementId)
                .HasColumnName("Parent")
                .IsRequired(false);

            builder.Property(x => x.Position)
                .HasColumnName("Position")
                .IsRequired();

            builder.HasDiscriminator<int>("Kind")
                .HasValue<FormGroupElement>((int)FormElementKind.Group)
                .HasValue<FormFieldRefElement>((int)FormElementKind.Field);

            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentElementId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.FormTabId, x.Position });
        }
    }

    /// <summary>
    /// Provides the additional Entity Framework Core configuration for the
    /// <see cref="FormGroupElement"/> subtype within the TPH hierarchy.
    /// </summary>
    internal class FormGroupElementConfiguration : IEntityTypeConfiguration<FormGroupElement>
    {
        /// <summary>
        /// Configuration of the form group element subtype.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<FormGroupElement> builder)
        {
            builder.Property(x => x.Label)
                .HasColumnName("Label")
                .HasMaxLength(256);

            builder.Property(x => x.Layout)
                .HasColumnName("Layout")
                .HasConversion<int>();
        }
    }

    /// <summary>
    /// Provides the additional Entity Framework Core configuration for the
    /// <see cref="FormFieldRefElement"/> subtype within the TPH hierarchy.
    /// </summary>
    internal class FormFieldRefElementConfiguration : IEntityTypeConfiguration<FormFieldRefElement>
    {
        /// <summary>
        /// Configuration of the form field reference element subtype.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<FormFieldRefElement> builder)
        {
            builder.Property(x => x.FieldId)
                .HasColumnName("Field")
                .IsRequired();

            builder.HasOne(x => x.Field)
                .WithMany()
                .HasForeignKey(x => x.FieldId)
                .HasPrincipalKey(f => f.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
