using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Workflow class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestWorkflow
    {
        /// <summary>
        /// Verifies that a new Workflow instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var workflow = new Workflow();

            // validation
            Assert.NotEqual(Guid.Empty, workflow.Id);
        }

        /// <summary>
        /// Sets the properties of a Workflow instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Workflow A", "Description A", WorkflowState.Active)]
        [InlineData("Workflow B", null, WorkflowState.Archived)]
        public void SetProperties(string name, string description, WorkflowState state)
        {
            // arrange
            var workflow = new Workflow();
            var classId = Guid.NewGuid();

            // act
            workflow.Name = name;
            workflow.Description = description;
            workflow.State = state;
            workflow.ClassId = classId;

            // validation
            Assert.Equal(name, workflow.Name);
            Assert.Equal(description, workflow.Description);
            Assert.Equal(state, workflow.State);
            Assert.Equal(classId, workflow.ClassId);
        }
    }
}
