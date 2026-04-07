using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a workflow state entity.
    /// </summary>
    public class WorkflowState : IEntity
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
        /// Returns the icon associated with this workflow transition.
        /// </summary>
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Returns the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the class associated with this workflow.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the class associated with the current workflow.
        /// </summary>
        public Class Class { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the workflow associated with this workflow state.
        /// </summary>
        public Guid WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the workflow associated with the current workflow state.
        /// </summary>
        public Workflow Workflow { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WorkflowState()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the 
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the workflow state.
        /// </param>
        public WorkflowState(Guid id)
        {
            Id = id;
        }
    }
}
