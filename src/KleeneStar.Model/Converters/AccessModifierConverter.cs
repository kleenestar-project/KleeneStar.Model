using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the id an access modifier selection of a class or field submits and an
    /// <see cref="AccessModifier"/>.
    /// </summary>
    /// <remarks>
    /// The selections (<c>/api/1/classes/accessmodifier</c>, <c>/api/1/fields/accessmodifier</c>)
    /// offer the ids of <see cref="AccessModifierExtensions.Id"/>. A submission that names none of
    /// them, an empty selection included, is read as <see cref="AccessModifier.Public"/>. Workspaces
    /// use <see cref="WorkspaceAccessModifierConverter"/>, whose ids differ.
    /// </remarks>
    public class AccessModifierConverter : IRestValueConverter
    {
        /// <summary>
        /// The access modifiers a selection can name.
        /// </summary>
        private static readonly AccessModifier[] _modifiers =
        [
            AccessModifier.Public,
            AccessModifier.Protected,
            AccessModifier.Private,
            AccessModifier.Internal
        ];

        /// <summary>
        /// Converts a raw value to an access modifier.
        /// </summary>
        /// <param name="rawValue">The submitted value: the id of a modifier, or a modifier.</param>
        /// <param name="targetType">The type of the target property.</param>
        /// <returns>
        /// The modifier the value names, <see cref="AccessModifier.Public"/> when it names none.
        /// </returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            if (rawValue is AccessModifier modifier)
            {
                return modifier;
            }

            var entry = rawValue?.ToString()?
                .Split(";", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .FirstOrDefault(x => x.Length > 0);

            if (Guid.TryParse(entry, out var id))
            {
                foreach (var candidate in _modifiers)
                {
                    if (candidate.Id() == id)
                    {
                        return candidate;
                    }
                }
            }
            else if (Enum.TryParse<AccessModifier>(entry, true, out var named)
                && Enum.IsDefined(named))
            {
                return named;
            }

            return AccessModifier.Public;
        }

        /// <summary>
        /// Converts an access modifier to the id the selection reads back.
        /// </summary>
        /// <param name="value">The access modifier.</param>
        /// <param name="sourceType">The type of the source property.</param>
        /// <returns>The id of the modifier, the id of public for an unknown value.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value is AccessModifier modifier && modifier.Id() != Guid.Empty
                ? modifier.Id().ToString()
                : AccessModifier.Public.Id().ToString();
        }
    }
}
