using KleeneStar.Model.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a one-time link that lets the owner of an internal account set its password.
    /// </summary>
    /// <remarks>
    /// There is no mail delivery in the installation, so an administrator issues the link and
    /// hands it over themselves. The secret the link carries is shown exactly once, when it is
    /// issued; only its hash is stored, like an <see cref="AccessToken"/>'s. A link is spent by
    /// its first use (<see cref="Used"/>), dies with <see cref="Expires"/>, and is superseded by
    /// the next link issued for the same account - so there is at most one open link per
    /// account, and the administrator's newest one is it.
    /// </remarks>
    public class PasswordReset : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the link.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the account whose password the link sets.
        /// </summary>
        public Guid IdentityId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the account.
        /// </summary>
        [JsonIgnore]
        public Identity Identity { get; set; }

        /// <summary>
        /// Gets or sets the identity that issued the link, or <see langword="null"/> when the
        /// system issued it.
        /// </summary>
        public Guid? IssuedById { get; set; }

        /// <summary>
        /// Gets or sets the hash of the link's secret. The secret itself is never persisted.
        /// </summary>
        [JsonIgnore]
        [AuditRedacted]
        public string TokenHash { get; set; }

        /// <summary>
        /// Gets or sets the date and time the link was issued.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time after which the link sets nothing.
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Gets or sets the date and time the link was spent - by setting the password, or by
        /// being superseded - or <see langword="null"/> while it is open.
        /// </summary>
        public DateTime? Used { get; set; }

        /// <summary>
        /// Determines whether the link still sets a password at the supplied point in time.
        /// </summary>
        /// <param name="now">The point in time, in UTC.</param>
        /// <returns><see langword="true"/> when the link is neither spent nor expired.</returns>
        public bool IsOpen(DateTime now)
        {
            return !Used.HasValue && Expires > now;
        }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public PasswordReset()
        {
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
        }
    }
}
