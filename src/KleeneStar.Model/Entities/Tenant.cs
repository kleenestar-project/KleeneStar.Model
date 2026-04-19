using KleeneStar.Model.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a tenant that can be associated with one or more workspaces.
    /// </summary>
    public class Tenant : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the tenant.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this tenant.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the tenant.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current state of the tenant.
        /// </summary>
        [RestConverter<TenantStateConverter>]
        public TenantState State { get; set; }

        /// <summary>
        /// Gets or sets the collection of workspaces associated with this tenant.
        /// </summary>
        [JsonIgnore]
        public List<Workspace> Workspaces { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Tenant()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the tenant.
        /// </param>
        public Tenant(Guid id)
        {
            Id = id;
        }
    }
}
