using KleeneStar.Model.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// A connection of a workspace to another system: an outgoing webhook the automation calls,
    /// or an incoming one another system calls.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Outgoing.</b> A rule's <c>webhook &lt;name&gt;</c> action posts the object and the event
    /// to <see cref="Url"/>, signed with <see cref="Secret"/> (HMAC-SHA256). The call runs as an
    /// automation job, so it is persistent, retried with a growing delay and parked when the
    /// other system stays down - and carries the job's delivery id, which lets the receiver
    /// discard a repeated delivery.
    /// </para>
    /// <para>
    /// <b>Incoming.</b> Another system posts to the hook's address with its token; the payload is
    /// translated by the integration named in <see cref="Integration"/> - the platform's generic
    /// one, or a plugin's for GitLab, Jenkins, Automic - into events on objects, which rules
    /// react to like to any other event. No system's logic lives in the core.
    /// </para>
    /// </remarks>
    public class Webhook : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the webhook.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the workspace the webhook belongs to.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the name rules call the webhook by. Unique per workspace.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets which way the webhook goes.
        /// </summary>
        public WebhookDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the address an outgoing webhook posts to.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the key of the integration that translates an incoming webhook's payload.
        /// </summary>
        public string Integration { get; set; }

        /// <summary>
        /// Gets or sets the shared secret: the signing key of an outgoing webhook, the token of an
        /// incoming one.
        /// </summary>
        [JsonIgnore]
        [AuditRedacted]
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets whether the webhook is used.
        /// </summary>
        public WebhookState State { get; set; }

        /// <summary>
        /// Gets or sets when the webhook was last used, or <c>null</c>.
        /// </summary>
        public DateTime? LastDelivery { get; set; }

        /// <summary>
        /// Gets or sets the outcome of the last use: the status code of an outgoing call, the
        /// number of events an incoming payload raised.
        /// </summary>
        public string LastResult { get; set; }

        /// <summary>
        /// Gets or sets the date and time the webhook was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time the webhook was updated.
        /// </summary>
        public DateTime Updated { get; set; }
    }

    /// <summary>
    /// Which way a <see cref="Webhook"/> goes. Persisted as the ordinal.
    /// </summary>
    public enum WebhookDirection
    {
        /// <summary>The platform calls another system.</summary>
        Outgoing,

        /// <summary>Another system calls the platform.</summary>
        Incoming
    }

    /// <summary>
    /// Whether a <see cref="Webhook"/> is used. Persisted as the ordinal.
    /// </summary>
    public enum WebhookState
    {
        /// <summary>The webhook is used.</summary>
        Active,

        /// <summary>The webhook is kept but neither called nor accepted.</summary>
        Disabled
    }
}
