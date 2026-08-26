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
        /// Hands a small set of objects to the seeded identities as shares and as watches, so
        /// the "shared with me" and "watched" entry paths of the landing page have something
        /// to show on a fresh installation - and so the share and watcher cards of an object
        /// are populated out of the box.
        /// </summary>
        /// <remarks>
        /// The two sets are drawn from opposite ends of the object list so a share and a
        /// watch land on different objects: what somebody was given access to and what they
        /// follow are different questions, and a seed that answered both with the same rows
        /// would hide the difference. Every seeded identity receives some of each, the
        /// admin included: a demo installation is normally read as the admin, and a path
        /// that is empty for whoever is looking demonstrates nothing.
        /// </remarks>
        /// <param name="db">The database context to which the rows are added. Cannot be null.</param>
        private static void SeedSharesAndWatchers(KleeneStarDbContext db)
        {
            var admin = Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD");
            var alice = Guid.Parse("BBF45E5D-AA35-4382-9B84-6055193CE544");
            var support = Guid.Parse("D1C5AED2-78D3-45F7-BB19-E87B8F134301");

            var identityIds = db.Identities
                .AsNoTracking()
                .Select(x => x.Id)
                .ToHashSet();

            var issues = db.Objects
                .AsNoTracking()
                .Where(x => x.Kind == ObjectKind.Issue && x.State == WorkspaceState.Active)
                .OrderBy(x => x.Key)
                .Select(x => x.Id)
                .Take(40)
                .ToList();

            if (issues.Count == 0)
            {
                return;
            }

            void share(Guid objectId, Guid identityId)
            {
                if (!identityIds.Contains(identityId))
                {
                    return;
                }

                db.ObjectShares.Add(new ObjectShare
                {
                    ObjectId = objectId,
                    IdentityId = identityId,
                    Created = DateTime.UtcNow
                });
            }

            void watch(Guid objectId, Guid identityId)
            {
                if (!identityIds.Contains(identityId))
                {
                    return;
                }

                db.ObjectWatchers.Add(new ObjectWatcher
                {
                    ObjectId = objectId,
                    IdentityId = identityId,
                    Created = DateTime.UtcNow
                });
            }

            // shared: the first few issues are handed round-robin to the seeded identities
            var recipients = new[] { admin, alice, support };

            foreach (var (objectId, index) in issues.Take(9).Select((id, i) => (id, i)))
            {
                share(objectId, recipients[index % recipients.Length]);
            }

            // watched: the last few are followed by the admin and by Alice, so both a
            // populated and a mixed watcher list exist
            var watched = new List<Guid>(issues.Skip(issues.Count - 8));

            foreach (var (objectId, index) in watched.Select((id, i) => (id, i)))
            {
                watch(objectId, admin);

                if (index % 3 == 0)
                {
                    watch(objectId, alice);
                }
            }
        }
    }
}
