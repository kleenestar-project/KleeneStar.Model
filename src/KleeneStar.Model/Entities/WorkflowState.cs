using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the state of a workflow, indicating whether it is active or deleted.
    /// </summary>
    public enum WorkflowState
    {
        /// <summary>
        /// Indicates that the workflow is fully configured, visible, and interactively usable.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates that the item is archived and no longer active.
        /// </summary>
        Archived
    }

    /// <summary>
    /// Provides extension methods for evaluating and working with values of the WorkflowState enumeration.
    /// </summary>
    public static class WorkflowStateExtensions
    {
        /// <summary>
        /// Determines whether the specified workflow state is active.
        /// </summary>
        /// <param name="state">The workflow state to check.</param>
        /// <returns><c>true</c> if the workflow state is active; otherwise, <c>false</c>.</returns>
        public static bool IsActive(this WorkflowState state)
        {
            return state == WorkflowState.Active;
        }

        /// <summary>
        /// Returns the unique identifier associated with the specified workflow state.
        /// </summary>
        /// <param name="state">The workflow state for which to retrieve the unique identifier.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the specified workflow state.</returns>
        public static Guid Id(this WorkflowState state)
        {
            return state switch
            {
                WorkflowState.Active => Guid.Parse("57B089B6-A3D2-42EC-97CC-48A2A522E780"),
                WorkflowState.Archived => Guid.Parse("3DA01C19-9585-48E0-8E5A-0F3496274C4A"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the textual label for the specified workflow state.
        /// </summary>
        /// <param name="state">The workflow state for which the text label should be retrieved.</param>
        /// <returns>A string containing the text label for the specified state; otherwise <c>null</c>.</returns>
        public static string Text(this WorkflowState state)
        {
            return state switch
            {
                WorkflowState.Active => "kleenestar.core:state.active.label",
                WorkflowState.Archived => "kleenestar.core:state.archived.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the color selection associated with the specified workflow state.
        /// </summary>
        /// <param name="state">The workflow state for which to retrieve the corresponding color selection.</param>
        /// <returns>A string representing the color class for the given workflow state.</returns>
        public static string Color(this WorkflowState state)
        {
            return state switch
            {
                WorkflowState.Active => TypeColorSelection.Success.ToClass(),
                WorkflowState.Archived => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }
    }
}
