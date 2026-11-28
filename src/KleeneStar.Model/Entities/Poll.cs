using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// The settings of an object of the poll kind: what kind of poll it is, how it is answered,
    /// until when, and how it ended. One row per poll object (<see cref="ObjectId"/> is unique).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The poll itself is an ordinary object - its question (the summary), description, class,
    /// workflow, comments, attachments and history are the object's. This row only carries what
    /// an object has no column for. It is kept by the poll plugin (<c>KleeneStar.Poll</c>);
    /// without the plugin it is simply not read, like the appointment tables.
    /// </para>
    /// <para>
    /// <see cref="Type"/> is a free key of the plugin's catalog of poll types (<c>schedule</c>,
    /// <c>decision</c>, …) rather than an enum, so further types need no change of the model.
    /// </para>
    /// </remarks>
    public class Poll : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the poll row.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the poll object the row belongs to.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the key of the poll type (<c>schedule</c> for finding a date,
        /// <c>decision</c> for choosing among options).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets whether a voter may choose several options. A scheduling poll always
        /// answers every option.
        /// </summary>
        public bool MultipleChoice { get; set; }

        /// <summary>
        /// Gets or sets whether a voter may answer "if need be" besides yes and no.
        /// </summary>
        public bool AllowMaybe { get; set; }

        /// <summary>
        /// Gets or sets whether the results show counts only, never who voted what.
        /// </summary>
        public bool Anonymous { get; set; }

        /// <summary>
        /// Gets or sets whether the results stay hidden from voters until the poll is closed.
        /// Whoever may change the poll sees them throughout.
        /// </summary>
        public bool HideResults { get; set; }

        /// <summary>
        /// Gets or sets when voting ends (UTC), or <see langword="null"/> for a poll that runs
        /// until it is closed by hand.
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        /// Gets or sets when the poll was closed, or <see langword="null"/> while it runs.
        /// </summary>
        public DateTime? Closed { get; set; }

        /// <summary>
        /// Gets or sets who closed the poll, or <see langword="null"/> for the system (the
        /// deadline ran out) or a poll that runs.
        /// </summary>
        public Guid? ClosedById { get; set; }

        /// <summary>
        /// Gets or sets the option the poll settled on - the date fixed or the decision taken -
        /// or <see langword="null"/>.
        /// </summary>
        public Guid? ChosenOptionId { get; set; }

        /// <summary>
        /// Gets or sets the time zone the dates of a scheduling poll were proposed in (a .NET or
        /// IANA id).
        /// </summary>
        public string TimeZone { get; set; }

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
        public Poll()
        {
            Id = Guid.NewGuid();
        }
    }
}
