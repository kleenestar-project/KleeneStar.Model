using KleeneStar.Model.Converters;
using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Converters
{
    /// <summary>
    /// Tests the converters the workspace dialogs bind through: the access modifier, the
    /// inherited workspace and the tenant list.
    /// </summary>
    public class UnitTestWorkspaceFormConverters
    {
        /// <summary>
        /// Verifies that every id the access modifier selection offers binds to its modifier
        /// and reads back as the same id.
        /// </summary>
        [Theory]
        [InlineData(WorkspaceAccessModifier.Public)]
        [InlineData(WorkspaceAccessModifier.Protected)]
        [InlineData(WorkspaceAccessModifier.Private)]
        [InlineData(WorkspaceAccessModifier.Internal)]
        public void AccessModifier_RoundTripsTheSelectionId(WorkspaceAccessModifier modifier)
        {
            var converter = new AccessModifierConverter();

            var bound = converter.FromRaw(modifier.Id().ToString(), typeof(WorkspaceAccessModifier));

            Assert.Equal(modifier, bound);
            Assert.Equal(modifier.Id().ToString(), converter.ToRaw(modifier, typeof(WorkspaceAccessModifier)));
        }

        /// <summary>
        /// Verifies that an empty or unknown submission binds the access modifier public.
        /// </summary>
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("not-a-guid")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void AccessModifier_EmptySelectionIsPublic(string raw)
        {
            var bound = new AccessModifierConverter().FromRaw(raw, typeof(WorkspaceAccessModifier));

            Assert.Equal(WorkspaceAccessModifier.Public, bound);
        }

        /// <summary>
        /// Verifies that the converter binds a value the property can hold, so the binder does not
        /// drop the assignment.
        /// </summary>
        [Fact]
        public void AccessModifier_BindsTheWorkspaceProperty()
        {
            var workspace = new Workspace();
            var property = typeof(Workspace).GetProperty(nameof(Workspace.AccessModifier));

            property.SetValue(workspace, new AccessModifierConverter().FromRaw(WorkspaceAccessModifier.Internal.Id().ToString(), property.PropertyType));

            Assert.Equal(WorkspaceAccessModifier.Internal, workspace.AccessModifier);
        }

        /// <summary>
        /// Verifies that the entry None, an empty submission and an invalid one store no
        /// inherited workspace.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("not-a-guid")]
        public void OptionalReference_NoneStoresNull(string raw)
        {
            Assert.Null(new OptionalReferenceConverter().FromRaw(raw, typeof(Guid?)));
        }

        /// <summary>
        /// Verifies that a named workspace is stored, and that no reference reads back as None.
        /// </summary>
        [Fact]
        public void OptionalReference_RoundTrips()
        {
            var converter = new OptionalReferenceConverter();
            var id = Guid.NewGuid();

            Assert.Equal(id, converter.FromRaw(id.ToString(), typeof(Guid?)));
            Assert.Equal(id.ToString(), converter.ToRaw(id, typeof(Guid?)));
            Assert.Equal(Guid.Empty.ToString(), converter.ToRaw(null, typeof(Guid?)));
        }

        /// <summary>
        /// Verifies that the tenant id list is a projection of the tenants and that the binder
        /// cannot write it.
        /// </summary>
        [Fact]
        public void TenantIds_ProjectsTheTenants()
        {
            var first = new Tenant();
            var second = new Tenant();
            var workspace = new Workspace() { Tenants = [first, second] };

            Assert.Equal([first.Id, second.Id], workspace.TenantIds);
            Assert.False(typeof(Workspace).GetProperty(nameof(Workspace.TenantIds)).CanWrite);
        }
    }
}
