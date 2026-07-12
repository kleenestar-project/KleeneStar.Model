using KleeneStar.Model.Converters;
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
