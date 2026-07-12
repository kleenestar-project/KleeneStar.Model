namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the layout strategy of a form group.
    /// </summary>
    public enum FormGroupLayout
    {
        /// <summary>
        /// Stacks children vertically.
        /// </summary>
        Vertical = 0,

        /// <summary>
        /// Arranges children horizontally.
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// Mixed flow layout where children may wrap.
        /// </summary>
        Mix = 2,

        /// <summary>
        /// Two-column layout, each column stacking children vertically.
        /// </summary>
        ColumnVertical = 3,

        /// <summary>
        /// Two-column layout, each column arranging children horizontally.
        /// </summary>
        ColumnHorizontal = 4,

        /// <summary>
        /// Two-column layout with mixed flow inside the columns.
        /// </summary>
        ColumnMix = 5
    }

    /// <summary>
    /// Provides conversion helpers between <see cref="FormGroupLayout"/> values and the
    /// string identifiers used by the editor frontend.
    /// </summary>
    public static class FormGroupLayoutExtensions
    {
        /// <summary>
        /// Returns the editor identifier for the specified layout.
        /// </summary>
        /// <param name="layout">The layout to convert.</param>
        /// <returns>The editor identifier (e.g. <c>"vertical"</c>).</returns>
        public static string ToEditorString(this FormGroupLayout layout)
        {
            return layout switch
            {
                FormGroupLayout.Vertical => "vertical",
                FormGroupLayout.Horizontal => "horizontal",
                FormGroupLayout.Mix => "mix",
                FormGroupLayout.ColumnVertical => "col-vertical",
                FormGroupLayout.ColumnHorizontal => "col-horizontal",
                FormGroupLayout.ColumnMix => "col-mix",
                _ => "vertical"
            };
        }

        /// <summary>
        /// Parses the layout from its editor identifier.
        /// </summary>
        /// <param name="value">
        /// The editor identifier (e.g. <c>"col-vertical"</c>).
        /// Unknown or null values resolve to <see cref="FormGroupLayout.Vertical"/>.
        /// </param>
        /// <returns>The parsed layout.</returns>
        public static FormGroupLayout FromEditorString(string value)
        {
            return value switch
            {
                "vertical" => FormGroupLayout.Vertical,
                "horizontal" => FormGroupLayout.Horizontal,
                "mix" => FormGroupLayout.Mix,
                "col-vertical" => FormGroupLayout.ColumnVertical,
                "col-horizontal" => FormGroupLayout.ColumnHorizontal,
                "col-mix" => FormGroupLayout.ColumnMix,
                _ => FormGroupLayout.Vertical
            };
        }
    }
}
