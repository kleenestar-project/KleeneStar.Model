using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Describes one of the languages the user interface is offered in: the ISO code stored on
    /// <see cref="Identity.Language"/>, the stable id a selection control round-trips it by, the
    /// label shown in that control, and the country whose flag stands for the language in the
    /// country control of the account page.
    /// </summary>
    /// <param name="Code">The ISO 639-1 code of the language ("de", "en", ...).</param>
    /// <param name="Id">The stable identifier of the entry in a selection control.</param>
    /// <param name="Label">The name of the language, written in that language.</param>
    /// <param name="Country">The ISO 3166-1 alpha-2 code of the country standing for the
    /// language ("DE", "GB", ...). One country per language, so the country control round-trips
    /// to exactly one language.</param>
    public sealed record UiLanguage(string Code, Guid Id, string Label, string Country)
    {
        /// <summary>
        /// Gets the languages the user interface is available in. The order is the one the
        /// account page offers them in; the first entry is the fallback for an identity that
        /// has not chosen a language.
        /// </summary>
        public static IReadOnlyList<UiLanguage> All { get; } =
        [
            new("de", Guid.Parse("E5C81A72-3B49-4D06-9F17-8A2D5C0B6E13"), "Deutsch", "DE"),
            new("en", Guid.Parse("7A26F0B4-9C51-4E83-B0D7-2F846A19C5E0"), "English", "GB"),
            new("fr", Guid.Parse("C0D93E58-146B-427F-8A95-E3B71D6058A2"), "Français", "FR"),
            new("es", Guid.Parse("48B7E2C1-5D30-49A6-B18E-70C25F9A3D64"), "Español", "ES"),
            new("it", Guid.Parse("9E1D6740-82AF-4C15-93B6-5A08D7E24C31"), "Italiano", "IT"),
            new("pl", Guid.Parse("21F5A8D3-6E07-4B92-A4C8-D96150B7E385"), "Polski", "PL")
        ];

        /// <summary>
        /// Returns the language with the given ISO code, or <see langword="null"/> when the code
        /// names none of the offered languages.
        /// </summary>
        /// <param name="code">The ISO 639-1 code to look up. Case is ignored.</param>
        /// <returns>The matching language, or <see langword="null"/>.</returns>
        public static UiLanguage FromCode(string code)
        {
            return string.IsNullOrWhiteSpace(code)
                ? null
                : All.FirstOrDefault(x => string.Equals(x.Code, code.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns the language with the given selection id, or <see langword="null"/> when the
        /// id names none of the offered languages.
        /// </summary>
        /// <param name="id">The selection id to look up.</param>
        /// <returns>The matching language, or <see langword="null"/>.</returns>
        public static UiLanguage FromId(Guid id)
        {
            return All.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Returns the language the given country stands for, or <see langword="null"/> when the
        /// country stands for none of the offered languages.
        /// </summary>
        /// <param name="country">The ISO 3166-1 alpha-2 code to look up. Case is ignored.</param>
        /// <returns>The matching language, or <see langword="null"/>.</returns>
        public static UiLanguage FromCountry(string country)
        {
            return string.IsNullOrWhiteSpace(country)
                ? null
                : All.FirstOrDefault(x => string.Equals(x.Country, country.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
