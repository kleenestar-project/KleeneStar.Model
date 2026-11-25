namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// The answer of a participant to an invitation, after the participation status of
    /// RFC 5545 (<c>PARTSTAT</c>).
    /// </summary>
    public enum AppointmentResponse
    {
        /// <summary>
        /// Not answered yet (<c>NEEDS-ACTION</c>).
        /// </summary>
        Pending,

        /// <summary>
        /// Accepted (<c>ACCEPTED</c>).
        /// </summary>
        Accepted,

        /// <summary>
        /// Accepted with reservations (<c>TENTATIVE</c>).
        /// </summary>
        Tentative,

        /// <summary>
        /// Declined (<c>DECLINED</c>).
        /// </summary>
        Declined
    }

    /// <summary>
    /// The part a participant plays in an appointment, after the participation role of
    /// RFC 5545 (<c>ROLE</c>).
    /// </summary>
    public enum AppointmentParticipantRole
    {
        /// <summary>
        /// The appointment needs the participant (<c>REQ-PARTICIPANT</c>).
        /// </summary>
        Required,

        /// <summary>
        /// The participant is welcome but not needed (<c>OPT-PARTICIPANT</c>).
        /// </summary>
        Optional,

        /// <summary>
        /// The participant leads the appointment (<c>CHAIR</c>).
        /// </summary>
        Chair
    }
}
