using System;
using WebExpress.WebUI.WebControl;

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

    /// <summary>
    /// Provides extension methods for working with the FieldCardinality enumeration, enabling retrieval
    /// of associated identifiers, text labels, and color selections.
    /// </summary>
    public static class FieldCardinalityExtensions
    {
        /// <summary>
        /// Returns the unique identifier associated with the specified field cardinality.
        /// </summary>
        /// <param name="cardinality">The field cardinality for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified field cardinality.</returns>
        public static Guid Id(this FieldCardinality cardinality)
        {
            return cardinality switch
            {
                FieldCardinality.Single   => Guid.Parse("3C7A1E4D-B9F2-4568-A0C8-E3D5B1F7A294"),
                FieldCardinality.Multiple => Guid.Parse("6F9B2D4A-C7E1-4893-B5A0-D2F8C3E61B47"),
                _                         => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the resource key label associated with the specified field cardinality.
        /// </summary>
        /// <param name="cardinality">The field cardinality for which to retrieve the resource key label.</param>
        /// <returns>A string containing the resource key label, or null if not recognized.</returns>
        public static string Text(this FieldCardinality cardinality)
        {
            return cardinality switch
            {
                FieldCardinality.Single   => "kleenestar.core:fieldcardinality.single.label",
                FieldCardinality.Multiple => "kleenestar.core:fieldcardinality.multiple.label",
                _                         => null
            };
        }

        /// <summary>
        /// Returns the CSS class name associated with the specified field cardinality for styling purposes.
        /// </summary>
        /// <param name="cardinality">The field cardinality for which to retrieve the CSS class name.</param>
        /// <returns>A string containing the CSS class name.</returns>
        public static string Color(this FieldCardinality cardinality)
        {
            return cardinality switch
            {
                _ => TypeColorSelection.Primary.ToClass()
            };
        }
    }
}
