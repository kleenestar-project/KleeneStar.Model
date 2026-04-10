using KleeneStar.Model.Entities;
using System;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Adds a predefined set of permission profile entities to the specified database context.
        /// </summary>
        /// <param name="db">The database context to which the permission profile entities will be added. Cannot be null.</param>
        private static void SeedPermissionProfiles(KleeneStarDbContext db)
        {
            void add(string id, string name, string description) => db.PermissionProfiles.Add(new PermissionProfile
            {
                Id = Guid.Parse(id),
                Name = name,
                Description = description
            });

            add("D4E5F6A7-B8C9-0123-DEFA-234567890123", "Administrator", "Full access to all workspace resources.");
            add("E5F6A7B8-C9D0-1234-EFAB-345678901234", "Editor", "Can create and modify workspace content.");
            add("F6A7B8C9-D0E1-2345-FABC-456789012345", "Viewer", "Read-only access to workspace resources.");
        }
    }
}
