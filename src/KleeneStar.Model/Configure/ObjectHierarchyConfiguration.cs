using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the materialized paths of objects.
    /// </summary>
    internal class ObjectPathConfiguration : IEntityTypeConfiguration<ObjectPath>
    {
        /// <summary>
        /// Configures the entity type mapping for the ObjectPath entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ObjectPath> builder)
        {
            builder.ToTable("ObjectPath");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.Property(x => x.ObjectId).HasColumnName("Object").IsRequired();
            builder.HasIndex(x => x.ObjectId).IsUnique();

            // derived from the object: it goes with it
            builder.HasOne<Object>().WithMany().HasForeignKey(x => x.ObjectId).HasPrincipalKey(o => o.Id).OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.WorkspaceId).HasColumnName("Workspace").IsRequired();

            // ordinal (binary) comparison is what makes the subtree a range: the ids are lower-case
            // hex, so "/" + id + "/" sorts before the same prefix with the slash replaced by "0"
            builder.Property(x => x.Path).HasColumnName("Path").IsRequired().HasMaxLength(2048);
            builder.Property(x => x.Depth).HasColumnName("Depth").IsRequired();
            builder.HasIndex(x => new { x.WorkspaceId, x.Path });
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the recursive operation runs.
    /// </summary>
    internal class ObjectOperationConfiguration : IEntityTypeConfiguration<ObjectOperation>
    {
        /// <summary>
        /// Configures the entity type mapping for the ObjectOperation entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ObjectOperation> builder)
        {
            builder.ToTable("ObjectOperation");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.HasAlternateKey(x => x.Id);
            builder.Property(x => x.Key).HasColumnName("Key").IsRequired().HasMaxLength(64);

            // a second submission finds the first run instead of starting beside it
            builder.HasIndex(x => x.Key).IsUnique();
            builder.Property(x => x.Kind).HasColumnName("Kind").IsRequired().HasMaxLength(16);
            builder.Property(x => x.Mode).HasColumnName("Mode").HasMaxLength(16);
            builder.Property(x => x.RootId).HasColumnName("Root").IsRequired();
            builder.Property(x => x.WorkspaceId).HasColumnName("Workspace").IsRequired();
            builder.Property(x => x.ReferenceId).HasColumnName("Reference");
            builder.Property(x => x.TargetWorkspaceId).HasColumnName("TargetWorkspace");
            builder.Property(x => x.Options).HasColumnName("Options");
            builder.Property(x => x.State).HasColumnName("State").IsRequired();
            builder.Property(x => x.Total).HasColumnName("Total").IsRequired();
            builder.Property(x => x.Done).HasColumnName("Done").IsRequired();
            builder.Property(x => x.Error).HasColumnName("Error");
            builder.Property(x => x.IdentityId).HasColumnName("Identity");
            builder.Property(x => x.Created).HasColumnName("Created").IsRequired();
            builder.Property(x => x.Updated).HasColumnName("Updated").IsRequired();
            builder.Property(x => x.Finished).HasColumnName("Finished");
            builder.HasIndex(x => new { x.RootId, x.Kind });
            builder.HasIndex(x => x.State);
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the objects of an operation run.
    /// </summary>
    internal class ObjectOperationItemConfiguration : IEntityTypeConfiguration<ObjectOperationItem>
    {
        /// <summary>
        /// Configures the entity type mapping for the ObjectOperationItem entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ObjectOperationItem> builder)
        {
            builder.ToTable("ObjectOperationItem");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.Property(x => x.OperationId).HasColumnName("Operation").IsRequired();
            builder.HasOne<ObjectOperation>().WithMany().HasForeignKey(x => x.OperationId).HasPrincipalKey(o => o.Id).OnDelete(DeleteBehavior.Cascade);

            // no foreign key on the object: a delete run outlives what it deleted
            builder.Property(x => x.ObjectId).HasColumnName("Object").IsRequired();
            builder.HasIndex(x => new { x.OperationId, x.ObjectId }).IsUnique();
            builder.Property(x => x.Position).HasColumnName("Position").IsRequired();
            builder.Property(x => x.Done).HasColumnName("Done").IsRequired();
            builder.Property(x => x.ResultId).HasColumnName("Result");
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the former keys of objects.
    /// </summary>
    internal class ObjectKeyAliasConfiguration : IEntityTypeConfiguration<ObjectKeyAlias>
    {
        /// <summary>
        /// Configures the entity type mapping for the ObjectKeyAlias entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ObjectKeyAlias> builder)
        {
            builder.ToTable("ObjectKeyAlias");
            builder.HasKey(x => x.RawId);
            builder.Property(x => x.RawId).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(x => x.Id).HasColumnName("Guid").IsRequired().HasMaxLength(36);
            builder.Property(x => x.Key).HasColumnName("Key").IsRequired().HasMaxLength(64);
            builder.HasIndex(x => x.Key).IsUnique();
            builder.Property(x => x.ObjectId).HasColumnName("Object").IsRequired();
            builder.HasIndex(x => x.ObjectId);
            builder.HasOne<Object>().WithMany().HasForeignKey(x => x.ObjectId).HasPrincipalKey(o => o.Id).OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.Created).HasColumnName("Created").IsRequired();
        }
    }
}
