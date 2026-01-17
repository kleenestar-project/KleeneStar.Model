using System.Collections.Generic;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebCore.WebDomain;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebIndex;

namespace KleeneStar.Model.Entity
{
    /// <summary>
    /// Represents a workspace that provides a unique key, name, and description.
    /// </summary>
    /// <remarks>
    public interface IWorkspace : IIndexItem, IDomain
    {
        /// <summary>
        /// Returns the key of the workspace.
        /// </summary>
        [ValidateMinLength(2)]
        [RestTableColumnName("Key")]
        string Key { get; }

        /// <summary>
        /// Returns the name of the workspace.
        /// </summary>
        [RestTableColumnName("Name")]
        [RestDropdownText]
        string Name { get; }

        /// <summary>
        /// Returns the collection of category names associated with the item.
        /// </summary>
        [RestTableColumnName("Category")]
        [RestTableJoin(';')]
        [RestApiTableColumnTemplateTag(true)]
        IEnumerable<string> Categories { get; }

        /// <summary>
        /// Returns the description of the workspace.
        /// </summary>
        [RestTableColumnName("Description")]
        string Description { get; }

        /// <summary>
        /// Returns the current state of the workspace.
        /// </summary>
        [RestTableColumnName("State")]
        TypeWorkspaceState State { get; }

        /// <summary>
        /// Returns the icon associated with this workspace.
        /// </summary>
        [RestTableRowIcon]
        [RestDropdownIcon]
        IIcon Icon { get; }
    }
}