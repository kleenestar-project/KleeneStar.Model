using KleeneStar.Model.Config;
using System;
using WebExpress.WebCore;
using WebExpress.WebCore.WebApplication;
using WebExpress.WebCore.WebComponent;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the KleeneStar.
    /// </summary>
    public static partial class ModelHub
    {
        /// <summary>
        /// Returns the shared instance of the component hub used for managing and coordinating application components.
        /// </summary>
        public static IComponentHub ComponentHub { get; set; }

        /// <summary>
        /// Returns the current application context, which provides access to application-wide services and configurations.
        /// </summary>
        public static IApplicationContext ApplicationContet { get; set; }

        /// <summary>
        /// Returns the current HTTP server context for the application.
        /// </summary>
        public static IHttpServerContext HttpServerContext { get; set; }

        /// <summary>
        /// Returns or sets the configuration settings for the database connection.
        /// </summary>
        public static DbConfig DatabaseConfig { get; set; }

        /// <summary>
        /// Returns a new instance of the application's database context configured with 
        /// the current database settings.
        /// </summary>
        /// <remarks>
        /// Each access returns a separate instance. The caller is responsible for 
        /// disposing the returned context when it is no longer needed.
        /// </remarks>
        /// <returns>A new instance of db context.</returns>
        public static KleeneStarDbContext CreateDbContext()
        {
            if (DatabaseConfig == null)
            {
                throw new InvalidOperationException("DatabaseConfig has not been initialized.");
            }

            return KleeneStarDbContextFactory.Create(DatabaseConfig);
        }

    }
}
