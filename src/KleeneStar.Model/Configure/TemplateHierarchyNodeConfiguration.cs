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
    /// Entity Framework Core configuration for <see cref="TemplateHierarchyNode"/>.
    /// </summary>
    internal class TemplateHierarchyNodeConfiguration : IEntityTypeConfiguration<TemplateHierarchyNode>
    {
        /// <summary>
        /// Configures the entity type mapping for the TemplateHierarchyNode entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<TemplateHierarchyNode> builder)
        {
            builder.ToTable("TemplateHierarchyNode");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.HierarchyId)
                .HasColumnName("Hierarchy")
                .IsRequired();

            // the nodes belong to their hierarchy and leave with it
            builder.HasOne(x => x.Hierarchy)
                .WithMany()
                .HasForeignKey(x => x.HierarchyId)
                .HasPrincipalKey(h => h.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // names a node of the same hierarchy; the tree is written as a whole
            builder.Property(x => x.ParentNodeId)
                .HasColumnName("ParentNode");

            builder.Property(x => x.TemplateId)
                .HasColumnName("Template")
                .IsRequired();

            // a template a hierarchy references is not removed beneath it
            builder.HasOne(x => x.Template)
                .WithMany()
                .HasForeignKey(x => x.TemplateId)
                .HasPrincipalKey(t => t.Id)
                .OnDelete(DeleteBehavior.Restrict);

            // no foreign key: a class the referenced template no longer serves is answered with
            // the bound class at instantiation
            builder.Property(x => x.ClassId)
                .HasColumnName("Class");

            builder.Property(x => x.Order)
                .HasColumnName("Order");

            // no database default: EF would write it in place of an explicit false
            builder.Property(x => x.AsChild)
                .HasColumnName("AsChild");

            builder.Property(x => x.AssigneeId)
                .HasColumnName("Assignee");

            builder.HasOne(x => x.Assignee)
                .WithMany()
                .HasForeignKey(x => x.AssigneeId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.SetNull);

            // the links travel as one JSON array; the targets name nodes of the same hierarchy,
            // which is written as a whole
            builder.Property(x => x.Links)
                .HasColumnName("Links")
                .HasConversion
                (
                    v => JsonSerializer.Serialize(v ?? new List<TemplateHierarchyNodeLink>(), (JsonSerializerOptions)null),
                    v => string.IsNullOrEmpty(v)
                        ? new List<TemplateHierarchyNodeLink>()
                        : JsonSerializer.Deserialize<List<TemplateHierarchyNodeLink>>(v, (JsonSerializerOptions)null) ?? new List<TemplateHierarchyNodeLink>(),
                    new ValueComparer<List<TemplateHierarchyNodeLink>>
                    (
                        (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions)null) == JsonSerializer.Serialize(b, (JsonSerializerOptions)null),
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null).GetHashCode(),
                        v => JsonSerializer.Deserialize<List<TemplateHierarchyNodeLink>>(JsonSerializer.Serialize(v, (JsonSerializerOptions)null), (JsonSerializerOptions)null)
                    )
                );

            builder.HasIndex(x => x.HierarchyId);
            builder.HasIndex(x => x.TemplateId);
        }
    }
}
