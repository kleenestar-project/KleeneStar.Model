using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// A person invited to an appointment and the answer they gave.
    /// </summary>
    /// <remarks>
    /// A participant is either an account of the installation (<see cref="IdentityId"/>) or a
    /// guest known only by name and mail address - an attendee of an imported event, for
    /// instance. The answer is one of the four of RFC 5545 (<c>PARTSTAT</c>): not yet answered,
    /// accepted, accepted with reservations (tentative) or declined; <see cref="Comment"/>
    /// carries the reservation or the reason in the participant's own words.
    /// <para>
    /// The identity is referenced without a foreign key: removing an account must not depend on
    /// the appointments it was invited to, and an invitation naming an account that is gone is
    /// shown as a guest under the stored name.
    /// </para>
    /// </remarks>
    public class AppointmentParticipant : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the participant.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the appointment object.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the account invited, or <see langword="null"/> for a guest.
        /// </summary>
        public Guid? IdentityId { get; set; }

        /// <summary>
        /// Gets or sets the display name (the account's at the time of the invitation, or the
        /// guest's).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the mail address, or <see langword="null"/>.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets whether the participant is needed, optional or chairs the appointment.
        /// </summary>
        public AppointmentParticipantRole Role { get; set; } = AppointmentParticipantRole.Required;

        /// <summary>
        /// Gets or sets the answer of the participant.
        /// </summary>
        public AppointmentResponse Response { get; set; } = AppointmentResponse.Pending;

        /// <summary>
        /// Gets or sets the reservation or reason the participant gave with the answer.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets when the participant answered, or <see langword="null"/>.
        /// </summary>
        public DateTime? Responded { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last change.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AppointmentParticipant()
        {
            Id = Guid.NewGuid();
        }
    }
}
