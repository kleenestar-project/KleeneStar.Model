using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="ObjectDraft"/>.
    /// </summary>
    internal class ObjectDraftConfiguration : IEntityTypeConfiguration<ObjectDraft>
    {
        /// <summary>
        /// Configures the entity type mapping for the ObjectDraft entity. Establishes:
        /// <list type="bullet">
        /// <item>FK <see cref="ObjectDraft.ObjectId"/> → <see cref="Object"/> (cascade delete -
        /// an unpublished draft of a deleted object has nothing left to be published onto).</item>
        /// <item>FK <see cref="ObjectDraft.UpdaterId"/> → <see cref="Identity"/> (set null -
        /// removing an author must not remove the text they were writing).</item>
        /// <item>A unique index on <see cref="ObjectDraft.ObjectId"/>, because a draft is the
        /// shared working copy of the object rather than a per-author copy.</item>
        /// </list>
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ObjectDraft> builder)
        {
            builder.ToTable("ObjectDraft");

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

            builder.HasOne(x => x.Object)
                .WithMany()
                .HasForeignKey(x => x.ObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Summary)
                .HasColumnName("Summary");

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.Property(x => x.UpdaterId)
                .HasColumnName("Updater");

            builder.HasOne(x => x.Updater)
                .WithMany()
                .HasForeignKey(x => x.UpdaterId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => x.ObjectId).IsUnique();
        }
    }
}
