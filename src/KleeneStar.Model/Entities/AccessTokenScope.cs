using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Describes one of the scopes a personal access token can grant: the name stored in
    /// <see cref="AccessToken.Scopes"/>, the stable id a selection control round-trips it by,
    /// and the translation key of the explanation shown next to it.
    /// </summary>
    /// <param name="Name">The scope as it appears in the token record ("read:tickets").</param>
    /// <param name="Id">The stable identifier of the entry in a selection control.</param>
    /// <param name="Description">The translation key of the explanation.</param>
    public sealed record AccessTokenScope(string Name, Guid Id, string Description)
    {
        /// <summary>
        /// Gets the scopes a token can be granted, in the order the create form offers them:
        /// reading before writing, administration last.
        /// </summary>
        public static IReadOnlyList<AccessTokenScope> All { get; } =
        [
            new("read:tickets", Guid.Parse("B3E70D19-4A85-42C6-90F1-7D5C82A0E43B"), "kleenestar.core:profile.tokens.scope.readtickets"),
            new("write:tickets", Guid.Parse("6F1C84A0-D253-4E97-B84D-0A9E37C6152F"), "kleenestar.core:profile.tokens.scope.writetickets"),
            new("read:workflows", Guid.Parse("D825B639-0C7E-41A4-95F8-3B6017DA84C5"), "kleenestar.core:profile.tokens.scope.readworkflows"),
            new("write:workflows", Guid.Parse("41A9E5C8-72B0-4D36-8E17-C05B9F2A63D4"), "kleenestar.core:profile.tokens.scope.writeworkflows"),
            new("read:forms", Guid.Parse("9C0B4D71-58E6-42AF-B3D9-6E71A50C82B3"), "kleenestar.core:profile.tokens.scope.readforms"),
            new("write:forms", Guid.Parse("2A76F3E0-B14C-4590-8D27-51C8E0B47A96"), "kleenestar.core:profile.tokens.scope.writeforms"),
            new("admin:tenant", Guid.Parse("7E304B85-9AD1-4C62-B07F-E4823D91506C"), "kleenestar.core:profile.tokens.scope.admintenant")
        ];

        /// <summary>
        /// Returns the scope with the given name, or <see langword="null"/> when the name
        /// denotes none of the known scopes.
        /// </summary>
        /// <param name="name">The scope name to look up. Case is ignored.</param>
        /// <returns>The matching scope, or <see langword="null"/>.</returns>
        public static AccessTokenScope FromName(string name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? null
                : All.FirstOrDefault(x => string.Equals(x.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns the scope with the given selection id, or <see langword="null"/> when the id
        /// denotes none of the known scopes.
        /// </summary>
        /// <param name="id">The selection id to look up.</param>
        /// <returns>The matching scope, or <see langword="null"/>.</returns>
        public static AccessTokenScope FromId(Guid id)
        {
            return All.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Splits the scope list of a token into its individual scope names.
        /// </summary>
        /// <param name="scopes">The scope list as stored on the token.</param>
        /// <returns>The individual scope names (possibly empty).</returns>
        public static IEnumerable<string> Split(string scopes)
        {
            return string.IsNullOrWhiteSpace(scopes)
                ? []
                : scopes.Split([' ', ',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }
}
