using KleeneStar.Model.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a template entity.
    /// A template provides predefined field values and metadata to accelerate object creation.
    /// </summary>
    public class Template : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the template.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the system name of the template.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the template.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the category of the template used for grouping in the UI.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this template.
        /// </summary>
        [RestConverter<RestValueConverterImageIcon>]
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the current state of the template.
        /// </summary>
        [RestConverter<TemplateStateConverter>]
        public TemplateState State { get; set; }

        /// <summary>
        /// Gets or sets the JSON serialized collection of predefined field value presets.
        /// </summary>
        public string Presets { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the parent template.
        /// </summary>
        /// <remarks>
        /// The parent reference composes templates: creating an object from a template also
        /// creates one object per child template, linked below the created object.
        /// </remarks>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the parent template.
        /// </summary>
        [JsonIgnore]
        public Template Parent { get; set; }

        /// <summary>
        /// Gets or sets the position of the template among its siblings, so the objects a
        /// composite template creates appear in a defined order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the identity the objects created from this template are assigned to,
        /// or <c>null</c> to leave them unassigned.
        /// </summary>
        /// <remarks>
        /// A work structure names who does each part - the onboarding's account request goes to
        /// the IT desk, the access card to facility management - so a child of a composite
        /// arrives in the right hands rather than in a pool somebody has to sort.
        /// </remarks>
        public Guid? AssigneeId { get; set; }

        /// <summary>
        /// Gets or sets the identity the objects created from this template are assigned to.
        /// </summary>
        [JsonIgnore]
        public Identity Assignee { get; set; }

        /// <summary>
        /// Gets or sets the sibling template whose object has to be finished before the object of
        /// this one can be, or <c>null</c> when it depends on nothing.
        /// </summary>
        /// <remarks>
        /// The dependency is created as a blocking relation between the two objects, so the
        /// workflow refuses to close the dependent one early - the account cannot be handed
        /// over before it exists. Only a sibling counts: the two objects are created by the same
        /// instantiation, and a reference outside it would point at nothing.
        /// </remarks>
        public Guid? PredecessorId { get; set; }

        /// <summary>
        /// Gets or sets the sibling template this one depends on.
        /// </summary>
        [JsonIgnore]
        public Template Predecessor { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the class this template is bound to.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the class this template is bound to.
        /// </summary>
        public Class Class { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Template()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the template.</param>
        public Template(Guid id)
        {
            Id = id;
        }
    }
}
