using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

using ObjectEntity = KleeneStar.Model.Entities.Object;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub security level.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubSecurityLevel
    {
        /// <summary>
        /// Points the hub at an isolated in-memory database.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        private static void Configure(string connectionString)
        {
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = connectionString,
                Assembly = "KleeneStar.Model.Test"
            };
        }

        /// <summary>
        /// Verifies that all security levels can be retrieved from the database.
        /// </summary>
        [Fact]
        public void AllSecurityLevels()
        {
            // arrange
            Configure(nameof(AllSecurityLevels));

            using (var db = ModelHub.CreateDbContext())
            {
                db.SecurityLevels.Add(new SecurityLevel { Id = Guid.NewGuid(), Name = "Public" });
                db.SecurityLevels.Add(new SecurityLevel { Id = Guid.NewGuid(), Name = "Confidential" });
                db.SaveChanges();
            }

            // act
            var result = ModelHub.GetSecurityLevels(new Query<SecurityLevel>()).ToList();

            // validation
            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the clearance round-trips through the serialized column.
        /// </summary>
        [Fact]
        public void ClearanceRoundTrips()
        {
            // arrange
            Configure(nameof(ClearanceRoundTrips));

            var id = Guid.NewGuid();
            var first = Guid.NewGuid();
            var second = Guid.NewGuid();

            // act
            ModelHub.Add(new SecurityLevel(id)
            {
                Name = "Confidential",
                PermittedGroupIds = [first, second]
            });

            var result = ModelHub.GetSecurityLevels(new Query<SecurityLevel>().WhereEquals(x => x.Id, id))
                .FirstOrDefault();

            // validation
            Assert.NotNull(result);
            Assert.Equal([first, second], result.PermittedGroupIds);
        }

        /// <summary>
        /// Verifies that adding a security level whose id already exists changes nothing.
        /// </summary>
        [Fact]
        public void AddSecurityLevelWhenNotExists()
        {
            // arrange
            Configure(nameof(AddSecurityLevelWhenNotExists));

            var id = Guid.NewGuid();

            // act
            ModelHub.Add(new SecurityLevel(id) { Name = "Public" });
            ModelHub.Add(new SecurityLevel(id) { Name = "Renamed" });

            var result = ModelHub.GetSecurityLevels(new Query<SecurityLevel>()).ToList();

            // validation
            Assert.Single(result);
            Assert.Equal("Public", result[0].Name);
        }

        /// <summary>
        /// Verifies that removing a security level declassifies the objects that carried it
        /// rather than leaving them pointing at a level that is gone.
        /// </summary>
        [Fact]
        public void RemoveSecurityLevelDeclassifiesObjects()
        {
            // arrange
            Configure(nameof(RemoveSecurityLevelDeclassifiesObjects));

            var workspaceId = Guid.NewGuid();
            var classId = Guid.NewGuid();
            var levelId = Guid.NewGuid();
            var objectId = Guid.NewGuid();

            using (var db = ModelHub.CreateDbContext())
            {
                db.Workspaces.Add(new Workspace { Id = workspaceId, Key = "ws-sec", Name = "main" });
                db.Classes.Add(new Class { Id = classId, Name = "Incident", WorkspaceId = workspaceId });
                db.SecurityLevels.Add(new SecurityLevel(levelId) { Name = "Confidential", ClassId = classId });
                db.Objects.Add(new ObjectEntity(objectId)
                {
                    Key = "SEC-1",
                    Summary = "Classified",
                    WorkspaceId = workspaceId,
                    ClassId = classId,
                    SecurityLevelId = levelId
                });
                db.SaveChanges();
            }

            // act
            var level = ModelHub.GetSecurityLevels(new Query<SecurityLevel>().WhereEquals(x => x.Id, levelId))
                .First();

            ModelHub.Remove(level);

            // validation
            var result = ModelHub.GetObjects(new Query<ObjectEntity>().WhereEquals(x => x.Id, objectId))
                .FirstOrDefault();

            Assert.NotNull(result);
            Assert.Null(result.SecurityLevelId);
            Assert.Empty(ModelHub.GetSecurityLevels(new Query<SecurityLevel>()));
        }
    }
}
