using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the <see cref="ObjectView"/> entity.
    /// </summary>
    internal class ObjectViewConfiguration : IEntityTypeConfiguration<ObjectView>
    {
        /// <summary>
        /// Configures the entity mapping.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<ObjectView> builder)
        {
            builder.ToTable("ObjectView");

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

            builder.Property(x => x.Kind)
                .HasColumnName("Kind")
                .IsRequired()
                .HasMaxLength(64)
                .HasDefaultValue(ObjectKind.Default);

            builder.Property(x => x.ViewType)
                .HasColumnName("ViewType")
                .IsRequired();

            builder.Property(x => x.Configuration)
                .HasColumnName("Configuration");

            builder.Property(x => x.Order)
                .HasColumnName("Order")
                .IsRequired();

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.WorkspaceId)
                .HasColumnName("Workspace")
                .IsRequired();

            builder.HasOne(x => x.Workspace)
                .WithMany()
                .HasForeignKey(x => x.WorkspaceId)
                .HasPrincipalKey(w => w.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => new { x.WorkspaceId, x.Kind, x.Name })
                .IsUnique();
        }
    }
}
