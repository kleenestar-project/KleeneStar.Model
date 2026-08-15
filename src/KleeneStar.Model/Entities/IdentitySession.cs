using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a device or browser that is currently signed in with an identity. One row
    /// per login, listed on the profile's "active sessions" page so the owner can end the ones
    /// they do not recognize.
    /// </summary>
    /// <remarks>
    /// Not to be confused with <see cref="UserSession"/>, which is the generic per-identity
    /// key/value store for UI preferences. This entity describes a login, not a setting.
    /// </remarks>
    public class IdentitySession : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the session.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identity that is signed in.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the signed-in identity.
        /// </summary>
        [JsonIgnore]
        public Identity Owner { get; set; }

        /// <summary>
        /// Gets or sets the device the session runs on (e.g. MacBook Pro 14, iPhone 15).
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Gets or sets the client the session was opened with ("Chrome 125", "KleeneStar iOS 4.12").
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets whether the device is a handheld one. Selects the icon shown in front
        /// of the row.
        /// </summary>
        public bool Mobile { get; set; }

        /// <summary>
        /// Gets or sets the location the session was last seen from ("Berlin, DE").
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the masked remote address of the session ("85.214.···.42"). Stored
        /// masked so the list never discloses a full address.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time the session was opened.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time the session last made a request.
        /// </summary>
        public DateTime LastActive { get; set; }

        /// <summary>
        /// Gets or sets whether this is the session the page is being served to. The current
        /// session carries a badge instead of a sign-out button.
        /// </summary>
        public bool Current { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public IdentitySession()
        {
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
            LastActive = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the session.</param>
        public IdentitySession(Guid id)
        {
            Id = id;
        }
    }
}
