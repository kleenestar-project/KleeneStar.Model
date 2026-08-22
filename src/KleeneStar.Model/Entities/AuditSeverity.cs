using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Says how much attention an <see cref="AuditEvent"/> warrants when the log is read as a
    /// stream rather than searched.
    /// </summary>
    /// <remarks>
    /// Severity is not the same as <see cref="AuditOutcome"/>. A successful action can be the
    /// most severe entry in the log — an administrator granting themselves a policy succeeds
    /// every time — and a failed one can be routine. The outcome says whether it took effect;
    /// the severity says whether somebody should look.
    /// <para>
    /// The values are persisted as their ordinal and ordered by increasing weight, so a filter
    /// can ask for everything at or above a level with a comparison rather than a set.
    /// </para>
    /// </remarks>
    public enum AuditSeverity
    {
        /// <summary>
        /// Routine activity. The bulk of the log.
        /// </summary>
        Info,

        /// <summary>
        /// Activity worth noticing in review: configuration changes, identity changes, anything
        /// that alters how the installation behaves for others.
        /// </summary>
        Notice,

        /// <summary>
        /// Activity that suggests something is wrong: refused access, failed authentication,
        /// an automation that could not complete.
        /// </summary>
        Warning,

        /// <summary>
        /// Activity that demands attention now: the integrity of the log itself, the loss of a
        /// record that cannot be recovered, an escalation of privilege.
        /// </summary>
        Critical
    }

    /// <summary>
    /// Provides extension methods for the <see cref="AuditSeverity"/> enumeration.
    /// </summary>
    public static class AuditSeverityExtensions
    {
        /// <summary>
        /// Returns the wire token the REST API and the quickfilters exchange the severity as.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <returns>The lower-case wire token.</returns>
        public static string Token(this AuditSeverity severity)
        {
            return severity switch
            {
                AuditSeverity.Info => "info",
                AuditSeverity.Notice => "notice",
                AuditSeverity.Warning => "warning",
                AuditSeverity.Critical => "critical",
                _ => "info"
            };
        }

        /// <summary>
        /// Returns the localized text key for the severity, suitable for passing to
        /// <c>I18N.Translate</c>.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <returns>The translation key.</returns>
        public static string Text(this AuditSeverity severity)
        {
            return string.Concat("kleenestar.core:audit.severity.", severity.Token());
        }

        /// <summary>
        /// Returns the CSS color-selection class the severity is tinted with in the audit list.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <returns>The CSS class string.</returns>
        public static string Color(this AuditSeverity severity)
        {
            return severity switch
            {
                AuditSeverity.Info => TypeColorSelection.Secondary.ToClass(),
                AuditSeverity.Notice => TypeColorSelection.Info.ToClass(),
                AuditSeverity.Warning => TypeColorSelection.Warning.ToClass(),
                AuditSeverity.Critical => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }

        /// <summary>
        /// Parses a wire token into the matching severity. An unknown, empty or <c>null</c>
        /// token reads as <see cref="AuditSeverity.Info"/>.
        /// </summary>
        /// <param name="token">The wire token.</param>
        /// <returns>The parsed severity.</returns>
        public static AuditSeverity Parse(string token)
        {
            return (token?.Trim() ?? string.Empty).ToLowerInvariant() switch
            {
                "notice" => AuditSeverity.Notice,
                "warning" => AuditSeverity.Warning,
                "critical" => AuditSeverity.Critical,
                _ => AuditSeverity.Info
            };
        }
    }
}
