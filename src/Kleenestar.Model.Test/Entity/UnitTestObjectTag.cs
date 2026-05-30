using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the <see cref="ObjectTag"/> entity.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestObjectTag
    {
        /// <summary>
        /// Verifies that the default constructor assigns a fresh <see cref="ObjectTag.Id"/>
        /// value.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            var tag = new ObjectTag();

            Assert.NotEqual(Guid.Empty, tag.Id);
        }

        /// <summary>
        /// Verifies that two freshly constructed tags receive distinct ids — i.e. the ctor
        /// really generates a new Guid each time rather than reusing a cached one.
        /// </summary>
        [Fact]
        public void InitializeIdsAreDistinct()
        {
            var first = new ObjectTag();
            var second = new ObjectTag();

            Assert.NotEqual(first.Id, second.Id);
        }

        /// <summary>
        /// Verifies that scalar properties are stored verbatim.
        /// </summary>
        [Fact]
        public void SetScalarProperties()
        {
            var objectId = Guid.NewGuid();
            var created = new DateTime(2026, 5, 30, 10, 30, 0, DateTimeKind.Utc);

            var tag = new ObjectTag
            {
                ObjectId = objectId,
                Name = "Urgent",
                Color = "#dc3545",
                Created = created
            };

            Assert.Equal(objectId, tag.ObjectId);
            Assert.Equal("Urgent", tag.Name);
            Assert.Equal("#dc3545", tag.Color);
            Assert.Equal(created, tag.Created);
        }

        /// <summary>
        /// Verifies that the object navigation property is <c>null</c> until it is explicitly
        /// populated; EF does not hydrate it on a detached instance.
        /// </summary>
        [Fact]
        public void NavigationPropertyStartsNull()
        {
            var tag = new ObjectTag();

            Assert.Null(tag.Object);
        }
    }
}
