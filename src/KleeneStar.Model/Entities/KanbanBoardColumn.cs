using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a board-owned column of a <see cref="KanbanBoard"/>. The column's display
    /// name and accent color are owned by the board and edited independently of the linked
    /// <see cref="StatusCategory"/> (renaming or recoloring a board column never rewrites the
    /// shared category row). The optional <see cref="CategoryId"/> is what places cards: a
    /// card whose resolved status category matches this column's category lands here.
    /// </summary>
    public class KanbanBoardColumn : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the board column.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the board-owned display name of the column.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional accent color of the column (a CSS color such as "#3273A3"),
        /// chosen through the column "…" menu. A null value falls back to the linked category's
        /// system color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the zero-based position of the column within its board. The value is
        /// authored by the client when columns are reordered and defines the render order.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the id of the workflow status category that places cards into this
        /// column. Null when the column was added beyond the set of available categories (every
        /// category is already represented on the board) and therefore never receives a
        /// card automatically.
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the transient client key of a column that was added on the board but not
        /// yet reloaded. The client keeps identifying a freshly added column by this non-GUID key
        /// across saves (the update response carries no ids), so it is persisted to correlate the
        /// same column between a board update and a later column update within a session. It is
        /// null for columns the client addresses by their business id (everything after a reload).
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the board that contains this column.
        /// </summary>
        public Guid BoardId { get; set; }

        /// <summary>
        /// Gets or sets the board that contains this column.
        /// </summary>
        [JsonIgnore]
        public KanbanBoard Board { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public KanbanBoardColumn()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the board column.</param>
        public KanbanBoardColumn(Guid id)
        {
            Id = id;
        }
    }
}
