using KleeneStar.Model.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the maintenance notice of the installation: an instruction text that is shown
    /// to every user as a toast for as long as it is active.
    /// </summary>
    /// <remarks>
    /// Unlike the other entities this one is a singleton. There is exactly one notice per
    /// installation, stored under <see cref="SingletonId"/>, because the notice is a property of
    /// the running system rather than something a user creates and lists.
    /// </remarks>
    public class Maintenance : IEntity
    {
        /// <summary>
        /// The id of the one and only maintenance record.
        /// </summary>
        /// <remarks>
        /// The id is fixed rather than generated so the settings page and the toast can address the
        /// record without first having to look up which one of them is meant.
        /// </remarks>
        public static readonly Guid SingletonId = Guid.Parse("D6F1A83C-5B27-4E9A-9C08-7E4B2D5F16A9");

        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the maintenance notice.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets whether the instruction text is currently shown to the users.
        /// </summary>
        /// <remarks>
        /// The converter is what makes the switch on the settings page take effect; without it the
        /// value a checkbox submits never reaches this property. See <see cref="RestValueConverterBool"/>.
        /// </remarks>
        [RestConverter<RestValueConverterBool>]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the instruction text shown to the users while the notice is enabled.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Maintenance()
        {
            Id = SingletonId;
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the maintenance notice.</param>
        public Maintenance(Guid id)
        {
            Id = id;
        }
    }
}
