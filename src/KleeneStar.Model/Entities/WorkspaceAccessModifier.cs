using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Controls the visibility and accessibility of a workspace.
    /// </summary>
    public enum WorkspaceAccessModifier
    {
        /// <summary>
        /// Indicates that the workspace is accessible only within its own scope.
        /// </summary>
        Private,

        /// <summary>
        /// Indicates that the workspace is accessible within its own scope and derived workspaces.
        /// </summary>
        Protected,

        /// <summary>
        /// Indicates that the workspace is accessible from any context.
        /// </summary>
        Public,

        /// <summary>
        /// Indicates that the workspace is accessible within the same module or assembly.
        /// </summary>
        Internal
    }

    /// <summary>
    /// Provides extension methods for working with the WorkspaceAccessModifier enumeration.
    /// </summary>
    public static class WorkspaceAccessModifierExtensions
    {
        /// <summary>
        /// Returns the unique identifier associated with the specified workspace access modifier.
        /// </summary>
        /// <param name="state">The workspace access modifier for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified workspace access modifier.</returns>
        public static Guid Id(this WorkspaceAccessModifier state)
        {
            return state switch
            {
                WorkspaceAccessModifier.Private => Guid.Parse("D4E8EFC6-9A56-4AAC-9FCF-E924F46CB7E5"),
                WorkspaceAccessModifier.Protected => Guid.Parse("04505D9D-F7EF-4CF9-9775-FA141C59BA95"),
                WorkspaceAccessModifier.Public => Guid.Parse("7530F626-A0BE-444F-99C4-F1548387E6D3"),
                WorkspaceAccessModifier.Internal => Guid.Parse("12419EF5-6426-43B2-A66B-CCEFD64BD23B"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the resource key label associated with the specified workspace access modifier.
        /// </summary>
        /// <param name="state">The workspace access modifier for which to retrieve the resource key label.</param>
        /// <returns>A string containing the resource key label, or null if not recognized.</returns>
        public static string Text(this WorkspaceAccessModifier state)
        {
            return state switch
            {
                WorkspaceAccessModifier.Public => "kleenestar.core:accessmodifier.public.label",
                WorkspaceAccessModifier.Protected => "kleenestar.core:accessmodifier.protected.label",
                WorkspaceAccessModifier.Private => "kleenestar.core:accessmodifier.private.label",
                WorkspaceAccessModifier.Internal => "kleenestar.core:accessmodifier.internal.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the CSS class name associated with the specified workspace access modifier.
        /// </summary>
        /// <param name="state">The workspace access modifier for which to retrieve the CSS class name.</param>
        /// <returns>A string containing the CSS class name.</returns>
        public static string Color(this WorkspaceAccessModifier state)
        {
            return state switch
            {
                _ => TypeColorSelection.Primary.ToClass()
            };
        }
    }
}
