using System;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the value a checkbox submits and a boolean property.
    /// </summary>
    /// <remarks>
    /// A checkbox rendered by <c>ControlFormItemInputCheck</c> carries no value attribute, so the
    /// browser reports its DOM default and the rest form submits the string <c>"on"</c> for a
    /// ticked box. The default binding runs that through <c>Convert.ChangeType</c>, which throws on
    /// <c>"on"</c>, and the surrounding bind swallows the exception -- the property silently keeps
    /// its previous value while the request still answers 200, so a toggled switch appears to save
    /// and does nothing.
    ///
    /// This converter is a workaround for that and can be dropped, together with the attribute on
    /// the property, once the framework either submits a real boolean for valueless checkboxes or
    /// accepts the html checkbox idiom when binding to a boolean.
    /// </remarks>
    public class RestValueConverterBool : IRestValueConverter
    {
        /// <summary>
        /// Converts a submitted value to a boolean.
        /// </summary>
        /// <param name="rawValue">
        /// The value to convert. May be a boolean already, or one of the strings a checkbox or a
        /// json payload can carry.
        /// </param>
        /// <param name="targetType">
        /// The type to which the raw value should be converted. Cannot be null.
        /// </param>
        /// <returns>
        /// The value as a boolean. An unticked checkbox submits nothing at all, so anything not
        /// recognised as affirmative is reported as false rather than left undecided.
        /// </returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            return rawValue switch
            {
                null => false,
                bool b => b,
                string s => s.Trim().ToLowerInvariant() is "on" or "true" or "1" or "yes" or "checked",
                _ => Convert.ToBoolean(rawValue)
            };
        }

        /// <summary>
        /// Converts a boolean to the representation handed to the client.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="sourceType">
        /// The type that describes how the value should be interpreted. Cannot be null.
        /// </param>
        /// <returns>
        /// The value as a boolean, so the checkbox is ticked from the loaded record rather than
        /// from a string the client would have to interpret.
        /// </returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value is bool b && b;
        }
    }
}
