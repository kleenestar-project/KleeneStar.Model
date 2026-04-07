using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a workflow entity.
    /// </summary>
    public class Workflow : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the workflow.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the workflow.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the description of the workflow.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Returns the current state of the workflow.
        /// </summary>
        public TypeWorkspaceState State { get; set; }

        /// <summary>
        /// Returns the icon associated with this workflow.
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
        /// Gets or sets the list of states defined in the workflow.
        /// </summary>
        public List<WorkflowState> States { get; set; }

        /// <summary>
        /// Gets or sets the collection of workflow transitions associated with this instance.
        /// </summary>
        /// <remarks>
        /// Each transition defines a possible state change within the workflow. Modifying this
        /// collection affects the available transitions for the workflow.
        /// </remarks>
        public List<WorkflowTransition> Transitions { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Workflow()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the 
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the workflow.
        /// </param>
        public Workflow(Guid id)
        {
            Id = id;
        }
    }
}
