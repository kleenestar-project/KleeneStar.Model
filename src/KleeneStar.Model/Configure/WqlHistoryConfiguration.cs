using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="WqlHistory"/>.
    /// </summary>
    internal class WqlHistoryConfiguration : IEntityTypeConfiguration<WqlHistory>
    {
        /// <summary>
        /// Configures the query-history entity. Establishes:
        /// <list type="bullet">
        /// <item>FK <see cref="WqlHistory.OwnerId"/> → <see cref="Identity"/> (cascade delete),
        /// so a removed account takes its search trail with it.</item>
        /// <item>A non-unique index on (Owner, Subject) — the lookup the prompt and the
        /// recorder both make.</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// The query itself is deliberately <b>not</b> part of an index. Uniqueness per
        /// (owner, subject, query) is what the recorder enforces, but doing it with a database
        /// constraint would mean indexing a column long enough to hold a WQL statement, and an
        /// index key of that width is past what SQL Server allows. The recorder already loads
        /// the owner's entries for the subject in order to trim them to the cap, so it
        /// recognizes a repeat in that set at no extra cost.
        /// </remarks>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<WqlHistory> builder)
        {
            builder.ToTable("WqlHistory");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.OwnerId)
                .HasColumnName("Owner")
                .IsRequired();

            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Subject)
                .HasColumnName("Subject")
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.Query)
                .HasColumnName("Query")
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(x => x.UseCount)
                .HasColumnName("UseCount")
                .HasDefaultValue(1);

            builder.Property(x => x.LastUsed)
                .HasColumnName("LastUsed")
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => new { x.OwnerId, x.Subject });
        }
    }
}
