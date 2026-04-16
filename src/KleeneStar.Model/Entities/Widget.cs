using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a widget that belongs to a dashboard and provides a dynamic data view
    /// via a WQL (Widget Query Language) query.
    /// </summary>
    public class Widget : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the widget.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the widget.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the WQL (Widget Query Language) query used to dynamically bind data sources.
        /// </summary>
        public string Wql { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the dashboard column that contains this widget.
        /// </summary>
        public Guid ColumnId { get; set; }

        /// <summary>
        /// Gets or sets the dashboard column that contains this widget.
        /// </summary>
        [JsonIgnore]
        public DashboardColumn Column { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Widget()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the widget.
        /// </param>
        public Widget(Guid id)
        {
            Id = id;
        }
    }
}
