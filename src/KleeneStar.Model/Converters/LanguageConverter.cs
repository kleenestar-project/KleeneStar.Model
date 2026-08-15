using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the selection ids the account form submits and the ISO language code
    /// stored on <see cref="Identity.Language"/>.
    /// </summary>
    /// <remarks>
    /// A selection control identifies its entries by id, while the identity stores the plain
    /// ISO code so everything reading the setting — the culture of a render, an outgoing
    /// e-mail — can use it without knowing about this control.
    /// </remarks>
    public class LanguageConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw value to the specified target type.
        /// </summary>
        /// <param name="rawValue">
        /// The value to convert. A selection control submits the id of the chosen entry,
        /// possibly as a semicolon separated list.
        /// </param>
        /// <param name="targetType">The type to which the raw value should be converted.</param>
        /// <returns>
        /// The ISO code of the chosen language, or <see langword="null"/> when nothing was
        /// chosen — which is how the identity says "use the culture of the application".
        /// </returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            if (rawValue is not string s)
            {
                return rawValue;
            }

            var id = s.Split(";", StringSplitOptions.RemoveEmptyEntries)
                       .Select(x => x.Trim())
                       .Where(x => x.Length > 0)
                       .Select(x => Guid.TryParse(x, out var g) ? (Guid?)g : null)
                       .Where(g => g.HasValue)
                       .Select(g => g.Value)
                       .FirstOrDefault();

            return UiLanguage.FromId(id)?.Code;
        }

        /// <summary>
        /// Converts the specified value to its raw representation based on the provided source type.
        /// </summary>
        /// <param name="value">The ISO code stored on the identity.</param>
        /// <param name="sourceType">The type that describes how the value should be interpreted.</param>
        /// <returns>
        /// The selection id of the language, or <see cref="Guid.Empty"/> when the identity has
        /// not chosen one, which the selection control renders as its "none" entry.
        /// </returns>
        public object ToRaw(object value, Type sourceType)
        {
            return UiLanguage.FromCode(value as string)?.Id ?? Guid.Empty;
        }
    }
}
