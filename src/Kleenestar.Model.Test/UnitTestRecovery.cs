using KleeneStar.Model.Entities;
using KleeneStar.Model.Settings;
using Microsoft.EntityFrameworkCore;

namespace KleeneStar.Model.Test
{
    /// <summary>
    /// Tests what recovery rests on: the migrations build a database from nothing that matches
    /// the model, a backup is a consistent and checked copy that holds the data, and a backup
    /// never overwrites another.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestRecovery : IDisposable
    {
        private readonly string _directory = Path.Combine(Path.GetTempPath(), "kleenestar-recovery-" + Guid.NewGuid().ToString("N"));
        private readonly DatabaseSettings _previous = ModelHub.DatabaseSettings;

        /// <summary>
        /// Points the model hub at a fresh SQLite file.
        /// </summary>
        public UnitTestRecovery()
        {
            Directory.CreateDirectory(_directory);

            ModelHub.DatabaseSettings = new DatabaseSettings
            {
                Provider = "Microsoft.Data.Sqlite",
                Assembly = "KleeneStar.Model.Sqlite",
                ConnectionString = $"Data Source={Path.Combine(_directory, "kleenestar.db")};Pooling=False"
            };
        }

        /// <summary>
        /// Restores the settings and removes the files.
        /// </summary>
        public void Dispose()
        {
            ModelHub.DatabaseSettings = _previous;

            try
            {
                Directory.Delete(_directory, recursive: true);
            }
            catch (IOException)
            {
                // a file still held by the sqlite runtime is left to the temp folder's cleanup
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Every migration applies to an empty database, and the result is the model: nothing
        /// in the entities or their configuration is left without a migration.
        /// </summary>
        [Fact]
        public void Migrations_BuildTheModelFromNothing()
        {
            using var db = ModelHub.CreateDbContext();

            db.Database.Migrate();

            Assert.Empty(db.Database.GetPendingMigrations());
            Assert.False(db.Database.HasPendingModelChanges(), "The model has changes no migration carries.");
        }

        /// <summary>
        /// A backup holds the data as it was, passes the integrity check, and is never written
        /// over an existing one.
        /// </summary>
        [Fact]
        public void Backup_IsAConsistentCheckedCopy()
        {
            using (var db = ModelHub.CreateDbContext())
            {
                db.Database.Migrate();
                db.Workspaces.Add(new Workspace { Id = Guid.NewGuid(), Key = "BAK", Name = "Backed up" });
                db.SaveChanges();
            }

            var file = Path.Combine(_directory, "backup", "kleenestar-1.db");

            ModelHub.Backup(file);

            Assert.Equal("ok", ModelHub.CheckIntegrity(file));
            Assert.Throws<IOException>(() => ModelHub.Backup(file));

            // the copy, opened on its own, holds what the database held
            var original = ModelHub.DatabaseSettings;

            ModelHub.DatabaseSettings = new DatabaseSettings { Provider = original.Provider, Assembly = original.Assembly, ConnectionString = $"Data Source={file};Pooling=False" };

            try
            {
                using var copy = ModelHub.CreateDbContext();

                Assert.Equal("BAK", Assert.Single(copy.Workspaces).Key);
            }
            finally
            {
                ModelHub.DatabaseSettings = original;
            }
        }

        /// <summary>
        /// A database the installation does not back up itself is refused with the reason.
        /// </summary>
        [Fact]
        public void Backup_RefusesOtherProviders()
        {
            ModelHub.DatabaseSettings = new DatabaseSettings { Provider = "Npgsql", ConnectionString = "Host=none" };

            Assert.Throws<NotSupportedException>(() => ModelHub.Backup(Path.Combine(_directory, "x.db")));
        }
    }
}
