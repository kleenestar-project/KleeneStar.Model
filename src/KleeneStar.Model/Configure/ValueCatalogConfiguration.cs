using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the value catalogs.
    /// </summary>
    internal class ValueCatalogConfiguration : IEntityTypeConfiguration<ValueCatalog>
    {
        /// <summary>
        /// Configures the entity type mapping for the ValueCatalog entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ValueCatalog> builder)
        {
            builder.ToTable("ValueCatalog");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.WorkspaceId).HasColumnName("Workspace");
            builder.HasOne<Workspace>().WithMany().HasForeignKey(x => x.WorkspaceId).HasPrincipalKey(w => w.Id).OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(128);
            builder.HasIndex(x => new { x.WorkspaceId, x.Name });
            builder.Property(x => x.Description).HasColumnName("Description").HasMaxLength(1024);
            builder.Property(x => x.Source).HasColumnName("Source").IsRequired();
            builder.Property(x => x.Query).HasColumnName("Query").HasMaxLength(2048);
            builder.Property(x => x.ValueAttribute).HasColumnName("ValueAttribute").HasMaxLength(32);
            builder.Property(x => x.Url).HasColumnName("Url").HasMaxLength(1024);
            builder.Property(x => x.RefreshMinutes).HasColumnName("RefreshMinutes");
            builder.Property(x => x.State).HasColumnName("State").IsRequired();
            builder.Property(x => x.CurrentVersion).HasColumnName("CurrentVersion").IsRequired();
            builder.Property(x => x.LastRefresh).HasColumnName("LastRefresh");
            builder.Property(x => x.LastError).HasColumnName("LastError").HasMaxLength(1024);
            builder.Property(x => x.CreatedById).HasColumnName("CreatedBy");
            builder.Property(x => x.Created).HasColumnName("Created").IsRequired();
            builder.Property(x => x.Updated).HasColumnName("Updated").IsRequired();
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the versions of value catalogs.
    /// </summary>
    internal class ValueCatalogVersionConfiguration : IEntityTypeConfiguration<ValueCatalogVersion>
    {
        /// <summary>
        /// Configures the entity type mapping for the ValueCatalogVersion entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ValueCatalogVersion> builder)
        {
            builder.ToTable("ValueCatalogVersion");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.Property(x => x.CatalogId).HasColumnName("Catalog").IsRequired();
            builder.HasOne<ValueCatalog>().WithMany().HasForeignKey(x => x.CatalogId).HasPrincipalKey(c => c.Id).OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.Number).HasColumnName("Number").IsRequired();

            // two refreshes racing for the same number: the second write fails rather than forks the history
            builder.HasIndex(x => new { x.CatalogId, x.Number }).IsUnique();
            builder.Property(x => x.Values).HasColumnName("Values").IsRequired();
            builder.Property(x => x.Added).HasColumnName("Added").IsRequired();
            builder.Property(x => x.Removed).HasColumnName("Removed").IsRequired();
            builder.Property(x => x.Comment).HasColumnName("Comment").HasMaxLength(256);
            builder.Property(x => x.CreatedById).HasColumnName("CreatedBy");
            builder.Property(x => x.Created).HasColumnName("Created").IsRequired();
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the catalog bindings of fields.
    /// </summary>
    internal class FieldCatalogBindingConfiguration : IEntityTypeConfiguration<FieldCatalogBinding>
    {
        /// <summary>
        /// Configures the entity type mapping for the FieldCatalogBinding entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<FieldCatalogBinding> builder)
        {
            builder.ToTable("FieldCatalogBinding");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.Property(x => x.FieldId).HasColumnName("Field").IsRequired();
            builder.HasIndex(x => x.FieldId).IsUnique();
            builder.HasOne<Field>().WithMany().HasForeignKey(x => x.FieldId).HasPrincipalKey(f => f.Id).OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.CatalogId).HasColumnName("Catalog").IsRequired();
            builder.HasOne<ValueCatalog>().WithMany().HasForeignKey(x => x.CatalogId).HasPrincipalKey(c => c.Id).OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.PinnedVersion).HasColumnName("PinnedVersion");
        }
    }
}
