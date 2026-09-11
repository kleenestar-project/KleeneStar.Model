using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents one WQL query an identity has actually run, kept so the query prompt can
    /// offer it again.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The prompt's history used to be two hard-coded example expressions per endpoint, which
    /// is a demonstration rather than a history: it never named anything the person in front
    /// of the screen had searched for, and it went stale the moment the data it referred to
    /// moved. A history is a record of what was run, so it is recorded when a query runs -
    /// see <c>KleeneStarRestApiTable.Filter</c>, which is reached only after the statement
    /// parsed without errors.
    /// </para>
    /// <para>
    /// A row is scoped by (<see cref="OwnerId"/>, <see cref="Subject"/>). The subject is what
    /// was being queried, not which page did the querying: the prompt over the global object
    /// search, the one over an issue list and the one over an asset inventory all write WQL
    /// against the same attributes, so a query typed in one is meaningful in the others and
    /// they share a history. Two prompts over different entities do not.
    /// </para>
    /// <para>
    /// It is deliberately <b>not</b> a <see cref="SavedSearch"/>. A saved search is named,
    /// curated and managed by its owner; this is the unnamed trail behind them, capped and
    /// silently overwritten, and it must never appear in the places a saved search does.
    /// </para>
    /// </remarks>
    public class WqlHistory : IEntity
    {
        /// <summary>
        /// How many queries are kept per identity and subject.
        /// </summary>
        /// <remarks>
        /// The prompt is navigated one step at a time with PageUp/PageDown, so a history
        /// longer than this is not reachable in practice - it is only rows nobody deletes.
        /// It lives on the entity because both ends need it: the data layer trims to it and
        /// the manager offers it as the default page size.
        /// </remarks>
        public const int Limit = 20;

        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the history entry.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identity that ran the query. A history is personal: each
        /// identity only ever sees its own.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the owning identity.
        /// </summary>
        [IndexIgnore]
        [JsonIgnore]
        public Identity Owner { get; set; }

        /// <summary>
        /// Gets or sets what the query was run against - the full name of the indexed type
        /// the prompt and the executing table share (e.g.
        /// <c>KleeneStar.Model.Entities.Object</c>).
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the query as it was submitted, which is what the prompt puts back
        /// into the input when the history is stepped through.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets how often the query has been run. Re-running one moves it to the
        /// front rather than adding a second row, so the count is what records that.
        /// </summary>
        public int UseCount { get; set; }

        /// <summary>
        /// Gets or sets the date and time the query was last run. Drives both the order the
        /// history is offered in and which entry is dropped when the cap is reached.
        /// </summary>
        public DateTime LastUsed { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entry was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entry was last written.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public WqlHistory()
        {
            Id = Guid.NewGuid();
        }
    }
}
