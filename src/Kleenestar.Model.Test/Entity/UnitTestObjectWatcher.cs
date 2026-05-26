using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the <see cref="ObjectWatcher"/> entity.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestObjectWatcher
    {
        /// <summary>
        /// Verifies that the default constructor assigns a fresh
        /// <see cref="ObjectWatcher.Id"/> value.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            var watcher = new ObjectWatcher();

            Assert.NotEqual(Guid.Empty, watcher.Id);
        }

        /// <summary>
        /// Verifies that two freshly constructed watchers receive distinct ids — i.e.
        /// the ctor really generates a new Guid each time rather than reusing a
        /// cached one.
        /// </summary>
        [Fact]
        public void InitializeIdsAreDistinct()
        {
            var first = new ObjectWatcher();
            var second = new ObjectWatcher();

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
            var created = new DateTime(2026, 5, 26, 10, 30, 0, DateTimeKind.Utc);

            var watcher = new ObjectWatcher
            {
                ObjectId = objectId,
                IdentityId = identityId,
                Created = created
            };

            Assert.Equal(objectId, watcher.ObjectId);
            Assert.Equal(identityId, watcher.IdentityId);
            Assert.Equal(created, watcher.Created);
        }

        /// <summary>
        /// Verifies that the navigation properties are <c>null</c> until they are
        /// explicitly populated; EF does not hydrate them on a detached instance.
        /// </summary>
        [Fact]
        public void NavigationPropertiesStartNull()
        {
            var watcher = new ObjectWatcher();

            Assert.Null(watcher.Object);
            Assert.Null(watcher.Identity);
        }
    }
}
