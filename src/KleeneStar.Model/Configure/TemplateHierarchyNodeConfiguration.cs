using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

            builder.Property(x => x.RelationTypeKey)
                .HasColumnName("RelationType")
                .HasMaxLength(64);

            builder.Property(x => x.RelationInverse)
                .HasColumnName("RelationInverse");

            builder.Property(x => x.AssigneeId)
                .HasColumnName("Assignee");

            builder.HasOne(x => x.Assignee)
                .WithMany()
                .HasForeignKey(x => x.AssigneeId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.PredecessorId)
                .HasColumnName("Predecessor");

            builder.HasIndex(x => x.HierarchyId);
            builder.HasIndex(x => x.TemplateId);
        }
    }
}
