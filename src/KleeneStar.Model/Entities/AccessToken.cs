using KleeneStar.Model.Converters;
using KleeneStar.Model.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a personal access token: a credential that authenticates API requests and
    /// integrations in the name of the identity that created it.
    /// </summary>
    /// <remarks>
    /// Only the <see cref="Prefix"/> is kept in clear so the owner can recognize the token in
    /// the list; the secret itself is stored as <see cref="TokenHash"/> and is shown exactly
    /// once, right after it was created.
    /// </remarks>
    public class AccessToken : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the token.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identity that owns this token.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the owning identity.
        /// </summary>
        [JsonIgnore]
        public Identity Owner { get; set; }

        /// <summary>
        /// Gets or sets the label the owner gave the token ("CI · GitHub Actions").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the leading, non-secret part of the token ("kls_AB12cd34"). Shown in
        /// the token list so the owner can tell one token from another.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the hash of the token secret. The secret itself is never persisted.
        /// </summary>
        [JsonIgnore]
        [AuditRedacted]
        public string TokenHash { get; set; }

        /// <summary>
        /// Gets or sets the scopes the token grants, separated by spaces
        /// ("read:tickets write:tickets").
        /// </summary>
        [RestConverter<AccessTokenScopesConverter>]
        public string Scopes { get; set; }

        /// <summary>
        /// Gets or sets the date and time the token was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time the token last authenticated a request, or
        /// <see langword="null"/> when it has never been used.
        /// </summary>
        public DateTime? LastUsed { get; set; }

        /// <summary>
        /// Gets or sets the date and time the token stops authenticating requests, or
        /// <see langword="null"/> when it never expires.
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// Gets or sets whether the token was revoked by its owner.
        /// </summary>
        [RestConverter<RestValueConverterBool>]
        public bool Revoked { get; set; }

        /// <summary>
        /// Gets the effective state of the token: revoked wins over expired, and a token is
        /// expired as soon as <see cref="Expires"/> lies in the past.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public AccessTokenState State => Revoked
            ? AccessTokenState.Revoked
            : Expires.HasValue && Expires.Value < DateTime.UtcNow
                ? AccessTokenState.Expired
                : AccessTokenState.Active;

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public AccessToken()
        {
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the token.</param>
        public AccessToken(Guid id)
        {
            Id = id;
        }
    }
}
