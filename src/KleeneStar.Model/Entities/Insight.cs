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
    /// Represents an insight: a user-defined view on the data of the installation. What the view
    /// is - a dashboard of widgets today, a calendar, a Gantt chart, a list, a table or a Kanban
    /// board tomorrow - is its <see cref="Type"/>; everything an insight has regardless of its
    /// type (name, description, categories, state, permissions) lives here.
    /// </summary>
    /// <remarks>
    /// Insights replaced the dashboards; every dashboard that existed carries on as an insight
    /// of the type <see cref="DashboardType"/>, and its columns and widgets are the content of
    /// that type (<see cref="Columns"/>).
    /// </remarks>
    public class Insight : IEntity
    {
        /// <summary>
        /// The key of the dashboard type - the type every insight that was a dashboard carries,
        /// and the one a create that names no type falls back to.
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
        /// Gets or sets the key of the insight type, which says what kind of view the insight
        /// is (for example <see cref="DashboardType"/>). The key is resolved against the open
        /// type catalog of the core; there is deliberately no enum, so a plugin can contribute
        /// a type without a schema change. The type is chosen when the insight is created and
        /// does not change afterwards, because the content of one type means nothing to another.
        /// </summary>
        public string Type { get; set; } = DashboardType;

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
        /// Gets or sets the columns of the insight's dashboard - the content of an insight of the
        /// type <see cref="DashboardType"/>. An insight of another type has none.
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
