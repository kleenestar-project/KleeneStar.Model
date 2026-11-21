using KleeneStar.Model.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KleeneStar.Model.Test
{
    /// <summary>
    /// Verifies on a real SQLite database that every save advances the versions of exactly the
    /// entity types it writes.
    /// </summary>
    public class UnitTestEntityVersions : IDisposable
    {
        private readonly SqliteConnection _connection;

        /// <summary>
        /// Opens an empty database.
        /// </summary>
        public UnitTestEntityVersions()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            using var db = Context();

            db.Database.EnsureCreated();
        }

        /// <summary>
        /// Creates a context on the shared connection.
        /// </summary>
        /// <returns>The context.</returns>
        private KleeneStarDbContext Context()
        {
            return new KleeneStarDbContext(new DbContextOptionsBuilder<KleeneStarDbContext>().UseSqlite(_connection).Options);
        }

        /// <summary>
        /// Closes the database.
        /// </summary>
        public void Dispose()
        {
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Adding, changing and removing a row each advance the version of its type. The
        /// versions are process wide and other tests write in parallel, so only an advance is
        /// asserted, never an unchanged version.
        /// </summary>
        [Fact]
        public void SaveAdvancesTheVersionsOfTheWrittenTypes()
        {
            var workspaceId = Guid.NewGuid();

            var before = EntityVersions.Of(typeof(Workspace));

            using (var db = Context())
            {
                db.Workspaces.Add(new Workspace { Id = workspaceId, Key = "VER", Name = "Versions" });
                db.SaveChanges();
            }

            var added = EntityVersions.Of(typeof(Workspace));

            using (var db = Context())
            {
                db.Workspaces.Single(x => x.Id == workspaceId).Name = "Renamed";
                db.SaveChanges();
            }

            var modified = EntityVersions.Of(typeof(Workspace));

            using (var db = Context())
            {
                db.Workspaces.Remove(db.Workspaces.Single(x => x.Id == workspaceId));
                db.SaveChanges();
            }

            var removed = EntityVersions.Of(typeof(Workspace));

            Assert.True(added > before);
            Assert.True(modified > added);
            Assert.True(removed > modified);
        }
    }
}
