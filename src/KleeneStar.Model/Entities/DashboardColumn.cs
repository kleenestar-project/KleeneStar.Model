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
        /// Gets or sets the optional size of the dashboard column (e.g., "small", "medium", "large").
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the optional accent color of the dashboard column (a CSS color such as
        /// "#3273A3" or a named color). A null value leaves the column without an accent.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the zero-based position of the column within its dashboard. The value is
        /// authored by the client when columns are reordered and defines the render order.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the transient client key of a column that was added on the board but not yet
        /// reloaded. The client keeps identifying a freshly added column by this non-GUID key across
        /// saves (the update response carries no ids), so it is persisted to correlate the same column
        /// between a board update and a later column update within a session. It is null for columns
        /// the client addresses by their business id (everything after a reload).
        /// </summary>
        public string Key { get; set; }

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
