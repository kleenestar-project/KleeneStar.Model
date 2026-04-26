namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the kind of a form element within the form structure tree.
    /// </summary>
    public enum FormElementKind
    {
        /// <summary>
        /// Indicates that the element is a layout group containing further child elements.
        /// </summary>
        Group = 0,

        /// <summary>
        /// Indicates that the element is a reference to a field defined on the form's class.
        /// </summary>
        Field = 1
    }
}
