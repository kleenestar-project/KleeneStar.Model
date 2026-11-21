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
    /// Represents a quickfilter an identity defined itself: a named WQL expression that appears as
    /// a chip in the quickfilter bar of one view, next to the ones the view ships with.
    /// </summary>
    /// <remarks>
    /// The filter is bound to a view by <see cref="ViewKey"/> and, for views that exist once per
    /// workspace or class, narrowed further by <see cref="ContextKey"/>. Keeping the two apart lets
    /// a global view store a filter without a context while a scoped view keeps one set of filters
    /// per workspace, rather than showing every workspace's filters everywhere.
    /// </remarks>
    public class CustomQuickfilter : IEntity
    {
        /// <summary>
        /// The prefix that marks a quickfilter id as belonging to a stored filter.
        /// </summary>
        /// <remarks>
        /// The quickfilter bar identifies every chip by a single string, so the id has to say on
        /// its own whether the view is meant to interpret it or to look it up here. The views'
        /// own chips use the plain <c>qf_</c> prefix.
        /// </remarks>
        public const string IdPrefix = "qf_custom_";

        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the quickfilter.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the label shown on the chip.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the view the filter belongs to, for example <c>tenants</c>. A filter is
        /// only offered in the bar of the view it was created in.
        /// </summary>
        public string ViewKey { get; set; }

        /// <summary>
        /// Gets or sets the context that narrows the view further, for example the workspace key of
        /// a per-workspace list. Null for views that exist only once.
        /// </summary>
        public string ContextKey { get; set; }

        /// <summary>
        /// Gets or sets the WQL expression evaluated when the chip is active. It is the same syntax
        /// the view's advanced query accepts, so a filter can be tried out in the search bar before
        /// it is stored.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the identity that created the filter.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the owning identity.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Identity Owner { get; set; }

        /// <summary>
        /// Gets or sets whether the filter is offered to everyone rather than to its owner alone.
        /// </summary>
        /// <remarks>
        /// The converter is what makes the switch on the form take effect; without it the value a
        /// checkbox submits never reaches this property. See <see cref="RestValueConverterBool"/>.
        /// </remarks>
        [RestConverter<RestValueConverterBool>]
        public bool Shared { get; set; }

        /// <summary>
        /// Gets or sets the position of the chip among the stored filters of the same view. Lower
        /// values are offered first.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets the quickfilter id under which the chip is offered and reported back.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public string FilterId => IdPrefix + Id.ToString();

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public CustomQuickfilter()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the quickfilter.</param>
        public CustomQuickfilter(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Extracts the stored filter's id from a quickfilter id.
        /// </summary>
        /// <param name="filterId">The quickfilter id reported by the bar.</param>
        /// <returns>
        /// The id of the stored filter, or null when the id does not denote one.
        /// </returns>
        public static Guid? ParseFilterId(string filterId)
        {
            if (string.IsNullOrEmpty(filterId) || !filterId.StartsWith(IdPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return Guid.TryParse(filterId[IdPrefix.Length..], out var id) ? id : null;
        }
    }
}
