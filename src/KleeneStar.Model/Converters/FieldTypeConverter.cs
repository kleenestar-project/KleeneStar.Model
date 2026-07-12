using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Provides methods to convert between raw data representations and strongly typed FieldType
    /// values for use in RESTful operations.
    /// </summary>
    public class FieldTypeConverter : IRestValueConverter
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
                var id = s.Split(";", StringSplitOptions.RemoveEmptyEntries)
                           .Select(x => x.Trim())
                           .Where(x => x.Length > 0)
                           .Select(x => Guid.TryParse(x, out var g) ? (Guid?)g : null)
                           .Where(g => g.HasValue)
                           .Select(g => g.Value)
                           .FirstOrDefault();

                foreach (FieldType value in Enum.GetValues(typeof(FieldType)))
                {
                    if (value.Id() == id)
                    {
                        return value;
                    }
                }

                return FieldType.Text;
            }

            return rawValue;
        }

        /// <summary>
        /// Converts the specified value to its raw representation based on the provided source type.
        /// </summary>
        /// <param name="value">
        /// The value to convert to a raw representation.
        /// </param>
        /// <param name="sourceType">
        /// The type that describes how the value should be interpreted and converted. Cannot be null.
        /// </param>
        /// <returns>
        /// An object representing the raw form of the input value.
        /// </returns>
        public object ToRaw(object value, Type sourceType)
        {
            if (value is FieldType fieldType)
            {
                return fieldType.Id();
            }

            return FieldType.Text.Id();
        }
    }
}
