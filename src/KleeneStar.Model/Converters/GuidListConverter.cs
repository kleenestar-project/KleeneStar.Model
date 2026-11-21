using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the semicolon separated list a multi-value selection submits and a
    /// <see cref="List{Guid}"/> property.
    /// </summary>
    /// <remarks>
    /// The generic binder of <c>RestApiCrud</c> converts text to a <see cref="Guid"/> and text
    /// to a <c>List&lt;string&gt;</c>, but not to a list of guids: the assignment throws and the
    /// binder swallows it, so a multi-select over ids would silently keep its former value. A
    /// property holding ids therefore names this converter explicitly.
    /// </remarks>
    public class GuidListConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw value to a list of guids.
        /// </summary>
        /// <param name="rawValue">
        /// The value to convert: the text a selection submits, or an enumeration of entries.
        /// </param>
        /// <param name="targetType">
        /// The type to which the raw value should be converted. Cannot be null.
        /// </param>
        /// <returns>
        /// The ids the raw value names, in the order they were given, without duplicates.
        /// Entries that are not a guid are dropped rather than failing the whole write.
        /// </returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            // an absent entry clears the list rather than leaving a stale clearance in place
            if (rawValue is null)
            {
                return new List<Guid>();
            }

            var entries = rawValue switch
            {
                string text => text.Split(";", StringSplitOptions.RemoveEmptyEntries),
                IEnumerable<string> enumerable => [.. enumerable],
                IEnumerable<object> objects => objects.Select(x => x?.ToString()).ToArray(),
                _ => [rawValue.ToString()]
            };

            return entries
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => Guid.TryParse(x, out var id) ? (Guid?)id : null)
                .Where(x => x.HasValue && x.Value != Guid.Empty)
                .Select(x => x.Value)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Converts a list of guids to the semicolon separated text a selection reads back.
        /// </summary>
        /// <param name="value">The list of ids, or null.</param>
        /// <param name="sourceType">
        /// The type that describes how the value should be interpreted. Cannot be null.
        /// </param>
        /// <returns>The ids as text, empty when there are none.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            if (value is not IEnumerable<Guid> ids)
            {
                return string.Empty;
            }

            return string.Join(";", ids.Where(x => x != Guid.Empty));
        }
    }
}
