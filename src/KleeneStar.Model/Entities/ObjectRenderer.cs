namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Provides the well-known renderer keys that decide <em>how</em> the objects of a
    /// <see cref="Class"/> are read and written, as opposed to <see cref="ObjectKind"/>,
    /// which decides <em>where</em> they appear. A document rendered as prose is written in
    /// the WYSIWYG editor and read as an article; the same document rendered as a form is
    /// written through the structured input mask of its class and read as an unchangeable
    /// view of the captured values.
    /// </summary>
    /// <remarks>
    /// The renderer is deliberately persisted as a free string key instead of an enum, for
    /// the same reason the kind is: the set stays open, so a plugin may contribute a further
    /// renderer without touching the data layer or shifting persisted ordinals. Keys are
    /// compared case-insensitively and stored lower-case.
    /// <para>
    /// Unlike <see cref="ObjectKind.Normalize"/>, <see cref="Normalize"/> maps an unset key
    /// to <see langword="null"/> rather than to a default. There is no renderer that is
    /// right for every kind - prose is the default of a document, a mask is the default of
    /// an issue - so "not set" is its own state, and the kind descriptor is asked what it
    /// means (see <c>IObjectKind.DefaultRenderer</c> in the core). A class that names no
    /// renderer therefore follows its kind, including when the kind is changed later.
    /// </para>
    /// </remarks>
    public static class ObjectRenderer
    {
        /// <summary>
        /// The renderer key of the rich-text surface: the WYSIWYG editor writes it and the
        /// reading view presents it as a page of prose. The default renderer of the
        /// document and blog kinds.
        /// </summary>
        public const string Prose = "prose";

        /// <summary>
        /// The renderer key of the structured surface: the input mask the class's
        /// <see cref="FormType.Edit"/> form describes writes it, and the reading view
        /// presents the captured values as an unchangeable view of the class's
        /// <see cref="FormType.View"/> form. The default renderer of the issue and asset
        /// kinds.
        /// </summary>
        public const string Form = "form";

        /// <summary>
        /// Normalizes a renderer key for persistence: trims and lower-cases the supplied
        /// key, and maps null, empty, or whitespace to <see langword="null"/> - the state
        /// "follow the kind". Unknown keys pass through unchanged so the renderer of an
        /// uninstalled add-on survives in the data.
        /// </summary>
        /// <param name="renderer">The raw renderer key to normalize. May be null.</param>
        /// <returns>The normalized renderer key, or <see langword="null"/> when none is set.</returns>
        public static string Normalize(string renderer)
        {
            return string.IsNullOrWhiteSpace(renderer)
                ? null
                : renderer.Trim().ToLowerInvariant();
        }
    }
}
