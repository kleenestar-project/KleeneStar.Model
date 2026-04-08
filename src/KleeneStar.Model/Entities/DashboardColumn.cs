using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a column within a dashboard, used to arrange widgets in a structured layout.
    /// </summary>
    public class DashboardColumn : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the dashboard column.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the dashboard column.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional size of the dashboard column (e.g. "small", "medium", "large").
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the dashboard that contains this column.
        /// </summary>
        public Guid DashboardId { get; set; }

        /// <summary>
        /// Gets or sets the dashboard that contains this column.
        /// </summary>
        [JsonIgnore]
        public Dashboard Dashboard { get; set; }

        /// <summary>
        /// Returns the collection of widgets arranged in this column.
        /// </summary>
        public List<Widget> Widgets { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public DashboardColumn()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the dashboard column.
        /// </param>
        public DashboardColumn(Guid id)
        {
            Id = id;
        }
    }
}
