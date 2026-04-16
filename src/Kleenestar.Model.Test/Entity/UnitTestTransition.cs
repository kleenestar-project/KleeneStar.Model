using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the WorkflowTransition class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestTransition
    {
        /// <summary>
        /// Verifies that a new WorkflowTransition instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var transition = new Transition();

            // validation
            Assert.NotEqual(Guid.Empty, transition.Id);
        }

        /// <summary>
        /// Sets the properties of a WorkflowTransition instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Transition A", "Description A", TransitionState.Active)]
        [InlineData("Transition B", null, TransitionState.Archived)]
        public void SetProperties(string name, string description, TransitionState state)
        {
            // arrange
            var transition = new Transition();
            var workflowId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();
            var targetId = Guid.NewGuid();

            // act
            transition.Name = name;
            transition.Description = description;
            transition.State = state;
            transition.WorkflowId = workflowId;
            transition.SourceId = sourceId;
            transition.TargetId = targetId;

            // validation
            Assert.Equal(name, transition.Name);
            Assert.Equal(description, transition.Description);
            Assert.Equal(state, transition.State);
            Assert.Equal(workflowId, transition.WorkflowId);
            Assert.Equal(sourceId, transition.SourceId);
            Assert.Equal(targetId, transition.TargetId);
        }
    }
}
