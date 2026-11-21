using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="TemplateHierarchy"/>.
    /// </summary>
    internal class TemplateHierarchyConfiguration : IEntityTypeConfiguration<TemplateHierarchy>
    {
        /// <summary>
        /// Configures the entity type mapping for the TemplateHierarchy entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<TemplateHierarchy> builder)
        {
            builder.ToTable("TemplateHierarchy");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.Category)
                .HasColumnName("Category")
                .HasMaxLength(128);

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

            builder.Property(x => x.WorkspaceId)
                .HasColumnName("Workspace")
                .IsRequired();

            builder.HasOne(x => x.Workspace)
                .WithMany()
                .HasForeignKey(x => x.WorkspaceId)
                .HasPrincipalKey(w => w.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.RootTemplateId)
                .HasColumnName("RootTemplate")
                .IsRequired();

            // a template a hierarchy is rooted in is not removed beneath it
            builder.HasOne(x => x.RootTemplate)
                .WithMany()
                .HasForeignKey(x => x.RootTemplateId)
                .HasPrincipalKey(t => t.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.RootClassId)
                .HasColumnName("RootClass");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => new { x.WorkspaceId, x.Name })
                .IsUnique();
        }
    }
}
