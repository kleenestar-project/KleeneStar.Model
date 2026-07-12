using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the GUID identifier used by REST clients and the strongly typed
    /// <see cref="CalendarState"/> enumeration.
    /// </summary>
    public class CalendarStateConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw selection value (a semicolon-separated list of GUID ids as
        /// emitted by the REST selection control) into the matching <see cref="CalendarState"/>.
        /// Unknown ids fall back to <see cref="CalendarState.Active"/>; <c>null</c> input
        /// returns <c>null</c>.
        /// </summary>
        /// <param name="rawValue">The raw value submitted by the client.</param>
        /// <param name="targetType">The declared target type. Ignored — the converter
        /// always produces a <see cref="CalendarState"/> when the input is a string.</param>
        /// <returns>The strongly typed <see cref="CalendarState"/>, or <c>null</c>.</returns>
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

                if (id == CalendarState.Archived.Id())
                {
                    return CalendarState.Archived;
                }

                return CalendarState.Active;
            }

            return rawValue;
        }

        /// <summary>
        /// Converts a <see cref="CalendarState"/> to its raw GUID id so the REST
        /// selection control can pre-select the matching option. Any unrecognized value
        /// falls back to <see cref="CalendarState.Active"/>'s id.
        /// </summary>
        /// <param name="value">The strongly typed value.</param>
        /// <param name="sourceType">The declared source type. Ignored.</param>
        /// <returns>The GUID identifier of the state.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value switch
            {
                CalendarState.Archived => CalendarState.Archived.Id(),
                _ => CalendarState.Active.Id()
            };
        }
    }
}
