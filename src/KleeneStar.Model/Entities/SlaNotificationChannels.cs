using System;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the channels through which SLA breach notifications are dispatched.
    /// </summary>
    [Flags]
    public enum SlaNotificationChannels
    {
        /// <summary>
        /// No notification channels are enabled.
        /// </summary>
        None = 0,

        /// <summary>
        /// Send notifications by e-mail.
        /// </summary>
        Email = 1,

        /// <summary>
        /// Send notifications to a Slack channel.
        /// </summary>
        Slack = 2,

        /// <summary>
        /// Send notifications by SMS.
        /// </summary>
        Sms = 4,

        /// <summary>
        /// Show in-app notifications.
        /// </summary>
        InApp = 8
    }
}
