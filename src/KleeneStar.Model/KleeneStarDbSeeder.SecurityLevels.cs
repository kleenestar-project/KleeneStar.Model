using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// The security levels every seeded class starts with, from the least to the most
        /// restrictive. Each names the groups whose members are cleared for it; a level that
        /// names no group would be closed to everyone, which is not a useful starting point.
        /// </summary>
        private static readonly (string Name, string Description, int Rank, bool IsDefault, string[] Groups)[] _securityLevelTemplates =
        [
            (
                "Public",
                "Visible to everyone who may open the workspace. The starting classification of a new record.",
                0,
                true,
                ["Admin", "Engineering", "Marketing", "Support"]
            ),
            (
                "Internal",
                "Visible to the operating teams. Keeps a record out of reach of accounts that are not part of them.",
                10,
                false,
                ["Admin", "Engineering", "Support"]
            ),
            (
                "Confidential",
                "Visible to administrators only. Records carrying it disappear from every list of anyone else.",
                20,
                false,
                ["Admin"]
            )
        ];

        /// <summary>
        /// Adds the standard set of security levels to every concrete class.
        /// </summary>
        /// <remarks>
        /// The levels are class scoped exactly the way fields are, so each class gets its own
        /// three rows rather than sharing one catalog. The groups are resolved by name; a
        /// template naming a group the installation does not have simply contributes nothing
        /// to the clearance rather than failing the seed.
        /// </remarks>
        /// <param name="db">The database context the entities are added to. Cannot be null.</param>
        private static void SeedSecurityLevels(KleeneStarDbContext db)
        {
            var groups = db.Groups
                .AsNoTracking()
                .ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase);

            var classes = db.Classes
                .AsNoTracking()
                .Where(x => !x.IsAbstract)
                .ToList();

            foreach (var cls in classes)
            {
                foreach (var template in _securityLevelTemplates)
                {
                    db.SecurityLevels.Add(new SecurityLevel
                    {
                        Id = Guid.NewGuid(),
                        Name = template.Name,
                        Description = template.Description,
                        State = SecurityLevelState.Active,
                        Rank = template.Rank,
                        IsDefault = template.IsDefault,
                        ClassId = cls.Id,
                        PermittedGroupIds =
                        [
                            .. template.Groups
                                .Where(groups.ContainsKey)
                                .Select(x => groups[x])
                        ],
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    });
                }
            }
        }

        /// <summary>
        /// Classifies a share of the seeded objects, so a fresh installation shows what a
        /// classification does rather than only that it can be configured.
        /// </summary>
        /// <remarks>
        /// Runs once, on an installation where nothing is classified yet - the caller guards it
        /// that way, because re-running it would put a classification back on a record somebody
        /// deliberately declassified. The share is picked by position rather than at random:
        /// the same checkout always produces the same classified records, which is what makes
        /// the effect reproducible when demonstrating it.
        /// </remarks>
        /// <param name="db">The database context holding the seeded rows. Cannot be null.</param>
        private static void SeedObjectSecurityLevels(KleeneStarDbContext db)
        {
            var levelsByClass = db.SecurityLevels
                .AsNoTracking()
                .ToList()
                .GroupBy(x => x.ClassId)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.Rank).ToList());

            var objects = db.Objects
                .Where(x => x.SecurityLevelId == null)
                .OrderBy(x => x.RawId)
                .ToList();

            var seen = new Dictionary<Guid, int>();

            foreach (var objectEntity in objects)
            {
                if (!levelsByClass.TryGetValue(objectEntity.ClassId, out var levels) || levels.Count == 0)
                {
                    continue;
                }

                var position = seen.TryGetValue(objectEntity.ClassId, out var count) ? count : 0;
                seen[objectEntity.ClassId] = position + 1;

                // every seventh record of a class is confidential and every third internal;
                // the rest stay on the default level the class starts with
                var level = (position % 7) == 6
                    ? levels[^1]
                    : (position % 3) == 2
                        ? levels[Math.Min(1, levels.Count - 1)]
                        : levels.FirstOrDefault(x => x.IsDefault) ?? levels[0];

                objectEntity.SecurityLevelId = level.Id;
            }
        }
    }
}
