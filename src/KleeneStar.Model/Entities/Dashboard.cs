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
    /// Represents a dashboard, which is a self-contained unit that aggregates modular widgets
    /// for visualizing and controlling modeled content.
    /// </summary>
    public class Dashboard : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the dashboard.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the dashboard.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this dashboard.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the collection of category names associated with the dashboard.
        /// </summary>
        [RestConverter<CategoryConverter>()]
        public List<Category> Categories { get; set; } = [];

        /// <summary>
        /// Gets or sets the description of the dashboard.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current state of the dashboard.
        /// </summary>
        public DashboardState State { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the collection of columns associated with this dashboard.
        /// </summary>
        public List<DashboardColumn> Columns { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Dashboard()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the dashboard.
        /// </param>
        public Dashboard(Guid id)
        {
            Id = id;
        }
    }
}
