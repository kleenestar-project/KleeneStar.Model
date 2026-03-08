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
        /// Adds a predefined set of category entities to the specified database context.
        /// </summary>
        /// <remarks>
        /// This method is intended to seed the database with a standard set of categories. 
        /// It does not save changes to the database; callers must call SaveChanges on the 
        /// context to persist the additions.
        /// </remarks>
        /// <param name="db">The database context to which the category entities will be added. Cannot be null.</param>
        private static void SeedCategories(KleeneStarDbContext db)
        {
            void add(string id, string name) => db.Categories.Add(new Category
            {
                Id = Guid.Parse(id),
                Name = name
            });

            add("6B90F75F-93C6-4186-965F-C2F974A04E05", "Compliance");
            add("AF7C999B-334F-4745-9799-AD46BCF7D7E0", "Development");
            add("DB91D6FD-1FB3-4F3B-B982-EDFCC733DEAF", "Engineering");
            add("9B08E835-4B55-4748-84BA-14AC1417C52E", "Finance");
            add("8CC28181-5DBC-4F29-8016-41885FF5A323", "HumanResources");
            add("26AC5FB6-CBAB-40A8-9D2F-5DA9E91910C9", "Infrastructure");
            add("9ECA0869-E563-41DB-81A2-50DC7D09478B", "Operations");
            add("F56D0256-4C67-47D7-AEF3-563D05EF9CEB", "Support");
        }
    }
}
