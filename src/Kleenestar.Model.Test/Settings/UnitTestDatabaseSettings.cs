using KleeneStar.Model.Settings;
using Microsoft.Extensions.Configuration;

namespace Kleenestar.Model.Test.Settings
{
    /// <summary>
    /// Contains unit tests for <see cref="DatabaseSettings"/>: the binding from a plugin's
    /// settings section and the defaults that stand in for whatever the settings leave out.
    /// </summary>
    public class UnitTestDatabaseSettings
    {
        /// <summary>
        /// Builds the settings of a plugin the way the framework hands them to the application:
        /// the plugin's own section of the merged configuration.
        /// </summary>
        /// <param name="values">The values below the plugin section, as configuration paths.</param>
        /// <returns>The plugin's settings section.</returns>
        private static IConfiguration PluginSettings(params (string Key, string Value)[] values)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(values.ToDictionary(x => $"Plugins:kleenestar.core:{x.Key}", x => x.Value))
                .Build();

            return configuration.GetSection("Plugins:kleenestar.core");
        }

        /// <summary>
        /// A plugin without settings, and a plugin whose settings say nothing about the
        /// database, both answer the sqlite defaults.
        /// </summary>
        [Fact]
        public void DefaultsWithoutSettings()
        {
            var none = DatabaseSettings.From(null);
            var empty = DatabaseSettings.From(PluginSettings());

            foreach (var settings in new[] { none, empty })
            {
                Assert.Equal("Microsoft.Data.Sqlite", settings.Provider);
                Assert.Equal("KleeneStar.Model.Sqlite", settings.Assembly);
                Assert.Equal("Data Source=data/db/kleenestar.db", settings.ConnectionString);
            }
        }

        /// <summary>
        /// A complete database section is bound as written.
        /// </summary>
        [Fact]
        public void BindsCompleteSection()
        {
            var settings = DatabaseSettings.From(PluginSettings
            (
                ("Database:Provider", "Microsoft.Data.SqlClient"),
                ("Database:Assembly", "KleeneStar.Model.SqlServer"),
                ("Database:ConnectionString", "Server=db;Database=kleenestar;Trusted_Connection=True")
            ));

            Assert.Equal("Microsoft.Data.SqlClient", settings.Provider);
            Assert.Equal("KleeneStar.Model.SqlServer", settings.Assembly);
            Assert.Equal("Server=db;Database=kleenestar;Trusted_Connection=True", settings.ConnectionString);
        }

        /// <summary>
        /// A section naming only some of the values keeps the defaults for the rest, so a file
        /// that only moves the sqlite database keeps the sqlite provider.
        /// </summary>
        [Fact]
        public void PartialSectionKeepsDefaults()
        {
            var settings = DatabaseSettings.From(PluginSettings(("Database:ConnectionString", "Data Source=/var/lib/kleenestar/kleenestar.db")));

            Assert.Equal("Microsoft.Data.Sqlite", settings.Provider);
            Assert.Equal("KleeneStar.Model.Sqlite", settings.Assembly);
            Assert.Equal("Data Source=/var/lib/kleenestar/kleenestar.db", settings.ConnectionString);
        }

        /// <summary>
        /// The section is matched without regard to case, as every configuration key is.
        /// </summary>
        [Fact]
        public void SectionIsCaseInsensitive()
        {
            var settings = DatabaseSettings.From(PluginSettings(("database:connectionstring", "Data Source=lower.db")));

            Assert.Equal("Data Source=lower.db", settings.ConnectionString);
        }
    }
}
