using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace KleeneStar.Model.Sqlite
{
    /// <summary>
    /// Provides a factory for creating a SQLite-specific KleeneStarDbContext instance.
    /// </summary>
    public static class SqliteDbContextFactory
    {
        /// <summary>
        /// The connection strings whose database has already been switched to the
        /// write-ahead log, so the pragma runs once per file rather than per context.
        /// </summary>
        private static readonly HashSet<string> _prepared = [];

        /// <summary>
        /// Guards <see cref="_prepared"/>; contexts are created from request threads.
        /// </summary>
        private static readonly object _sync = new();

        /// <summary>
        /// Creates a SQLite-configured DbContext using the given connection string.
        /// </summary>
        /// <param name="connectionString">The SQLite connection string.</param>
        /// <returns>A configured KleeneStarDbContext instance.</returns>
        public static KleeneStarDbContext Create(string connectionString)
        {
            var effective = BuildConnectionString(connectionString);

            EnsureWriteAheadLog(effective);

            var options = new DbContextOptionsBuilder<KleeneStarDbContext>()
                .UseSqlite
                (
                    effective,
                    x => x.MigrationsAssembly("KleeneStar.Model.Sqlite")
                )
                .Options;

            return new KleeneStarDbContext(options);
        }

        /// <summary>
        /// Adds the busy timeout to the configured connection string.
        /// </summary>
        /// <remarks>
        /// Without it a connection that meets a locked database fails at once with
        /// "database is locked" instead of waiting for the writer to finish — the request
        /// then surfaces an error where a short wait would have served it. The timeout is
        /// the upper bound on that wait, not a delay every connection pays.
        /// </remarks>
        /// <param name="connectionString">The configured connection string.</param>
        /// <returns>The effective connection string.</returns>
        private static string BuildConnectionString(string connectionString)
        {
            var builder = new SqliteConnectionStringBuilder(connectionString)
            {
                DefaultTimeout = 30
            };

            return builder.ToString();
        }

        /// <summary>
        /// Switches the database to the write-ahead log, once per database file.
        /// </summary>
        /// <remarks>
        /// In the default rollback-journal mode a writer locks the whole database, so any
        /// read arriving meanwhile fails. The write-ahead log lets readers work while a
        /// write is in flight, which is what an application serving several requests at
        /// once needs. The setting is a property of the database file rather than of the
        /// connection, so it survives in the file and one execution is enough — hence the
        /// guard, which keeps the pragma off the path of every context creation.
        ///
        /// The mode adds a <c>-wal</c> and a <c>-shm</c> file beside the database. Both
        /// belong to it and are checkpointed back on a clean shutdown.
        /// </remarks>
        /// <param name="connectionString">The effective connection string.</param>
        private static void EnsureWriteAheadLog(string connectionString)
        {
            lock (_sync)
            {
                if (!_prepared.Add(connectionString))
                {
                    return;
                }
            }

            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "PRAGMA journal_mode=WAL;";
                command.ExecuteNonQuery();
            }
            catch (SqliteException)
            {
                // a database that cannot be switched (a read-only file, a mode another
                // connection holds) still works in its current journal mode, so this must
                // not keep the context from being created. Forgetting the attempt lets a
                // later creation try again.
                lock (_sync)
                {
                    _prepared.Remove(connectionString);
                }
            }
        }
    }
}
