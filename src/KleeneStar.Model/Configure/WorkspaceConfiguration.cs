using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the Workspace entity type.
    /// </summary>
    internal class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
    {
        /// <summary>
        /// Configuration of the Workspace entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Workspace> builder)
        {
            builder.ToTable("Workspace");

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

            // MANY-TO-MANY: Workspace <-> Category
            builder.HasMany(w => w.Categories)
                   .WithMany(c => c.Workspaces)
                   .UsingEntity<Dictionary<string, object>>(
                        "WorkspaceCategory",
                        j => j
                            .HasOne<Category>()
                            .WithMany()
                            .HasForeignKey("CategoryId")
                            .OnDelete(DeleteBehavior.Cascade),
                        j => j
                            .HasOne<Workspace>()
                            .WithMany()
                            .HasForeignKey("WorkspaceId")
                            .OnDelete(DeleteBehavior.Cascade)
                   );

            builder.Property(x => x.Icon)
                   .HasColumnName("Icon")
                   .HasMaxLength(256)
                   .HasConversion(
                        icon => icon != null && icon.Uri != null ? icon.Uri.ToString() : null,
                        uri => string.IsNullOrEmpty(uri) ? null : ImageIcon.FromString(uri)
                   );

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

            builder.HasIndex(x => x.Name)
                   .IsUnique();
        }
    }
}
