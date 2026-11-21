using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace KleeneStar.Model
{
    internal static partial class ModelHub
    {
        /// <summary>
        /// Determines whether the database is SQLite, the one provider backed up here.
        /// </summary>
        public static bool IsSqlite => DatabaseSettings?.Provider?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true;

        /// <summary>
        /// Writes a consistent copy of the database to a file.
        /// </summary>
        /// <remarks>
        /// <c>VACUUM INTO</c> copies the database as one transaction sees it, while the
        /// installation keeps running: no write is half in the copy. A file that exists is not
        /// overwritten - a backup never replaces another.
        /// </remarks>
        /// <param name="file">The file to write.</param>
        /// <exception cref="NotSupportedException">The database is not SQLite.</exception>
        /// <exception cref="IOException">The file exists.</exception>
        public static void Backup(string file)
        {
            if (!IsSqlite)
            {
                throw new NotSupportedException("Only a SQLite database is backed up by the installation; back up other databases with their own tools.");
            }

            if (File.Exists(file))
            {
                throw new IOException($"The backup '{file}' exists already.");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(file)));

            using var db = CreateDbContext();

            db.Database.ExecuteSqlRaw("VACUUM INTO {0}", Path.GetFullPath(file));
        }

        /// <summary>
        /// Checks a SQLite database file - a backup before it is trusted, or the database after
        /// it was restored.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns><c>ok</c>, or what the check found.</returns>
        public static string CheckIntegrity(string file)
        {
            var settings = new Settings.DatabaseSettings
            {
                Provider = DatabaseSettings?.Provider ?? "Microsoft.Data.Sqlite",
                Assembly = DatabaseSettings?.Assembly ?? "KleeneStar.Model.Sqlite",
                ConnectionString = $"Data Source={Path.GetFullPath(file)};Mode=ReadOnly;Pooling=False"
            };

            using var db = KleeneStarDbContextFactory.Create(settings);
            var connection = db.Database.GetDbConnection();

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = "PRAGMA integrity_check";

            return command.ExecuteScalar()?.ToString();
        }
    }
}
