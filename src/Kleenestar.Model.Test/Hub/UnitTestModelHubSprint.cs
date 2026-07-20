using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;
using KleeneStarObject = KleeneStar.Model.Entities.Object;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub sprint helpers: sprint CRUD and the sprint
    /// assignment columns of the object entity (move, rank, story points).
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubSprint
    {
        /// <summary>
        /// Points the ModelHub at an isolated in-memory database and seeds a workspace
        /// with three backlog objects.
        /// </summary>
        /// <param name="connectionString">The per-test in-memory database name.</param>
        /// <returns>The workspace id and the ids of the three seeded objects in rank order.</returns>
        private static (Guid WorkspaceId, Guid First, Guid Second, Guid Third) Seed(string connectionString)
        {
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig()
            {
                ConnectionString = connectionString,
                Assembly = "KleeneStar.Model.Test"
            };

            var workspaceId = Guid.NewGuid();
            var first = Guid.NewGuid();
            var second = Guid.NewGuid();
            var third = Guid.NewGuid();

            using var db = ModelHub.CreateDbContext();

            db.Workspaces.Add(new Workspace { Id = workspaceId, Name = "W", Key = $"W-{connectionString}" });
            db.Objects.Add(new KleeneStarObject { Id = first, Key = $"A-{connectionString}", Summary = "Summary A", WorkspaceId = workspaceId, SprintRank = 1 });
            db.Objects.Add(new KleeneStarObject { Id = second, Key = $"B-{connectionString}", Summary = "Summary B", WorkspaceId = workspaceId, SprintRank = 2 });
            db.Objects.Add(new KleeneStarObject { Id = third, Key = $"C-{connectionString}", Summary = "Summary C", WorkspaceId = workspaceId, SprintRank = 3 });
            db.SaveChanges();

            return (workspaceId, first, second, third);
        }

        /// <summary>
        /// Creates and persists a sprint in the given workspace.
        /// </summary>
        /// <param name="workspaceId">The owning workspace id.</param>
        /// <param name="name">The sprint name.</param>
        /// <returns>The persisted sprint.</returns>
        private static Sprint AddSprint(Guid workspaceId, string name = "Sprint 1")
        {
            var sprint = new Sprint
            {
                Id = Guid.NewGuid(),
                Name = name,
                State = SprintState.Planned,
                WorkspaceId = workspaceId,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            ModelHub.Add(sprint);

            return sprint;
        }

        /// <summary>
        /// Verifies that a sprint round-trips through Add and GetSprints.
        /// </summary>
        [Fact]
        public void AddSprint_RoundTrip()
        {
            var (workspaceId, _, _, _) = Seed(nameof(AddSprint_RoundTrip));

            var sprint = AddSprint(workspaceId);

            var loaded = ModelHub.GetSprints(new Query<Sprint>().WhereEquals(x => x.Id, sprint.Id)).ToList();

            Assert.Single(loaded);
            Assert.Equal("Sprint 1", loaded[0].Name);
            Assert.Equal(workspaceId, loaded[0].WorkspaceId);
        }

        /// <summary>
        /// Verifies that Update writes the scalar properties back and refreshes the
        /// Updated timestamp.
        /// </summary>
        [Fact]
        public void UpdateSprint_ChangesScalars()
        {
            var (workspaceId, _, _, _) = Seed(nameof(UpdateSprint_ChangesScalars));

            var sprint = AddSprint(workspaceId);

            sprint.Name = "Renamed";
            sprint.State = SprintState.Active;
            sprint.Capacity = 21;
            ModelHub.Update(sprint);

            var loaded = ModelHub.GetSprints(new Query<Sprint>().WhereEquals(x => x.Id, sprint.Id)).Single();

            Assert.Equal("Renamed", loaded.Name);
            Assert.Equal(SprintState.Active, loaded.State);
            Assert.Equal(21, loaded.Capacity);
        }

        /// <summary>
        /// Verifies that SetObjectSprint commits an object to a sprint, appends it at
        /// the end and keeps the backlog ranks dense.
        /// </summary>
        [Fact]
        public void SetObjectSprint_MovesAndReRanks()
        {
            var (workspaceId, first, second, third) = Seed(nameof(SetObjectSprint_MovesAndReRanks));

            var sprint = AddSprint(workspaceId);

            ModelHub.SetObjectSprint(second, sprint.Id, null);

            var committed = ModelHub.GetObjectsBySprint(workspaceId, sprint.Id);
            var backlog = ModelHub.GetObjectsBySprint(workspaceId, null);

            Assert.Single(committed);
            Assert.Equal(second, committed[0].Id);
            Assert.Equal(1, committed[0].SprintRank);

            Assert.Equal(2, backlog.Count);
            Assert.Equal(first, backlog[0].Id);
            Assert.Equal(1, backlog[0].SprintRank);
            Assert.Equal(third, backlog[1].Id);
            Assert.Equal(2, backlog[1].SprintRank);
        }

        /// <summary>
        /// Verifies that SetObjectSprint honors an explicit rank inside the same group,
        /// reordering without duplicating ranks.
        /// </summary>
        [Fact]
        public void SetObjectSprint_ReordersWithinGroup()
        {
            var (workspaceId, first, second, third) = Seed(nameof(SetObjectSprint_ReordersWithinGroup));

            ModelHub.SetObjectSprint(third, null, 1);

            var backlog = ModelHub.GetObjectsBySprint(workspaceId, null);

            Assert.Equal(3, backlog.Count);
            Assert.Equal(third, backlog[0].Id);
            Assert.Equal(first, backlog[1].Id);
            Assert.Equal(second, backlog[2].Id);
            Assert.Equal(new[] { 1, 2, 3 }, backlog.Select(x => x.SprintRank).ToArray());
        }

        /// <summary>
        /// Verifies that SetObjectStoryPoints persists and clears the estimate.
        /// </summary>
        [Fact]
        public void SetObjectStoryPoints_PersistsAndClears()
        {
            var (workspaceId, first, _, _) = Seed(nameof(SetObjectStoryPoints_PersistsAndClears));

            ModelHub.SetObjectStoryPoints(first, 5);

            using (var db = ModelHub.CreateDbContext())
            {
                Assert.Equal(5, db.Objects.Single(x => x.Id == first).StoryPoints);
            }

            ModelHub.SetObjectStoryPoints(first, null);

            using (var db = ModelHub.CreateDbContext())
            {
                Assert.Null(db.Objects.Single(x => x.Id == first).StoryPoints);
            }
        }

        /// <summary>
        /// Verifies that Remove deletes the sprint and moves its committed objects back
        /// to the backlog behind the existing backlog items.
        /// </summary>
        [Fact]
        public void RemoveSprint_MovesObjectsBackToBacklog()
        {
            var (workspaceId, first, second, third) = Seed(nameof(RemoveSprint_MovesObjectsBackToBacklog));

            var sprint = AddSprint(workspaceId);
            ModelHub.SetObjectSprint(first, sprint.Id, null);
            ModelHub.SetObjectSprint(second, sprint.Id, null);

            ModelHub.Remove(sprint);

            Assert.Empty(ModelHub.GetSprints(new Query<Sprint>().WhereEquals(x => x.Id, sprint.Id)));

            var backlog = ModelHub.GetObjectsBySprint(workspaceId, null);

            Assert.Equal(3, backlog.Count);
            Assert.Equal(third, backlog[0].Id);
            Assert.Equal(first, backlog[1].Id);
            Assert.Equal(second, backlog[2].Id);
            Assert.Equal(new[] { 1, 2, 3 }, backlog.Select(x => x.SprintRank).ToArray());
        }
    }
}
