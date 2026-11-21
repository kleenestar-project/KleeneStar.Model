using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the raw selection ids the profile form submits and the strongly typed
    /// <see cref="WeekStart"/> value stored on the identity.
    /// </summary>
    public class WeekStartConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw value to the specified target type.
        /// </summary>
        /// <param name="rawValue">
        /// The value to convert. A selection control submits the id of the chosen item, possibly
        /// as a semicolon separated list.
        /// </param>
        /// <param name="targetType">
        /// The type to which the raw value should be converted. Cannot be null.
        /// </param>
        /// <returns>
        /// The <see cref="WeekStart"/> the raw value denotes; <see cref="WeekStart.Monday"/> when
        /// the value is not recognized.
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

                if (id == WeekStart.Sunday.Id())
                {
                    return WeekStart.Sunday;
                }

                if (id == WeekStart.Saturday.Id())
                {
                    return WeekStart.Saturday;
                }

                return WeekStart.Monday;
            }

            return rawValue;
        }

        /// <summary>
        /// Converts the specified value to its raw representation based on the provided source type.
        /// </summary>
        /// <param name="value">The value to convert to a raw representation.</param>
        /// <param name="sourceType">The type that describes how the value should be interpreted.</param>
        /// <returns>The id of the week start, which the selection control matches its items against.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value switch
            {
                WeekStart.Sunday => WeekStart.Sunday.Id(),
                WeekStart.Saturday => WeekStart.Saturday.Id(),
                _ => WeekStart.Monday.Id()
            };
        }
    }
}
