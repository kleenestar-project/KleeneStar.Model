using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace KleeneStar.Model.Test
{
    /// <summary>
    /// Provides a factory for creating a SQLite-specific KleeneStarDbContext instance.
    /// </summary>
    public static class InMemoryDbContextFactory
    {
        private static readonly ConcurrentDictionary<string, DbContextOptions<KleeneStarDbContext>> _optionsCache
            = new();

        /// <summary>
        /// Creates a SQLite-configured DbContext using the given connection string.
        /// </summary>
        /// <param name="connectionString">The SQLite connection string.</param>
        /// <returns>A configured KleeneStarDbContext instance.</returns>
        public static KleeneStarDbContext Create(string connectionString)
        {
            var key = string.IsNullOrWhiteSpace(connectionString)
                ? "DefaultInMemoryDb"
                : connectionString;

            var options = _optionsCache.GetOrAdd(key, _ =>
            {
                return new DbContextOptionsBuilder<KleeneStarDbContext>()
                    .UseInMemoryDatabase(key)
                    .Options;
            });

            return new KleeneStarDbContext(options);

        }
    }
}