using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the selection id the identity dialogs submit and the key stored on
    /// <see cref="Identity.AuthenticationSource"/>.
    /// </summary>
    /// <remarks>
    /// A picker identifies its entries by guid, so a source travels as
    /// <see cref="IdentitySource.IdOf"/> and is read back through
    /// <see cref="IdentitySource.FromId"/>. The internal source is stored as
    /// <see langword="null"/> but offered under the id of <see cref="IdentitySource.Local"/>: a
    /// dialog opened on an internal account has to show the entry it is filed under.
    /// </remarks>
    public class AuthenticationSourceConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts the submitted selection id into the stored key.
        /// </summary>
        /// <param name="rawValue">The submitted id, possibly as a semicolon separated list.</param>
        /// <param name="targetType">The type to which the raw value should be converted.</param>
        /// <returns>The key, or <see langword="null"/> for the internal source.</returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            if (rawValue is not string s)
            {
                return rawValue;
            }

            var key = s.Split(";", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .FirstOrDefault(x => x.Length > 0);

            return IdentitySource.Normalize(IdentitySource.FromId(key));
        }

        /// <summary>
        /// Converts the stored key into the selection id.
        /// </summary>
        /// <param name="value">The stored key.</param>
        /// <param name="sourceType">The type that describes how the value should be interpreted.</param>
        /// <returns>The selection id of the source.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            return IdentitySource.IdOf(value as string);
        }
    }
}
