using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents an in-app notification addressed to one identity: what happened, when, and
    /// where to go to see it. Rows accumulate into the notification center the bell in the
    /// header opens.
    /// </summary>
    /// <remarks>
    /// The counterpart in WebExpress — <c>WebUI.WebNotification.Model.Notification</c> — is a
    /// transient toast: it lives in memory, is global rather than addressed to anybody, and is
    /// consumed by the first client that polls for it. That is the right shape for "your save
    /// worked" and the wrong one for a list the user can come back to, which is why the center
    /// keeps its own record.
    ///
    /// <see cref="TitleKey"/> and <see cref="MessageKey"/> hold translation keys rather than
    /// finished sentences, so a notification is rendered in the language of whoever reads it
    /// instead of the language that happened to be active when it was raised. A value that is
    /// not a known key is rendered verbatim, which is how a caller passes literal text.
    /// </remarks>
    public class UserNotification : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the notification.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identity the notification is addressed to.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the addressed identity.
        /// </summary>
        [JsonIgnore]
        public Identity Owner { get; set; }

        /// <summary>
        /// Gets or sets the identity that caused the event, or <see langword="null"/> when it
        /// was not caused by a person — a scheduled job, an SLA that ran out, an import.
        /// </summary>
        public Guid? ActorId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the identity that caused the event.
        /// </summary>
        [JsonIgnore]
        public Identity Actor { get; set; }

        /// <summary>
        /// Gets or sets the translation key of the heading
        /// (e.g. <c>kleenestar.core:notification.title.created</c>).
        /// </summary>
        public string TitleKey { get; set; }

        /// <summary>
        /// Gets or sets the translation key of the message
        /// (e.g. <c>kleenestar.core:notification.object.created</c>).
        /// </summary>
        public string MessageKey { get; set; }

        /// <summary>
        /// Gets or sets the subject the notification is about — an object key, a workspace
        /// name — appended to the message so a list of otherwise identical entries stays
        /// readable. <see langword="null"/> when the notification names nothing in particular.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the path the notification links to, relative to the host
        /// (e.g. <c>/kleenestar/issue/BUG-1</c>), or <see langword="null"/> when there is
        /// nothing to open.
        /// </summary>
        public string TargetUri { get; set; }

        /// <summary>
        /// Gets or sets the icon of the record the notification is about, as the path it is
        /// served from. Kept on the notification rather than looked up when the list is
        /// rendered, so an entry still shows the icon the record carried at the time — and
        /// still shows one at all once the record is gone.
        /// </summary>
        public string SubjectIcon { get; set; }

        /// <summary>
        /// Gets or sets whether the addressee has seen the notification.
        /// </summary>
        public bool Read { get; set; }

        /// <summary>
        /// Gets or sets the date and time the notification was raised.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public UserNotification()
        {
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the notification.</param>
        public UserNotification(Guid id)
        {
            Id = id;
        }
    }
}
