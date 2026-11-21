using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the lifecycle state of a personal access token.
    /// </summary>
    public enum AccessTokenState
    {
        /// <summary>
        /// The token authenticates API requests.
        /// </summary>
        Active,

        /// <summary>
        /// The token has passed its expiry date and no longer authenticates requests. Derived
        /// from <see cref="AccessToken.Expires"/> rather than stored, so a token becomes expired
        /// without anybody having to write to the record.
        /// </summary>
        Expired,

        /// <summary>
        /// The token was revoked by its owner and no longer authenticates requests.
        /// </summary>
        Revoked
    }

    /// <summary>
    /// Provides extension methods for working with values of the <see cref="AccessTokenState"/>
    /// enumeration.
    /// </summary>
    public static class AccessTokenStateExtensions
    {
        /// <summary>
        /// Determines whether a token in the specified state still authenticates requests.
        /// </summary>
        /// <param name="state">The token state to check.</param>
        /// <returns><c>true</c> when the token is active; otherwise <c>false</c>.</returns>
        public static bool IsActive(this AccessTokenState state)
        {
            return state == AccessTokenState.Active;
        }

        /// <summary>
        /// Returns the stable identifier associated with the specified token state.
        /// </summary>
        /// <param name="state">The token state for which to retrieve the identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the specified token state.</returns>
        public static Guid Id(this AccessTokenState state)
        {
            return state switch
            {
                AccessTokenState.Expired => Guid.Parse("0B5E4A21-77C6-4D93-8F10-9A2C6E5B3D48"),
                AccessTokenState.Revoked => Guid.Parse("3D8F1C60-42A5-4B7E-91D3-6C0B8E52F7A9"),
                _ => Guid.Parse("C41A7E93-6D28-4F50-8B7C-15E9D0A34B62")
            };
        }

        /// <summary>
        /// Returns the translation key of the textual label for the specified token state.
        /// </summary>
        /// <param name="state">The token state for which the label should be retrieved.</param>
        /// <returns>The translation key of the label.</returns>
        public static string Text(this AccessTokenState state)
        {
            return state switch
            {
                AccessTokenState.Expired => "kleenestar.core:profile.tokens.state.expired",
                AccessTokenState.Revoked => "kleenestar.core:profile.tokens.state.revoked",
                _ => "kleenestar.core:profile.tokens.state.active"
            };
        }

        /// <summary>
        /// Returns the color class associated with the specified token state.
        /// </summary>
        /// <param name="state">The token state for which to retrieve the color.</param>
        /// <returns>The CSS class of the color the badge is painted in.</returns>
        public static string Color(this AccessTokenState state)
        {
            return state switch
            {
                AccessTokenState.Expired => TypeColorSelection.Warning.ToClass(),
                AccessTokenState.Revoked => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Success.ToClass()
            };
        }
    }
}
