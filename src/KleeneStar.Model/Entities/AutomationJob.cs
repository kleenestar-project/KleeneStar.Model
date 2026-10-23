using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// One execution of an automation rule for one object, kept in a persistent queue.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Persistent.</b> An event that matches a rule is written down as a job before anything
    /// is done about it, so a host that goes down between the event and its consequence does the
    /// work after the restart instead of losing it.
    /// </para>
    /// <para>
    /// <b>Deduplicated.</b> <see cref="Deduplication"/> is unique: an event delivered twice - a
    /// replayed message, a retried webhook, a scheduler run that overlaps the previous one -
    /// finds the job of the first delivery and adds none.
    /// </para>
    /// <para>
    /// <b>Leased.</b> A worker claims a job for a while (<see cref="LockedUntil"/>) under the
    /// optimistic concurrency of <see cref="Version"/>, so two workers - two replicas of a
    /// cluster - never run the same job, and a job whose worker died is claimed again once the
    /// lease ran out. <see cref="CompletedActions"/> records the actions already done, so a
    /// resumed job continues after the last one rather than repeating them.
    /// </para>
    /// <para>
    /// <b>Retried, then parked.</b> A failing job is retried with a growing delay; after the last
    /// attempt it is <see cref="AutomationJobState.Failed"/> and waits for somebody to look at
    /// <see cref="LastError"/> and run it again by hand.
    /// </para>
    /// </remarks>
    public class AutomationJob : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the job.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the rule the job executes.
        /// </summary>
        public Guid RuleId { get; set; }

        /// <summary>
        /// Gets or sets the object the job acts on.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the trigger that caused the job.
        /// </summary>
        public string Trigger { get; set; }

        /// <summary>
        /// Gets or sets the key that identifies the event the job answers. Unique.
        /// </summary>
        public string Deduplication { get; set; }

        /// <summary>
        /// Gets or sets how deep in a chain of automation the job stands: zero for an event a
        /// person caused, one for an event a rule caused, and so on.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Gets or sets where the job stands.
        /// </summary>
        public AutomationJobState State { get; set; }

        /// <summary>
        /// Gets or sets how often the job was run.
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// Gets or sets how many of the rule's actions are done.
        /// </summary>
        public int CompletedActions { get; set; }

        /// <summary>
        /// Gets or sets the earliest time the job is run (again).
        /// </summary>
        public DateTime NextAttempt { get; set; }

        /// <summary>
        /// Gets or sets the worker holding the job, or <c>null</c>.
        /// </summary>
        public string LockedBy { get; set; }

        /// <summary>
        /// Gets or sets until when the worker holds the job, or <c>null</c>.
        /// </summary>
        public DateTime? LockedUntil { get; set; }

        /// <summary>
        /// Gets or sets the error of the last attempt, or <c>null</c>.
        /// </summary>
        public string LastError { get; set; }

        /// <summary>
        /// Gets or sets the concurrency stamp a claim compares and renews.
        /// </summary>
        public Guid Version { get; set; }

        /// <summary>
        /// Gets or sets the date and time the job was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time the job last changed.
        /// </summary>
        public DateTime Updated { get; set; }
    }

    /// <summary>
    /// Where an <see cref="AutomationJob"/> stands. Persisted as the ordinal; new members are appended.
    /// </summary>
    public enum AutomationJobState
    {
        /// <summary>The job waits for its next attempt.</summary>
        Pending,

        /// <summary>A worker holds the job.</summary>
        Running,

        /// <summary>Every action is done.</summary>
        Succeeded,

        /// <summary>The last attempt failed; the job waits to be run again by hand.</summary>
        Failed,

        /// <summary>The job was dropped: its rule or object is gone, or its condition no longer holds.</summary>
        Skipped
    }
}
