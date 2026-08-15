using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the <see cref="SlaPolicy"/> entity.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestSlaPolicy
    {
        /// <summary>
        /// Verifies that a new policy is assigned a non-empty unique identifier.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            var policy = new SlaPolicy();

            Assert.NotEqual(Guid.Empty, policy.Id);
        }

        /// <summary>
        /// Verifies that the explicit-id constructor sets the supplied id.
        /// </summary>
        [Fact]
        public void InitializeWithExplicitId()
        {
            var id = Guid.NewGuid();

            var policy = new SlaPolicy(id);

            Assert.Equal(id, policy.Id);
        }

        /// <summary>
        /// Verifies that scalar properties can be set and round-tripped.
        /// </summary>
        [Theory]
        [InlineData("Incident · P1", "24/7 coverage", SlaPolicyState.Active,   SlaPriority.Critical)]
        [InlineData("Incident · P3", null,            SlaPolicyState.Draft,    SlaPriority.Low)]
        [InlineData("Legacy",        "Migrated",      SlaPolicyState.Inactive, SlaPriority.Medium)]
        public void SetScalarProperties(string name, string description, SlaPolicyState state, SlaPriority priority)
        {
            var calendarId = Guid.NewGuid();

            var policy = new SlaPolicy
            {
                Name = name,
                Description = description,
                State = state,
                Priority = priority,
                CalendarId = calendarId
            };

            Assert.Equal(name, policy.Name);
            Assert.Equal(description, policy.Description);
            Assert.Equal(state, policy.State);
            Assert.Equal(priority, policy.Priority);
            Assert.Equal(calendarId, policy.CalendarId);
        }

        /// <summary>
        /// Verifies that the child collections start empty and accept added entries.
        /// </summary>
        [Fact]
        public void ChildCollectionsAcceptEntries()
        {
            var policy = new SlaPolicy();

            Assert.Empty(policy.Targets);
            Assert.Empty(policy.Scope);
            Assert.Empty(policy.Escalations);

            policy.Targets.Add(new SlaTarget { Name = "Erstreaktion", Kind = SlaTargetKind.Response, TargetValue = 30, Unit = SlaTargetUnit.Minutes });
            policy.Scope.Add(new SlaScopeRule { RuleType = SlaScopeRuleType.Priority, Value = "High" });
            policy.Escalations.Add(new SlaEscalationLevel { Level = 1, AfterValue = 15, Unit = SlaTargetUnit.Minutes, Notify = "Team Lead" });

            Assert.Single(policy.Targets);
            Assert.Single(policy.Scope);
            Assert.Single(policy.Escalations);
        }

        /// <summary>
        /// Verifies that <see cref="SlaNotificationChannels"/> is a flags enum that combines values.
        /// </summary>
        [Fact]
        public void NotificationChannelsCombineAsFlags()
        {
            var combined = SlaNotificationChannels.Email | SlaNotificationChannels.InApp;

            Assert.True(combined.HasFlag(SlaNotificationChannels.Email));
            Assert.True(combined.HasFlag(SlaNotificationChannels.InApp));

            // a single channel must not carry the other one, which is what tells a flags enum
            // apart from one whose values happen to be distinct numbers
            Assert.False(SlaNotificationChannels.Email.HasFlag(SlaNotificationChannels.InApp));
            Assert.False(SlaNotificationChannels.InApp.HasFlag(SlaNotificationChannels.Email));
        }

        /// <summary>
        /// Verifies the state-extension helpers (IsActive/Id/Text/Color).
        /// </summary>
        [Theory]
        [InlineData(SlaPolicyState.Draft, false)]
        [InlineData(SlaPolicyState.Active, true)]
        [InlineData(SlaPolicyState.Inactive, false)]
        [InlineData(SlaPolicyState.Archived, false)]
        public void PolicyStateExtensions(SlaPolicyState state, bool expectedActive)
        {
            Assert.Equal(expectedActive, state.IsActive());
            Assert.NotEqual(Guid.Empty, state.Id());
            Assert.False(string.IsNullOrEmpty(state.Text()));
            Assert.False(string.IsNullOrEmpty(state.Color()));
        }
    }
}
