using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies who a <see cref="Comment"/> is addressed to.
    /// </summary>
    /// <remarks>
    /// The audience is a property of the comment, not of the surface it was written on:
    /// the operator-side thread and the customer portal read the same rows, so a comment
    /// meant for the service team has to say so itself. The values are persisted as their
    /// ordinal, so new entries are appended rather than inserted, and
    /// <see cref="CommentVisibility.Public"/> is first so rows written before the column
    /// existed default to the wider audience they were posted with.
    /// </remarks>
    public enum CommentVisibility
    {
        /// <summary>
        /// The comment is visible to the requester, the service team, and everyone the
        /// object is shared with. This is the default for every comment.
        /// </summary>
        Public,

        /// <summary>
        /// The comment is limited to the assigned service group plus the requester;
        /// identities that merely have the object shared with them or watch it do not
        /// see it.
        /// </summary>
        InternalTeam
    }

    /// <summary>
    /// Provides extension methods for the <see cref="CommentVisibility"/> enumeration.
    /// </summary>
    public static class CommentVisibilityExtensions
    {
        /// <summary>
        /// The wire token of <see cref="CommentVisibility.Public"/>.
        /// </summary>
        public const string PublicToken = "public";

        /// <summary>
        /// The wire token of <see cref="CommentVisibility.InternalTeam"/>.
        /// </summary>
        public const string InternalTeamToken = "internal-team";

        /// <summary>
        /// Returns a stable unique identifier for the specified visibility, used by the
        /// REST selection control and persisted in the URI / form payload.
        /// </summary>
        /// <param name="visibility">The visibility.</param>
        /// <returns>The visibility's GUID.</returns>
        public static Guid Id(this CommentVisibility visibility)
        {
            return visibility switch
            {
                CommentVisibility.Public => Guid.Parse("A1F0C4B9-2D6E-4C81-9F35-6B0A7D2E4C10"),
                CommentVisibility.InternalTeam => Guid.Parse("B2E1D5CA-3E7F-4D92-A046-7C1B8E3F5D21"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the wire token the REST APIs exchange the visibility as
        /// (<c>public</c> / <c>internal-team</c>).
        /// </summary>
        /// <param name="visibility">The visibility.</param>
        /// <returns>The wire token.</returns>
        public static string Token(this CommentVisibility visibility)
        {
            return visibility switch
            {
                CommentVisibility.InternalTeam => InternalTeamToken,
                _ => PublicToken
            };
        }

        /// <summary>
        /// Returns the localized text key for the specified visibility, suitable for
        /// passing to <c>I18N.Translate</c>.
        /// </summary>
        /// <param name="visibility">The visibility.</param>
        /// <returns>The translation key, or <c>null</c> for unknown values.</returns>
        public static string Text(this CommentVisibility visibility)
        {
            return visibility switch
            {
                CommentVisibility.Public => "kleenestar.core:comment.visibility.public.label",
                CommentVisibility.InternalTeam => "kleenestar.core:comment.visibility.internal-team.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the CSS color-selection class associated with the visibility, used by
        /// the REST selection control to color the chip.
        /// </summary>
        /// <param name="visibility">The visibility.</param>
        /// <returns>The CSS class string.</returns>
        public static string Color(this CommentVisibility visibility)
        {
            return visibility switch
            {
                CommentVisibility.Public => TypeColorSelection.Success.ToClass(),
                CommentVisibility.InternalTeam => TypeColorSelection.Warning.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }

        /// <summary>
        /// Parses a wire token into the matching visibility. An unknown, empty or
        /// <c>null</c> token reads as <see cref="CommentVisibility.Public"/>, so a client
        /// that omits the field never silently narrows the audience of its comment.
        /// </summary>
        /// <param name="token">The wire token (<c>public</c> or <c>internal-team</c>).</param>
        /// <returns>The parsed visibility.</returns>
        public static CommentVisibility Parse(string token)
        {
            return string.Equals(token?.Trim(), InternalTeamToken, StringComparison.OrdinalIgnoreCase)
                ? CommentVisibility.InternalTeam
                : CommentVisibility.Public;
        }

        /// <summary>
        /// Determines whether a token names a visibility this model knows. Used by the
        /// REST endpoints to reject a malformed value instead of quietly widening it to
        /// <see cref="CommentVisibility.Public"/>.
        /// </summary>
        /// <param name="token">The wire token to check.</param>
        /// <returns><c>true</c> when the token is known, or is <c>null</c>/empty (the
        /// caller then gets the <see cref="CommentVisibility.Public"/> default).</returns>
        public static bool IsKnownToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return true;
            }

            var trimmed = token.Trim();

            return string.Equals(trimmed, PublicToken, StringComparison.OrdinalIgnoreCase)
                || string.Equals(trimmed, InternalTeamToken, StringComparison.OrdinalIgnoreCase);
        }
    }
}
