using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a board-owned swimlane of a <see cref="KanbanBoard"/>. The swimlane's display
    /// name is owned by the board and edited independently of the linked <see cref="Class"/>
    /// (renaming a board swimlane never renames the shared class). The optional
    /// <see cref="ClassId"/> is what places cards: a card whose class matches this swimlane's
    /// class lands here.
    /// </summary>
    public class KanbanBoardSwimlane : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the board swimlane.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the board-owned display name of the swimlane.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional WQL filter of the swimlane, submitted through the swimlane
        /// settings dialog. It is echoed back so the dialog can seed its filter field, but is not
        /// yet applied to narrow the swimlane's cards (no WQL query engine is wired up).
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the zero-based position of the swimlane within its board. The value is
        /// authored by the client when swimlanes are reordered and defines the render order.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the id of the class that places cards into this swimlane. Null when the
        /// swimlane was added beyond the set of available classes (every class of the workspace
        /// is already represented on the board) and therefore never receives a card automatically.
        /// </summary>
        public Guid? ClassId { get; set; }

        /// <summary>
        /// Gets or sets the transient client key of a swimlane that was added on the board but not
        /// yet reloaded. The client keeps identifying a freshly added swimlane by this non-GUID
        /// key across saves (the update response carries no ids), so it is persisted to correlate
        /// the same swimlane between saves within a session. It is null for swimlanes the client
        /// addresses by their business id (everything after a reload).
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the board that contains this swimlane.
        /// </summary>
        public Guid BoardId { get; set; }

        /// <summary>
        /// Gets or sets the board that contains this swimlane.
        /// </summary>
        [JsonIgnore]
        public KanbanBoard Board { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public KanbanBoardSwimlane()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the board swimlane.</param>
        public KanbanBoardSwimlane(Guid id)
        {
            Id = id;
        }
    }
}
