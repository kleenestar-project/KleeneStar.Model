using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with object-to-object relations.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the relations that satisfy the supplied query. Both ends and the creating
        /// identity are eagerly hydrated, because every one of them is rendered on the relation
        /// surface and a lazy read would issue one query per row.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The materialized collection of relations.</returns>
        public static IEnumerable<ObjectRelation> GetObjectRelations(IQuery<ObjectRelation> query)
        {
            using var db = CreateDbContext();

            return [.. GetObjectRelations(query, db)];
        }

        /// <summary>
        /// Returns the relations that satisfy the supplied query, executed inside the
        /// supplied DbContext.
        /// </summary>
        public static IEnumerable<ObjectRelation> GetObjectRelations(IQuery<ObjectRelation> query, KleeneStarDbContext context)
        {
            var data = context.ObjectRelations
                .AsNoTracking()
                .Include(x => x.SourceObject).ThenInclude(x => x.Class)
                .Include(x => x.TargetObject).ThenInclude(x => x.Class)
                .Include(x => x.CreatedBy);

            return query.Apply(data);
        }

        /// <summary>
        /// Returns a single relation by its unique identifier, with both ends hydrated.
        /// </summary>
        /// <param name="id">The unique identifier of the relation.</param>
        /// <returns>The relation, or <see langword="null"/> when it is unknown.</returns>
        public static ObjectRelation GetObjectRelation(Guid id)
        {
            using var db = CreateDbContext();

            return db.ObjectRelations
                .AsNoTracking()
                .Include(x => x.SourceObject).ThenInclude(x => x.Class)
                .Include(x => x.TargetObject).ThenInclude(x => x.Class)
                .Include(x => x.CreatedBy)
                .FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Returns how many stored relations carry the supplied relation, which is the number the
        /// type administration judges a change by and the delete guards against.
        /// </summary>
        /// <param name="typeKey">The key of the relation.</param>
        /// <returns>The number of relations.</returns>
        public static int CountObjectRelations(string typeKey)
        {
            if (string.IsNullOrWhiteSpace(typeKey))
            {
                return 0;
            }

            using var db = CreateDbContext();

            return db.ObjectRelations.Count(x => x.TypeKey == typeKey);
        }

        /// <summary>
        /// Adds the supplied relation to the database if no entry with the same id exists.
        /// </summary>
        /// <param name="relation">The relation to add. Cannot be null.</param>
        public static void Add(ObjectRelation relation)
        {
            ArgumentNullException.ThrowIfNull(relation);

            using var db = CreateDbContext();

            var query = new Query<ObjectRelation>()
                .WhereEquals(x => x.Id, relation.Id);

            if (query.Apply(db.ObjectRelations).Any())
            {
                return;
            }

            if (relation.Created == default)
            {
                relation.Created = DateTime.UtcNow;
            }

            relation.Updated = DateTime.UtcNow;

            db.ObjectRelations.Add(relation);
            db.SaveChanges();
        }

        /// <summary>
        /// Applies the changeable fields of the supplied relation to the stored row. The two ends
        /// and the owning system are deliberately not written back: a relation between other
        /// objects is a different relation, so moving an end would rewrite history rather than
        /// correct it.
        /// </summary>
        /// <param name="relation">The relation carrying the new values. Cannot be null.</param>
        public static void Update(ObjectRelation relation)
        {
            ArgumentNullException.ThrowIfNull(relation);

            using var db = CreateDbContext();

            var existing = db.ObjectRelations.FirstOrDefault(x => x.Id == relation.Id);

            if (existing is null)
            {
                return;
            }

            existing.TypeKey = relation.TypeKey;
            existing.Direction = relation.Direction;
            existing.Status = relation.Status;
            existing.Comment = relation.Comment;
            existing.TargetTitle = relation.TargetTitle;
            existing.Metadata = relation.Metadata ?? [];
            existing.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Removes the supplied relation from the database.
        /// </summary>
        /// <param name="relation">The relation to remove. Cannot be null.</param>
        public static void Remove(ObjectRelation relation)
        {
            ArgumentNullException.ThrowIfNull(relation);

            using var db = CreateDbContext();

            var existing = db.ObjectRelations.FirstOrDefault(x => x.Id == relation.Id);

            if (existing is null)
            {
                return;
            }

            db.ObjectRelations.Remove(existing);
            db.SaveChanges();
        }
    }
}
