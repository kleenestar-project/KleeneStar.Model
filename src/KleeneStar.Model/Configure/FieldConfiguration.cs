using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text.Json;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the field entity type.
    /// </summary>
    internal class FieldConfiguration : IEntityTypeConfiguration<Field>
    {
        /// <summary>
        /// Configuration of the field entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Field> builder)
        {
            builder.ToTable("Field");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.HelpText)
                .HasColumnName("HelpText");

            builder.Property(x => x.Placeholder)
                .HasColumnName("Placeholder")
                .HasMaxLength(256);

            builder.Property(x => x.Icon)
                .HasColumnName("Icon")
                .HasMaxLength(256)
                .HasConversion
                (
                    icon => icon != null && icon.Uri != null ? icon.Uri.ToString() : null,
                    uri => string.IsNullOrEmpty(uri) ? null : ImageIcon.FromString(uri)
                );

            builder.Property(x => x.State)
                .HasColumnName("State");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.ClassId)
                .HasColumnName("Class")
                .IsRequired();

            builder.HasOne(x => x.Class)
                .WithMany()
                .HasForeignKey(x => x.ClassId)
                .HasPrincipalKey(w => w.Id);

            builder.Property(x => x.FieldType)
                .HasColumnName("FieldType")
                .HasMaxLength(128);

            builder.Property(x => x.Cardinality)
                .HasColumnName("Cardinality");

            builder.Property(x => x.ValidationRules)
                .HasColumnName("ValidationRules")
                .HasConversion
                (
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => string.IsNullOrEmpty(v)
                        ? new List<string>()
                        : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
                );

            builder.Property(x => x.DefaultSpec)
                .HasColumnName("DefaultSpec");

            builder.Property(x => x.Required)
                .HasColumnName("Required");

            builder.Property(x => x.Unique)
                .HasColumnName("Unique");

            builder.Property(x => x.Deprecated)
                .HasColumnName("Deprecated");

            builder.Property(x => x.AccessModifier)
                .HasColumnName("AccessModifier");

            builder.HasIndex(x => new { x.ClassId, x.Name })
                .IsUnique();
        }
    }
}
