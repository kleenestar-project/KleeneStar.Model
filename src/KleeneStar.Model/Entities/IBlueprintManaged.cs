using System;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// A part of a class structure - a field, a status, a priority, a workflow or one of its
    /// transitions - that can be provided by a blueprint class rather than defined locally.
    /// </summary>
    /// <remarks>
    /// A class following a blueprint (<see cref="Class.BlueprintId"/>) receives the blueprint's
    /// structure as copies, and each copy names the element it was made from. The name is what
    /// lets a later synchronization find and update the copy instead of adding a second one, and
    /// what tells an endpoint that the element is managed centrally and is not changed locally.
    /// A locally defined element carries none and stays the follower's own.
    /// </remarks>
    public interface IBlueprintManaged
    {
        /// <summary>
        /// Gets or sets the blueprint element this one is a copy of, or <c>null</c> for an element
        /// defined locally.
        /// </summary>
        Guid? BlueprintSourceId { get; set; }
    }
}
