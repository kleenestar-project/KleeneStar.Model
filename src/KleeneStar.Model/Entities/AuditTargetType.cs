using System;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Names the kind of record an <see cref="AuditEvent"/> is about.
    /// </summary>
    /// <remarks>
    /// The target type is what makes <see cref="AuditEvent.TargetId"/> unambiguous. Ids are
    /// GUIDs and therefore unique across the installation, but a reader asking "everything that
    /// happened to the workflows" has no way to select them from ids alone, and a projection
    /// replaying the attributes of a target has to know which kind of record it is
    /// reconstructing. Recording the type beside the id also keeps the trail readable after the
    /// record itself is deleted, which is when a forensic reader needs it most.
    /// <para>
    /// A record type that is not listed here cannot be audited with a resolvable target, so a
    /// new audited entity means a new member. The values are persisted as their ordinal, so new
    /// entries are appended rather than inserted.
    /// </para>
    /// </remarks>
    public enum AuditTargetType
    {
        /// <summary>
        /// The event is about nothing in particular, or about something the log has no record
        /// type for. <see cref="AuditEvent.TargetId"/> is <c>null</c>.
        /// </summary>
        None,

        /// <summary>
        /// The installation as a whole: startup, shutdown, migration, seeding, retention.
        /// </summary>
        Installation,

        /// <summary>
        /// A workspace.
        /// </summary>
        Workspace,

        /// <summary>
        /// A class definition.
        /// </summary>
        Class,

        /// <summary>
        /// A field definition.
        /// </summary>
        Field,

        /// <summary>
        /// A form definition.
        /// </summary>
        Form,

        /// <summary>
        /// An object template.
        /// </summary>
        Template,

        /// <summary>
        /// A priority.
        /// </summary>
        Priority,

        /// <summary>
        /// A workflow definition.
        /// </summary>
        Workflow,

        /// <summary>
        /// A workflow status.
        /// </summary>
        Status,

        /// <summary>
        /// An object.
        /// </summary>
        Object,

        /// <summary>
        /// A comment posted on an object.
        /// </summary>
        Comment,

        /// <summary>
        /// A file attached to an object.
        /// </summary>
        Attachment,

        /// <summary>
        /// A tag attached to an object.
        /// </summary>
        Tag,

        /// <summary>
        /// A typed link between two objects.
        /// </summary>
        Link,

        /// <summary>
        /// A share relationship on an object.
        /// </summary>
        Share,

        /// <summary>
        /// A sprint.
        /// </summary>
        Sprint,

        /// <summary>
        /// An identity.
        /// </summary>
        Identity,

        /// <summary>
        /// A group.
        /// </summary>
        Group,

        /// <summary>
        /// A tenant.
        /// </summary>
        Tenant,

        /// <summary>
        /// A signed-in device or browser.
        /// </summary>
        Session,

        /// <summary>
        /// A personal access token.
        /// </summary>
        AccessToken,

        /// <summary>
        /// A group-to-policy grant on a scope.
        /// </summary>
        Permission,

        /// <summary>
        /// An SLA policy.
        /// </summary>
        SlaPolicy,

        /// <summary>
        /// A calendar.
        /// </summary>
        Calendar,

        /// <summary>
        /// A dashboard.
        /// </summary>
        Dashboard,

        /// <summary>
        /// A persisted object view.
        /// </summary>
        ObjectView,

        /// <summary>
        /// A link shown in the app navigator.
        /// </summary>
        NavigatorLink,

        /// <summary>
        /// The branding of the installation.
        /// </summary>
        Branding,

        /// <summary>
        /// The maintenance notice of the installation.
        /// </summary>
        Maintenance,

        /// <summary>
        /// A REST API or portal endpoint, for events that record access rather than a record
        /// change.
        /// </summary>
        Endpoint
    }

    /// <summary>
    /// Provides extension methods for the <see cref="AuditTargetType"/> enumeration.
    /// </summary>
    public static class AuditTargetTypeExtensions
    {
        /// <summary>
        /// Returns the wire token the REST API and the quickfilters exchange the target type as.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The lower-case wire token.</returns>
        public static string Token(this AuditTargetType type)
        {
            return Enum.GetName(type)?.ToLowerInvariant() ?? "none";
        }

        /// <summary>
        /// Returns the localized text key for the target type, suitable for passing to
        /// <c>I18N.Translate</c>.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The translation key.</returns>
        public static string Text(this AuditTargetType type)
        {
            return string.Concat("kleenestar.core:audit.target.", type.Token());
        }

        /// <summary>
        /// Parses a wire token into the matching target type. An unknown, empty or <c>null</c>
        /// token reads as <see cref="AuditTargetType.None"/>.
        /// </summary>
        /// <param name="token">The wire token.</param>
        /// <returns>The parsed target type.</returns>
        public static AuditTargetType Parse(string token)
        {
            var normalized = (token?.Trim() ?? string.Empty).ToLowerInvariant();

            foreach (var candidate in Enum.GetValues<AuditTargetType>())
            {
                if (candidate.Token() == normalized)
                {
                    return candidate;
                }
            }

            return AuditTargetType.None;
        }
    }
}
