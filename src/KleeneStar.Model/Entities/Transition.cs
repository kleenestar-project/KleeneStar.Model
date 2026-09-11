using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a workflow transition entity.
    /// </summary>
    public class Transition : IEntity
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
        /// Gets or sets the description of the workflow transition.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current state of the workflow transition.
        /// </summary>
        public TransitionState State { get; set; }

        /// <summary>
        /// Gets or sets the colour the edge is drawn in on the designer canvas. Null leaves the
        /// choice to the canvas.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the SVG dash pattern the edge is drawn with, for example <c>6 4</c> for a
        /// dashed line. Null or empty draws a solid line.
        /// </summary>
        public string DashArray { get; set; }

        /// <summary>
        /// Gets or sets the intermediate points the edge is routed through on the designer
        /// canvas, in the editor's model coordinate space.
        /// </summary>
        public List<TransitionWaypoint> Waypoints { get; set; } = [];

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
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
        public Status Source { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the target entity.
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// Gets or sets the target workflow state for the operation.
        /// </summary>
        public Status Target { get; set; }

        /// <summary>
        /// Gets or sets the condition under which the transition may be taken at all, written as
        /// a disjunctive normal form over the keys of the registered guards -
        /// <c>a;b|c</c> reads as <em>(a and b) or c</em>. Empty means unconditional.
        /// </summary>
        /// <remarks>
        /// The expression is stored rather than a list of rows because it is one statement about
        /// this transition, always read as a whole, and because that is exactly the shape the
        /// administering control produces: a disjunction of conjunctions. Which keys exist is not
        /// modelled here - the guards are registered at runtime, and a key naming one that is no
        /// longer installed simply fails to hold rather than breaking the workflow.
        /// </remarks>
        public string GuardExpression { get; set; }

        /// <summary>
        /// Gets or sets what has to be true of the object for the transition to be accepted,
        /// written as a disjunctive normal form over the keys of the registered validators.
        /// Empty means the transition validates nothing.
        /// </summary>
        /// <remarks>
        /// A guard says whether the move may be offered, a validator whether the move as
        /// submitted is acceptable - the distinction is what lets a transition be visible and
        /// still refuse an object that has not been filled in. Both are the same shape because
        /// both are conditions; only the moment they are asked and the message they leave differ.
        /// </remarks>
        public string ValidatorExpression { get; set; }

        /// <summary>
        /// Gets or sets the keys of the post functions the transition runs once it has been
        /// applied, in the order they run.
        /// </summary>
        /// <remarks>
        /// A list rather than an expression: post functions are actions, and actions are not
        /// combined with and/or - they are performed, one after another, and the order is part
        /// of what the administrator decided.
        /// </remarks>
        public List<string> PostFunctionKeys { get; set; } = [];

        /// <summary>
        /// Gets or sets the form shown before the transition runs - the screen a person fills in
        /// while making the move - or <see langword="null"/> when the move needs no input.
        /// </summary>
        /// <remarks>
        /// It is an ordinary <see cref="Form"/> of the class rather than a screen type of its
        /// own: what a transition asks for is the same kind of mask an object is edited through,
        /// and modelling it twice would mean two form designers, two renderers and two ways for
        /// a field to end up on a page.
        /// </remarks>
        public Guid? ScreenFormId { get; set; }

        /// <summary>
        /// Gets or sets the form shown before the transition runs.
        /// </summary>
        public Form ScreenForm { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Transition()
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
        public Transition(Guid id)
        {
            Id = id;
        }
    }
}
