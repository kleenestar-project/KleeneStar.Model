using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the GUID identifier used by REST clients and the strongly typed
    /// <see cref="SlaPolicyState"/> enumeration.
    /// </summary>
    public class SlaPolicyStateConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw value to the target type.
        /// </summary>
        /// <param name="rawValue">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>The converted value.</returns>
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

                if (id == SlaPolicyState.Draft.Id())
                {
                    return SlaPolicyState.Draft;
                }

                if (id == SlaPolicyState.Inactive.Id())
                {
                    return SlaPolicyState.Inactive;
                }

                if (id == SlaPolicyState.Archived.Id())
                {
                    return SlaPolicyState.Archived;
                }

                return SlaPolicyState.Active;
            }

            return rawValue;
        }

        /// <summary>
        /// Converts the specified value to its raw representation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="sourceType">The declared source type.</param>
        /// <returns>The raw representation of the value.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value switch
            {
                SlaPolicyState.Draft => SlaPolicyState.Draft.Id(),
                SlaPolicyState.Inactive => SlaPolicyState.Inactive.Id(),
                SlaPolicyState.Archived => SlaPolicyState.Archived.Id(),
                _ => SlaPolicyState.Active.Id()
            };
        }
    }
}
