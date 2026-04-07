using System.Xml.Serialization;

namespace KleeneStar.Model.Config
{
    /// <summary>
    /// Class for reading the configuration file.
    /// </summary>
    [XmlRoot("config", IsNullable = false)]
    public sealed class DbConfig
    {
        /// <summary>
        /// Gets or sets the name of the provider associated with this instance.
        /// </summary>
        [XmlElement("provider")]
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets the connection string used to establish a connection to the data source.
        /// </summary>
        [XmlElement("connectionstring")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the assembly associated with the provider.
        /// </summary>
        [XmlElement("assembly")]
        public string Assembly { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public DbConfig()
        {
        }
    }
}
