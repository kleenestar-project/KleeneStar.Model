using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the <see cref="ModelHub"/> SLA surface.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubSla
    {
        private static readonly Guid WorkspaceId = Guid.Parse("3946B811-DFBB-4575-A83B-5C1C0240DF22");
        private static readonly Guid ClassId = Guid.Parse("B54AA5B2-01D5-490A-90A3-4D57FE50320B");

        private static void SeedClassFor(string connectionString)
        {
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig
            {
                ConnectionString = connectionString,
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();

            if (!db.Workspaces.Any(x => x.Id == WorkspaceId))
            {
                db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-sla", Name = "workspace" });
            }

            if (!db.Classes.Any(x => x.Id == ClassId))
            {
                db.Classes.Add(new Class { Id = ClassId, Name = "Incident", WorkspaceId = WorkspaceId });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies that all SLA policies attached to a class can be retrieved with their children.
        /// </summary>
        [Fact]
        public void AllPolicies()
        {
            var connectionString = nameof(AllPolicies);
            SeedClassFor(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.SlaPolicies.Add(new SlaPolicy { Id = Guid.NewGuid(), Name = "Alpha", ClassId = ClassId });
                db.SlaPolicies.Add(new SlaPolicy { Id = Guid.NewGuid(), Name = "Beta",  ClassId = ClassId });
                db.SaveChanges();
            }

            var result = ModelHub.GetSlaPolicies(new Query<SlaPolicy>()).ToList();

            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies the policy filter pipeline returns only matching entries.
        /// </summary>
        [Fact]
        public void FilteredPolicies()
        {
            var connectionString = nameof(FilteredPolicies);
            SeedClassFor(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.SlaPolicies.Add(new SlaPolicy { Id = Guid.NewGuid(), Name = "Alpha", ClassId = ClassId });
                db.SlaPolicies.Add(new SlaPolicy { Id = Guid.NewGuid(), Name = "Beta",  ClassId = ClassId });
                db.SaveChanges();
            }

            var result = ModelHub.GetSlaPolicies(new Query<SlaPolicy>().Where(x => x.Name.StartsWith("A"))).ToList();

            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
        }

        /// <summary>
        /// Verifies that adding a policy persists the policy and its child collections.
        /// </summary>
        [Fact]
        public void AddPolicyPersistsChildren()
        {
            var connectionString = nameof(AddPolicyPersistsChildren);
            SeedClassFor(connectionString);

            var policy = new SlaPolicy
            {
                Id = Guid.NewGuid(),
                Name = "VIP",
                ClassId = ClassId,
                State = SlaPolicyState.Active,
                Priority = SlaPriority.Critical,
                CalendarId = null,
                Notifications = SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
                Targets =
                {
                    new SlaTarget { Name = "First response", Kind = SlaTargetKind.Response,   TargetValue = 5, Unit = SlaTargetUnit.Minutes },
                    new SlaTarget { Name = "Resolution",     Kind = SlaTargetKind.Resolution, TargetValue = 1, Unit = SlaTargetUnit.Hours },
                },
                Scope =
                {
                    new SlaScopeRule { RuleType = SlaScopeRuleType.Tag, Value = "VIP-User" },
                },
                Escalations =
                {
                    new SlaEscalationLevel { AfterValue = 3, Unit = SlaTargetUnit.Minutes, Notify = "VIP Support Lead" },
                }
            };

            ModelHub.Add(policy);

            var loaded = ModelHub.GetSlaPolicies(new Query<SlaPolicy>().WhereEquals(x => x.Id, policy.Id)).Single();

            Assert.Equal("VIP", loaded.Name);
            Assert.Equal(2, loaded.Targets.Count);
            Assert.Single(loaded.Scope);
            Assert.Single(loaded.Escalations);
            Assert.True(loaded.Notifications.HasFlag(SlaNotificationChannels.InApp));
        }

        /// <summary>
        /// Verifies that adding a policy whose id already exists is a no-op.
        /// </summary>
        [Fact]
        public void AddPolicyWhenIdExistsIsNoOp()
        {
            var connectionString = nameof(AddPolicyWhenIdExistsIsNoOp);
            SeedClassFor(connectionString);

            var id = Guid.NewGuid();

            ModelHub.Add(new SlaPolicy { Id = id, Name = "Alpha", ClassId = ClassId });
            ModelHub.Add(new SlaPolicy { Id = id, Name = "Beta",  ClassId = ClassId });

            using var db = ModelHub.CreateDbContext();
            var policies = db.SlaPolicies.Where(x => x.Id == id).ToList();
            Assert.Single(policies);
            Assert.Equal("Alpha", policies[0].Name);
        }

        /// <summary>
        /// Verifies that updating a policy replaces scalar properties and the child collections.
        /// </summary>
        [Fact]
        public void UpdatePolicyReplacesChildren()
        {
            var connectionString = nameof(UpdatePolicyReplacesChildren);
            SeedClassFor(connectionString);

            var policy = new SlaPolicy
            {
                Id = Guid.NewGuid(),
                Name = "Initial",
                ClassId = ClassId,
                Targets = { new SlaTarget { Name = "Resp", Kind = SlaTargetKind.Response, TargetValue = 1, Unit = SlaTargetUnit.Hours } },
                Escalations = { new SlaEscalationLevel { AfterValue = 1, Unit = SlaTargetUnit.Days, Notify = "old" } }
            };
            ModelHub.Add(policy);

            var update = new SlaPolicy
            {
                Id = policy.Id,
                Name = "Updated",
                ClassId = ClassId,
                Priority = SlaPriority.High,
                Targets =
                {
                    new SlaTarget { Name = "Resp", Kind = SlaTargetKind.Response,   TargetValue = 30, Unit = SlaTargetUnit.Minutes },
                    new SlaTarget { Name = "Res",  Kind = SlaTargetKind.Resolution, TargetValue = 4,  Unit = SlaTargetUnit.Hours },
                },
                Escalations =
                {
                    new SlaEscalationLevel { AfterValue = 15, Unit = SlaTargetUnit.Minutes, Notify = "lead" },
                    new SlaEscalationLevel { AfterValue = 45, Unit = SlaTargetUnit.Minutes, Notify = "head" },
                }
            };

            ModelHub.Update(update);

            var loaded = ModelHub.GetSlaPolicies(new Query<SlaPolicy>().WhereEquals(x => x.Id, policy.Id)).Single();

            Assert.Equal("Updated", loaded.Name);
            Assert.Equal(SlaPriority.High, loaded.Priority);
            Assert.Equal(2, loaded.Targets.Count);
            Assert.Equal(2, loaded.Escalations.Count);
            Assert.Contains(loaded.Escalations, e => e.Level == 1 && e.Notify == "lead");
            Assert.Contains(loaded.Escalations, e => e.Level == 2 && e.Notify == "head");
        }

        /// <summary>
        /// Verifies that removing a policy cascades to its child collections.
        /// </summary>
        [Fact]
        public void RemovePolicyCascadesChildren()
        {
            var connectionString = nameof(RemovePolicyCascadesChildren);
            SeedClassFor(connectionString);

            var policy = new SlaPolicy
            {
                Id = Guid.NewGuid(),
                Name = "ToRemove",
                ClassId = ClassId,
                Targets = { new SlaTarget { Name = "Resp", Kind = SlaTargetKind.Response, TargetValue = 1, Unit = SlaTargetUnit.Hours } },
                Scope = { new SlaScopeRule { RuleType = SlaScopeRuleType.Tag, Value = "x" } },
                Escalations = { new SlaEscalationLevel { AfterValue = 1, Unit = SlaTargetUnit.Days, Notify = "x" } }
            };
            ModelHub.Add(policy);

            ModelHub.Remove(policy);

            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.SlaPolicies.Where(x => x.Id == policy.Id));
            Assert.Empty(db.SlaTargets.Where(x => x.PolicyId == policy.Id));
            Assert.Empty(db.SlaScopeRules.Where(x => x.PolicyId == policy.Id));
            Assert.Empty(db.SlaEscalationLevels.Where(x => x.PolicyId == policy.Id));
        }

        /// <summary>
        /// Verifies that removing a non-existent policy is a no-op.
        /// </summary>
        [Fact]
        public void RemoveWhenPolicyNotExistsIsNoOp()
        {
            var connectionString = nameof(RemoveWhenPolicyNotExistsIsNoOp);
            SeedClassFor(connectionString);

            ModelHub.Remove(new SlaPolicy { Id = Guid.NewGuid(), ClassId = ClassId });

            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.SlaPolicies);
        }
    }
}
