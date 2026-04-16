namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the type of a form, distinguishing between standard forms and additional forms.
    /// </summary>
    public enum FormType
    {
        /// <summary>
        /// Indicates that the form is the standard form of its class.
        /// Each class has exactly one standard form. It is created automatically
        /// when the class is created and cannot be deleted.
        /// The standard form provides three predefined views: new, edit, and view.
        /// </summary>
        Standard,

        /// <summary>
        /// Indicates that the form is an additional form.
        /// Additional forms are flexible UI masks (e.g., workflow screens, wizard steps,
        /// or other specialized layouts). They can be freely created and deleted by the user.
        /// </summary>
        Additional
    }
}
