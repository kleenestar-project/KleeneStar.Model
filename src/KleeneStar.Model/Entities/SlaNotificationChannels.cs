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

        // bits 2 and 4 were the Slack and SMS channels, which the product no longer offers.
        // The values are left unassigned rather than reused, so a policy stored while those
        // channels still existed does not silently turn into a different channel when it is
        // read back.

        /// <summary>
        /// Show in-app notifications.
        /// </summary>
        InApp = 8
    }
}
