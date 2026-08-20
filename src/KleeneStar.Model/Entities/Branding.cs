using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the identity of the installation: the title and the icon the application is
    /// presented under.
    /// </summary>
    /// <remarks>
    /// Both are declared in code through the <c>[Name]</c> and <c>[Icon]</c> attributes of the
    /// application, which fixes them at compile time. That is right for the product and wrong for
    /// an installation of it: a deployment named after the team it serves, a tenant with its own
    /// logo, a test system that has to be recognizable as one at a glance - none of those can be
    /// expressed by rebuilding the application. This record carries what the installation chose,
    /// and an empty field means "keep what the application declared" rather than "show nothing".
    /// <para>
    /// Like <see cref="Maintenance"/> this is a singleton: there is exactly one identity per
    /// installation, stored under <see cref="SingletonId"/>, because it is a property of the
    /// running system rather than something a user creates and lists.
    /// </para>
    /// </remarks>
    public class Branding : IEntity
    {
        /// <summary>
        /// The id of the one and only branding record.
        /// </summary>
        /// <remarks>
        /// The id is fixed rather than generated so the settings page and the startup code can
        /// address the record without first having to look up which one of them is meant.
        /// </remarks>
        public static readonly Guid SingletonId = Guid.Parse("B4E7C21D-9A36-4F58-8D0E-3C6A1F27B905");

        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the branding record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title the application is presented under. An empty value keeps the
        /// name the application declared.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the icon the application is presented under. An empty value keeps the icon
        /// the application declared.
        /// </summary>
        /// <remarks>
        /// The converter is what makes the icon control on the settings page take effect; without
        /// it the value the control submits never reaches this property.
        /// </remarks>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Branding()
        {
            Id = SingletonId;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the branding record.</param>
        public Branding(Guid id)
        {
            Id = id;
        }
    }
}
