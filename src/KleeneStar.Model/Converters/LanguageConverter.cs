using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the country code the account form submits and the ISO language code
    /// stored on <see cref="Identity.Language"/>.
    /// </summary>
    /// <remarks>
    /// The account page offers the interface languages through the country control - each
    /// language under the flag of its country (<see cref="UiLanguage.Country"/>) - while the
    /// identity stores the plain ISO language code, so everything reading the setting - the
    /// culture of a render, an outgoing e-mail - can use it without knowing about the control.
    /// The selection id a former selection control submitted is still read.
    /// </remarks>
    public class LanguageConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw value to the specified target type.
        /// </summary>
        /// <param name="rawValue">
        /// The value to convert: the country code of the chosen entry (possibly as a semicolon
        /// separated list), or the selection id of a language.
        /// </param>
        /// <param name="targetType">The type to which the raw value should be converted.</param>
        /// <returns>
        /// The ISO code of the chosen language, or <see langword="null"/> when nothing was
        /// chosen - which is how the identity says "use the culture of the application".
        /// </returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            if (rawValue is not string s)
            {
                return rawValue;
            }

            return s.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => UiLanguage.FromCountry(x)
                    ?? (Guid.TryParse(x, out var id) ? UiLanguage.FromId(id) : null))
                .FirstOrDefault(x => x is not null)?
                .Code;
        }

        /// <summary>
        /// Converts the stored language code to the country code the country control selects.
        /// </summary>
        /// <param name="value">The ISO language code, or <see langword="null"/>.</param>
        /// <param name="sourceType">The type of the stored value.</param>
        /// <returns>The country code, or an empty string when no language is chosen.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            return UiLanguage.FromCode(value as string)?.Country ?? string.Empty;
        }
    }
}
