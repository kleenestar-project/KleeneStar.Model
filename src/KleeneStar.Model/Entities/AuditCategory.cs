using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Names the functional area an <see cref="AuditEvent"/> belongs to. Where
    /// <see cref="AuditOrigin"/> says who set the event in motion and
    /// <see cref="AuditAction"/> says what was done, the category says what part of the
    /// installation it was done to.
    /// </summary>
    /// <remarks>
    /// The category exists so that the log can be filtered down to a question a reader actually
    /// has — "show me everything security-relevant", "show me every change to the schema" —
    /// without them having to enumerate the actions that happen to fall under it. It is
    /// therefore deliberately coarse: a handful of stable areas rather than one entry per
    /// entity type, which is what <see cref="AuditTargetType"/> is for.
    /// <para>
    /// The values are persisted as their ordinal, so new entries are appended rather than
    /// inserted.
    /// </para>
    /// </remarks>
    public enum AuditCategory
    {
        /// <summary>
        /// The installation itself: startup, shutdown, schema migration, seeding, retention
        /// runs. Events that describe the audited system rather than anything inside it.
        /// </summary>
        Lifecycle,

        /// <summary>
        /// Authentication and credentials: sign-in, sign-out, failed sign-in, session
        /// revocation, access tokens issued and revoked.
        /// </summary>
        Security,

        /// <summary>
        /// Who exists: identities, groups, tenants and the memberships between them.
        /// </summary>
        Identity,

        /// <summary>
        /// Who may do what: permission grants and revocations, and the refusals that follow
        /// from them.
        /// </summary>
        Authorization,

        /// <summary>
        /// The shape of the installation: classes, fields, forms, workflows, statuses,
        /// priorities, templates, SLA policies, calendars, dashboards, branding. Changing any
        /// of these changes how every object behaves, which is why they are audited apart from
        /// the objects themselves.
        /// </summary>
        Configuration,

        /// <summary>
        /// The data the installation holds: objects and their field values, comments,
        /// attachments, tags, links, shares.
        /// </summary>
        Content,

        /// <summary>
        /// Movement through a workflow: transitions travelled, and the state changes they
        /// produced.
        /// </summary>
        Workflow,

        /// <summary>
        /// Traffic across the installation boundary: REST API calls, portal requests, imports
        /// and exports.
        /// </summary>
        Integration
    }

    /// <summary>
    /// Provides extension methods for the <see cref="AuditCategory"/> enumeration.
    /// </summary>
    public static class AuditCategoryExtensions
    {
        /// <summary>
        /// Returns the wire token the REST API and the quickfilters exchange the category as.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The lower-case wire token.</returns>
        public static string Token(this AuditCategory category)
        {
            return category switch
            {
                AuditCategory.Lifecycle => "lifecycle",
                AuditCategory.Security => "security",
                AuditCategory.Identity => "identity",
                AuditCategory.Authorization => "authorization",
                AuditCategory.Configuration => "configuration",
                AuditCategory.Content => "content",
                AuditCategory.Workflow => "workflow",
                AuditCategory.Integration => "integration",
                _ => "lifecycle"
            };
        }

        /// <summary>
        /// Returns the localized text key for the category, suitable for passing to
        /// <c>I18N.Translate</c>.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The translation key.</returns>
        public static string Text(this AuditCategory category)
        {
            return category switch
            {
                AuditCategory.Lifecycle => "kleenestar.core:audit.category.lifecycle",
                AuditCategory.Security => "kleenestar.core:audit.category.security",
                AuditCategory.Identity => "kleenestar.core:audit.category.identity",
                AuditCategory.Authorization => "kleenestar.core:audit.category.authorization",
                AuditCategory.Configuration => "kleenestar.core:audit.category.configuration",
                AuditCategory.Content => "kleenestar.core:audit.category.content",
                AuditCategory.Workflow => "kleenestar.core:audit.category.workflow",
                AuditCategory.Integration => "kleenestar.core:audit.category.integration",
                _ => null
            };
        }

        /// <summary>
        /// Returns the CSS color-selection class the category is tinted with in the audit list.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The CSS class string.</returns>
        public static string Color(this AuditCategory category)
        {
            return category switch
            {
                AuditCategory.Lifecycle => TypeColorSelection.Secondary.ToClass(),
                AuditCategory.Security => TypeColorSelection.Danger.ToClass(),
                AuditCategory.Identity => TypeColorSelection.Info.ToClass(),
                AuditCategory.Authorization => TypeColorSelection.Warning.ToClass(),
                AuditCategory.Configuration => TypeColorSelection.Primary.ToClass(),
                AuditCategory.Content => TypeColorSelection.Success.ToClass(),
                AuditCategory.Workflow => TypeColorSelection.Info.ToClass(),
                AuditCategory.Integration => TypeColorSelection.Warning.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }

        /// <summary>
        /// Parses a wire token into the matching category. An unknown, empty or <c>null</c>
        /// token reads as <see cref="AuditCategory.Lifecycle"/>.
        /// </summary>
        /// <param name="token">The wire token.</param>
        /// <returns>The parsed category.</returns>
        public static AuditCategory Parse(string token)
        {
            return (token?.Trim() ?? string.Empty).ToLowerInvariant() switch
            {
                "security" => AuditCategory.Security,
                "identity" => AuditCategory.Identity,
                "authorization" => AuditCategory.Authorization,
                "configuration" => AuditCategory.Configuration,
                "content" => AuditCategory.Content,
                "workflow" => AuditCategory.Workflow,
                "integration" => AuditCategory.Integration,
                _ => AuditCategory.Lifecycle
            };
        }
    }
}
