using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRestApi;

namespace KleeneStar.Model.Converters
{
    /// <summary>
    /// Converts between the id a workspace access modifier selection submits and a
    /// <see cref="WorkspaceAccessModifier"/>.
    /// </summary>
    /// <remarks>
    /// The selection (<c>/api/1/workspaces/accessmodifier</c>) offers the ids of
    /// <see cref="WorkspaceAccessModifierExtensions.Id"/>. A submission that names none of them,
    /// an empty selection included, is read as <see cref="WorkspaceAccessModifier.Public"/>.
    /// </remarks>
    public class WorkspaceAccessModifierConverter : IRestValueConverter
    {
        /// <summary>
        /// The access modifiers a selection can name.
        /// </summary>
        private static readonly WorkspaceAccessModifier[] _modifiers =
        [
            WorkspaceAccessModifier.Public,
            WorkspaceAccessModifier.Protected,
            WorkspaceAccessModifier.Private,
            WorkspaceAccessModifier.Internal
        ];

        /// <summary>
        /// Converts a raw value to an access modifier.
        /// </summary>
        /// <param name="rawValue">The submitted value: the id of a modifier, or a modifier.</param>
        /// <param name="targetType">The type of the target property.</param>
        /// <returns>
        /// The modifier the value names, <see cref="WorkspaceAccessModifier.Public"/> when it
        /// names none.
        /// </returns>
        public object FromRaw(object rawValue, Type targetType)
        {
            if (rawValue is WorkspaceAccessModifier modifier)
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
            else if (Enum.TryParse<WorkspaceAccessModifier>(entry, true, out var named)
                && Enum.IsDefined(named))
            {
                return named;
            }

            return WorkspaceAccessModifier.Public;
        }

        /// <summary>
        /// Converts an access modifier to the id the selection reads back.
        /// </summary>
        /// <param name="value">The access modifier.</param>
        /// <param name="sourceType">The type of the source property.</param>
        /// <returns>The id of the modifier, the id of public for an unknown value.</returns>
        public object ToRaw(object value, Type sourceType)
        {
            return value is WorkspaceAccessModifier modifier && modifier.Id() != Guid.Empty
                ? modifier.Id().ToString()
                : WorkspaceAccessModifier.Public.Id().ToString();
        }
    }
}
