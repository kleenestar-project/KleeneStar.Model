using Microsoft.EntityFrameworkCore;

namespace KleeneStar.Model.Sqlite
{
    /// <summary>
    /// Provides a factory for creating a SQLite-specific KleeneStarDbContext instance.
    /// </summary>
    public static class SqlServerDbContextFactory
    {
        /// <summary>
        /// Creates a SqlServer-configured DbContext using the given connection string.
        /// </summary>
        /// <param name="connectionString">The SqlServer connection string.</param>
        /// <returns>A configured KleeneStarDbContext instance.</returns>
        public static KleeneStarDbContext Create(string connectionString)
        {
            var options = new DbContextOptionsBuilder<KleeneStarDbContext>()
                .UseSqlite
                (
                    connectionString,
                    x => x.MigrationsAssembly("KleeneStar.Model.Sqlite")
                )
                .Options;

            return new KleeneStarDbContext(options);
        }
    }
}
