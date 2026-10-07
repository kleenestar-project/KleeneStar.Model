using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the persisted layout configuration of a workspace's object-kind dashboard
    /// (e.g. the KPI overview of the issue or asset tab): the board-owned column and widget
    /// list. There is at most one board per workspace, kind and tab. Until a board is persisted, the REST endpoint computes
    /// the default layout (the total/active/archived KPI tiles) dynamically. A persisted empty board remains empty.
    /// </summary>
    public class KindDashboard : IEntity
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
        /// Gets or sets the owning tab identifier, or an empty identifier for the legacy board.
        /// </summary>
        public Guid ViewId { get; set; }

        /// <summary>
        /// Gets or sets the workspace the board belongs to.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the persisted object kind the board is scoped to (e.g. "issue", "asset").
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the columns of the board.
        /// </summary>
        public List<KindDashboardColumn> Columns { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public KindDashboard()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the board.</param>
        public KindDashboard(Guid id)
        {
            Id = id;
        }
    }
}
