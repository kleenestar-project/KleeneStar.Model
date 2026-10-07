using KleeneStar.Model.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents an insight: a user-defined view on the data of the installation. An insight
    /// is a set of objects - the ones its <see cref="Query"/> selects, across workspaces - shown
    /// through tabs (<see cref="InsightView"/>) the way a workspace overview shows its issues:
    /// tables and lists, a dashboard, Kanban, Scrum, Gantt, calendar and reports.
    /// </summary>
    /// <remarks>
    /// Insights replaced the dashboards; every dashboard that existed carries on as an insight
    /// with one dashboard tab. Each column identifies the tab whose independent board it belongs to.
    /// </remarks>
    public class Insight : IEntity
    {
        /// <summary>
        /// The key of the dashboard view type - the type the tab of every insight that was a
        /// dashboard carries.
        /// </summary>
        public const string DashboardType = "dashboard";

        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the insight.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the insight.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type the insight was created as before an insight hosted tabs. It is
        /// kept only as a record of that: the type moved to the tabs
        /// (<see cref="InsightView.ViewType"/>), the migration that introduced them turned every
        /// insight into one with a tab of this type, and nothing reads the column since.
        /// </summary>
        public string Type { get; set; } = DashboardType;

        /// <summary>
        /// Gets or sets the WQL expression that selects the objects the insight is about, across
        /// workspaces - <c>Workspace.Key = "SD" and Kind = "issue"</c>, say. Every tab of the
        /// insight shows these objects, narrowed as every read is to what the reader may see.
        /// Blank selects every object the reader may see.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this insight.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the collection of category names associated with the insight.
        /// </summary>
        [RestConverter<CategoryConverter>()]
        public List<Category> Categories { get; set; } = [];

        /// <summary>
        /// Gets or sets the description of the insight.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current state of the insight.
        /// </summary>
        public InsightState State { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the columns of all dashboard tabs, distinguished by their owning tab identifier.
        /// </summary>
        public List<DashboardColumn> Columns { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Insight()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the insight.
        /// </param>
        public Insight(Guid id)
        {
            Id = id;
        }
    }
}
