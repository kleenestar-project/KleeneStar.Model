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
        /// Adds a predefined set of tenant entities to the specified database context.
        /// </summary>
        /// <param name="db">The database context to which the tenant entities will be added. Cannot be null.</param>
        private static void SeedTenants(KleeneStarDbContext db)
        {
            void add(string id, string name, string description) => db.Tenants.Add(new Tenant
            {
                Id = Guid.Parse(id),
                Name = name,
                Description = description
            });

            add("A1B2C3D4-E5F6-7890-ABCD-EF1234567890", "Acme Corp", "Primary enterprise tenant.");
            add("B2C3D4E5-F6A7-8901-BCDE-F12345678901", "Globex Inc", "Global operations tenant.");
            add("C3D4E5F6-A7B8-9012-CDEF-123456789012", "Initech", "Technology services tenant.");
        }
    }
}
