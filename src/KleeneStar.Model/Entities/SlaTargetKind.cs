namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies which lifecycle milestone an <see cref="SlaTarget"/> measures.
    /// </summary>
    public enum SlaTargetKind
    {
        /// <summary>
        /// Time until the first response is recorded against the ticket.
        /// </summary>
        Response,

        /// <summary>
        /// Time until the ticket reaches a resolved state.
        /// </summary>
        Resolution,

        /// <summary>
        /// Time between mandatory status updates while the ticket is open.
        /// </summary>
        Update,

        /// <summary>
        /// Time until the change is approved by the relevant authority (CAB).
        /// </summary>
        Approval,

        /// <summary>
        /// Time until an approved change is implemented.
        /// </summary>
        Implementation,

        /// <summary>
        /// Time until the service request is fulfilled.
        /// </summary>
        Fulfillment,

        /// <summary>
        /// A non-standard milestone described in the target's name.
        /// </summary>
        Custom
    }
}
