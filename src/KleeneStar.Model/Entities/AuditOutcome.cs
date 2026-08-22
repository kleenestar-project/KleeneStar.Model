using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Says whether the action an <see cref="AuditEvent"/> records actually took effect.
    /// </summary>
    /// <remarks>
    /// A log that only records what succeeded answers half the questions asked of it. The
    /// attempts that failed and the ones that were refused are the ones a security review is
    /// looking for, and they are indistinguishable from noise unless the log itself separates
    /// them. An event is therefore written for the attempt, not for the success, and the outcome
    /// says which it was.
    /// <para>
    /// The values are persisted as their ordinal, so new entries are appended rather than
    /// inserted.
    /// </para>
    /// </remarks>
    public enum AuditOutcome
    {
        /// <summary>
        /// The action completed. Its deltas describe the state change it produced.
        /// </summary>
        Succeeded,

        /// <summary>
        /// The action was attempted and did not complete: a validation error, a conflict, a
        /// failure in the store. Its deltas describe what was attempted, not what took effect.
        /// </summary>
        Failed,

        /// <summary>
        /// The action was refused before it was attempted, because the caller was not permitted
        /// to perform it. Nothing changed.
        /// </summary>
        Denied
    }

    /// <summary>
    /// Provides extension methods for the <see cref="AuditOutcome"/> enumeration.
    /// </summary>
    public static class AuditOutcomeExtensions
    {
        /// <summary>
        /// Returns the wire token the REST API and the quickfilters exchange the outcome as.
        /// </summary>
        /// <param name="outcome">The outcome.</param>
        /// <returns>The lower-case wire token.</returns>
        public static string Token(this AuditOutcome outcome)
        {
            return outcome switch
            {
                AuditOutcome.Succeeded => "succeeded",
                AuditOutcome.Failed => "failed",
                AuditOutcome.Denied => "denied",
                _ => "succeeded"
            };
        }

        /// <summary>
        /// Returns the localized text key for the outcome, suitable for passing to
        /// <c>I18N.Translate</c>.
        /// </summary>
        /// <param name="outcome">The outcome.</param>
        /// <returns>The translation key.</returns>
        public static string Text(this AuditOutcome outcome)
        {
            return string.Concat("kleenestar.core:audit.outcome.", outcome.Token());
        }

        /// <summary>
        /// Returns the CSS color-selection class the outcome is tinted with in the audit list.
        /// </summary>
        /// <param name="outcome">The outcome.</param>
        /// <returns>The CSS class string.</returns>
        public static string Color(this AuditOutcome outcome)
        {
            return outcome switch
            {
                AuditOutcome.Succeeded => TypeColorSelection.Success.ToClass(),
                AuditOutcome.Failed => TypeColorSelection.Danger.ToClass(),
                AuditOutcome.Denied => TypeColorSelection.Warning.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }

        /// <summary>
        /// Parses a wire token into the matching outcome. An unknown, empty or <c>null</c> token
        /// reads as <see cref="AuditOutcome.Succeeded"/>.
        /// </summary>
        /// <param name="token">The wire token.</param>
        /// <returns>The parsed outcome.</returns>
        public static AuditOutcome Parse(string token)
        {
            return (token?.Trim() ?? string.Empty).ToLowerInvariant() switch
            {
                "failed" => AuditOutcome.Failed,
                "denied" => AuditOutcome.Denied,
                _ => AuditOutcome.Succeeded
            };
        }
    }
}
