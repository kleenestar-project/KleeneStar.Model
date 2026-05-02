using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Provides methods to convert between raw data representations and strongly typed template state objects.
    /// </summary>
    public class TemplateStateConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw value to the specified target type.
        /// </summary>
        /// <param name="rawValue">The value to convert.</param>
        /// <param name="targetType">The type to which the raw value should be converted.</param>
        /// <returns>An object of the specified target type representing the converted value.</returns>
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

                if (id == TemplateState.Archived.Id())
                {
                    return TemplateState.Archived;
                }

                return TemplateState.Active;
            }

            return rawValue;
        }

        /// <summary>
        /// Converts the specified value to its raw representation based on the provided source type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="sourceType">The type that describes how the value should be interpreted.</param>
        /// <returns>An object representing the raw form of the input value.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value switch
            {
                TemplateState.Archived => TemplateState.Archived.Id(),
                _ => TemplateState.Active.Id()
            };
        }
    }
}