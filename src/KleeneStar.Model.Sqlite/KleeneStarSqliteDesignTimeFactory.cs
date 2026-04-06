using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KleeneStar.Model.Sqlite
{
    /// <summary>
    /// Provides a design-time factory for creating instances of KleeneStarDbContext configured 
    /// to use a local SQLite database.
    /// </summary>
    public class KleeneStarSqliteDesignTimeFactory : IDesignTimeDbContextFactory<KleeneStarDbContext>
    {
        /// <summary>
        /// Creates a new instance of the KleeneStarDbContext configured for design-time operations.
        /// </summary>
        /// <remarks>
        /// This method is typically used by design-time tools such as Entity Framework Core
        /// migrations to create a context instance when the application's runtime services are 
        /// not available.
        /// </remarks>
        /// <param name="args">
        /// An array of command-line arguments. This parameter is not used in this implementation.
        /// </param>
        /// <returns>
        /// A new KleeneStarDbContext instance configured to use a local SQLite database for 
        /// design-time purposes.
        /// </returns>
        public KleeneStarDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<KleeneStarDbContext>()
                .UseSqlite
                (
                    "Data Source=design-time.db",
                    x => x.MigrationsAssembly("KleeneStar.Model.Sqlite")
                )
                .Options;

            return new KleeneStarDbContext(options);
        }
    }
}