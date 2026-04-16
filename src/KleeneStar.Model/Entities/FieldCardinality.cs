namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the cardinality of a field, indicating whether it holds a single value or multiple values.
    /// </summary>
    public enum FieldCardinality
    {
        /// <summary>
        /// Indicates that the field holds exactly one value.
        /// </summary>
        Single,

        /// <summary>
        /// Indicates that the field can hold multiple values.
        /// </summary>
        Multiple
    }
}
