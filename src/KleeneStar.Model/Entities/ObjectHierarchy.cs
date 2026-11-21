using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// The materialized path of an object: the ids of its ancestors, root first - a derived,
    /// rebuildable index of <see cref="Object.ParentId"/> for subtree questions
    /// (<c>kleenestar.hierarchy.md</c>).
    /// </summary>
    /// <remarks>
    /// <see cref="Path"/> holds the ancestor ids in <c>N</c> format, each followed by a slash and
    /// led by one (<c>/</c> for an object at the root, <c>/a1…/b2…/</c> two levels down). The
    /// subtree of an object X is then every row whose path starts with
    /// <c>Path(X) + X + "/"</c> - a range on an indexed string every provider translates, rather
    /// than a recursive query. The adjacency list stays the only truth: the row is written right
    /// after every change of <c>ParentId</c>, and the start-up verifier rebuilds any row that does
    /// not match. It is a side table, not a column of <c>Object</c>, because every column of
    /// <c>Object</c> becomes part of its WebIndex schema.
    /// </remarks>
    public class ObjectPath : IEntity
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
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the object the path describes (unique; deleted with it).
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the workspace of the object.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the ancestor ids, root first, each in <c>N</c> format followed by a slash,
        /// led by a slash.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the number of ancestors (0 at the root).
        /// </summary>
        public int Depth { get; set; }
    }

    /// <summary>
    /// A recursive operation on a subtree - archive, restore, delete, dissolve, clone - run object
    /// by object through the object manager, resumable and reported (the pattern of
    /// <see cref="ObjectCreation"/>).
    /// </summary>
    public class ObjectOperation : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the run.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the idempotency key (unique): a second submission finds the first run.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the kind: <c>archive</c>, <c>restore</c>, <c>delete</c>, <c>dissolve</c>,
        /// <c>clone</c> (<see cref="ObjectOperationKinds"/>).
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the option of the kind (clone: <c>root</c> or
        /// <c>contents</c>).
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Gets or sets the object the operation starts from.
        /// </summary>
        public Guid RootId { get; set; }

        /// <summary>
        /// Gets or sets the workspace of the root.
        /// </summary>
        public Guid WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets a run this one refers to (a restore: the archive run it undoes) or an
        /// object it produced (a clone: the new root).
        /// </summary>
        public Guid? ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the workspace a transfer moves into, or <c>null</c> for every other kind.
        /// </summary>
        public Guid? TargetWorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the options a run was started with, as JSON (a transfer: the class
        /// mapping, whether to declassify), so a resumed run continues with
        /// exactly them.
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// Gets or sets the state of the run.
        /// </summary>
        public ObjectOperationState State { get; set; }

        /// <summary>
        /// Gets or sets the number of objects the run acts on.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the number of objects done.
        /// </summary>
        public int Done { get; set; }

        /// <summary>
        /// Gets or sets the reason the run stopped, when it failed.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the identity on whose behalf the run acts.
        /// </summary>
        public Guid? IdentityId { get; set; }

        /// <summary>
        /// Gets or sets when the run was started.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets when the run was last advanced.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets when the run ended.
        /// </summary>
        public DateTime? Finished { get; set; }
    }

    /// <summary>
    /// An object an operation run acts on, in the order it is processed. A restore of an archive
    /// run restores exactly these - objects archived on their own earlier stay archived.
    /// </summary>
    public class ObjectOperationItem : IEntity
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
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the run (deleted with it).
        /// </summary>
        public Guid OperationId { get; set; }

        /// <summary>
        /// Gets or sets the object - no foreign key: a delete run outlives what it deleted.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the processing order.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets whether the object is done.
        /// </summary>
        public bool Done { get; set; }

        /// <summary>
        /// Gets or sets the object the item produced (a clone: the copy), so a resumed run maps
        /// the children of an item onto the copy made before the interruption.
        /// </summary>
        public Guid? ResultId { get; set; }
    }

    /// <summary>
    /// The states of an <see cref="ObjectOperation"/>.
    /// </summary>
    public enum ObjectOperationState
    {
        /// <summary>The run waits for the worker.</summary>
        Pending,

        /// <summary>The run is being processed.</summary>
        Running,

        /// <summary>Every object is done.</summary>
        Completed,

        /// <summary>The run stopped at an object it could not process; what is done stays done.</summary>
        Failed
    }

    /// <summary>
    /// The kinds of an <see cref="ObjectOperation"/>.
    /// </summary>
    public static class ObjectOperationKinds
    {
        /// <summary>Archive the subtree (always with its contents).</summary>
        public const string Archive = "archive";

        /// <summary>Restore exactly what an archive run archived.</summary>
        public const string Restore = "restore";

        /// <summary>Delete the subtree, children before parents.</summary>
        public const string Delete = "delete";

        /// <summary>Move the direct children to the parent, then delete the object.</summary>
        public const string Dissolve = "dissolve";

        /// <summary>Clone the object alone or with its contents.</summary>
        public const string Clone = "clone";

        /// <summary>Move an object (with its subtree) into another workspace.</summary>
        public const string Transfer = "transfer";
    }

    /// <summary>
    /// A former key of an object - left behind when the object moved into another workspace and
    /// got a new key there - so saved links, mentions and relations by key still find it.
    /// </summary>
    public class ObjectKeyAlias : IEntity
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
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the former key (unique: a key names one object, then or now).
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the object the key now leads to (deleted with it).
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets when the key was given up.
        /// </summary>
        public DateTime Created { get; set; }
    }
}
