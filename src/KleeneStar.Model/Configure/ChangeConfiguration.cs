using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="Change"/>.
    /// </summary>
    /// <remarks>
    /// The relationship to <see cref="Commit"/> is declared on the principal side (see
    /// <see cref="CommitConfiguration"/>); this configuration owns the column mapping and the
    /// index the replay reads by. <see cref="Change.FieldId"/> carries no foreign key for the
    /// same reason <see cref="Commit.ObjectId"/> does not — a field that is deleted must not
    /// take the record of what it once held with it.
    /// </remarks>
    internal class ChangeConfiguration : IEntityTypeConfiguration<Change>
    {
        /// <summary>
        /// Configures the change entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Change> builder)
        {
            builder.ToTable("Change");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.CommitId)
                .HasColumnName("Commit")
                .IsRequired();

            builder.Property(x => x.FieldId)
                .HasColumnName("Field");

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(x => x.OldValue)
                .HasColumnName("OldValue");

            builder.Property(x => x.NewValue)
                .HasColumnName("NewValue");

            builder.Property(x => x.Ordinal)
                .HasColumnName("Ordinal")
                .HasDefaultValue(0);

            builder.Ignore(x => x.Field);

            builder.HasIndex(x => new { x.CommitId, x.Ordinal });
        }
    }
}
