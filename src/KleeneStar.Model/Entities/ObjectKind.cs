namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Provides the well-known kind keys that partition <see cref="Object"/> entities into
    /// subtypes. The kind of an object decides which overview view presents it: documents
    /// form a hierarchical page tree, blog posts a chronological timeline, and issues a
    /// filterable work-item list.
    /// </summary>
    /// <remarks>
    /// The kind is deliberately persisted as a free string key instead of an enum so the
    /// set of kinds stays open: add-ons may introduce further keys (and register a matching
    /// descriptor in the core's object-kind catalog) without touching the data layer or
    /// shifting persisted ordinals. Keys are compared case-insensitively and stored
    /// lower-case; <see cref="Normalize"/> maps null, empty, or whitespace keys to
    /// <see cref="Default"/>.
    /// </remarks>
    public static class ObjectKind
    {
        /// <summary>
        /// The kind key of document objects (hierarchical pages organized as a tree).
        /// </summary>
        public const string Document = "document";

        /// <summary>
        /// The kind key of blog-post objects (chronological entries).
        /// </summary>
        public const string Blog = "blog";

        /// <summary>
        /// The kind key of issue objects (work items such as incidents or tasks).
        /// </summary>
        public const string Issue = "issue";

        /// <summary>
        /// The kind assigned when none is specified. Issues are the default because every
        /// object predating the kind partition behaves like a work item.
        /// </summary>
        public const string Default = Issue;

        /// <summary>
        /// Normalizes a kind key for persistence: trims and lower-cases the supplied key
        /// and falls back to <see cref="Default"/> when the key is null, empty, or
        /// whitespace. Unknown keys pass through unchanged so add-on kinds survive.
        /// </summary>
        /// <param name="kind">The raw kind key to normalize. May be null.</param>
        /// <returns>The normalized kind key. Never null or empty.</returns>
        public static string Normalize(string kind)
        {
            return string.IsNullOrWhiteSpace(kind)
                ? Default
                : kind.Trim().ToLowerInvariant();
        }
    }
}
