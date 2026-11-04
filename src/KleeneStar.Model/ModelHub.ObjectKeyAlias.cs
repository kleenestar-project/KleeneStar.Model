using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// The former keys of objects moved into another workspace (<see cref="ObjectKeyAlias"/>).
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the object a former key now leads to, or <see langword="null"/>.
        /// </summary>
        public static Guid? GetObjectIdByAlias(string key)
        {
            using var db = CreateDbContext();

            var lowered = key.ToLower();

            return db.ObjectKeyAliases.AsNoTracking().Where(x => x.Key.ToLower() == lowered).Select(x => (Guid?)x.ObjectId).FirstOrDefault();
        }

        /// <summary>
        /// Records a former key of an object (a second record of the same key is ignored).
        /// </summary>
        public static void AddObjectKeyAlias(string key, Guid objectId)
        {
            using var db = CreateDbContext();

            if (db.ObjectKeyAliases.Any(x => x.Key == key))
            {
                return;
            }

            db.ObjectKeyAliases.Add(new ObjectKeyAlias { Key = key, ObjectId = objectId, Created = DateTime.UtcNow });
            db.SaveChanges();
        }

        /// <summary>
        /// Returns the former keys that start with a workspace key and a dash - the key numbering
        /// must not hand one of them out again.
        /// </summary>
        public static IReadOnlyList<string> GetAliasKeysWithPrefix(string prefix)
        {
            using var db = CreateDbContext();

            var start = prefix + "-";

            return [.. db.ObjectKeyAliases.AsNoTracking().Where(x => x.Key.StartsWith(start)).Select(x => x.Key)];
        }

        /// <summary>
        /// Returns the former keys of an object.
        /// </summary>
        public static IReadOnlyList<string> GetObjectKeyAliases(Guid objectId)
        {
            using var db = CreateDbContext();

            return [.. db.ObjectKeyAliases.AsNoTracking().Where(x => x.ObjectId == objectId).OrderBy(x => x.Created).Select(x => x.Key)];
        }
    }
}
