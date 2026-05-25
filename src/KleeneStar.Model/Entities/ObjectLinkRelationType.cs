namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the directional semantics of an <see cref="ObjectLink"/>. The relation
    /// describes what the source object "is" to the target object (e.g. <c>BlockedBy</c>
    /// means the source is blocked by the target).
    /// </summary>
    public enum ObjectLinkRelationType
    {
        /// <summary>
        /// Generic non-directional relationship between two objects.
        /// </summary>
        RelatesTo,

        /// <summary>
        /// The source object cannot progress until the target is resolved.
        /// </summary>
        BlockedBy,

        /// <summary>
        /// The source object blocks the target object from progressing.
        /// </summary>
        Blocks,

        /// <summary>
        /// The source object duplicates an existing target object.
        /// </summary>
        DuplicateOf,

        /// <summary>
        /// The source object was caused by the target object.
        /// </summary>
        CausedBy,

        /// <summary>
        /// The source object caused the target object.
        /// </summary>
        Causes,

        /// <summary>
        /// The source object is a part / sub-task of the target object.
        /// </summary>
        PartOf
    }
}
