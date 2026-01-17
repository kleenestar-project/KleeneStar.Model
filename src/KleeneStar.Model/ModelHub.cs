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
    }
}
