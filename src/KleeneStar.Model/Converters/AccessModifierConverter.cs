using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Provides methods to convert access modifier values between their raw representations and strongly typed forms
    /// for use in RESTful APIs.
    /// </summary>
    public class AccessModifierConverter : IRestValueConverter
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

                if (id == AccessModifier.Public.Id())
                {
                    return AccessModifier.Public;
                }
                else if (id == AccessModifier.Protected.Id())
                {
                    return AccessModifier.Protected;
                }
                else if (id == AccessModifier.Internal.Id())
                {
                    return AccessModifier.Internal;
                }
                else if (id == AccessModifier.Private.Id())
                {
                    return AccessModifier.Private;
                }
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
            return value switch
            {
                AccessModifier.Protected => AccessModifier.Protected.Id(),
                AccessModifier.Private => AccessModifier.Private.Id(),
                AccessModifier.Internal => AccessModifier.Internal.Id(),
                _ => AccessModifier.Public.Id()
            };
        }
    }
}
