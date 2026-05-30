using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Attaches a small, colored set of tags (labels) to every seeded object so the object
        /// tag card is populated out of the box. Each object receives two distinct tags picked
        /// round-robin from a fixed palette, which keeps the composite unique index on
        /// (ObjectId, Name) satisfied.
        /// </summary>
        /// <param name="db">The database context to which the tags are added. Cannot be null.</param>
        private static void SeedTags(KleeneStarDbContext db)
        {
            var palette = new (string Name, string Color)[]
            {
                ("Urgent", "#dc3545"),
                ("Backend", "#0d6efd"),
                ("Frontend", "#6f42c1"),
                ("Bug", "#fd7e14"),
                ("Enhancement", "#198754"),
                ("Documentation", "#0dcaf0")
            };

            var objectIds = db.Objects
                .AsNoTracking()
                .Select(o => o.Id)
                .ToList();

            var index = 0;
            foreach (var objectId in objectIds)
            {
                var first = palette[index % palette.Length];
                var second = palette[(index + 1) % palette.Length];

                db.ObjectTags.Add(new ObjectTag
                {
                    ObjectId = objectId,
                    Name = first.Name,
                    Color = first.Color,
                    Created = DateTime.UtcNow
                });

                db.ObjectTags.Add(new ObjectTag
                {
                    ObjectId = objectId,
                    Name = second.Name,
                    Color = second.Color,
                    Created = DateTime.UtcNow
                });

                index++;
            }
        }
    }
}
