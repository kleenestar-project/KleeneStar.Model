using KleeneStar.Model.Entities;
using WebExpress.WebUI.WebIcon;
using KleeneStarObject = KleeneStar.Model.Entities.Object;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the Workspace class.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestWorkspace
    {
        /// <summary>
        /// Verifies that a new Workspace instance is assigned a non-empty unique identifier upon initialization.
        /// </summary>
        [Fact]
        public void InitializeId()
        {
            // act
            var workspace = new Workspace();

            // validation
            Assert.NotEqual(Guid.Empty, workspace.Id);
        }

        /// <summary>
        /// Sets the properties of a Workspace instance and verifies that the values are assigned correctly.
        /// </summary>
        [Theory]
        [InlineData("key-1", "Workspace A", WorkspaceState.Active, "Description A")]
        [InlineData("key-2", "Workspace B", WorkspaceState.Archived, null)]
        public void SetProperties(string key, string name, WorkspaceState state, string description)
        {
            // arrange
            var workspace = new Workspace();
            var icon = ImageIcon.FromString("/icon");

            // act
            workspace.Key = key;
            workspace.Name = name;
            workspace.State = state;
            workspace.Description = description;
            workspace.Icon = icon;

            // validation
            Assert.Equal(key, workspace.Key);
            Assert.Equal(name, workspace.Name);
            Assert.Equal(state, workspace.State);
            Assert.Equal(description, workspace.Description);
            Assert.Equal(icon, workspace.Icon);
        }

        /// <summary>
        /// Sets the categories for the workspace using the specified category names.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("A", "B", "C")]
        public void SetCategories(params string[] categories)
        {
            // arrange
            var workspace = new Workspace();
            var categoriesList = categories?.Select(name => new Category { Name = name })
                .ToList() ?? [];

            // act
            workspace.Categories = categoriesList;

            // validation
            Assert.Equal(categoriesList, workspace.Categories);
        }

        /// <summary>
        /// Verifies that a new Workspace instance has an empty Classes collection by default.
        /// </summary>
        [Fact]
        public void DefaultClassesCollectionIsEmpty()
        {
            // act
            var workspace = new Workspace();

            // validation
            Assert.NotNull(workspace.Classes);
            Assert.Empty(workspace.Classes);
        }

        /// <summary>
        /// Verifies that a new Workspace instance has an empty Objects collection by default.
        /// </summary>
        [Fact]
        public void DefaultObjectsCollectionIsEmpty()
        {
            // act
            var workspace = new Workspace();

            // validation
            Assert.NotNull(workspace.Objects);
            Assert.Empty(workspace.Objects);
        }

        /// <summary>
        /// Sets the classes for the workspace and verifies the collection is assigned correctly.
        /// </summary>
        [Fact]
        public void SetClasses()
        {
            // arrange
            var workspace = new Workspace();
            var classesList = new List<Class>
            {
                new Class { Name = "ClassA" },
                new Class { Name = "ClassB" },
                new Class { Name = "ClassC" }
            };

            // act
            workspace.Classes = classesList;

            // validation
            Assert.Equal(3, workspace.Classes.Count);
            Assert.Equal(classesList, workspace.Classes);
        }

        /// <summary>
        /// Sets the objects for the workspace and verifies the collection is assigned correctly.
        /// </summary>
        [Fact]
        public void SetObjects()
        {
            // arrange
            var workspace = new Workspace();
            var objectsList = new List<KleeneStarObject>
            {
                new KleeneStarObject { Key = "OBJ-1", Summary = "Object 1" },
                new KleeneStarObject { Key = "OBJ-2", Summary = "Object 2" }
            };

            // act
            workspace.Objects = objectsList;

            // validation
            Assert.Equal(2, workspace.Objects.Count);
            Assert.Equal(objectsList, workspace.Objects);
        }

        /// <summary>
        /// Verifies that the default value of Sealed is false.
        /// </summary>
        [Fact]
        public void DefaultSealedIsFalse()
        {
            // act
            var workspace = new Workspace();

            // validation
            Assert.False(workspace.Sealed);
        }

        /// <summary>
        /// Verifies that Sealed can be set explicitly.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetSealed(bool isSealed)
        {
            // arrange
            var workspace = new Workspace();

            // act
            workspace.Sealed = isSealed;

            // validation
            Assert.Equal(isSealed, workspace.Sealed);
        }

        /// <summary>
        /// Verifies that the default value of AccessModifier is Private.
        /// </summary>
        [Fact]
        public void DefaultAccessModifierIsPrivate()
        {
            // act
            var workspace = new Workspace();

            // validation
            Assert.Equal(WorkspaceAccessModifier.Private, workspace.AccessModifier);
        }

        /// <summary>
        /// Verifies that all AccessModifier enum values can be assigned.
        /// </summary>
        [Theory]
        [InlineData(WorkspaceAccessModifier.Private)]
        [InlineData(WorkspaceAccessModifier.Protected)]
        [InlineData(WorkspaceAccessModifier.Public)]
        [InlineData(WorkspaceAccessModifier.Internal)]
        public void SetAccessModifier(WorkspaceAccessModifier accessModifier)
        {
            // arrange
            var workspace = new Workspace();

            // act
            workspace.AccessModifier = accessModifier;

            // validation
            Assert.Equal(accessModifier, workspace.AccessModifier);
        }

        /// <summary>
        /// Verifies that InheritedId is null by default and Inherited is null.
        /// </summary>
        [Fact]
        public void DefaultInheritedIsNull()
        {
            // act
            var workspace = new Workspace();

            // validation
            Assert.Null(workspace.InheritedId);
            Assert.Null(workspace.Inherited);
        }

        /// <summary>
        /// Verifies that the Inherited navigation property can be set.
        /// </summary>
        [Fact]
        public void SetInherited()
        {
            // arrange
            var parent = new Workspace { Name = "Parent", Key = "PARENT" };
            var child = new Workspace { Name = "Child", Key = "CHILD" };

            // act
            child.InheritedId = parent.Id;
            child.Inherited = parent;

            // validation
            Assert.Equal(parent.Id, child.InheritedId);
            Assert.Equal(parent, child.Inherited);
        }

        /// <summary>
        /// Verifies that a new Workspace instance has an empty Tenants collection by default.
        /// </summary>
        [Fact]
        public void DefaultTenantsCollectionIsEmpty()
        {
            // act
            var workspace = new Workspace();

            // validation
            Assert.NotNull(workspace.Tenants);
            Assert.Empty(workspace.Tenants);
        }

        /// <summary>
        /// Sets the tenants for the workspace and verifies the collection is assigned correctly.
        /// </summary>
        [Fact]
        public void SetTenants()
        {
            // arrange
            var workspace = new Workspace();
            var tenantsList = new List<Tenant>
            {
                new Tenant { Name = "Tenant A" },
                new Tenant { Name = "Tenant B" }
            };

            // act
            workspace.Tenants = tenantsList;

            // validation
            Assert.Equal(2, workspace.Tenants.Count);
            Assert.Equal(tenantsList, workspace.Tenants);
        }

        /// <summary>
        /// Verifies that a new Workspace instance has an empty PermissionProfiles collection by default.
        /// </summary>
        [Fact]
        public void DefaultPermissionProfilesCollectionIsEmpty()
        {
            // act
            var workspace = new Workspace();

            // validation
            Assert.NotNull(workspace.PermissionProfiles);
            Assert.Empty(workspace.PermissionProfiles);
        }

        ///// <summary>
        ///// Sets the permission profiles for the workspace and verifies the collection is assigned correctly.
        ///// </summary>
        //[Fact]
        //public void SetPermissionProfiles()
        //{
        //    // arrange
        //    var workspace = new Workspace();
        //    var profilesList = new List<PermissionProfile>
        //    {
        //        new PermissionProfile { GroupId = Guid.NewGuid(), PolicyId = Guid.NewGuid(), WorkspaceId = Guid.NewGuid() },
        //        new PermissionProfile { GroupId = Guid.NewGuid(), PolicyId = Guid.NewGuid(), WorkspaceId = Guid.NewGuid() }
        //    };

        //    // act
        //    workspace.PermissionProfiles = profilesList;

        //    // validation
        //    Assert.Equal(2, workspace.PermissionProfiles.Count);
        //    Assert.Equal(profilesList, workspace.PermissionProfiles);
        //}
    }
}
