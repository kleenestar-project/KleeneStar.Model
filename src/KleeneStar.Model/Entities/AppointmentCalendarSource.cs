using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// A further layer of the calendar of a workspace: the appointments of another workspace,
    /// or the objects a WQL query selects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A source is a view, not a copy. Its entries are read when the calendar is shown, through
    /// the object manager and therefore with the reader's content visibility and security
    /// levels: including a workspace shows each reader only what that reader may see of it.
    /// </para>
    /// <para>
    /// <see cref="Kind"/> is <c>workspace</c> (the appointments of
    /// <see cref="SourceWorkspaceId"/>) or <c>query</c> (the objects <see cref="Query"/>
    /// selects, placed by their schedule if they are appointments and by the start and end
    /// date fields of their class otherwise).
    /// </para>
    /// </remarks>
    public class AppointmentCalendarSource : IEntity
    {
        /// <summary>
        /// The kind of a source showing the appointments of another workspace.
        /// </summary>
        public const string WorkspaceKind = "workspace";

        /// <summary>
        /// The kind of a source showing the objects of a WQL query.
        /// </summary>
        public const string QueryKind = "query";

        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the source.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the workspace whose calendar shows the source.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the kind of the source, <see cref="WorkspaceKind"/> or
        /// <see cref="QueryKind"/>.
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the workspace whose appointments are shown, for a workspace source.
        /// </summary>
        public Guid? SourceWorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the WQL query selecting the objects shown, for a query source.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the colour of the layer's entries (a CSS colour such as <c>#2ec4b6</c>).
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets whether the layer is shown.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the position of the layer among the sources of the workspace.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets who included the source, or <see langword="null"/>.
        /// </summary>
        public Guid? CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last change.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AppointmentCalendarSource()
        {
            Id = Guid.NewGuid();
        }
    }
}
