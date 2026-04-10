using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the class entity type.
    /// </summary>
    internal class ClassConfiguration : IEntityTypeConfiguration<Class>
    {
        /// <summary>
        /// Configuration of the class entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Class> builder)
        {
            builder.ToTable("Class");

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

            builder.Property(x => x.IsAbstract)
                .HasColumnName("IsAbstract");

            builder.Property(x => x.InheritedId)
                .HasColumnName("Inherited");

            builder.HasOne(x => x.Inherited)
                .WithMany()
                .HasForeignKey(x => x.InheritedId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Sealed)
                .HasColumnName("Sealed");

            builder.Property(x => x.ParentId)
                .HasColumnName("Parent");

            builder.HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => x.ParentId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.AccessModifier)
                .HasColumnName("AccessModifier");

            // MANY-TO-MANY: Class <-> AllowedChildren (self-referencing)
            builder.HasMany(x => x.AllowedChildren)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>
                (
                    "ClassAllowedChild",
                    j => j
                        .HasOne<Class>()
                        .WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Class>()
                        .WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                );

            builder.Property(x => x.WorkspaceId)
                .HasColumnName("Workspace")
                .IsRequired();

            builder.HasOne(x => x.Workspace)
                .WithMany()
                .HasForeignKey(x => x.WorkspaceId)
                .HasPrincipalKey(w => w.Id);

            builder.HasIndex(x => new { x.WorkspaceId, x.Name })
                .IsUnique();
        }
    }
}
