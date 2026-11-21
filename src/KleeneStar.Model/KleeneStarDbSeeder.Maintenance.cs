using KleeneStar.Model.Entities;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Adds the maintenance notice of the installation to the specified database context.
        /// </summary>
        /// <remarks>
        /// The record is created disabled and without a text. It exists from the start so the
        /// settings page has something to edit and the toast something to read, rather than the
        /// two having to agree on how a missing record is to be treated.
        /// </remarks>
        /// <param name="db">
        /// The database context to which the maintenance notice will be added. Cannot be null.
        /// </param>
        private static void SeedMaintenance(KleeneStarDbContext db)
        {
            db.Maintenances.Add(new Maintenance
            {
                Enabled = false,
                Message = null
            });
        }
    }
}
