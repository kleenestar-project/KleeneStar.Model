using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a single holiday entry on a <see cref="Calendar"/>. When the holiday is
    /// enabled, the SLA timer for any policy referencing the calendar pauses on the
    /// configured date.
    /// </summary>
    public class Holiday : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the holiday.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date of the holiday (calendar-local).
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Gets or sets the display name of the holiday (e.g. "Neujahr").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the holiday region tag (e.g. "DE", "DE-BW").
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the holiday is currently enforced.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the calendar this holiday belongs to.
        /// </summary>
        public Guid CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the calendar this holiday belongs to.
        /// </summary>
        [JsonIgnore]
        public Calendar Calendar { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Holiday()
        {
            Id = Guid.NewGuid();
            Enabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        public Holiday(Guid id)
        {
            Id = id;
            Enabled = true;
        }
    }
}
