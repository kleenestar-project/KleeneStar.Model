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
}
