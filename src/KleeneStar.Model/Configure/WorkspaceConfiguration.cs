using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure;

/// <summary>
/// Configures the EF Core entity mapping for <see cref="Workspace"/>.
/// </summary>
public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    /// <summary>
    /// Configures the entity properties, keys, and relationships for the Workspace entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(w => w.Description)
            .HasMaxLength(512);

        builder.HasIndex(w => w.Name)
            .IsUnique();
    }
}
