using KleeneStar.Model.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KleeneStar.Model.Test
{
    /// <summary>
    /// Holds the tables of automation, integration, approval, planning and catalogs to their
    /// integrity on a real SQLite database - the in-memory provider of the other tests enforces
    /// neither foreign keys nor unique indexes: what belongs to an object goes with it, what
    /// belongs to a workspace goes with it, a record that must outlive its source does, and the
    /// indexes that keep a decision, a delivery or a version single refuse a second one.
    /// </summary>
    public class UnitTestProcessIntegrity : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly Guid _workspaceId = Guid.NewGuid();
        private readonly Guid _classId = Guid.NewGuid();
        private readonly Guid _objectId = Guid.NewGuid();

        /// <summary>
        /// Opens a database with a workspace, a class and an object.
        /// </summary>
        public UnitTestProcessIntegrity()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            using var db = Context();

            db.Database.EnsureCreated();
            db.Workspaces.Add(new Workspace { Id = _workspaceId, Key = "INT", Name = "Integrity" });
            db.Classes.Add(new Class { Id = _classId, Name = "Ticket", WorkspaceId = _workspaceId });
            db.Objects.Add(new Entities.Object(_objectId) { Key = "INT-1", Summary = "First", WorkspaceId = _workspaceId, ClassId = _classId });
            db.SaveChanges();
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
        /// The approvals, decisions, worklogs and planning figures of an object go with it.
        /// </summary>
        [Fact]
        public void ObjectDeletion_TakesItsRecordsAlong()
        {
            var approvalId = Guid.NewGuid();

            using (var db = Context())
            {
                db.Approvals.Add(new Approval { Id = approvalId, ObjectId = _objectId, PolicyId = Guid.NewGuid(), Steps = "[]", Requested = DateTime.UtcNow, StepStarted = DateTime.UtcNow, Version = Guid.NewGuid() });
                db.ApprovalDecisions.Add(new ApprovalDecision { Id = Guid.NewGuid(), ApprovalId = approvalId, IdentityId = Guid.NewGuid(), Decided = DateTime.UtcNow });
                db.Worklogs.Add(new Worklog { Id = Guid.NewGuid(), ObjectId = _objectId, IdentityId = Guid.NewGuid(), Minutes = 30, Started = DateTime.UtcNow, Created = DateTime.UtcNow, Updated = DateTime.UtcNow });
                db.ObjectPlannings.Add(new ObjectPlanning { Id = Guid.NewGuid(), ObjectId = _objectId, OriginalEstimateMinutes = 60, Updated = DateTime.UtcNow });
                db.SaveChanges();
            }

            using (var db = Context())
            {
                db.Objects.Remove(db.Objects.Single(x => x.Id == _objectId));
                db.SaveChanges();
            }

            using var check = Context();

            Assert.Empty(check.Approvals);
            Assert.Empty(check.ApprovalDecisions);
            Assert.Empty(check.Worklogs);
            Assert.Empty(check.ObjectPlannings);
        }

        /// <summary>
        /// The configuration of a workspace - rules, webhooks, policies, milestones, catalogs with
        /// their versions - goes with it.
        /// </summary>
        [Fact]
        public void WorkspaceDeletion_TakesItsConfigurationAlong()
        {
            var catalogId = Guid.NewGuid();

            using (var db = Context())
            {
                db.AutomationRules.Add(new AutomationRule { Id = Guid.NewGuid(), WorkspaceId = _workspaceId, Name = "r", Trigger = "object.created", Actions = "escalate", Created = DateTime.UtcNow, Updated = DateTime.UtcNow });
                db.Webhooks.Add(new Webhook { Id = Guid.NewGuid(), WorkspaceId = _workspaceId, Name = "hook", Created = DateTime.UtcNow, Updated = DateTime.UtcNow });
                db.ApprovalPolicies.Add(new ApprovalPolicy { Id = Guid.NewGuid(), WorkspaceId = _workspaceId, ClassId = _classId, Name = "p", Steps = "A: any x", Created = DateTime.UtcNow, Updated = DateTime.UtcNow });
                db.Milestones.Add(new Milestone { Id = Guid.NewGuid(), WorkspaceId = _workspaceId, Name = "m", Created = DateTime.UtcNow, Updated = DateTime.UtcNow });
                db.ValueCatalogs.Add(new ValueCatalog { Id = catalogId, WorkspaceId = _workspaceId, Name = "c", Created = DateTime.UtcNow, Updated = DateTime.UtcNow });
                db.ValueCatalogVersions.Add(new ValueCatalogVersion { Id = Guid.NewGuid(), CatalogId = catalogId, Number = 1, Values = "[]", Created = DateTime.UtcNow });
                db.WorkspacePlannings.Add(new WorkspacePlanning { Id = Guid.NewGuid(), WorkspaceId = _workspaceId, TimeTracking = true });
                db.SaveChanges();
            }

            using (var db = Context())
            {
                db.Workspaces.Remove(db.Workspaces.Single(x => x.Id == _workspaceId));
                db.SaveChanges();
            }

            using var check = Context();

            Assert.Empty(check.AutomationRules);
            Assert.Empty(check.Webhooks);
            Assert.Empty(check.ApprovalPolicies);
            Assert.Empty(check.Milestones);
            Assert.Empty(check.ValueCatalogs);
            Assert.Empty(check.ValueCatalogVersions);
            Assert.Empty(check.WorkspacePlannings);
        }

        /// <summary>
        /// An object outlives its milestone, and an approval outlives its policy - it is the
        /// record of what was decided.
        /// </summary>
        [Fact]
        public void Records_OutliveTheirSources()
        {
            var milestoneId = Guid.NewGuid();
            var policyId = Guid.NewGuid();

            using (var db = Context())
            {
                db.Milestones.Add(new Milestone { Id = milestoneId, WorkspaceId = _workspaceId, Name = "m", Created = DateTime.UtcNow, Updated = DateTime.UtcNow });
                db.ObjectPlannings.Add(new ObjectPlanning { Id = Guid.NewGuid(), ObjectId = _objectId, MilestoneId = milestoneId, Updated = DateTime.UtcNow });
                db.ApprovalPolicies.Add(new ApprovalPolicy { Id = policyId, WorkspaceId = _workspaceId, ClassId = _classId, Name = "p", Steps = "A: any x", Created = DateTime.UtcNow, Updated = DateTime.UtcNow });
                db.Approvals.Add(new Approval { Id = Guid.NewGuid(), ObjectId = _objectId, PolicyId = policyId, PolicyName = "p", Steps = "[]", Requested = DateTime.UtcNow, StepStarted = DateTime.UtcNow, Version = Guid.NewGuid() });
                db.SaveChanges();
            }

            using (var db = Context())
            {
                db.Milestones.Remove(db.Milestones.Single(x => x.Id == milestoneId));
                db.ApprovalPolicies.Remove(db.ApprovalPolicies.Single(x => x.Id == policyId));
                db.SaveChanges();
            }

            using var check = Context();

            Assert.Null(Assert.Single(check.ObjectPlannings).MilestoneId);
            Assert.Equal("p", Assert.Single(check.Approvals).PolicyName);
        }

        /// <summary>
        /// The indexes that keep things single refuse a second one: a decision per approver and
        /// step, a job per rule and occurrence, a version number per catalog, a webhook name per
        /// workspace.
        /// </summary>
        [Fact]
        public void UniqueIndexes_RefuseDuplicates()
        {
            var approvalId = Guid.NewGuid();
            var approverId = Guid.NewGuid();
            var catalogId = Guid.NewGuid();
            var ruleId = Guid.NewGuid();

            using (var db = Context())
            {
                db.Approvals.Add(new Approval { Id = approvalId, ObjectId = _objectId, PolicyId = Guid.NewGuid(), Steps = "[]", Requested = DateTime.UtcNow, StepStarted = DateTime.UtcNow, Version = Guid.NewGuid() });
                db.ValueCatalogs.Add(new ValueCatalog { Id = catalogId, WorkspaceId = _workspaceId, Name = "c", Created = DateTime.UtcNow, Updated = DateTime.UtcNow });
                db.AutomationRules.Add(new AutomationRule { Id = ruleId, WorkspaceId = _workspaceId, Name = "r", Trigger = "object.created", Actions = "escalate", Created = DateTime.UtcNow, Updated = DateTime.UtcNow });
                db.SaveChanges();
            }

            void Refused(Action<KleeneStarDbContext> twice)
            {
                using (var db = Context())
                {
                    twice(db);
                    db.SaveChanges();
                }

                using var again = Context();

                twice(again);
                Assert.Throws<DbUpdateException>(() => again.SaveChanges());
            }

            Refused(db => db.ApprovalDecisions.Add(new ApprovalDecision { Id = Guid.NewGuid(), ApprovalId = approvalId, Step = 0, IdentityId = approverId, Decided = DateTime.UtcNow }));
            Refused(db => db.ValueCatalogVersions.Add(new ValueCatalogVersion { Id = Guid.NewGuid(), CatalogId = catalogId, Number = 1, Values = "[]", Created = DateTime.UtcNow }));
            Refused(db => db.AutomationJobs.Add(new AutomationJob { Id = Guid.NewGuid(), RuleId = ruleId, ObjectId = _objectId, Trigger = "object.created", Deduplication = "same", Version = Guid.NewGuid(), Created = DateTime.UtcNow, Updated = DateTime.UtcNow, NextAttempt = DateTime.UtcNow }));
            Refused(db => db.Webhooks.Add(new Webhook { Id = Guid.NewGuid(), WorkspaceId = _workspaceId, Name = "same", Created = DateTime.UtcNow, Updated = DateTime.UtcNow }));
        }
    }
}
