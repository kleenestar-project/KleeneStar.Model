using KleeneStar.Model.Entities;
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
        /// The seeded admin identity that owns the example workspace bookmarks. Mirrors the id
        /// assigned in <see cref="SeedIdentities"/> (and the saved-search owner).
        /// </summary>
        private static readonly Guid WorkspaceBookmarkOwnerId = Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD");

        /// <summary>
        /// Adds a default set of workspace bookmarks owned by the seeded admin identity, if none
        /// already exist, so the workspace dropdown shows favorites and recent visits on first run.
        /// Each entry is resolved by workspace key; missing keys are skipped. The recency hours are
        /// staggered so the dropdown's "newest first" ordering is immediately visible.
        /// </summary>
        /// <param name="db">The database context to seed. Cannot be null.</param>
        private static void SeedWorkspaceBookmarks(KleeneStarDbContext db)
        {
            void add(string workspaceKey, bool favorite, int recencyHours)
            {
                var workspace = db.Workspaces.FirstOrDefault(x => x.Key == workspaceKey);
                if (workspace is null)
                {
                    return;
                }

                var now = DateTime.UtcNow;
                db.WorkspaceBookmarks.Add(new WorkspaceBookmark
                {
                    OwnerId = WorkspaceBookmarkOwnerId,
                    WorkspaceId = workspace.Id,
                    Favorite = favorite,
                    LastVisited = now.AddHours(-recencyHours),
                    Created = now,
                    Updated = now
                });
            }

            // newest visit first; "SD" and "CMDB" are also pinned as favorites
            add("SD", favorite: true, recencyHours: 1);
            add("DEV", favorite: false, recencyHours: 3);
            add("CMDB", favorite: true, recencyHours: 8);
            add("FIN", favorite: false, recencyHours: 30);
            add("HR", favorite: false, recencyHours: 50);
        }
    }
}
