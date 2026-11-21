using KleeneStar.Model.Entities;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Adds the branding record of the installation to the specified database context.
        /// </summary>
        /// <remarks>
        /// The record is created empty, which reads as "use what the application declared". It
        /// exists from the start so the settings page has something to edit rather than the page
        /// and the endpoint having to agree on how a missing record is to be treated.
        /// </remarks>
        /// <param name="db">
        /// The database context to which the branding record will be added. Cannot be null.
        /// </param>
        private static void SeedBranding(KleeneStarDbContext db)
        {
            db.Brandings.Add(new Branding
            {
                Title = null,
                Icon = null
            });
        }
    }
}
