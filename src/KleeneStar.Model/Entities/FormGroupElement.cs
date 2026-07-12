namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a layout group within a form, containing nested child elements arranged
    /// according to its layout strategy.
    /// </summary>
    public class FormGroupElement : FormElement
    {
        /// <summary>
        /// Gets or sets the optional label displayed for the group.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the layout strategy used to arrange the children of this group.
        /// </summary>
        public FormGroupLayout Layout { get; set; } = FormGroupLayout.Vertical;
    }
}
