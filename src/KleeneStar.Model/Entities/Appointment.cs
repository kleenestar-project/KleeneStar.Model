using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// The schedule of an object of the appointment kind: when it takes place, where, and how
    /// it repeats. One row per appointment object (<see cref="ObjectId"/> is unique).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The appointment itself is an ordinary object - its title, description, class, workflow,
    /// comments, attachments and history are the object's. This row only carries what an object
    /// has no column for: the span in time, the place and the rule a series repeats by. It is
    /// kept by the calendar plugin (<c>KleeneStar.Calendar</c>); without the plugin it is
    /// simply not read, like the automation tables.
    /// </para>
    /// <para>
    /// <see cref="Start"/> and <see cref="End"/> are instants in UTC. A series repeats in the
    /// local time of <see cref="TimeZone"/> - a weekly meeting at ten stays at ten across a
    /// change to daylight saving time - so the rule is expanded in that zone and converted back.
    /// An all-day appointment covers whole days: its start is the first day at midnight and its
    /// end the midnight after the last day, both in that zone.
    /// </para>
    /// </remarks>
    public class Appointment : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the schedule row.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the appointment object the schedule belongs to.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the start of the (first) occurrence, in UTC.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets the end of the (first) occurrence, in UTC. Never before <see cref="Start"/>.
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets whether the appointment covers whole days rather than a time span.
        /// </summary>
        public bool AllDay { get; set; }

        /// <summary>
        /// Gets or sets the time zone the appointment was planned in (a .NET or IANA id). The
        /// recurrence is expanded in it and the forms show the times in it.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets where the appointment takes place, or <see langword="null"/>.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the recurrence rule of a series in the notation of RFC 5545
        /// (<c>FREQ=WEEKLY;BYDAY=MO,WE;COUNT=10</c>), or <see langword="null"/> for a single
        /// appointment.
        /// </summary>
        public string Recurrence { get; set; }

        /// <summary>
        /// Gets or sets the occurrences removed from a series: their starts in UTC, written as
        /// <c>yyyyMMddTHHmmssZ</c> and separated by semicolons; <see langword="null"/> for none.
        /// </summary>
        public string Exceptions { get; set; }

        /// <summary>
        /// Gets or sets the end of the last occurrence in UTC - the end for a single appointment,
        /// the end of the last occurrence for a bounded series and <see langword="null"/> for a
        /// series without end. Kept so that a period can be narrowed in the store rather than by
        /// expanding every series.
        /// </summary>
        public DateTime? SeriesEnd { get; set; }

        /// <summary>
        /// Gets or sets whether the appointment is moved into a closing state of its workflow
        /// once its last occurrence is over.
        /// </summary>
        public bool AutoClose { get; set; } = true;

        /// <summary>
        /// Gets or sets when the appointment was closed because its time had run out, or
        /// <see langword="null"/>.
        /// </summary>
        public DateTime? Closed { get; set; }

        /// <summary>
        /// Gets or sets the identity organizing the appointment, or <see langword="null"/>.
        /// </summary>
        public Guid? OrganizerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the appointment in an external calendar (the
        /// <c>UID</c> of an imported event), so that importing the same file again updates
        /// rather than duplicates.
        /// </summary>
        public string ExternalId { get; set; }

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
        public Appointment()
        {
            Id = Guid.NewGuid();
        }
    }
}
