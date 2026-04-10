using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure;

/// <summary>
/// Configures the EF Core entity mapping for <see cref="Group"/>.
/// </summary>
public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    /// <summary>
    /// Configures the entity properties, keys, and relationships for the Group entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(g => g.Description)
            .HasMaxLength(512);

        builder.HasIndex(g => g.Name)
            .IsUnique();
    }
}
