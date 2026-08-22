using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Names what set an <see cref="AuditEvent"/> in motion. This is the coarsest and the most
    /// important axis of the audit log: it separates what a person did from what the
    /// installation did to itself, and both from what somebody outside the installation asked
    /// it to do.
    /// </summary>
    /// <remarks>
    /// The origin is not derivable from the actor. A change carrying an identity may still have
    /// been made by a scheduled escalation running in that identity's name, and a change with no
    /// identity may be an anonymous API call rather than a system task. Recording the two
    /// separately is what lets a forensic reader ask "what did this user do" and "what did the
    /// system do on its own" as two different questions.
    /// <para>
    /// The values are persisted as their ordinal, so new entries are appended rather than
    /// inserted.
    /// </para>
    /// </remarks>
    public enum AuditOrigin
    {
        /// <summary>
        /// The installation acting on itself: startup and shutdown, schema migration, seeding,
        /// retention. No person is responsible for the event.
        /// </summary>
        System,

        /// <summary>
        /// A person acting through the user interface. The actor is the authenticated identity
        /// that made the request.
        /// </summary>
        User,

        /// <summary>
        /// A process the installation runs on its own schedule: SLA escalation, notification
        /// dispatch, recurring imports. An automation may act in an identity's name, which is
        /// then recorded as the actor while the origin stays <see cref="Automation"/>.
        /// </summary>
        Automation,

        /// <summary>
        /// A caller outside the installation: the REST API, the customer portal, a webhook.
        /// The actor is whichever identity the credential resolved to, and
        /// <see cref="AuditEvent.Agent"/> names the caller.
        /// </summary>
        External
    }

    /// <summary>
    /// Provides extension methods for the <see cref="AuditOrigin"/> enumeration.
    /// </summary>
    public static class AuditOriginExtensions
    {
        /// <summary>
        /// Returns the wire token the REST API and the quickfilters exchange the origin as.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns>The lower-case wire token.</returns>
        public static string Token(this AuditOrigin origin)
        {
            return origin switch
            {
                AuditOrigin.System => "system",
                AuditOrigin.User => "user",
                AuditOrigin.Automation => "automation",
                AuditOrigin.External => "external",
                _ => "system"
            };
        }

        /// <summary>
        /// Returns the localized text key for the origin, suitable for passing to
        /// <c>I18N.Translate</c>.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns>The translation key.</returns>
        public static string Text(this AuditOrigin origin)
        {
            return origin switch
            {
                AuditOrigin.System => "kleenestar.core:audit.origin.system",
                AuditOrigin.User => "kleenestar.core:audit.origin.user",
                AuditOrigin.Automation => "kleenestar.core:audit.origin.automation",
                AuditOrigin.External => "kleenestar.core:audit.origin.external",
                _ => null
            };
        }

        /// <summary>
        /// Returns the CSS color-selection class the origin is tinted with in the audit list.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns>The CSS class string.</returns>
        public static string Color(this AuditOrigin origin)
        {
            return origin switch
            {
                AuditOrigin.System => TypeColorSelection.Secondary.ToClass(),
                AuditOrigin.User => TypeColorSelection.Primary.ToClass(),
                AuditOrigin.Automation => TypeColorSelection.Info.ToClass(),
                AuditOrigin.External => TypeColorSelection.Warning.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }

        /// <summary>
        /// Parses a wire token into the matching origin. An unknown, empty or <c>null</c> token
        /// reads as <see cref="AuditOrigin.System"/>.
        /// </summary>
        /// <param name="token">The wire token.</param>
        /// <returns>The parsed origin.</returns>
        public static AuditOrigin Parse(string token)
        {
            return (token?.Trim() ?? string.Empty).ToLowerInvariant() switch
            {
                "user" => AuditOrigin.User,
                "automation" => AuditOrigin.Automation,
                "external" => AuditOrigin.External,
                _ => AuditOrigin.System
            };
        }
    }
}
