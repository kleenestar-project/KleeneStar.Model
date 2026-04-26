using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the form class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestForm
    {
        /// <summary>
        /// Verifies that a new form instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var form = new Form();

            // validation
            Assert.NotEqual(Guid.Empty, form.Id);
        }

        /// <summary>
        /// Sets the properties of a form instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Form A", "Description A", FormState.Active, FormType.Create)]
        [InlineData("Form B", null, FormState.Archived, FormType.Default)]
        public void SetProperties(string name, string description, FormState state, FormType formType)
        {
            // arrange
            var form = new Form();

            // act
            form.Name = name;
            form.Description = description;
            form.State = state;
            form.FormType = formType;

            // validation
            Assert.Equal(name, form.Name);
            Assert.Equal(description, form.Description);
            Assert.Equal(state, form.State);
            Assert.Equal(formType, form.FormType);
        }

        /// <summary>
        /// Verifies that a new form instance defaults to the Standard form type.
        /// </summary>
        [Fact]
        public void DefaultFormTypeIsStandard()
        {
            // act
            var form = new Form();

            // validation
            Assert.Equal(FormType.Default, form.FormType);
        }
    }
}
