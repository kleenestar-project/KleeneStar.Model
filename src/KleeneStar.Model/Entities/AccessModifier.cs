namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the access level of a field, controlling its visibility and usage scope.
    /// </summary>
    public enum AccessModifier
    {
        /// <summary>
        /// Indicates that the field is accessible only within its own class.
        /// </summary>
        Private,

        /// <summary>
        /// Indicates that the field is accessible within its own class and derived classes.
        /// </summary>
        Protected,

        /// <summary>
        /// Indicates that the field is accessible from any context.
        /// </summary>
        Public,

        /// <summary>
        /// Indicates that the field is accessible within the same module or assembly.
        /// </summary>
        Internal
    }
}
