using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the type of a field, determining what kind of data it can hold and which
    /// input control it is edited with.
    /// </summary>
    /// <remarks>
    /// Every input control the framework offers is reachable through one of these types, so
    /// a class can be modelled without falling back to a plain text field. The values are
    /// persisted as their ordinal, so new entries are appended rather than inserted.
    /// </remarks>
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
        Tag,

        /// <summary>
        /// Indicates a field that holds a predefined priority level.
        /// </summary>
        Priority,

        /// <summary>
        /// Indicates a text field spanning several lines.
        /// </summary>
        Multiline,

        /// <summary>
        /// Indicates a text field written as prose, edited with the rich text editor.
        /// </summary>
        RichText,

        /// <summary>
        /// Indicates a secret that is masked while it is typed.
        /// </summary>
        Password,

        /// <summary>
        /// Indicates a color, picked from a color picker.
        /// </summary>
        Color,

        /// <summary>
        /// Indicates a rating expressed in stars.
        /// </summary>
        Rating,

        /// <summary>
        /// Indicates a number picked from a continuous scale.
        /// </summary>
        Slider,

        /// <summary>
        /// Indicates a span between two numbers.
        /// </summary>
        Range,

        /// <summary>
        /// Indicates an effort picked from a configurable scale, such as story points.
        /// </summary>
        Estimate,

        /// <summary>
        /// Indicates a red / yellow / green status.
        /// </summary>
        TrafficLight,

        /// <summary>
        /// Indicates a span between two dates.
        /// </summary>
        DateRange,

        /// <summary>
        /// Indicates a date picked from an inline calendar.
        /// </summary>
        Calendar,

        /// <summary>
        /// Indicates a span of dates picked from an inline calendar.
        /// </summary>
        CalendarRange,

        /// <summary>
        /// Indicates an image chosen as an avatar.
        /// </summary>
        Avatar,

        /// <summary>
        /// Indicates one of a few options, offered side by side as a segmented choice.
        /// </summary>
        Choice,

        /// <summary>
        /// Indicates a choice made from a grid of tile cards.
        /// </summary>
        Tile,

        /// <summary>
        /// Indicates a selection assembled by moving entries between two lists.
        /// </summary>
        Move,

        /// <summary>
        /// Indicates a selection made through dependent levels, each narrowing the next.
        /// </summary>
        Cascading,

        /// <summary>
        /// Indicates one of a few options, offered as radio buttons.
        /// </summary>
        Radio,

        /// <summary>
        /// Indicates a selection of several values from a predefined list.
        /// </summary>
        MultiSelection
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
        /// <remarks>
        /// The id is the stable handle a field type is addressed by outside the database —
        /// the selection controls submit it — so it must never change for an existing type.
        /// </remarks>
        /// <param name="type">The field type for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified field type.</returns>
        public static Guid Id(this FieldType type)
        {
            return type switch
            {
                FieldType.Text          => Guid.Parse("F1C5A3B7-92D4-4E68-B0A1-C8D7E3F24591"),
                FieldType.Number        => Guid.Parse("7D9B2E4A-C1F3-4A56-8E7B-29D4F6A03C18"),
                FieldType.Date          => Guid.Parse("3A7C5E91-B2D4-4F68-A0C3-17E5B9D24F6A"),
                FieldType.Boolean       => Guid.Parse("9C2E4F6A-B8D0-4318-E5A7-C1F94B2D3E08"),
                FieldType.Selection     => Guid.Parse("5B8D1F3A-C4E6-4892-B7D9-A0E2C5F3718B"),
                FieldType.Reference     => Guid.Parse("6E4A8C2F-D71B-4390-C5F8-B1A3D2E9C647"),
                FieldType.Workflow      => Guid.Parse("4D7F1B3A-E8C2-4765-A9D0-B3E1C5F29A47"),
                FieldType.Attachment    => Guid.Parse("8A3C6E1B-F4D9-4207-B5A8-C2E3D7F16B94"),
                FieldType.User          => Guid.Parse("2F5A8C4E-B1D7-4390-E6F9-A4C2B8D51E03"),
                FieldType.Tag           => Guid.Parse("7B1E4A9C-D2F6-4850-B3A7-E9C1D4F28B06"),
                FieldType.Priority      => Guid.Parse("2BC90EA1-B284-40D9-8688-A176CEDCA719"),
                FieldType.Multiline     => Guid.Parse("1C6F2A85-3B94-4D71-9E08-5A7C1D3F6B22"),
                FieldType.RichText      => Guid.Parse("2D7A3B96-4C05-4E82-8F19-6B8D2E4A7C33"),
                FieldType.Password      => Guid.Parse("3E8B4CA7-5D16-4F93-9028-7C9E3F5B8D44"),
                FieldType.Color         => Guid.Parse("4F9C5DB8-6E27-40A4-8139-8DAF40619E55"),
                FieldType.Rating        => Guid.Parse("50AD6EC9-7F38-41B5-924A-9EB051720F66"),
                FieldType.Slider        => Guid.Parse("61BE7FDA-8049-42C6-A35B-AFC162831077"),
                FieldType.Range         => Guid.Parse("72CF80EB-915A-43D7-B46C-B0D273942188"),
                FieldType.Estimate      => Guid.Parse("83D091FC-A26B-44E8-C57D-C1E384A53299"),
                FieldType.TrafficLight  => Guid.Parse("94E1A20D-B37C-45F9-D68E-D2F495B643AA"),
                FieldType.DateRange     => Guid.Parse("A5F2B31E-C48D-460A-E79F-E305A6C754BB"),
                FieldType.Calendar      => Guid.Parse("B603C42F-D59E-471B-F8A0-F416B7D865CC"),
                FieldType.CalendarRange => Guid.Parse("C714D530-E6AF-482C-09B1-0527C8E976DD"),
                FieldType.Avatar        => Guid.Parse("D825E641-F7B0-493D-1AC2-1638D9FA87EE"),
                FieldType.Choice        => Guid.Parse("E936F752-08C1-4A4E-2BD3-2749EA0B98FF"),
                FieldType.Tile          => Guid.Parse("FA470863-19D2-4B5F-3CE4-385AFB1CA900"),
                FieldType.Move          => Guid.Parse("0B581974-2AE3-4C60-4DF5-496B0C2DBA11"),
                FieldType.Cascading     => Guid.Parse("1C692A85-3BF4-4D71-5E06-5A7C1D3ECB22"),
                FieldType.Radio         => Guid.Parse("2D7A3B96-4C05-4E82-6F17-6B8D2E4FDC33"),
                FieldType.MultiSelection => Guid.Parse("3E8B4CA7-5D16-4F93-7028-7C9E3F50ED44"),
                _                       => Guid.Empty
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
                FieldType.Text          => "kleenestar.core:fieldtype.text.label",
                FieldType.Number        => "kleenestar.core:fieldtype.number.label",
                FieldType.Date          => "kleenestar.core:fieldtype.date.label",
                FieldType.Boolean       => "kleenestar.core:fieldtype.boolean.label",
                FieldType.Selection     => "kleenestar.core:fieldtype.selection.label",
                FieldType.Reference     => "kleenestar.core:fieldtype.reference.label",
                FieldType.Workflow      => "kleenestar.core:fieldtype.workflow.label",
                FieldType.Attachment    => "kleenestar.core:fieldtype.attachment.label",
                FieldType.User          => "kleenestar.core:fieldtype.user.label",
                FieldType.Tag           => "kleenestar.core:fieldtype.tag.label",
                FieldType.Priority      => "kleenestar.core:fieldtype.priority.label",
                FieldType.Multiline     => "kleenestar.core:fieldtype.multiline.label",
                FieldType.RichText      => "kleenestar.core:fieldtype.richtext.label",
                FieldType.Password      => "kleenestar.core:fieldtype.password.label",
                FieldType.Color         => "kleenestar.core:fieldtype.color.label",
                FieldType.Rating        => "kleenestar.core:fieldtype.rating.label",
                FieldType.Slider        => "kleenestar.core:fieldtype.slider.label",
                FieldType.Range         => "kleenestar.core:fieldtype.range.label",
                FieldType.Estimate      => "kleenestar.core:fieldtype.estimate.label",
                FieldType.TrafficLight  => "kleenestar.core:fieldtype.trafficlight.label",
                FieldType.DateRange     => "kleenestar.core:fieldtype.daterange.label",
                FieldType.Calendar      => "kleenestar.core:fieldtype.calendar.label",
                FieldType.CalendarRange => "kleenestar.core:fieldtype.calendarrange.label",
                FieldType.Avatar        => "kleenestar.core:fieldtype.avatar.label",
                FieldType.Choice        => "kleenestar.core:fieldtype.choice.label",
                FieldType.Tile          => "kleenestar.core:fieldtype.tile.label",
                FieldType.Move          => "kleenestar.core:fieldtype.move.label",
                FieldType.Cascading     => "kleenestar.core:fieldtype.cascading.label",
                FieldType.Radio         => "kleenestar.core:fieldtype.radio.label",
                FieldType.MultiSelection => "kleenestar.core:fieldtype.multiselection.label",
                _                       => null
            };
        }

        /// <summary>
        /// Returns the discriminator the form editor represents the field type by.
        /// </summary>
        /// <remarks>
        /// The editor works with a coarser vocabulary than the field types: several types
        /// share a shape and are therefore previewed alike — a calendar reads as a date, a
        /// cascading selection as a list. The mapping lives here rather than in the endpoint
        /// that serves the editor, so a new field type is described in one place.
        /// </remarks>
        /// <param name="type">The field type.</param>
        /// <returns>The editor discriminator.</returns>
        public static string Editor(this FieldType type)
        {
            return type switch
            {
                FieldType.Text          => "string",
                FieldType.Number        => "number",
                FieldType.Date          => "timestamp",
                FieldType.Boolean       => "enum",
                FieldType.Selection     => "enum",
                FieldType.Reference     => "ref",
                FieldType.Workflow      => "enum",
                FieldType.Attachment    => "file",
                FieldType.User          => "ref",
                FieldType.Tag           => "tags",
                FieldType.Priority      => "choice",
                FieldType.Multiline     => "text",
                FieldType.RichText      => "richtext",
                FieldType.Password      => "password",
                FieldType.Color         => "color",
                FieldType.Rating        => "rating",
                FieldType.Slider        => "range",
                FieldType.Range         => "range",
                FieldType.Estimate      => "estimate",
                FieldType.TrafficLight  => "choice",
                FieldType.DateRange     => "daterange",
                FieldType.Calendar      => "timestamp",
                FieldType.CalendarRange => "daterange",
                FieldType.Avatar        => "avatar",
                FieldType.Choice        => "choice",
                FieldType.Tile          => "tile",
                FieldType.Move          => "move",
                FieldType.Cascading     => "enum",
                FieldType.Radio         => "choice",
                FieldType.MultiSelection => "enum",
                _                       => "string"
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
