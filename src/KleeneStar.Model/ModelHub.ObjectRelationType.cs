using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the relation definitions the object relations
    /// are classified by.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the relation types that satisfy the supplied query, in the administered
        /// order.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The materialized collection of relation types.</returns>
        public static IEnumerable<ObjectRelationType> GetObjectRelationTypes(IQuery<ObjectRelationType> query)
        {
            using var db = CreateDbContext();

            return [.. query.Apply(db.ObjectRelationTypes.AsNoTracking()).OrderBy(x => x.Order).ThenBy(x => x.Key)];
        }

        /// <summary>
        /// Returns every relation type in the administered order.
        /// </summary>
        /// <returns>The materialized collection of relation types.</returns>
        public static IEnumerable<ObjectRelationType> GetObjectRelationTypes()
        {
            using var db = CreateDbContext();

            return [.. db.ObjectRelationTypes
                .AsNoTracking()
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Key)];
        }

        /// <summary>
        /// Returns a single relation type by its stable wire key.
        /// </summary>
        /// <param name="key">The key of the relation.</param>
        /// <returns>The relation type, or <see langword="null"/> when it is unknown.</returns>
        public static ObjectRelationType GetObjectRelationType(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            using var db = CreateDbContext();

            return db.ObjectRelationTypes.AsNoTracking().FirstOrDefault(x => x.Key == key);
        }

        /// <summary>
        /// Stores a relation type, inserting it when its key is new and overwriting the
        /// stored definition otherwise. It is one method rather than an add and an update
        /// because the type administration only ever states the whole definition, and the
        /// caller does not have to know whether the key was already taken.
        /// </summary>
        /// <param name="type">The relation type to store. Cannot be null.</param>
        /// <returns>The stored relation type.</returns>
        public static ObjectRelationType Store(ObjectRelationType type)
        {
            ArgumentNullException.ThrowIfNull(type);

            using var db = CreateDbContext();

            var existing = db.ObjectRelationTypes.FirstOrDefault(x => x.Key == type.Key);

            if (existing is null)
            {
                if (type.Created == default)
                {
                    type.Created = DateTime.UtcNow;
                }

                type.Updated = DateTime.UtcNow;

                db.ObjectRelationTypes.Add(type);
                db.SaveChanges();

                return type;
            }

            existing.Label = type.Label;
            existing.InverseLabel = type.InverseLabel;
            existing.Symmetric = type.Symmetric;
            existing.System = type.System;
            existing.TargetClasses = type.TargetClasses ?? [];
            existing.Cardinality = type.Cardinality;
            existing.Effect = type.Effect;
            existing.Active = type.Active;
            existing.Icon = type.Icon;
            existing.Order = type.Order;
            existing.Description = type.Description;
            existing.Updated = DateTime.UtcNow;

            // whether a relation is shipped is a property of where it came from, never of
            // what a request claims, so the stored flag survives every edit
            db.SaveChanges();

            return existing;
        }

        /// <summary>
        /// Removes the relation type with the supplied key.
        /// </summary>
        /// <param name="key">The key of the relation to remove.</param>
        /// <returns><see langword="true"/> when the type existed and was removed.</returns>
        public static bool RemoveObjectRelationType(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            using var db = CreateDbContext();

            var existing = db.ObjectRelationTypes.FirstOrDefault(x => x.Key == key);

            if (existing is null)
            {
                return false;
            }

            db.ObjectRelationTypes.Remove(existing);
            db.SaveChanges();

            return true;
        }
    }
}
