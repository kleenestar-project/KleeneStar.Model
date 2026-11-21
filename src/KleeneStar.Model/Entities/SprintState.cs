namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the lifecycle state of a <see cref="Sprint"/>. Persisted as the int
    /// ordinal — new values must be appended at the end, never inserted.
    /// </summary>
    public enum SprintState
    {
        /// <summary>
        /// The sprint is planned but has not been started yet.
        /// </summary>
        Planned,

        /// <summary>
        /// The sprint is the currently running iteration of its workspace. At most one
        /// sprint per workspace should be active at a time.
        /// </summary>
        Active,

        /// <summary>
        /// The sprint has been completed and is kept for history and velocity data.
        /// </summary>
        Completed
    }

    /// <summary>
    /// Provides extension methods for <see cref="SprintState"/>.
    /// </summary>
    public static class SprintStateExtensions
    {
        /// <summary>
        /// Returns the stable REST status string of the sprint state as expected by the
        /// WebExpress scrum controls ("planned", "active", "closed").
        /// </summary>
        /// <param name="state">The sprint state.</param>
        /// <returns>The REST status string.</returns>
        public static string Code(this SprintState state)
        {
            return state switch
            {
                SprintState.Planned => "planned",
                SprintState.Active => "active",
                SprintState.Completed => "closed",
                _ => "planned"
            };
        }

        /// <summary>
        /// Parses a REST status string ("planned", "active", "closed") back into the
        /// corresponding sprint state. Unknown values fall back to
        /// <see cref="SprintState.Planned"/>.
        /// </summary>
        /// <param name="code">The REST status string.</param>
        /// <returns>The sprint state.</returns>
        public static SprintState FromCode(string code)
        {
            return code?.Trim().ToLowerInvariant() switch
            {
                "active" => SprintState.Active,
                "closed" => SprintState.Completed,
                "completed" => SprintState.Completed,
                _ => SprintState.Planned
            };
        }
    }
}
