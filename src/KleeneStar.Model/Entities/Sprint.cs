using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a Scrum iteration of a workspace. Objects are committed to a sprint
    /// via <see cref="Object.SprintId"/>; objects without a sprint form the product
    /// backlog of their workspace.
    /// </summary>
    public class Sprint : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the sprint.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the sprint (e.g. "Sprint 12").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sprint goal — the short mission statement the team commits to.
        /// </summary>
        public string Goal { get; set; }

        /// <summary>
        /// Gets or sets the lifecycle state of the sprint.
        /// </summary>
        public SprintState State { get; set; }

        /// <summary>
        /// Gets or sets the first day of the sprint, or <see langword="null"/> when not
        /// scheduled yet.
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// Gets or sets the last day of the sprint, or <see langword="null"/> when not
        /// scheduled yet.
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Gets or sets the planned capacity of the sprint in story points. Zero means
        /// no capacity has been planned.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the owning workspace.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the workspace the sprint belongs to.
        /// </summary>
        public Workspace Workspace { get; set; }

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
        public Sprint()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the sprint.</param>
        public Sprint(Guid id)
        {
            Id = id;
        }
    }
}
