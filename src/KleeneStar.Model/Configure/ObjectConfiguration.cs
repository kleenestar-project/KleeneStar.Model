using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the object entity type.
    /// </summary>
    internal class ObjectConfiguration : IEntityTypeConfiguration<Object>
    {
        /// <summary>
        /// Configuration of the class entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Object> builder)
        {
            builder.ToTable("Object");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Key)
               .HasColumnName("Key")
               .IsRequired()
               .HasMaxLength(64);

            builder.Property(x => x.Summary)
                .HasColumnName("Summary")
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

            builder.Property(x => x.Kind)
                .HasColumnName("Kind")
                .IsRequired()
                .HasMaxLength(64)
                .HasDefaultValue(ObjectKind.Default);

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

            builder.Property(x => x.WorkspaceId)
                .HasColumnName("Workspace")
                .IsRequired();

            builder.HasOne(x => x.Workspace)
                .WithMany(w => w.Objects)
                .HasForeignKey(x => x.WorkspaceId)
                .HasPrincipalKey(w => w.Id);

            builder.Property(x => x.ClassId)
                .HasColumnName("Class")
                .IsRequired();

            builder.HasOne(x => x.Class)
                .WithMany()
                .HasForeignKey(x => x.ClassId)
                .HasPrincipalKey(w => w.Id);

            builder.Property(x => x.ParentId)
                .HasColumnName("Parent");

            builder.HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => x.ParentId)
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.CreatorId)
                .HasColumnName("Creator");

            builder.HasOne(x => x.Creator)
                .WithMany()
                .HasForeignKey(x => x.CreatorId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.AssigneeId)
                .HasColumnName("Assignee");

            builder.HasOne(x => x.Assignee)
                .WithMany()
                .HasForeignKey(x => x.AssigneeId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.SprintId)
                .HasColumnName("Sprint");

            builder.HasOne(x => x.Sprint)
                .WithMany()
                .HasForeignKey(x => x.SprintId)
                .HasPrincipalKey(s => s.Id)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.SprintRank)
                .HasColumnName("SprintRank");

            builder.Property(x => x.StoryPoints)
                .HasColumnName("StoryPoints");

            builder.Property(x => x.UpdaterId)
                .HasColumnName("Updater");

            builder.HasOne(x => x.Updater)
                .WithMany()
                .HasForeignKey(x => x.UpdaterId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.Key)
                .IsUnique();

            builder.HasIndex(x => new { x.WorkspaceId, x.Kind });
        }
    }
}
