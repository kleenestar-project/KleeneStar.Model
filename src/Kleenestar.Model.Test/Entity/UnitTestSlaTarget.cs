using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for <see cref="SlaTarget"/>, <see cref="SlaScopeRule"/>, and <see cref="SlaEscalationLevel"/>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestSlaTarget
    {
        /// <summary>
        /// Verifies that <see cref="SlaTarget"/> defaults its id and persists scalar properties.
        /// </summary>
        [Fact]
        public void TargetInitializesIdAndSetsProperties()
        {
            var target = new SlaTarget
            {
                Name = "First response",
                Kind = SlaTargetKind.Response,
                TargetValue = 30,
                Unit = SlaTargetUnit.Minutes
            };

            Assert.NotEqual(Guid.Empty, target.Id);
            Assert.Equal("First response", target.Name);
            Assert.Equal(SlaTargetKind.Response, target.Kind);
            Assert.Equal(30, target.TargetValue);
            Assert.Equal(SlaTargetUnit.Minutes, target.Unit);
        }

        /// <summary>
        /// Verifies that <see cref="SlaScopeRule"/> defaults its id and persists scalar properties.
        /// </summary>
        [Fact]
        public void ScopeRuleInitializesIdAndSetsProperties()
        {
            var rule = new SlaScopeRule
            {
                RuleType = SlaScopeRuleType.Tag,
                Value = "VIP-User"
            };

            Assert.NotEqual(Guid.Empty, rule.Id);
            Assert.Equal(SlaScopeRuleType.Tag, rule.RuleType);
            Assert.Equal("VIP-User", rule.Value);
        }

        /// <summary>
        /// Verifies that <see cref="SlaEscalationLevel"/> defaults its id and persists scalar properties.
        /// </summary>
        [Fact]
        public void EscalationInitializesIdAndSetsProperties()
        {
            var escalation = new SlaEscalationLevel
            {
                Level = 2,
                AfterValue = 45,
                Unit = SlaTargetUnit.Minutes,
                Notify = "Head of IT Operations"
            };

            Assert.NotEqual(Guid.Empty, escalation.Id);
            Assert.Equal(2, escalation.Level);
            Assert.Equal(45, escalation.AfterValue);
            Assert.Equal(SlaTargetUnit.Minutes, escalation.Unit);
            Assert.Equal("Head of IT Operations", escalation.Notify);
        }
    }
}
