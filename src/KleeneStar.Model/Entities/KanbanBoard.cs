using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the persisted layout configuration of a workspace's Kanban board for a
    /// given object kind (e.g. issue, asset): the board-owned column and swimlane lists and
    /// the board-level WQL filter. There is at most one board per workspace/kind pair. As
    /// long as no column or swimlane has been added through the board, the board carries none
    /// and the REST endpoint falls back to computing the default layout (one column per
    /// workflow status category, one swimlane per populated class) dynamically.
    /// </summary>
    public class KanbanBoard : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the board.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the workspace the board belongs to.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the persisted object kind the board is scoped to (e.g. "issue", "asset").
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the board-level WQL filter submitted through the board settings dialog. It
        /// restricts which cards the board loads. A null value means the board is unfiltered.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the columns of the board.
        /// </summary>
        public List<KanbanBoardColumn> Columns { get; set; } = [];

        /// <summary>
        /// Gets or sets the swimlanes of the board.
        /// </summary>
        public List<KanbanBoardSwimlane> Swimlanes { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public KanbanBoard()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the board.</param>
        public KanbanBoard(Guid id)
        {
            Id = id;
        }
    }
}
