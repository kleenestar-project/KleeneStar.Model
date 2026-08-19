using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="Commit"/>.
    /// </summary>
    internal class CommitConfiguration : IEntityTypeConfiguration<Commit>
    {
        /// <summary>
        /// Configures the commit entity. Establishes:
        /// <list type="bullet">
        /// <item>The one-to-many relationship to <see cref="Change"/> (cascade — a change has no
        /// meaning without the commit it belongs to).</item>
        /// <item>A unique index on (Object, Number) so the chain of an object cannot grow two
        /// commits with the same revision number, which is what the human-readable reference
        /// <c>INC-00123#4</c> and every replay depend on.</item>
        /// <item>A composite index on (Object, Created) so the history endpoint can page an
        /// object's commits in chronological order without a full scan.</item>
        /// </list>
        /// No foreign key is declared for <see cref="Commit.ObjectId"/> or
        /// <see cref="Commit.CreatedById"/>: the history is an append-only audit trail that has
        /// to survive the deletion of both the object and the identity it names (see the remarks
        /// on <see cref="Commit"/>). The <see cref="Commit.Object"/> and
        /// <see cref="Commit.CreatedBy"/> navigation properties are consequently ignored by the
        /// model and resolved on read instead.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Commit> builder)
        {
            builder.ToTable("Commit");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.ObjectId)
                .HasColumnName("Object")
                .IsRequired();

            builder.Property(x => x.ObjectKey)
                .HasColumnName("ObjectKey");

            builder.Property(x => x.ParentId)
                .HasColumnName("Parent");

            builder.Property(x => x.Number)
                .HasColumnName("Number")
                .IsRequired();

            builder.Property(x => x.Type)
                .HasColumnName("Type")
                .IsRequired();

            builder.Property(x => x.CreatedById)
                .HasColumnName("CreatedBy");

            builder.Property(x => x.CreatedByName)
                .HasColumnName("CreatedByName");

            builder.Property(x => x.Message)
                .HasColumnName("Message");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            // the history outlives the rows it describes, so neither reference is a foreign key
            builder.Ignore(x => x.Object);
            builder.Ignore(x => x.CreatedBy);

            builder.HasMany(x => x.Changes)
                .WithOne(x => x.Commit)
                .HasForeignKey(x => x.CommitId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ObjectId, x.Number })
                .IsUnique();

            builder.HasIndex(x => new { x.ObjectId, x.Created });
        }
    }
}
