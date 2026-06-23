using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a saved search — a named, reusable query over the object model that a
    /// single identity can star, run, and manage. Saved searches back the global
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
        /// Gets or sets the optional long description of the saved search.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the query expression that is run when the saved search is invoked.
        /// The value is a WQL statement evaluated against the object index (the same syntax
        /// the global object search accepts).
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the identity that owns this saved search. Saved
        /// searches are personal: each identity only sees its own.
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
