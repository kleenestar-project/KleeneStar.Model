using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the lifecycle state of a <see cref="Comment"/>.
    /// </summary>
    public enum CommentState
    {
        /// <summary>
        /// The comment is visible and editable by its author.
        /// </summary>
        Active,

        /// <summary>
        /// The comment is visible but its content has been edited at least once after
        /// the initial post; the UI renders an "(edited)" marker.
        /// </summary>
        Edited,

        /// <summary>
        /// The comment has been soft-deleted by its author or a moderator. The row is
        /// kept for thread continuity (replies still resolve), but the content is no
        /// longer shown.
        /// </summary>
        Deleted,

        /// <summary>
        /// The comment has been hidden by a moderator pending review.
        /// </summary>
        Hidden
    }

    /// <summary>
    /// Provides extension methods for the <see cref="CommentState"/> enumeration.
    /// </summary>
    public static class CommentStateExtensions
    {
        /// <summary>
        /// Determines whether the supplied state corresponds to a comment that is
        /// currently visible (active or edited).
        /// </summary>
        /// <param name="state">The comment state.</param>
        /// <returns><c>true</c> when the comment is visible; <c>false</c> otherwise.</returns>
        public static bool IsVisible(this CommentState state)
        {
            return state == CommentState.Active || state == CommentState.Edited;
        }

        /// <summary>
        /// Returns a stable unique identifier for the specified state, used by the
        /// REST selection control and persisted in the URI / form payload.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The state's GUID.</returns>
        public static Guid Id(this CommentState state)
        {
            return state switch
            {
                CommentState.Active => Guid.Parse("D2B1F5A0-9C3E-4F7B-8A1D-1F2E3A4B5C6D"),
                CommentState.Edited => Guid.Parse("E3C2F6B1-AD4F-508C-9B2E-2F3F4A5B6C7E"),
                CommentState.Deleted => Guid.Parse("F4D3F7C2-BE5F-619D-AC3F-3F4F5A6B7C8F"),
                CommentState.Hidden => Guid.Parse("050E408D-CF6F-71AE-BD4F-4F5F6A7B8C9F"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the localized text key for the specified state, suitable for
        /// passing to <c>I18N.Translate</c>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The translation key, or <c>null</c> for unknown values.</returns>
        public static string Text(this CommentState state)
        {
            return state switch
            {
                CommentState.Active => "kleenestar.core:comment.state.active.label",
                CommentState.Edited => "kleenestar.core:comment.state.edited.label",
                CommentState.Deleted => "kleenestar.core:comment.state.deleted.label",
                CommentState.Hidden => "kleenestar.core:comment.state.hidden.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the CSS color-selection class associated with the state, used by
        /// the REST selection control to color the chip.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The CSS class string.</returns>
        public static string Color(this CommentState state)
        {
            return state switch
            {
                CommentState.Active => TypeColorSelection.Success.ToClass(),
                CommentState.Edited => TypeColorSelection.Info.ToClass(),
                CommentState.Deleted => TypeColorSelection.Secondary.ToClass(),
                CommentState.Hidden => TypeColorSelection.Warning.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
