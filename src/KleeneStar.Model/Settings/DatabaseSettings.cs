using Microsoft.Extensions.Configuration;

namespace KleeneStar.Model.Settings
{
    /// <summary>
    /// The settings of the database connection, bound from the <see cref="Section"/> section of
    /// the plugin's settings. The defaults describe the sqlite database under the data
    /// directory, so an installation that says nothing about its database still comes up.
    /// </summary>
    /// <remarks>
    /// The class is a plain options object: it carries no knowledge of where the values came
    /// from and is filled once at start-up by the configuration binder. A property the settings
    /// leave out keeps its default, so a file that only names a connection string keeps the
    /// sqlite provider.
    /// </remarks>
    public sealed class DatabaseSettings
    {
        /// <summary>
        /// The name of the section, below the plugin's own settings, the database settings are
        /// read from.
        /// </summary>
        public const string Section = "Database";

        /// <summary>
        /// Gets or sets the name of the provider associated with this instance.
        /// </summary>
        public string Provider { get; set; } = "Microsoft.Data.Sqlite";

        /// <summary>
        /// Gets or sets the connection string used to establish a connection to the data source.
        /// </summary>
        public string ConnectionString { get; set; } = "Data Source=data/db/kleenestar.db";

        /// <summary>
        /// Gets or sets the name of the assembly that provides the provider-specific context.
        /// </summary>
        public string Assembly { get; set; } = "KleeneStar.Model.Sqlite";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public DatabaseSettings()
        {
        }

        /// <summary>
        /// Binds the database settings from a plugin's settings. A missing section, or one
        /// without values, answers the defaults; a section naming only some of the values keeps
        /// the defaults for the rest.
        /// </summary>
        /// <param name="pluginSettings">The settings of the plugin, or <see langword="null"/> when the plugin has none.</param>
        /// <returns>The database settings.</returns>
        public static DatabaseSettings From(IConfiguration pluginSettings)
        {
            var settings = new DatabaseSettings();

            pluginSettings?.GetSection(Section).Bind(settings);

            return settings;
        }
    }
}
