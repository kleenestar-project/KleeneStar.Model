using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Provides methods to convert between raw values and strongly typed field representations for REST operations.
    /// </summary>
    public class FieldConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw value to the specified target type.
        /// </summary>
        /// <param name="rawValue">
        /// The value to convert. Can be any object representing the raw data to be transformed.
        /// </param>
        /// <param name="targetType">
        /// The type to which the raw value should be converted. Cannot be null.
        /// </param>
        /// <returns>
        /// An object of the specified target type representing the converted value.
        /// </returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            if (rawValue is null)
            {
                return null;
            }

            if (rawValue is string s)
            {
                var ids = s.Split(";", StringSplitOptions.RemoveEmptyEntries)
                           .Select(x => x.Trim())
                           .Where(x => x.Length > 0)
                           .Select(x => Guid.TryParse(x, out var g) ? (Guid?)g : null)
                           .Where(g => g.HasValue)
                           .Select(g => g.Value);

                //return ids
                //    .Select(id => CoreHub.FieldManager?.GetField(id))
                //    .Where(entity => entity is not null)
                //    .ToList();
            }

            return rawValue;
        }

        /// <summary>
        /// Converts the specified value to its raw representation based on the provided source type.
        /// </summary>
        /// <param name="value">
        /// The value to convert to a raw representation. Can be null if the conversion supports
        /// null values.</param>
        /// <param name="sourceType">
        /// The type that describes how the value should be interpreted and converted. Cannot be null.
        /// </param>
        /// <returns>
        /// An object representing the raw form of the input value, as determined by the source type.
        /// </returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value;
        }
    }
}
