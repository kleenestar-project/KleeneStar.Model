using KleeneStar.Model.Converters;
using KleeneStar.Model.Entities;

namespace KleeneStar.Model.Test.Converters
{
    /// <summary>
    /// Provides unit tests for <see cref="LanguageConverter"/> - the account page offers the
    /// interface languages through the country control, while the identity stores the language
    /// code.
    /// </summary>
    public class UnitTestLanguageConverter
    {
        /// <summary>
        /// Verifies that a submitted country becomes the language it stands for.
        /// </summary>
        [Theory]
        [InlineData("DE", "de")]
        [InlineData("gb", "en")]
        [InlineData("PL;", "pl")]
        public void FromRaw_Country_IsLanguageCode(string raw, string expected)
        {
            Assert.Equal(expected, new LanguageConverter().FromRaw(raw, typeof(string)));
        }

        /// <summary>
        /// Verifies that the selection id a former control submitted is still read.
        /// </summary>
        [Fact]
        public void FromRaw_SelectionId_IsLanguageCode()
        {
            var raw = UiLanguage.FromCode("fr").Id.ToString();

            Assert.Equal("fr", new LanguageConverter().FromRaw(raw, typeof(string)));
        }

        /// <summary>
        /// Verifies that nothing chosen, or a country standing for no offered language, is no
        /// language - the identity then follows the culture of the application.
        /// </summary>
        [Theory]
        [InlineData("")]
        [InlineData("US")]
        public void FromRaw_NoLanguage_IsNull(string raw)
        {
            Assert.Null(new LanguageConverter().FromRaw(raw, typeof(string)));
        }

        /// <summary>
        /// Verifies that a stored language is shown as its country, and every offered language
        /// round-trips through its country.
        /// </summary>
        [Fact]
        public void ToRaw_EveryLanguage_RoundTrips()
        {
            var converter = new LanguageConverter();

            foreach (var language in UiLanguage.All)
            {
                var raw = converter.ToRaw(language.Code, typeof(string));

                Assert.Equal(language.Country, raw);
                Assert.Equal(language.Code, converter.FromRaw(raw, typeof(string)));
            }

            Assert.Equal(string.Empty, converter.ToRaw(null, typeof(string)));
        }

        /// <summary>
        /// Verifies that no two languages share a country, or the control could not tell them
        /// apart.
        /// </summary>
        [Fact]
        public void Countries_AreDistinct()
        {
            Assert.Equal(UiLanguage.All.Count, UiLanguage.All.Select(x => x.Country).Distinct().Count());
        }
    }
}
