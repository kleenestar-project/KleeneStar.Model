using KleeneStar.Model.Converters;
using KleeneStar.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebIdentity;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents an identity (user account) in the system.
    /// </summary>
    public class Identity : IEntity, IIdentity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the identity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address of the identity.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Returns the avatar associated with this identity.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Avatar { get; set; }

        /// <summary>
        /// Gets or sets the identity state (active, locked, disabled, etc.).
        /// </summary>
        [RestConverter<IdentityStateConverter>]
        public IdentityState State { get; set; }

        /// <summary>
        /// Gets or sets the login name of the identity. Unique across the installation and
        /// used in profile URLs and @-mentions, whereas <see cref="Name"/> is the free-form
        /// display name shown in comments.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets whether the address in <see cref="Email"/> has been confirmed by the
        /// user. Drives the verified badge on the account page.
        /// </summary>
        [RestConverter<RestValueConverterBool>]
        public bool EmailVerified { get; set; }

        /// <summary>
        /// Gets or sets the short self-description shown on the profile page. Markdown is
        /// supported by the renderer.
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        /// Gets or sets the international dialling prefix of <see cref="Phone"/> (e.g. "+49").
        /// Held separately so the profile form can offer it as a selection next to the number.
        /// </summary>
        public string PhoneCountry { get; set; }

        /// <summary>
        /// Gets or sets the phone number without the dialling prefix. Only visible to members
        /// of the identity's own tenant.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the personal web site of the identity (portfolio, blog, ...), stored
        /// without the scheme — the form prepends "https://".
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets the free-form location of the identity ("Berlin, Deutschland").
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the job title of the identity inside the tenant.
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets the culture of the user interface as an ISO code ("de", "en", ...).
        /// <see langword="null"/> falls back to the culture configured for the application.
        /// </summary>
        [RestConverter<LanguageConverter>]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the IANA time zone id used to render dates and times for this identity.
        /// <see langword="null"/> means the time zone is detected automatically.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the .NET date pattern the identity prefers ("dd.MM.yyyy", "yyyy-MM-dd", ...).
        /// <see langword="null"/> falls back to the pattern of <see cref="Language"/>.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Gets or sets the first day of the week in calendars and schedules.
        /// </summary>
        [RestConverter<WeekStartConverter>]
        public WeekStart WeekStart { get; set; }

        /// <summary>
        /// Gets or sets the role the identity holds inside its tenant, as set by the workspace
        /// admins ("Workspace-Admin · Klasse Bug"). Free text — the effective permissions come
        /// from the group memberships, not from this label.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the point in time from which <see cref="Role"/> has been held.
        /// </summary>
        public DateTime? RoleSince { get; set; }

        /// <summary>
        /// Gets or sets the department of the identity inside its tenant ("Engineering · QA").
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the cost center used for internal billing ("CC-4711"). Optional.
        /// </summary>
        public string CostCenter { get; set; }

        /// <summary>
        /// Gets or sets the personnel number of the identity. Only shown to tenant admins.
        /// </summary>
        public string PersonnelNumber { get; set; }

        /// <summary>
        /// Gets or sets the identity that takes over this identity's tickets while it is
        /// absent, or <see langword="null"/> when no deputy has been named.
        /// </summary>
        public Guid? DeputyId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the deputy named by <see cref="DeputyId"/>.
        /// </summary>
        [JsonIgnore]
        public Identity Deputy { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the tenant the identity belongs to.
        /// <see langword="null"/> for operator-side accounts that are not members of
        /// any tenant (the fallback admin identity, integration users). Portal-side
        /// user accounts always carry a tenant id so the
        /// <see cref="KleeneStar.Portal.WebManager.PortalManager"/> can scope
        /// <c>IssueScope.Organization</c> queries to the identity's tenant.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the tenant the identity belongs
        /// to. <see langword="null"/> for operator-side accounts.
        /// </summary>
        [JsonIgnore]
        public Tenant Tenant { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for group memberships (m:n).
        /// </summary>
        [JsonIgnore]
        public List<IdentityGroupMembership> GroupMemberships { get; set; } = [];

        /// <summary>
        /// Returns the groups associated with this identity.
        /// </summary>
        IEnumerable<IIdentityGroup> IIdentity.Groups => GroupMemberships.Select(x => x.Group);

        /// <summary>
        /// Gets or sets the hashed representation of the user's password.
        /// </summary>
        [JsonIgnore]
        [AuditRedacted]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Initializes a new instance of the Identity class.
        /// </summary>
        public Identity()
        {
            Id = Guid.NewGuid();
            State = IdentityState.Active;
        }

        /// <summary>
        /// Initializes a new instance of the class with the
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the identity.
        /// </param>
        public Identity(Guid id)
        {
            Id = id;
        }
    }
}
