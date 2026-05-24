using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="CommentLike"/>.
    /// </summary>
    internal class CommentLikeConfiguration : IEntityTypeConfiguration<CommentLike>
    {
        /// <summary>
        /// Configures the comment-like entity. Establishes:
        /// <list type="bullet">
        /// <item>FK <see cref="CommentLike.CommentId"/> → <see cref="Comment"/> (cascade delete).</item>
        /// <item>FK <see cref="CommentLike.AuthorId"/> → <see cref="Identity"/> (restrict).</item>
        /// <item>Unique composite index on (Comment, Author) so the same identity cannot like
        /// a comment twice; toggling off removes the row.</item>
        /// </list>
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<CommentLike> builder)
        {
            builder.ToTable("CommentLike");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.CommentId)
                .HasColumnName("Comment")
                .IsRequired();

            builder.HasOne(x => x.Comment)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.CommentId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.AuthorId)
                .HasColumnName("Author")
                .IsRequired();

            builder.HasOne(x => x.Author)
                .WithMany()
                .HasForeignKey(x => x.AuthorId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.CommentId, x.AuthorId }).IsUnique();
        }
    }
}
