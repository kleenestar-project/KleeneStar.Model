using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the selection ids the token form submits and the space separated scope
    /// list stored on <see cref="AccessToken.Scopes"/>.
    /// </summary>
    /// <remarks>
    /// A multi-select submits its chosen entries as a semicolon separated list of ids, whereas
    /// the token stores the scope names as they appear in an API request, so anything checking
    /// a token's permissions reads them without knowing about this control.
    /// </remarks>
    public class AccessTokenScopesConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw value to the specified target type.
        /// </summary>
        /// <param name="rawValue">The ids of the chosen scopes, separated by semicolons.</param>
        /// <param name="targetType">The type to which the raw value should be converted.</param>
        /// <returns>
        /// The scope names, separated by spaces, or <see langword="null"/> when nothing was
        /// chosen.
        /// </returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            if (rawValue is not string s)
            {
                return rawValue;
            }

            var names = s.Split(";", StringSplitOptions.RemoveEmptyEntries)
                         .Select(x => x.Trim())
                         .Where(x => x.Length > 0)
                         .Select(x => Guid.TryParse(x, out var g) ? AccessTokenScope.FromId(g) : null)
                         .Where(x => x is not null)
                         .Select(x => x.Name)
                         .Distinct()
                         .ToList();

            return names.Count == 0 ? null : string.Join(" ", names);
        }

        /// <summary>
        /// Converts the specified value to its raw representation based on the provided source type.
        /// </summary>
        /// <param name="value">The scope list as stored on the token.</param>
        /// <param name="sourceType">The type that describes how the value should be interpreted.</param>
        /// <returns>
        /// The ids of the granted scopes, separated by semicolons, which the multi-select
        /// matches its entries against.
        /// </returns>
        public object ToRaw(object value, Type sourceType)
        {
            var ids = AccessTokenScope.Split(value as string)
                .Select(AccessTokenScope.FromName)
                .Where(x => x is not null)
                .Select(x => x.Id.ToString())
                .ToList();

            return string.Join(";", ids);
        }
    }
}
