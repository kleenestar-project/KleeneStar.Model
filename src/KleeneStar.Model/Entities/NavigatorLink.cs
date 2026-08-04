using KleeneStar.Model.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents an additional link that is shown in the primary area of the app navigator.
    /// </summary>
    public class NavigatorLink : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the navigator link.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this navigator link.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the label of the navigator link as shown in the app navigator.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the navigator link, used as its tooltip.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the target address the navigator link points to. May be an address within
        /// this server or an absolute address of an external system.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the sort order of the navigator link within the app navigator. Lower values
        /// are listed first.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the current state of the navigator link.
        /// </summary>
        [RestConverter<NavigatorLinkStateConverter>]
        public NavigatorLinkState State { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public NavigatorLink()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the navigator link.
        /// </param>
        public NavigatorLink(Guid id)
        {
            Id = id;
        }
    }
}
