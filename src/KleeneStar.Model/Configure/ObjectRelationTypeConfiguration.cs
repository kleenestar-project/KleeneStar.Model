using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the <see cref="ObjectRelationType"/>
    /// entity.
    /// </summary>
    internal class ObjectRelationTypeConfiguration : IEntityTypeConfiguration<ObjectRelationType>
    {
        /// <summary>
        /// The separator the accepted target classes are joined with. A class name is a
        /// single-line value, so a newline cannot occur inside one, which is what makes the
        /// round trip lossless.
        /// </summary>
        private const char ClassSeparator = '\n';

        /// <summary>
        /// Configures the relation type entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<ObjectRelationType> builder)
        {
            builder.ToTable("ObjectRelationType");

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

            builder.Property(x => x.Label)
                .HasColumnName("Label")
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.InverseLabel)
                .HasColumnName("InverseLabel")
                .HasMaxLength(256);

            builder.Property(x => x.Symmetric)
                .HasColumnName("Symmetric")
                .IsRequired();

            builder.Property(x => x.System)
                .HasColumnName("System")
                .IsRequired()
                .HasMaxLength(128);

            // the accepted classes are a set of names rather than a relation, because a
            // relation type is not owned by a workspace while a class is - a join would
            // claim a containment the model does not have
            builder.Property(x => x.TargetClasses)
                .HasColumnName("TargetClasses")
                .HasConversion
                (
                    value => value == null || value.Count == 0 ? null : string.Join(ClassSeparator, value),
                    text => string.IsNullOrEmpty(text)
                        ? new List<string>()
                        : text.Split(ClassSeparator, StringSplitOptions.RemoveEmptyEntries).ToList(),
                    new ValueComparer<List<string>>
                    (
                        (left, right) => left.SequenceEqual(right),
                        value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                        value => value.ToList()
                    )
                );

            // both enums are stored by name rather than by ordinal: they are declared in
            // WebExpress, so a member inserted upstream would silently re-read every stored
            // row if the column carried the position instead of the word
            builder.Property(x => x.Cardinality)
                .HasColumnName("Cardinality")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(x => x.Effect)
                .HasColumnName("Effect")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(x => x.Active)
                .HasColumnName("Active")
                .IsRequired();

            builder.Property(x => x.Icon)
                .HasColumnName("Icon")
                .HasMaxLength(64);

            builder.Property(x => x.Order)
                .HasColumnName("Order")
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => x.Key)
                .IsUnique();
        }
    }
}
