using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Says what an <see cref="AuditDelta"/> did to the attribute it names: brought it into
    /// existence, moved it, or took it away.
    /// </summary>
    /// <remarks>
    /// The kind is stored, not inferred. Deriving it from whether <see cref="AuditDelta.OldValue"/>
    /// and <see cref="AuditDelta.NewValue"/> are null cannot tell an attribute that was set to
    /// nothing from one that ceased to exist, and cannot tell an attribute that was created
    /// empty from one that was never touched. Those are different facts, a replay that confuses
    /// them produces a different state, and a diff view that confuses them shows the reader
    /// something that never happened. Recording the kind explicitly is what keeps the three
    /// apart for good.
    /// <para>
    /// The values are persisted as their ordinal, so new entries are appended rather than
    /// inserted.
    /// </para>
    /// </remarks>
    public enum AuditDeltaKind
    {
        /// <summary>
        /// The attribute did not exist on the target before this event and does after.
        /// <see cref="AuditDelta.OldValue"/> is <c>null</c> and carries no meaning.
        /// </summary>
        Added,

        /// <summary>
        /// The attribute existed before and after, and its value moved.
        /// <see cref="AuditDelta.OldValue"/> is the value the log knew, which may itself be
        /// <c>null</c> when the attribute was explicitly empty.
        /// </summary>
        Modified,

        /// <summary>
        /// The attribute existed before this event and does not after.
        /// <see cref="AuditDelta.NewValue"/> is <c>null</c> and carries no meaning;
        /// <see cref="AuditDelta.OldValue"/> preserves what was lost.
        /// </summary>
        Removed
    }

    /// <summary>
    /// Provides extension methods for the <see cref="AuditDeltaKind"/> enumeration.
    /// </summary>
    public static class AuditDeltaKindExtensions
    {
        /// <summary>
        /// Returns the wire token the REST API exchanges the delta kind as.
        /// </summary>
        /// <param name="kind">The delta kind.</param>
        /// <returns>The lower-case wire token.</returns>
        public static string Token(this AuditDeltaKind kind)
        {
            return kind switch
            {
                AuditDeltaKind.Added => "added",
                AuditDeltaKind.Modified => "modified",
                AuditDeltaKind.Removed => "removed",
                _ => "modified"
            };
        }

        /// <summary>
        /// Returns the localized text key for the delta kind, suitable for passing to
        /// <c>I18N.Translate</c>.
        /// </summary>
        /// <param name="kind">The delta kind.</param>
        /// <returns>The translation key.</returns>
        public static string Text(this AuditDeltaKind kind)
        {
            return string.Concat("kleenestar.core:audit.delta.kind.", kind.Token());
        }

        /// <summary>
        /// Returns the CSS color-selection class the delta kind is tinted with in the detail
        /// view.
        /// </summary>
        /// <param name="kind">The delta kind.</param>
        /// <returns>The CSS class string.</returns>
        public static string Color(this AuditDeltaKind kind)
        {
            return kind switch
            {
                AuditDeltaKind.Added => TypeColorSelection.Success.ToClass(),
                AuditDeltaKind.Modified => TypeColorSelection.Primary.ToClass(),
                AuditDeltaKind.Removed => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }

        /// <summary>
        /// Parses a wire token into the matching delta kind. An unknown, empty or <c>null</c>
        /// token reads as <see cref="AuditDeltaKind.Modified"/>.
        /// </summary>
        /// <param name="token">The wire token.</param>
        /// <returns>The parsed delta kind.</returns>
        public static AuditDeltaKind Parse(string token)
        {
            return (token?.Trim() ?? string.Empty).ToLowerInvariant() switch
            {
                "added" => AuditDeltaKind.Added,
                "removed" => AuditDeltaKind.Removed,
                _ => AuditDeltaKind.Modified
            };
        }
    }
}
