using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the <see cref="ObjectRelation"/> entity.
    /// </summary>
    internal class ObjectRelationConfiguration : IEntityTypeConfiguration<ObjectRelation>
    {
        /// <summary>
        /// Configures the object-relation entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<ObjectRelation> builder)
        {
            builder.ToTable("ObjectRelation");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            // the two enums are stored by name rather than by ordinal: they are declared in
            // WebExpress, so a member inserted upstream would silently re-read every stored
            // row if the column carried the position instead of the word
            builder.Property(x => x.System)
                .HasColumnName("System")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.TypeKey)
                .HasColumnName("Type")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Direction)
                .HasColumnName("Direction")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("Status")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(x => x.SourceObjectId)
                .HasColumnName("Source")
                .IsRequired();

            builder.HasOne(x => x.SourceObject)
                .WithMany()
                .HasForeignKey(x => x.SourceObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // an external relation has no target object, so the column is optional - it is the
            // one difference between the two relation categories the schema has to carry
            builder.Property(x => x.TargetObjectId)
                .HasColumnName("Target");

            builder.HasOne(x => x.TargetObject)
                .WithMany()
                .HasForeignKey(x => x.TargetObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.TargetUri)
                .HasColumnName("TargetUri")
                .HasMaxLength(2048);

            builder.Property(x => x.TargetTitle)
                .HasColumnName("TargetTitle")
                .HasMaxLength(512);

            builder.Property(x => x.Comment)
                .HasColumnName("Comment");

            builder.Property(x => x.CreatedById)
                .HasColumnName("CreatedBy");

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.SetNull);

            // the extension is opaque to the application - it is written by whoever
            // contributed the link system and read back by the same, so it travels as one
            // json document rather than as a table nobody but that plugin can interpret
            builder.Property(x => x.Metadata)
                .HasColumnName("Metadata")
                .HasConversion
                (
                    value => value == null || value.Count == 0 ? null : JsonSerializer.Serialize(value, (JsonSerializerOptions)null),
                    json => string.IsNullOrEmpty(json)
                        ? new Dictionary<string, string>()
                        : JsonSerializer.Deserialize<Dictionary<string, string>>(json, (JsonSerializerOptions)null) ?? new Dictionary<string, string>(),
                    new ValueComparer<Dictionary<string, string>>
                    (
                        (left, right) => left.SequenceEqual(right),
                        value => value.Aggregate(0, (hash, entry) => System.HashCode.Combine(hash, entry.Key.GetHashCode(), entry.Value.GetHashCode())),
                        value => new Dictionary<string, string>(value)
                    )
                );

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => x.SourceObjectId);
            builder.HasIndex(x => x.TargetObjectId);

            // the same relation must not be stored twice between the same two ends; an
            // external relation is excluded because its target column is null and null never
            // collides in a unique index
            builder.HasIndex(x => new { x.SourceObjectId, x.TargetObjectId, x.TypeKey })
                .IsUnique();
        }
    }
}
