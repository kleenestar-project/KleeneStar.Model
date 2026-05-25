using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a generic per-identity key/value setting persisted across
    /// sessions. Used as a backing store for UI preferences (e.g. REST API
    /// table column order and width) and any other free-form per-user data
    /// the application wants to remember between logins.
    ///
    /// Identified by the composite (<see cref="OwnerId"/>, <see cref="Scope"/>,
    /// <see cref="Key"/>) — the scope namespaces keys so different features
    /// (table layouts, dashboards, dialogs, ...) cannot collide.
    /// </summary>
    public class UserSession : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of this setting row.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identity that owns this setting.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the owning identity.
        /// </summary>
        [JsonIgnore]
        public Identity Owner { get; set; }

        /// <summary>
        /// Gets or sets the scope/namespace of the setting (e.g.
        /// <c>"rest-table"</c>, <c>"dashboard"</c>). Used together with
        /// <see cref="Key"/> to look the value up.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the key inside the scope (e.g. the fully qualified
        /// type name of the REST API table whose layout is stored here).
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the serialized value. The producer/consumer pair
        /// decides the format; JSON is the recommended default.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the setting was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the setting was last written.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class with a fresh id.
        /// </summary>
        public UserSession()
        {
            Id = Guid.NewGuid();
        }
    }
}
