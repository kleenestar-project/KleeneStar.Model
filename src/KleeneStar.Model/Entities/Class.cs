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
    /// Represents a class entity.
    /// </summary>
    public class Class : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the class.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the class.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current state of the class.
        /// </summary>
        [RestConverter<ClassStateConverter>]
        public ClassState State { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this class.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the class is abstract.
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the class this class inherits from.
        /// </summary>
        public Guid? InheritedId { get; set; }

        /// <summary>
        /// Gets or sets the class this class inherits from.
        /// </summary>
        [JsonIgnore]
        public Class Inherited { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the class is sealed.
        /// </summary>
        public bool Sealed { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the parent class.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the parent class.
        /// </summary>
        [JsonIgnore]
        public Class Parent { get; set; }

        /// <summary>
        /// Gets or sets the access modifier controlling the visibility of this class.
        /// </summary>
        [RestConverter<AccessModifierConverter>]
        public AccessModifier AccessModifier { get; set; }

        /// <summary>
        /// Gets or sets the collection of classes allowed as children of this class.
        /// </summary>
        [JsonIgnore]
        public List<Class> AllowedChildren { get; set; } = [];

        /// <summary>
        /// Gets or sets the unique identifier of the workspace associated with this class.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the workspace associated with the current class.
        /// </summary>
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Class()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the 
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the class.
        /// </param>
        public Class(Guid id)
        {
            Id = id;
        }
    }
}
