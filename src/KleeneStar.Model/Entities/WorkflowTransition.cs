using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a workflow transition entity.
    /// </summary>
    public class WorkflowTransition : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the workflow transition.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the workflow transition.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the description of the workflow transition.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns the current state of the workflow transition.
        /// </summary>
        public TypeWorkspaceState State { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the workflow associated with this workflow transition.
        /// </summary>
        public Guid WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the workflow associated with the current workflow transition.
        /// </summary>
        public Workflow Workflow { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the source associated with this instance.
        /// </summary>
        public Guid SourceId { get; set; }

        /// <summary>
        /// Gets or sets the source workflow state for the transition.
        /// </summary>
        public WorkflowState Source { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the target entity.
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// Gets or sets the target workflow state for the operation.
        /// </summary>
        public WorkflowState Target { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WorkflowTransition()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the 
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the workflow transition.
        /// </param>
        public WorkflowTransition(Guid id)
        {
            Id = id;
        }
    }
}
