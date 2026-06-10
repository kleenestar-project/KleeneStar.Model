using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the <see cref="ObjectShare"/> entity.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestObjectShare
    {
        /// <summary>
        /// Verifies that the default constructor assigns a fresh
        /// <see cref="ObjectShare.Id"/> value.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            var share = new ObjectShare();

            Assert.NotEqual(Guid.Empty, share.Id);
        }

        /// <summary>
        /// Verifies that two freshly constructed shares receive distinct ids — i.e.
        /// the ctor really generates a new Guid each time rather than reusing a
        /// cached one.
        /// </summary>
        [Fact]
        public void InitializeIdsAreDistinct()
        {
            var first = new ObjectShare();
            var second = new ObjectShare();

            Assert.NotEqual(first.Id, second.Id);
        }

        /// <summary>
        /// Verifies that scalar foreign-key properties are stored verbatim.
        /// </summary>
        [Fact]
        public void SetScalarProperties()
        {
            var objectId = Guid.NewGuid();
            var identityId = Guid.NewGuid();
            var created = new DateTime(2026, 6, 10, 9, 15, 0, DateTimeKind.Utc);

            var share = new ObjectShare
            {
                ObjectId = objectId,
                IdentityId = identityId,
                Created = created
            };

            Assert.Equal(objectId, share.ObjectId);
            Assert.Equal(identityId, share.IdentityId);
            Assert.Equal(created, share.Created);
        }

        /// <summary>
        /// Verifies that the navigation properties are <c>null</c> until they are
        /// explicitly populated; EF does not hydrate them on a detached instance.
        /// </summary>
        [Fact]
        public void NavigationPropertiesStartNull()
        {
            var share = new ObjectShare();

            Assert.Null(share.Object);
            Assert.Null(share.Identity);
        }
    }
}
