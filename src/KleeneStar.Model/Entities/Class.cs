using System;
using WebExpress.WebApp.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a class entity.
    /// </summary>
    public class Class : IClass
    {
        /// <summary>
        /// Returns or sets the unique identifier for the class.
        /// </summary>
        [RestTableColumnHidden()]
        public Guid Id { get; set; }

        /// <summary>
        /// Returns or sets the name of the class.
        /// </summary>
        public string Name { get; set; }
    }
}
