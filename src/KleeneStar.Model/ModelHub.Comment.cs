using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the KleeneStar.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns a materialized collection of comments from the database.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The matching comments, including author and parent navigation.</returns>
        public static IEnumerable<Comment> GetComments(IQuery<Comment> query)
        {
            using var db = CreateDbContext();

            return [.. GetComments(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of comments using the supplied DbContext.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The DbContext.</param>
        /// <returns>The filtered collection.</returns>
        public static IEnumerable<Comment> GetComments(IQuery<Comment> query, KleeneStarDbContext context)
        {
            var data = context.Comments
                .Include(x => x.Author)
                .Include(x => x.Object)
                .Include(x => x.ParentComment)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Adds the supplied comment to the database when no comment with the same id exists.
        /// </summary>
        /// <param name="commentEntry">The comment to add.</param>
        public static void Add(Comment commentEntry)
        {
            ArgumentNullException.ThrowIfNull(commentEntry);

            using var db = CreateDbContext();

            var query = new Query<Comment>()
                .WhereEquals(x => x.Id, commentEntry.Id);

            if (query.Apply(db.Comments).Any())
            {
                return;
            }

            if (commentEntry.Created == default)
            {
                commentEntry.Created = DateTime.UtcNow;
            }

            commentEntry.Updated = DateTime.UtcNow;

            db.Comments.Add(commentEntry);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the supplied comment in the database. Re-loads the existing row and
        /// overwrites scalar properties only — child navigation (replies) is never
        /// rewritten through the update path.
        /// </summary>
        /// <param name="commentEntry">The comment to update.</param>
        public static void Update(Comment commentEntry)
        {
            ArgumentNullException.ThrowIfNull(commentEntry);

            using var db = CreateDbContext();

            var existing = db.Comments.FirstOrDefault(x => x.Id == commentEntry.Id);

            if (existing is null)
            {
                return;
            }

            existing.Content = commentEntry.Content;
            existing.State = commentEntry.State;
            existing.DeletedAt = commentEntry.DeletedAt;
            existing.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified comment from the data store if it exists. Replies are
        /// left orphaned (their <c>ParentCommentId</c> still references the deleted id)
        /// because the relationship is configured with <c>OnDelete=Restrict</c>; callers
        /// should soft-delete via <see cref="Update(Comment)"/> with
        /// <see cref="CommentState.Deleted"/> instead.
        /// </summary>
        /// <param name="commentEntry">The comment entity to remove.</param>
        public static void Remove(Comment commentEntry)
        {
            ArgumentNullException.ThrowIfNull(commentEntry);

            using var db = CreateDbContext();

            var existing = db.Comments.FirstOrDefault(x => x.Id == commentEntry.Id);

            if (existing is null)
            {
                return;
            }

            db.Comments.Remove(existing);
            db.SaveChanges();
        }
    }
}
