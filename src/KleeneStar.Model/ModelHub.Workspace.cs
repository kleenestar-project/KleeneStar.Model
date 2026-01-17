using KleeneStar.Model.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the KleeneStar.
    /// </summary>
    public static partial class ModelHub
    {
        /// <summary>
        /// Loads all workspace definitions from XML files in the specified directory.
        /// </summary>
        /// <param name="directoryPath">
        /// The path to the directory containing the XML files to load. The directory must exist.
        /// </param>
        /// <returns>
        /// A list of <see cref="IWorkspace"/> instances representing the loaded workspaces. The list will be empty if
        /// no valid workspaces are found.
        /// </returns>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified <paramref name="directoryPath"/> does not exist.
        /// </exception>
        public static List<IWorkspace> LoadAllWorkspaces(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return [];
            }

            return Directory.GetFiles(directoryPath, "*.xml")
                .Select(static file =>
                {
                    try
                    {
                        var doc = XDocument.Load(file);
                        var root = doc.Root;
                        var iconPath = root?.Element("Icon")?.Value;
                        var iconUri = !string.IsNullOrWhiteSpace(iconPath)
                            ? ApplicationContet.Route.ToUri().Concat(iconPath)
                            : null;

                        return new Workspace()
                        {
                            Id = Guid.Parse(root?.Element("Id")?.Value ?? ""),
                            Key = root?.Element("Key")?.Value ?? "",
                            Name = root?.Element("Name")?.Value ?? "",
                            Icon = iconUri is not null ? new ImageIcon(iconUri) : null,
                            State = Enum.TryParse<TypeWorkspaceState>(root?.Element("State")?.Value, out var state)
                                ? state
                                : TypeWorkspaceState.Active,
                            Description = root?.Element("Description")?.Value ?? "",
                            Categories = root?.Elements("Category").Select(e => e.Value).ToList() ?? []
                        };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading {file}: {ex.Message}");
                        return null as IWorkspace;
                    }
                })
                .Where(ws => ws is not null)
                .ToList()!;
        }

        /// <summary>
        /// Saves a workspace definition as an XML file in the specified directory.
        /// </summary>
        /// <param name="workspace">The workspace instance to save.</param>
        /// <param name="directoryPath">
        /// The path to the directory where the XML file will be saved. The directory must exist.
        /// </param>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified <paramref name="directoryPath"/> does not exist.
        /// </exception>
        public static void SaveWorkspace(IWorkspace workspace, string directoryPath)
        {
            ArgumentNullException.ThrowIfNull(workspace?.Key);

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
            }

            var doc = new XDocument(
                new XElement("Workspace",
                    new XElement("Id", workspace.Id),
                    new XElement("Key", workspace.Key),
                    new XElement("Name", workspace.Name),
                    new XElement("Icon", (workspace.Icon as ImageIcon)?.Uri?.ToString() ?? ""),
                    new XElement("State", workspace.State.ToString()),
                    new XElement("Description", workspace.Description ?? ""),
                    workspace.Categories?.Select(c => new XElement("Category", c))
                )
            );

            var filePath = Path.Combine(directoryPath, $"{workspace.Key}.xml");
            doc.Save(filePath);
        }
    }
}
