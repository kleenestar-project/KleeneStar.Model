using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Class class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestClass
    {
        /// <summary>
        /// Verifies that a new Class instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var classEntry = new Class();

            // validation
            Assert.NotEqual(Guid.Empty, classEntry.Id);
        }

        /// <summary>
        /// Sets the properties of a Class instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("Class A", "Description A", ClassState.Active)]
        [InlineData("Class B", null, ClassState.Archived)]
        public void SetProperties(string name, string description, ClassState state)
        {
            // arrange
            var classEntry = new Class();

            // act
            classEntry.Name = name;
            classEntry.Description = description;
            classEntry.State = state;

            // validation
            Assert.Equal(name, classEntry.Name);
            Assert.Equal(description, classEntry.Description);
            Assert.Equal(state, classEntry.State);
        }

        /// <summary>
        /// Sets the IsAbstract property and verifies the value.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetIsAbstract(bool isAbstract)
        {
            // arrange
            var classEntry = new Class();

            // act
            classEntry.IsAbstract = isAbstract;

            // validation
            Assert.Equal(isAbstract, classEntry.IsAbstract);
        }

        /// <summary>
        /// Sets the Sealed property and verifies the value.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetSealed(bool isSealed)
        {
            // arrange
            var classEntry = new Class();

            // act
            classEntry.Sealed = isSealed;

            // validation
            Assert.Equal(isSealed, classEntry.Sealed);
        }

        /// <summary>
        /// Sets the AccessModifier property and verifies the value.
        /// </summary>
        [Theory]
        [InlineData(AccessModifier.Private)]
        [InlineData(AccessModifier.Protected)]
        [InlineData(AccessModifier.Public)]
        [InlineData(AccessModifier.Internal)]
        public void SetAccessModifier(AccessModifier accessModifier)
        {
            // arrange
            var classEntry = new Class();

            // act
            classEntry.AccessModifier = accessModifier;

            // validation
            Assert.Equal(accessModifier, classEntry.AccessModifier);
        }

        /// <summary>
        /// Sets the Inherited navigation property and verifies the value.
        /// </summary>
        [Fact]
        public void SetInherited()
        {
            // arrange
            var baseClass = new Class { Name = "Base" };
            var derivedClass = new Class { Name = "Derived" };

            // act
            derivedClass.InheritedId = baseClass.Id;
            derivedClass.Inherited = baseClass;

            // validation
            Assert.Equal(baseClass.Id, derivedClass.InheritedId);
            Assert.Same(baseClass, derivedClass.Inherited);
        }

        /// <summary>
        /// Verifies that InheritedId defaults to null.
        /// </summary>
        [Fact]
        public void InheritedIdDefaultsToNull()
        {
            // act
            var classEntry = new Class();

            // validation
            Assert.Null(classEntry.InheritedId);
        }

        /// <summary>
        /// Sets the Parent navigation property and verifies the value.
        /// </summary>
        [Fact]
        public void SetParent()
        {
            // arrange
            var parentClass = new Class { Name = "Parent" };
            var childClass = new Class { Name = "Child" };

            // act
            childClass.ParentId = parentClass.Id;
            childClass.Parent = parentClass;

            // validation
            Assert.Equal(parentClass.Id, childClass.ParentId);
            Assert.Same(parentClass, childClass.Parent);
        }

        /// <summary>
        /// Verifies that ParentId defaults to null.
        /// </summary>
        [Fact]
        public void ParentIdDefaultsToNull()
        {
            // act
            var classEntry = new Class();

            // validation
            Assert.Null(classEntry.ParentId);
        }

        /// <summary>
        /// Sets the AllowedChildren collection and verifies the value.
        /// </summary>
        [Fact]
        public void SetAllowedChildren()
        {
            // arrange
            var parentClass = new Class { Name = "Parent" };
            var child1 = new Class { Name = "Child1" };
            var child2 = new Class { Name = "Child2" };

            // act
            parentClass.AllowedChildren.Add(child1);
            parentClass.AllowedChildren.Add(child2);

            // validation
            Assert.Equal(2, parentClass.AllowedChildren.Count);
            Assert.Contains(child1, parentClass.AllowedChildren);
            Assert.Contains(child2, parentClass.AllowedChildren);
        }

        /// <summary>
        /// Verifies that AllowedChildren defaults to an empty collection.
        /// </summary>
        [Fact]
        public void AllowedChildrenDefaultsToEmpty()
        {
            // act
            var classEntry = new Class();

            // validation
            Assert.Empty(classEntry.AllowedChildren);
        }
    }
}
