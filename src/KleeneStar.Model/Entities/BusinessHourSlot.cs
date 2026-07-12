using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the business-hour configuration for a single weekday on a <see cref="Calendar"/>.
    /// </summary>
    public class BusinessHourSlot : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the slot.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the weekday this slot represents.
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this weekday is considered a working day.
        /// When <c>false</c>, the <see cref="StartTime"/>/<see cref="EndTime"/> are ignored.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the start of the working window (inclusive), in local calendar time.
        /// </summary>
        public TimeOnly StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end of the working window (inclusive), in local calendar time.
        /// </summary>
        public TimeOnly EndTime { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the calendar this slot belongs to.
        /// </summary>
        public Guid CalendarId { get; set; }

        /// <summary>
        /// Gets or sets the calendar this slot belongs to.
        /// </summary>
        [JsonIgnore]
        public Calendar Calendar { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public BusinessHourSlot()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        public BusinessHourSlot(Guid id)
        {
            Id = id;
        }
    }
}
