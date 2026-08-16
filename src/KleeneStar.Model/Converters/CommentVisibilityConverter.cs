using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the GUID identifier used by REST clients and the strongly typed
    /// <see cref="CommentVisibility"/> enumeration.
    /// </summary>
    public class CommentVisibilityConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw selection value (a semicolon-separated list of GUID ids as
        /// emitted by the REST selection control) into the matching
        /// <see cref="CommentVisibility"/>. The wire tokens <c>public</c> and
        /// <c>internal-team</c> are accepted as well, so a hand-written JSON body reaches
        /// the same value. Unknown input falls back to
        /// <see cref="CommentVisibility.Public"/>; <c>null</c> input returns <c>null</c>.
        /// </summary>
        /// <param name="rawValue">The raw value submitted by the client.</param>
        /// <param name="targetType">The declared target type. Ignored — the converter
        /// always produces a <see cref="CommentVisibility"/> when the input is a string.</param>
        /// <returns>The strongly typed <see cref="CommentVisibility"/>, or <c>null</c>.</returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            if (rawValue is null)
            {
                return null;
            }

            if (rawValue is string s)
            {
                var token = s.Split(";", StringSplitOptions.RemoveEmptyEntries)
                             .Select(x => x.Trim())
                             .FirstOrDefault(x => x.Length > 0);

                if (token is null)
                {
                    return CommentVisibility.Public;
                }

                if (Guid.TryParse(token, out var id))
                {
                    return id == CommentVisibility.InternalTeam.Id()
                        ? CommentVisibility.InternalTeam
                        : CommentVisibility.Public;
                }

                return CommentVisibilityExtensions.Parse(token);
            }

            return rawValue;
        }

        /// <summary>
        /// Converts a <see cref="CommentVisibility"/> to its raw GUID id so the REST
        /// selection control can pre-select the matching option. Any unrecognized value
        /// falls back to <see cref="CommentVisibility.Public"/>'s id.
        /// </summary>
        /// <param name="value">The strongly typed value.</param>
        /// <param name="sourceType">The declared source type. Ignored.</param>
        /// <returns>The GUID identifier of the visibility.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value switch
            {
                CommentVisibility.InternalTeam => CommentVisibility.InternalTeam.Id(),
                _ => CommentVisibility.Public.Id()
            };
        }
    }
}
