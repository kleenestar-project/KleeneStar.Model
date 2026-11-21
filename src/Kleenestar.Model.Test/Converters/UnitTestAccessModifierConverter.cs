using KleeneStar.Model.Converters;
using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Converters
{
    /// <summary>
    /// Tests the access modifier converter that classes and fields bind through.
    /// </summary>
    public class UnitTestAccessModifierConverter
    {
        /// <summary>
        /// Verifies that every id the class and field selections offer binds to its modifier
        /// and reads back as the same id.
        /// </summary>
        [Theory]
        [InlineData(AccessModifier.Public)]
        [InlineData(AccessModifier.Protected)]
        [InlineData(AccessModifier.Private)]
        [InlineData(AccessModifier.Internal)]
        public void RoundTripsTheSelectionId(AccessModifier modifier)
        {
            var converter = new AccessModifierConverter();

            var bound = converter.FromRaw(modifier.Id().ToString(), typeof(AccessModifier));

            Assert.Equal(modifier, bound);
            Assert.Equal(modifier.Id().ToString(), converter.ToRaw(modifier, typeof(AccessModifier)));
        }

        /// <summary>
        /// Verifies that the converter binds a value the class and field properties can hold.
        /// </summary>
        [Fact]
        public void BindsTheClassAndFieldProperty()
        {
            var converter = new AccessModifierConverter();
            var item = new Class();
            var field = new Field();

            typeof(Class).GetProperty(nameof(Class.AccessModifier))
                .SetValue(item, converter.FromRaw(AccessModifier.Private.Id().ToString(), typeof(AccessModifier)));
            typeof(Field).GetProperty(nameof(Field.AccessModifier))
                .SetValue(field, converter.FromRaw(AccessModifier.Internal.Id().ToString(), typeof(AccessModifier)));

            Assert.Equal(AccessModifier.Private, item.AccessModifier);
            Assert.Equal(AccessModifier.Internal, field.AccessModifier);
        }

        /// <summary>
        /// Verifies that a workspace modifier id is not mistaken for a class or field modifier.
        /// </summary>
        [Fact]
        public void WorkspaceIdIsNoClassModifier()
        {
            var bound = new AccessModifierConverter()
                .FromRaw(WorkspaceAccessModifier.Private.Id().ToString(), typeof(AccessModifier));

            Assert.Equal(AccessModifier.Public, bound);
        }
    }
}
