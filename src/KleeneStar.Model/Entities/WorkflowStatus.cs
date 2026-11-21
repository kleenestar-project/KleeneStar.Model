using System;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents the participation of a status in a workflow (m:n relation), together with the
    /// data that belongs to the pairing rather than to either side of it.
    /// </summary>
    /// <remarks>
    /// A status is defined per class and can take part in several workflows, so where it sits on
    /// the designer canvas and whether it opens or terminates the state machine differ per
    /// workflow and cannot live on the status itself.
    /// </remarks>
    public class WorkflowStatus
    {
        /// <summary>
        /// Gets or sets the unique identifier of the workflow the status takes part in.
        /// </summary>
        public Guid WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the workflow the status takes part in.
        /// </summary>
        public Workflow Workflow { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the participating status.
        /// </summary>
        public Guid StatusId { get; set; }

        /// <summary>
        /// Gets or sets the participating status.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Gets or sets the horizontal position of the state on the designer canvas, addressing
        /// its top left corner in the editor's model coordinate space.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the vertical position of the state on the designer canvas, addressing
        /// its top left corner in the editor's model coordinate space.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workflow enters at this status. The
        /// designer computes reachability from the entry states; without one it cannot tell an
        /// unreachable state from a merely unusual one.
        /// </summary>
        public bool IsStart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workflow terminates at this status. A
        /// terminal status legitimately has no outgoing transition, which must not be reported
        /// as a dead end.
        /// </summary>
        public bool IsEnd { get; set; }
    }
}
