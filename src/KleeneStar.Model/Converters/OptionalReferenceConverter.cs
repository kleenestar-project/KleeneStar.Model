using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the id an optional reference selection submits and a nullable
    /// <see cref="Guid"/> property.
    /// </summary>
    /// <remarks>
    /// A selection over an optional reference offers the entry <i>None</i> under the empty guid.
    /// The property stores no reference for it, and for an empty submission, because the column
    /// is a foreign key and the empty guid names no row. Reading back, a property without a
    /// reference answers the empty guid, so the selection shows <i>None</i> rather than nothing.
    /// </remarks>
    public class OptionalReferenceConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw value to a nullable guid.
        /// </summary>
        /// <param name="rawValue">The submitted value: an id, or a guid.</param>
        /// <param name="targetType">The type of the target property.</param>
        /// <returns>
        /// The id the value names, null when it names none or the empty guid.
        /// </returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            if (rawValue is Guid guid)
            {
                return guid == Guid.Empty ? null : guid;
            }

            var entry = rawValue?.ToString()?
                .Split(";", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .FirstOrDefault(x => x.Length > 0);

            return Guid.TryParse(entry, out var id) && id != Guid.Empty ? id : null;
        }

        /// <summary>
        /// Converts a nullable guid to the id the selection reads back.
        /// </summary>
        /// <param name="value">The stored reference, or null.</param>
        /// <param name="sourceType">The type of the source property.</param>
        /// <returns>The id as text, the empty guid when there is no reference.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value is Guid id && id != Guid.Empty
                ? id.ToString()
                : Guid.Empty.ToString();
        }
    }
}
