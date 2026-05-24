using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the GUID identifier used by REST clients and the strongly typed
    /// <see cref="CommentState"/> enumeration.
    /// </summary>
    public class CommentStateConverter : IRestValueConverter
    {
        /// <summary>
        /// Converts a raw selection value (a semicolon-separated list of GUID ids as
        /// emitted by the REST selection control) into the matching <see cref="CommentState"/>.
        /// Unknown ids fall back to <see cref="CommentState.Active"/>; <c>null</c> input
        /// returns <c>null</c>.
        /// </summary>
        /// <param name="rawValue">The raw value submitted by the client.</param>
        /// <param name="targetType">The declared target type. Ignored — the converter
        /// always produces a <see cref="CommentState"/> when the input is a string.</param>
        /// <returns>The strongly typed <see cref="CommentState"/>, or <c>null</c>.</returns>
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

                if (id == CommentState.Edited.Id())
                {
                    return CommentState.Edited;
                }

                if (id == CommentState.Deleted.Id())
                {
                    return CommentState.Deleted;
                }

                if (id == CommentState.Hidden.Id())
                {
                    return CommentState.Hidden;
                }

                return CommentState.Active;
            }

            return rawValue;
        }

        /// <summary>
        /// Converts a <see cref="CommentState"/> to its raw GUID id so the REST
        /// selection control can pre-select the matching option. Any unrecognized value
        /// falls back to <see cref="CommentState.Active"/>'s id.
        /// </summary>
        /// <param name="value">The strongly typed value.</param>
        /// <param name="sourceType">The declared source type. Ignored.</param>
        /// <returns>The GUID identifier of the state.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value switch
            {
                CommentState.Edited => CommentState.Edited.Id(),
                CommentState.Deleted => CommentState.Deleted.Id(),
                CommentState.Hidden => CommentState.Hidden.Id(),
                _ => CommentState.Active.Id()
            };
        }
    }
}
