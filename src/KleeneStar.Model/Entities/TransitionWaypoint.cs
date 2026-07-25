namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents an intermediate point a workflow transition is routed through on the designer
    /// canvas.
    /// </summary>
    /// <remarks>
    /// Waypoints are pure presentation and only ever read as a whole sequence, so they are stored
    /// as a serialized list on the transition rather than as rows of their own.
    /// </remarks>
    public class TransitionWaypoint
    {
        /// <summary>
        /// Gets or sets the horizontal position in the editor's model coordinate space.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the vertical position in the editor's model coordinate space.
        /// </summary>
        public int Y { get; set; }
    }
}
