namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the type of a form, distinguishing between standard forms and additional forms.
    /// </summary>
    public enum FormType
    {
        /// <summary>
        /// Indicates the default form type. This value is used when no specific
        /// view type (create, edit, or view) is assigned.
        /// </summary>
        Default,

        /// <summary>
        /// Indicates that the form represents the "create" view of the standard form.
        /// </summary>
        Create,

        /// <summary>
        /// Indicates that the form represents the "edit" view of the standard form.
        /// </summary>
        Edit,

        /// <summary>
        /// Indicates that the form represents the "view" (read‑only) view of the standard form.
        /// </summary>
        View
    }
}
