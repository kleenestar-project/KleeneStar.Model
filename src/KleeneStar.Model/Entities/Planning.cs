using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Time spent on an object by a person.
    /// </summary>
    /// <remarks>
    /// A worklog is personal data: the object's readers see the totals, the entries themselves
    /// are visible to their author and to the administrators of the workspace.
    /// </remarks>
    public class Worklog : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the worklog.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the object worked on.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets who worked.
        /// </summary>
        public Guid IdentityId { get; set; }

        /// <summary>
        /// Gets or sets when the work began, in UTC.
        /// </summary>
        public DateTime Started { get; set; }

        /// <summary>
        /// Gets or sets how long the work took, in minutes.
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Gets or sets what was done.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the date and time the worklog was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time the worklog was updated.
        /// </summary>
        public DateTime Updated { get; set; }
    }

    /// <summary>
    /// The planning figures of an object: its estimates and the milestone it is due with.
    /// </summary>
    /// <remarks>
    /// Kept beside the object rather than on it, so the planning stays optional - an object
    /// without a row has neither estimates nor a milestone - and the object's search index is
    /// not rebuilt for figures it does not search by.
    /// </remarks>
    public class ObjectPlanning : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the row.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the object; one row per object.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the estimate before work began, in minutes, or <c>null</c>.
        /// </summary>
        public int? OriginalEstimateMinutes { get; set; }

        /// <summary>
        /// Gets or sets the work believed left, in minutes, or <c>null</c>.
        /// </summary>
        public int? RemainingEstimateMinutes { get; set; }

        /// <summary>
        /// Gets or sets the milestone the object is due with, or <c>null</c>.
        /// </summary>
        public Guid? MilestoneId { get; set; }

        /// <summary>
        /// Gets or sets the date and time the row was updated.
        /// </summary>
        public DateTime Updated { get; set; }
    }

    /// <summary>
    /// A point a workspace delivers to - a release, a go-live, a phase end - and the objects
    /// due with it.
    /// </summary>
    public class Milestone : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the milestone.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the workspace.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the name, unique per workspace.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date the milestone is due, or <c>null</c>.
        /// </summary>
        public DateTime? Due { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public MilestoneState State { get; set; }

        /// <summary>
        /// Gets or sets the date and time the milestone was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time the milestone was updated.
        /// </summary>
        public DateTime Updated { get; set; }
    }

    /// <summary>
    /// The state of a <see cref="Milestone"/>. Persisted as the ordinal.
    /// </summary>
    public enum MilestoneState
    {
        /// <summary>Work is due with the milestone.</summary>
        Open,

        /// <summary>The milestone was delivered.</summary>
        Reached,

        /// <summary>The milestone is kept for the record only.</summary>
        Archived
    }

    /// <summary>
    /// The planning settings of a workspace.
    /// </summary>
    public class WorkspacePlanning : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the row.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the workspace; one row per workspace.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets whether work is logged on the workspace's objects. Off by default: time
        /// tracking is a decision of the organization, not of the tool.
        /// </summary>
        public bool TimeTracking { get; set; }
    }

    /// <summary>
    /// How much time a person has for planned work per week.
    /// </summary>
    public class IdentityCapacity : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the row.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the person; one row per person.
        /// </summary>
        public Guid IdentityId { get; set; }

        /// <summary>
        /// Gets or sets the minutes per week.
        /// </summary>
        public int WeeklyMinutes { get; set; }
    }
}
