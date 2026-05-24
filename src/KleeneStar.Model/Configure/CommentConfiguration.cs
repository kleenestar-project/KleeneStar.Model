using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="Comment"/>.
    /// </summary>
    internal class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        /// <summary>
        /// Configures the comment entity. Establishes:
        /// <list type="bullet">
        /// <item>FK <see cref="Comment.ObjectId"/> → <see cref="Object"/> (cascade delete).</item>
        /// <item>FK <see cref="Comment.AuthorId"/> → <see cref="Identity"/> (restrict — identities
        /// must not be hard-deleted without re-assigning their comments).</item>
        /// <item>Self FK <see cref="Comment.ParentCommentId"/> → <see cref="Comment"/> (restrict
        /// to keep reply threads navigable; soft delete is the supported flow).</item>
        /// <item>Composite index on (Object, Created) so the comment-list endpoint can
        /// retrieve comments per object in chronological order without a full scan.</item>
        /// </list>
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comment");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Content)
                .HasColumnName("Content")
                .IsRequired();

            builder.Property(x => x.State)
                .HasColumnName("State");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.Property(x => x.DeletedAt)
                .HasColumnName("DeletedAt");

            builder.Property(x => x.IsPinned)
                .HasColumnName("IsPinned")
                .HasDefaultValue(false);

            builder.Property(x => x.ObjectId)
                .HasColumnName("Object")
                .IsRequired();

            builder.HasOne(x => x.Object)
                .WithMany()
                .HasForeignKey(x => x.ObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.AuthorId)
                .HasColumnName("Author")
                .IsRequired();

            builder.HasOne(x => x.Author)
                .WithMany()
                .HasForeignKey(x => x.AuthorId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ParentCommentId)
                .HasColumnName("ParentComment");

            builder.HasOne(x => x.ParentComment)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.ParentCommentId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ObjectId, x.Created });
        }
    }
}
