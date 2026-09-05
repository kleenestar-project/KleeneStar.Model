using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebApp.WebRestApi.WebExpress.WebApp.WebRestApi;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a object entity.
    /// </summary>
    public class Object : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the object.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the key of the object.
        /// </summary>
        [ValidateMinLength(2)]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the summary of the object.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Returns the description of the object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns the current state of the object.
        /// </summary>
        public WorkspaceState State { get; set; }

        /// <summary>
        /// Returns the icon associated with this object.
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
        /// Gets or sets the kind key that assigns the object to one of the object kinds
        /// (subtypes), e.g. <see cref="ObjectKind.Document"/>, <see cref="ObjectKind.Blog"/>,
        /// or <see cref="ObjectKind.Issue"/>. The set of kinds is open — add-ons may
        /// introduce further keys — and the kind decides which overview view presents the
        /// object. Defaults to <see cref="ObjectKind.Default"/>.
        /// </summary>
        public string Kind { get; set; } = ObjectKind.Default;

        /// <summary>
        /// Gets or sets the unique identifier of the workspace associated with this object.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the workspace associated with the current object.
        /// </summary>
        public Workspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the class associated with this instance.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the class associated with the current context.
        /// </summary>
        public Class Class { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the security level the object is classified
        /// with, or <c>null</c> when the object is unclassified.
        /// </summary>
        /// <remarks>
        /// The level is one of the levels defined on the object's class. An unclassified object
        /// is visible to everyone; a classified one is visible only to the identities cleared
        /// for its level through one of the level's groups. The rule is enforced centrally by
        /// the object manager, so every list, overview and detail view obeys it.
        /// </remarks>
        public Guid? SecurityLevelId { get; set; }

        /// <summary>
        /// Gets or sets the security level the object is classified with.
        /// </summary>
        public SecurityLevel SecurityLevel { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the parent object, if any. The parent
        /// expresses a containment relationship inside the same workspace (e.g. an
        /// "Epic" object owning child "Story" objects).
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the parent object.
        /// </summary>
        public Object Parent { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity that created the object,
        /// if known.
        /// </summary>
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// Gets or sets the identity that created the object.
        /// </summary>
        public Identity Creator { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity the object is currently
        /// assigned to, or <c>null</c> when the object is unassigned.
        /// </summary>
        public Guid? AssigneeId { get; set; }

        /// <summary>
        /// Gets or sets the identity the object is currently assigned to.
        /// </summary>
        public Identity Assignee { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the sprint the object is committed to,
        /// or <c>null</c> when the object sits in the product backlog of its workspace.
        /// </summary>
        public Guid? SprintId { get; set; }

        /// <summary>
        /// Gets or sets the sprint the object is committed to.
        /// </summary>
        public Sprint Sprint { get; set; }

        /// <summary>
        /// Gets or sets the 1-based ordering rank of the object within its sprint, or —
        /// when <see cref="SprintId"/> is <c>null</c> — within the product backlog of its
        /// workspace. Zero means the object has not been ranked yet.
        /// </summary>
        public int SprintRank { get; set; }

        /// <summary>
        /// Gets or sets the story-point estimate of the object, or <c>null</c> when the
        /// object has not been estimated.
        /// </summary>
        public int? StoryPoints { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the identity that last updated the
        /// object, or <c>null</c> when the updater is unknown.
        /// </summary>
        public Guid? UpdaterId { get; set; }

        /// <summary>
        /// Gets or sets the identity that last updated the object.
        /// </summary>
        public Identity Updater { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Object()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the 
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the object.
        /// </param>
        public Object(Guid id)
        {
            Id = id;
        }
    }
}
