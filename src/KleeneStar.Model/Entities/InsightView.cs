using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a tab of an insight: one way of showing the objects the insight selects - a
    /// table, a dashboard, a Kanban board, the reports and so on. It is to an insight what an
    /// <see cref="ObjectView"/> is to a workspace overview.
    /// </summary>
    /// <remarks>
    /// What a tab shows is its <see cref="ViewType"/>, a key of the open view type catalog of
    /// the core - there is deliberately no enum, so a plugin contributes a type of tab without
    /// a schema change. A tab whose type nobody registers any more is kept and simply not shown
    /// until its plugin returns.
    /// </remarks>
    public class InsightView : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the tab.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name shown on the tab.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the key of the view type the tab shows, for example <c>table</c> or
        /// <c>reports</c>. Keys are stored trimmed and lower-cased.
        /// </summary>
        public string ViewType { get; set; }

        /// <summary>
        /// Gets or sets the optional configuration of the tab, serialized as JSON - what the
        /// view type keeps per tab, such as the report the reports tab opens with.
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Gets or sets the position of the tab among the tabs of its insight.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the state of the tab.
        /// </summary>
        public ObjectViewState State { get; set; }

        /// <summary>
        /// Gets or sets the id of the insight the tab belongs to.
        /// </summary>
        public Guid InsightId { get; set; }

        /// <summary>
        /// Gets or sets the insight the tab belongs to.
        /// </summary>
        public Insight Insight { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public InsightView()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the tab.</param>
        public InsightView(Guid id)
        {
            Id = id;
        }
    }
}
