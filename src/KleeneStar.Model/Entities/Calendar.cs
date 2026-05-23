using KleeneStar.Model.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a working-hours calendar that <see cref="SlaPolicy"/>s and other
    /// time-aware features can reference. A calendar belongs to a single <see cref="Class"/>
    /// and aggregates a weekly schedule of <see cref="BusinessHourSlot"/>s plus a list of
    /// <see cref="Holiday"/> entries.
    /// </summary>
    public class Calendar : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the calendar.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the calendar (e.g. "Standard · Europe/Berlin").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the free-text description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the IANA timezone identifier (e.g. "Europe/Berlin").
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the holiday region tag (e.g. "DE", "DE-BW", "US-CA"). Used to filter
        /// the relevant holiday set.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the current lifecycle state.
        /// </summary>
        [RestConverter<CalendarStateConverter>]
        public CalendarState State { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this calendar is the workspace default.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this calendar.
        /// </summary>
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the class the calendar is bound to.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the class the calendar is bound to.
        /// </summary>
        [JsonIgnore]
        public Class Class { get; set; }

        /// <summary>
        /// Gets or sets the weekly business-hour slots. Exactly seven entries are expected,
        /// one per <see cref="DayOfWeek"/>.
        /// </summary>
        [JsonIgnore]
        public List<BusinessHourSlot> BusinessHours { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of holidays on which the calendar pauses.
        /// </summary>
        [JsonIgnore]
        public List<Holiday> Holidays { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Calendar()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        public Calendar(Guid id)
        {
            Id = id;
        }
    }
}
