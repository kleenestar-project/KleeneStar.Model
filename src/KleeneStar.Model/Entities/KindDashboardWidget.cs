using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a widget that belongs to a <see cref="KindDashboardColumn"/>.
    /// </summary>
    public class KindDashboardWidget : IEntity
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
        /// Gets or sets the client registry type id of the widget (e.g. "widget_info",
        /// "widget_bignumber", "widget_kleenestar_note"). It selects the client-side render.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the optional accent color of the widget (a CSS color such as "#3273A3"
        /// or a named color). A null value leaves the widget without an accent.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the type-specific widget parameters as a JSON-serialized string
        /// dictionary. These carry the settings a user edits through the widget "…" menu so
        /// they survive the round trip to the server. A null value denotes a widget without
        /// configured parameters.
        /// </summary>
        public string Params { get; set; }

        /// <summary>
        /// Gets or sets the zero-based position of the widget within its column. The value is
        /// authored by the client when widgets are reordered and defines the render order.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the column that contains this widget.
        /// </summary>
        public Guid ColumnId { get; set; }

        /// <summary>
        /// Gets or sets the column that contains this widget.
        /// </summary>
        [JsonIgnore]
        public KindDashboardColumn Column { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public KindDashboardWidget()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the widget.</param>
        public KindDashboardWidget(Guid id)
        {
            Id = id;
        }
    }
}
