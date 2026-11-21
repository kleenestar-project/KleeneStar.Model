using KleeneStar.Model;
using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the <see cref="ModelHub"/> WQL history surface — the queries
    /// an identity has actually run, which the query prompts offer back instead of the
    /// hand-written examples they used to carry.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubWqlHistory
    {
        private static readonly Guid OwnerId = Guid.Parse("2B0C8A16-9B1E-4B2E-9F5B-6E4E2D9C1A70");
        private static readonly Guid OtherOwnerId = Guid.Parse("7D2F4A88-3C55-4A61-8B0D-1F9E7C4B5A32");

        private const string Objects = "KleeneStar.Model.Entities.Object";
        private const string Tenants = "KleeneStar.Model.Entities.Tenant";

        /// <summary>
        /// Points the hub at an isolated database and puts the two identities in it. An entry
        /// hangs off its owner, so a history cannot be written for an identity that is not
        /// there.
        /// </summary>
        /// <param name="connectionString">The name of the isolated database.</param>
        private static void SeedIdentities(string connectionString)
        {
            ModelHub.DatabaseSettings = new KleeneStar.Model.Settings.DatabaseSettings
            {
                ConnectionString = connectionString,
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();

            foreach (var id in new[] { OwnerId, OtherOwnerId })
            {
                if (!db.Identities.Any(x => x.Id == id))
                {
                    db.Identities.Add(new Identity
                    {
                        Id = id,
                        Name = $"user-{id:N}",
                        Email = $"{id:N}@example.org",
                        PasswordHash = "x"
                    });
                }
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies the basic contract: what was run comes back, most recent first.
        /// </summary>
        [Fact]
        public void RecordedQueriesComeBackMostRecentFirst()
        {
            SeedIdentities(nameof(RecordedQueriesComeBackMostRecentFirst));

            ModelHub.RecordWqlQuery(OwnerId, Objects, "Summary ~ \"login\"");
            ModelHub.RecordWqlQuery(OwnerId, Objects, "Key ~ \"SD-9000\"");

            Assert.Equal(["Key ~ \"SD-9000\"", "Summary ~ \"login\""], ModelHub.GetWqlHistory(OwnerId, Objects));
        }

        /// <summary>
        /// Verifies that re-running a query moves it to the front instead of adding a second
        /// row. A history of the same expression over and over is not a history, and repeating
        /// one must not push everything else out of a capped list.
        /// </summary>
        [Fact]
        public void RunningTheSameQueryAgainMovesItRatherThanRepeatingIt()
        {
            SeedIdentities(nameof(RunningTheSameQueryAgainMovesItRatherThanRepeatingIt));

            ModelHub.RecordWqlQuery(OwnerId, Objects, "Summary ~ \"login\"");
            ModelHub.RecordWqlQuery(OwnerId, Objects, "Key ~ \"SD-9000\"");
            var again = ModelHub.RecordWqlQuery(OwnerId, Objects, "Summary ~ \"login\"");

            Assert.Equal(["Summary ~ \"login\"", "Key ~ \"SD-9000\""], ModelHub.GetWqlHistory(OwnerId, Objects));
            Assert.Equal(2, again.UseCount);

            using var db = ModelHub.CreateDbContext();
            Assert.Equal(2, db.WqlHistories.Count(x => x.OwnerId == OwnerId && x.Subject == Objects));
        }

        /// <summary>
        /// Verifies that the surrounding whitespace of a submitted query does not make it a
        /// different entry — the prompt trims before it sends, but nothing guarantees every
        /// caller does.
        /// </summary>
        [Fact]
        public void WhitespaceDoesNotMakeASecondEntry()
        {
            SeedIdentities(nameof(WhitespaceDoesNotMakeASecondEntry));

            ModelHub.RecordWqlQuery(OwnerId, Objects, "State = Active");
            ModelHub.RecordWqlQuery(OwnerId, Objects, "  State = Active  ");

            Assert.Equal(["State = Active"], ModelHub.GetWqlHistory(OwnerId, Objects));
        }

        /// <summary>
        /// Verifies the cap: the newest entries are kept and the oldest falls off, so the
        /// table cannot grow without bound behind a list nobody can navigate that far into.
        /// </summary>
        [Fact]
        public void TheOldestEntryFallsOffAtTheCap()
        {
            SeedIdentities(nameof(TheOldestEntryFallsOffAtTheCap));

            for (var i = 0; i <= WqlHistory.Limit; i++)
            {
                ModelHub.RecordWqlQuery(OwnerId, Objects, $"Summary ~ \"q{i}\"");
            }

            var history = ModelHub.GetWqlHistory(OwnerId, Objects, WqlHistory.Limit * 2);

            Assert.Equal(WqlHistory.Limit, history.Count);
            Assert.Equal($"Summary ~ \"q{WqlHistory.Limit}\"", history[0]);
            Assert.DoesNotContain("Summary ~ \"q0\"", history);
        }

        /// <summary>
        /// Verifies that a history belongs to one identity and one subject. The subject is
        /// what lets the prompt over an entity read back what the table over the same entity
        /// ran, and it is what keeps two unrelated prompts from showing each other's queries.
        /// </summary>
        [Fact]
        public void HistoriesAreSeparatePerOwnerAndSubject()
        {
            SeedIdentities(nameof(HistoriesAreSeparatePerOwnerAndSubject));

            ModelHub.RecordWqlQuery(OwnerId, Objects, "Summary ~ \"mine\"");
            ModelHub.RecordWqlQuery(OwnerId, Tenants, "Name ~ \"acme\"");
            ModelHub.RecordWqlQuery(OtherOwnerId, Objects, "Summary ~ \"theirs\"");

            Assert.Equal(["Summary ~ \"mine\""], ModelHub.GetWqlHistory(OwnerId, Objects));
            Assert.Equal(["Name ~ \"acme\""], ModelHub.GetWqlHistory(OwnerId, Tenants));
            Assert.Equal(["Summary ~ \"theirs\""], ModelHub.GetWqlHistory(OtherOwnerId, Objects));
        }

        /// <summary>
        /// Verifies that nothing is written for input there is no history to keep for: a blank
        /// query, no subject, no owner, or an identity that does not exist. The last one is the
        /// one worth guarding — the row hangs off its owner, so without the check the insert
        /// would fail on the foreign key rather than being skipped.
        /// </summary>
        [Fact]
        public void NothingIsRecordedWithoutAnOwnerASubjectAndAQuery()
        {
            SeedIdentities(nameof(NothingIsRecordedWithoutAnOwnerASubjectAndAQuery));

            Assert.Null(ModelHub.RecordWqlQuery(OwnerId, Objects, "   "));
            Assert.Null(ModelHub.RecordWqlQuery(OwnerId, Objects, null));
            Assert.Null(ModelHub.RecordWqlQuery(OwnerId, "  ", "State = Active"));
            Assert.Null(ModelHub.RecordWqlQuery(Guid.Empty, Objects, "State = Active"));
            Assert.Null(ModelHub.RecordWqlQuery(Guid.NewGuid(), Objects, "State = Active"));

            Assert.Empty(ModelHub.GetWqlHistory(OwnerId, Objects));
        }

        /// <summary>
        /// Verifies that reading answers empty rather than throwing when there is nothing to
        /// read, so a prompt on a fresh installation simply offers no history.
        /// </summary>
        [Fact]
        public void ReadingAnEmptyHistoryAnswersEmpty()
        {
            SeedIdentities(nameof(ReadingAnEmptyHistoryAnswersEmpty));

            Assert.Empty(ModelHub.GetWqlHistory(OwnerId, Objects));
            Assert.Empty(ModelHub.GetWqlHistory(Guid.Empty, Objects));
            Assert.Empty(ModelHub.GetWqlHistory(OwnerId, null));
            Assert.Empty(ModelHub.GetWqlHistory(OwnerId, Objects, 0));
        }

        /// <summary>
        /// Verifies that a history can be cleared, for one subject or entirely.
        /// </summary>
        [Fact]
        public void ClearingRemovesOneSubjectOrEverything()
        {
            SeedIdentities(nameof(ClearingRemovesOneSubjectOrEverything));

            ModelHub.RecordWqlQuery(OwnerId, Objects, "Summary ~ \"a\"");
            ModelHub.RecordWqlQuery(OwnerId, Tenants, "Name ~ \"b\"");
            ModelHub.RecordWqlQuery(OtherOwnerId, Objects, "Summary ~ \"c\"");

            Assert.Equal(1, ModelHub.ClearWqlHistory(OwnerId, Objects));
            Assert.Empty(ModelHub.GetWqlHistory(OwnerId, Objects));
            Assert.Equal(["Name ~ \"b\""], ModelHub.GetWqlHistory(OwnerId, Tenants));

            Assert.Equal(1, ModelHub.ClearWqlHistory(OwnerId));
            Assert.Empty(ModelHub.GetWqlHistory(OwnerId, Tenants));

            // another identity's trail is untouched
            Assert.Equal(["Summary ~ \"c\""], ModelHub.GetWqlHistory(OtherOwnerId, Objects));
        }
    }
}
