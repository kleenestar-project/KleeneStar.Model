using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the WorkflowState class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestStatus
    {
        /// <summary>
        /// Verifies that a new WorkflowState instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var workflowState = new Status();

            // validation
            Assert.NotEqual(Guid.Empty, workflowState.Id);
        }

        /// <summary>
        /// Sets the properties of a WorkflowState instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("State A", "Description A", StatusState.Active)]
        [InlineData("State B", null, StatusState.Archived)]
        public void SetProperties(string name, string description, StatusState state)
        {
            // arrange
            var workflowState = new Status();
            var classId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            // act
            workflowState.Name = name;
            workflowState.Description = description;
            workflowState.State = state;
            workflowState.ClassId = classId;
            workflowState.CategoryId = categoryId;

            // validation
            Assert.Equal(name, workflowState.Name);
            Assert.Equal(description, workflowState.Description);
            Assert.Equal(state, workflowState.State);
            Assert.Equal(classId, workflowState.ClassId);
            Assert.Equal(categoryId, workflowState.CategoryId);
        }
    }
}
