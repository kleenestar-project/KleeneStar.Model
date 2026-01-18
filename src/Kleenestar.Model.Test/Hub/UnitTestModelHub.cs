using Kleenestar.Model.Test.Fixture;
using KleeneStar.Model;
using KleeneStar.Model.Entity;
using System.Xml.Linq;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebIcon;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the ModelHub workspace.
    /// </summary>
    public class UnitTestModelHub
    {
        /// <summary>
        /// Verifies that LoadAllWorkspaces returns an empty list when the specified directory does not exist.
        /// </summary>
        [Fact]
        public void DirectoryDoesNotExist()
        {
            // act
            var result = ModelHub.LoadAllWorkspaces("non-existing-dir");

            // validation
            Assert.Empty(result);
        }

        /// <summary>
        /// Tests that all workspaces are loaded correctly from the specified directory.
        /// </summary>
        [Theory]
        [InlineData("ws1", "Workspace One", "Active", "Test workspace", new string[] { "A", "B" })]
        public void LoadAllWorkspaces(string key, string name, string state, string description, string[] categories)
        {
            // arrange
            using var dir = new TempDir();

            var xml = new XDocument(
                new XElement("Workspace",
                    new XElement("Id", Guid.NewGuid()),
                    new XElement("Key", key),
                    new XElement("Name", name),
                    new XElement("Icon", "icons/ws1.png"),
                    new XElement("State", state),
                    new XElement("Description", description),
                    categories.Select(c => new XElement("Category", c))
                )
            );

            var filePath = Path.Combine(dir.Path, $"{key}.xml");
            xml.Save(filePath);

            // act
            var result = ModelHub.LoadAllWorkspaces(dir.Path);

            // validation
            Assert.Single(result);
            var ws = result.First();

            Assert.Equal(key, ws.Key);
            Assert.Equal(name, ws.Name);
            Assert.Equal(Enum.Parse<TypeWorkspaceState>(state), ws.State);
            Assert.Equal(description, ws.Description);
            Assert.Equal(categories, ws.Categories);
        }

        /// <summary>
        /// Verifies that LoadAllWorkspaces returns an empty collection when encountering invalid 
        /// XML files in the specified directory.
        /// </summary>
        [Fact]
        public void LoadAllWorkspacesInvalidXml()
        {
            // arrange
            using var dir = new TempDir();
            File.WriteAllText(Path.Combine(dir.Path, "invalid.xml"), "NOT XML");

            // act
            var result = ModelHub.LoadAllWorkspaces(dir.Path);

            // validation
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that loading multiple workspace files returns the expected number of workspaces.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void LoadAllWorkspacesMultipleFiles(int count)
        {
            // arrange
            using var dir = new TempDir();

            for (int i = 1; i <= count; i++)
            {
                var xml = new XDocument(
                    new XElement("Workspace",
                        new XElement("Id", Guid.NewGuid()),
                        new XElement("Key", $"ws{i}"),
                        new XElement("Name", $"Workspace {i}")
                    )
                );

                xml.Save(Path.Combine(dir.Path, $"ws{i}.xml"));
            }

            // act
            var result = ModelHub.LoadAllWorkspaces(dir.Path);

            // validation
            Assert.Equal(count, result.Count);
        }

        /// <summary>
        /// Verifies that attempting to save a workspace to a non-existent directory throws a
        /// DirectoryNotFoundException.
        /// </summary>
        [Fact]
        public void SaveWorkspaceDoesNotExist()
        {
            // arrange
            var ws = new Workspace { Key = "ws1" };

            // validation
            Assert.Throws<DirectoryNotFoundException>
            (
                () =>
                // act
                ModelHub.SaveWorkspace(ws, "non-existing-dir")
            );
        }

        /// <summary>
        /// Verifies that the SaveWorkspace method throws an ArgumentNullException when the 
        /// Workspace object's Key property is null.
        /// </summary>
        [Fact]
        public void SaveWorkspaceKeyIsNull()
        {
            // arrange
            var ws = new Workspace { Key = null };
            using var dir = new TempDir();

            // validation
            Assert.Throws<ArgumentNullException>
            (
                () =>
                // act
                ModelHub.SaveWorkspace(ws, dir.Path)
            );
        }

        /// <summary>
        /// Verifies that a workspace can be saved to disk and that its data is correctly 
        /// persisted in XML format.
        /// </summary>
        [Fact]
        public void SaveWorkspace()
        {
            // arrange
            using var dir = new TempDir();

            var ws = new Workspace
            {
                Id = Guid.NewGuid(),
                Key = "ws1",
                Name = "Workspace One",
                Description = "Test",
                State = TypeWorkspaceState.Active,
                Categories = ["A", "B"],
                Icon = new ImageIcon(new UriEndpoint("http://example.com/icon.png"))
            };

            // act
            ModelHub.SaveWorkspace(ws, dir.Path);

            // validation
            var file = Path.Combine(dir.Path, "ws1.xml");
            Assert.True(File.Exists(file));

            var doc = XDocument.Load(file);
            var root = doc.Root;

            Assert.Equal(ws.Id.ToString(), root.Element("Id")?.Value);
            Assert.Equal("ws1", root.Element("Key")?.Value);
            Assert.Equal("Workspace One", root.Element("Name")?.Value);
            Assert.Equal("http://example.com/icon.png", root.Element("Icon")?.Value);
            Assert.Equal("Active", root.Element("State")?.Value);
            Assert.Equal("Test", root.Element("Description")?.Value);

            var categories = root.Elements("Category").Select(e => e.Value).ToList();
            Assert.Equal(new[] { "A", "B" }, categories);
        }
    }
}
