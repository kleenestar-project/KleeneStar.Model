using KleeneStar.Model.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a saved search — a named, reusable query over the object model that its
    /// owner can star, run, manage and share with groups through its permission dialog. Saved searches back the global
    /// "search over all workspaces" experience: the navigation dropdown lists the most
    /// recently run ones, and the search page sidebar lists all of them (starred first).
    /// </summary>
    public class SavedSearch : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the saved search.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the saved search (shown in the dropdown and sidebar).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional long description of the saved search - the prose editor's
        /// versioned document (<c>{"version":1,"doc":…}</c>), so a plain-text surface prints
        /// it through <c>ProseText</c>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the query expression that is run when the saved search is invoked.
        /// The value is a WQL statement evaluated against the object index (the same syntax
        /// the global object search accepts).
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the column layout the results table shows this saved search with - the
        /// order, visibility and width of each column, as the JSON the per-user table layouts are
        /// stored in. Null for a saved search that never had one; the table then shows the
        /// reader's own layout.
        /// </summary>
        public string Columns { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the identity that owns this saved search. The owner
        /// may do everything with it; anybody else sees it only once its permission dialog
        /// shares it (scope <c>savedsearch</c>) - a saved search nobody shared is private.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the owning identity.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Identity Owner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the saved search is starred (pinned to
        /// the top of the sidebar) by its owner.
        /// </summary>
        /// <remarks>
        /// The converter is what lets the switch of the edit dialog save at all: a ticked
        /// checkbox submits <c>"on"</c>, which the default binding drops silently.
        /// </remarks>
        [RestConverter<RestValueConverterBool>]
        public bool Starred { get; set; }

        /// <summary>
        /// Gets or sets the date and time the saved search was last run. Drives the
        /// "recently used" ordering in the navigation dropdown.
        /// </summary>
        public DateTime LastUsed { get; set; }

        /// <summary>
        /// Gets or sets the current state of the saved search.
        /// </summary>
        public SavedSearchState State { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public SavedSearch()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the saved search.</param>
        public SavedSearch(Guid id)
        {
            Id = id;
        }
    }
}
