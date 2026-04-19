using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the type of a field, determining what kind of data it can hold.
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// Indicates a plain text field.
        /// </summary>
        Text,

        /// <summary>
        /// Indicates a numeric field.
        /// </summary>
        Number,

        /// <summary>
        /// Indicates a date or date-time field.
        /// </summary>
        Date,

        /// <summary>
        /// Indicates a boolean (true/false) field.
        /// </summary>
        Boolean,

        /// <summary>
        /// Indicates a field with a predefined list of selectable values.
        /// </summary>
        Selection,

        /// <summary>
        /// Indicates a field that references another entity.
        /// </summary>
        Reference,

        /// <summary>
        /// Indicates a field that is linked to a workflow.
        /// </summary>
        Workflow,

        /// <summary>
        /// Indicates a field for file attachments.
        /// </summary>
        Attachment,

        /// <summary>
        /// Indicates a field that references a user identity.
        /// </summary>
        User,

        /// <summary>
        /// Indicates a tag field for free-form categorization.
        /// </summary>
        Tag
    }

    /// <summary>
    /// Provides extension methods for working with the FieldType enumeration, enabling retrieval
    /// of associated identifiers, text labels, and color selections.
    /// </summary>
    public static class FieldTypeExtensions
    {
        /// <summary>
        /// Returns the unique identifier associated with the specified field type.
        /// </summary>
        /// <param name="type">The field type for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified field type.</returns>
        public static Guid Id(this FieldType type)
        {
            return type switch
            {
                FieldType.Text       => Guid.Parse("F1C5A3B7-92D4-4E68-B0A1-C8D7E3F24591"),
                FieldType.Number     => Guid.Parse("7D9B2E4A-C1F3-4A56-8E7B-29D4F6A03C18"),
                FieldType.Date       => Guid.Parse("3A7C5E91-B2D4-4F68-A0C3-17E5B9D24F6A"),
                FieldType.Boolean    => Guid.Parse("9C2E4F6A-B8D0-4318-E5A7-C1F94B2D3E08"),
                FieldType.Selection  => Guid.Parse("5B8D1F3A-C4E6-4892-B7D9-A0E2C5F3718B"),
                FieldType.Reference  => Guid.Parse("6E4A8C2F-D71B-4390-C5F8-B1A3D2E9C647"),
                FieldType.Workflow   => Guid.Parse("4D7F1B3A-E8C2-4765-A9D0-B3E1C5F29A47"),
                FieldType.Attachment => Guid.Parse("8A3C6E1B-F4D9-4207-B5A8-C2E3D7F16B94"),
                FieldType.User       => Guid.Parse("2F5A8C4E-B1D7-4390-E6F9-A4C2B8D51E03"),
                FieldType.Tag        => Guid.Parse("7B1E4A9C-D2F6-4850-B3A7-E9C1D4F28B06"),
                _                    => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the resource key label associated with the specified field type.
        /// </summary>
        /// <param name="type">The field type for which to retrieve the resource key label.</param>
        /// <returns>A string containing the resource key label, or null if not recognized.</returns>
        public static string Text(this FieldType type)
        {
            return type switch
            {
                FieldType.Text       => "kleenestar.core:fieldtype.text.label",
                FieldType.Number     => "kleenestar.core:fieldtype.number.label",
                FieldType.Date       => "kleenestar.core:fieldtype.date.label",
                FieldType.Boolean    => "kleenestar.core:fieldtype.boolean.label",
                FieldType.Selection  => "kleenestar.core:fieldtype.selection.label",
                FieldType.Reference  => "kleenestar.core:fieldtype.reference.label",
                FieldType.Workflow   => "kleenestar.core:fieldtype.workflow.label",
                FieldType.Attachment => "kleenestar.core:fieldtype.attachment.label",
                FieldType.User       => "kleenestar.core:fieldtype.user.label",
                FieldType.Tag        => "kleenestar.core:fieldtype.tag.label",
                _                    => null
            };
        }

        /// <summary>
        /// Returns the CSS class name associated with the specified field type for styling purposes.
        /// </summary>
        /// <param name="type">The field type for which to retrieve the CSS class name.</param>
        /// <returns>A string containing the CSS class name.</returns>
        public static string Color(this FieldType type)
        {
            return type switch
            {
                _ => TypeColorSelection.Primary.ToClass()
            };
        }
    }
}
