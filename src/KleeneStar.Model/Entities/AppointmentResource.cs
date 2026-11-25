using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// The booking of a resource - a room, a projector, a vehicle - for an appointment.
    /// </summary>
    /// <remarks>
    /// A resource is an object of the asset kind: the inventory the installation already keeps
    /// is what can be booked, and a booking is no more than the pair of the two objects. The
    /// resource is referenced without a foreign key, because a second cascading path into the
    /// object table is refused by some databases; a booking of a removed resource is dropped
    /// when the resource goes and ignored where it is read.
    /// </remarks>
    public class AppointmentResource : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the booking.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the appointment object.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the booked resource (an object of the asset kind).
        /// </summary>
        public Guid ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AppointmentResource()
        {
            Id = Guid.NewGuid();
        }
    }
}
