using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model.Transfer
{
    /// <summary>
    /// What a table is to the transfer of workspaces.
    /// </summary>
    public enum WorkspaceTransferRole
    {
        /// <summary>
        /// The workspaces themselves: the rows the transfer starts from.
        /// </summary>
        Root,

        /// <summary>
        /// Data of a workspace: a row belongs to an export when one of its owners does, and is
        /// written in full.
        /// </summary>
        Content,

        /// <summary>
        /// Installation-wide reference data a workspace needs (categories, status categories,
        /// relation types): only the rows the content refers to are written; an import reuses a
        /// row of the same id or natural key and creates it only when there is none.
        /// </summary>
        Shared,

        /// <summary>
        /// Data that belongs to the installation and is never exported (accounts, groups,
        /// tenants): only the id and the name of a referenced row are written, so an import can
        /// map the reference onto the row of the same id or name, or leave it empty.
        /// </summary>
        External,

        /// <summary>
        /// Not part of a workspace export: per-user state, logs, runtime queues, indexes that
        /// are rebuilt, and the configuration of the installation.
        /// </summary>
        Excluded
    }

    /// <summary>
    /// A condition under which a row belongs to an export: the value of <see cref="Property"/>
    /// is one of the values <see cref="SourceProperty"/> has in the rows of <see cref="Source"/>
    /// that belong to it.
    /// </summary>
    /// <remarks>
    /// The condition is stated in the terms of the export file, where a reference always names
    /// the id of the row it points to. A foreign key on a join table that points to a database
    /// id is translated by the engine.
    /// </remarks>
    /// <param name="Property">The property of the table the condition is on.</param>
    /// <param name="Source">The table whose rows give the values.</param>
    /// <param name="SourceProperty">The property of the source table.</param>
    public sealed record WorkspaceTransferLink(string Property, string Source, string SourceProperty);

    /// <summary>
    /// A reference that the database does not declare as a foreign key, but which points to a
    /// row of another table all the same.
    /// </summary>
    /// <param name="Property">The property holding the reference.</param>
    /// <param name="Target">The table referred to.</param>
    /// <param name="TargetProperty">The property of the target the value names.</param>
    public sealed record WorkspaceTransferReference(string Property, string Target, string TargetProperty = "Id");

    /// <summary>
    /// The description of one table for the transfer of workspaces.
    /// </summary>
    public sealed class WorkspaceTransferTable
    {
        /// <summary>
        /// Returns the name of the entity (the class name of an entity type, the name of a join
        /// table).
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Returns what the table is to the transfer.
        /// </summary>
        public WorkspaceTransferRole Role { get; init; }

        /// <summary>
        /// Returns the conditions under which a row belongs to an export; any one of them is
        /// enough. Only content tables have owners.
        /// </summary>
        public IReadOnlyList<WorkspaceTransferLink> Owners { get; init; } = [];

        /// <summary>
        /// Returns the references the database does not declare as foreign keys.
        /// </summary>
        public IReadOnlyList<WorkspaceTransferReference> References { get; init; } = [];

        /// <summary>
        /// Returns the properties a row of a shared or external table is recognised by when the
        /// target installation holds it under another id (compared ignoring case).
        /// </summary>
        public IReadOnlyList<string> MatchBy { get; init; } = [];

        /// <summary>
        /// Returns the optional references without which a row loses its meaning: when the row
        /// they name is neither imported nor present, the row is left out instead of having the
        /// reference cleared.
        /// </summary>
        public IReadOnlyList<string> Requires { get; init; } = [];

        /// <summary>
        /// Returns why an excluded table is not exported.
        /// </summary>
        public string Reason { get; init; }
    }

    /// <summary>
    /// The classification of every table of the model for the export and import of workspaces.
    /// </summary>
    /// <remarks>
    /// Every entity type of the model has exactly one entry (<c>UnitTestWorkspaceTransfer</c>
    /// checks it), so a new table cannot silently stay out of an export or slip into one: its
    /// author decides. Foreign keys are read from the model; only memberships and references the
    /// database does not declare are listed here.
    /// </remarks>
    public static class WorkspaceTransferCatalog
    {
        private static readonly IReadOnlyList<WorkspaceTransferTable> _tables =
        [
            // the root
            Root("Workspace", new WorkspaceTransferReference("HomeId", "Object")),

            // workspace level
            Owned("WorkspaceCategory", "WorkspaceId", "Workspace"),
            Owned("WorkspaceTenant", "WorkspaceId", "Workspace"),
            Owned("WorkspacePlanning", "WorkspaceId", "Workspace"),
            Owned("Class", "WorkspaceId", "Workspace"),
            Owned("ObjectView", "WorkspaceId", "Workspace"),
            Owned("Sprint", "WorkspaceId", "Workspace"),
            Owned("Milestone", "WorkspaceId", "Workspace"),
            Owned("ApprovalPolicy", "WorkspaceId", "Workspace", new WorkspaceTransferReference("CreatedById", "Identity")),
            Owned("TemplateHierarchy", "WorkspaceId", "Workspace"),
            Owned("AutomationRule", "WorkspaceId", "Workspace", new WorkspaceTransferReference("RunAsId", "Identity"), new WorkspaceTransferReference("CreatedById", "Identity")),
            Owned("Webhook", "WorkspaceId", "Workspace"),
            Owned("AppointmentCalendarSource", "WorkspaceId", "Workspace", new WorkspaceTransferReference("CreatedById", "Identity")),
            Owned("KanbanBoard", "WorkspaceId", "Workspace"),
            Owned("KindDashboard", "WorkspaceId", "Workspace"),
            new WorkspaceTransferTable
            {
                Name = "ValueCatalog",
                Role = WorkspaceTransferRole.Content,
                // a catalog of the workspace, or an installation-wide one a field of it is bound to
                Owners = [new("WorkspaceId", "Workspace", "Id"), new("Id", "FieldCatalogBinding", "CatalogId")],
                References = [new("CreatedById", "Identity")]
            },
            Owned("ValueCatalogVersion", "CatalogId", "ValueCatalog", new WorkspaceTransferReference("CreatedById", "Identity")),

            // class level
            Owned("ClassAllowedChild", "ClassId", "Class"),
            Owned("Field", "ClassId", "Class"),
            Owned("FieldCatalogBinding", "FieldId", "Field"),
            Owned("Priority", "ClassId", "Class"),
            Owned("SecurityLevel", "ClassId", "Class", new WorkspaceTransferReference("PermittedGroupIds", "Group")),
            Owned("Status", "ClassId", "Class"),
            Owned("Workflow", "ClassId", "Class"),
            Owned("WorkflowStatus", "WorkflowId", "Workflow"),
            Owned("Transition", "WorkflowId", "Workflow"),
            Owned("Form", "ClassId", "Class"),
            Owned("FormTab", "FormId", "Form"),
            Owned("FormElement", "FormTabId", "FormTab"),
            Owned("Template", "ClassId", "Class"),
            Owned("TemplateClass", "TemplateId", "Template"),
            Owned("TemplateHierarchyNode", "HierarchyId", "TemplateHierarchy"),
            Owned("Calendar", "ClassId", "Class"),
            Owned("BusinessHourSlot", "CalendarId", "Calendar"),
            Owned("Holiday", "CalendarId", "Calendar"),
            Owned("SlaPolicy", "ClassId", "Class"),
            Owned("SlaTarget", "PolicyId", "SlaPolicy"),
            Owned("SlaScopeRule", "PolicyId", "SlaPolicy"),
            Owned("SlaEscalationLevel", "PolicyId", "SlaPolicy"),
            Owned("KanbanBoardColumn", "BoardId", "KanbanBoard", new WorkspaceTransferReference("CategoryId", "StatusCategory")),
            Owned("KanbanBoardSwimlane", "BoardId", "KanbanBoard"),
            Owned("KindDashboardColumn", "BoardId", "KindDashboard"),
            Owned("KindDashboardWidget", "ColumnId", "KindDashboardColumn"),

            // the grants of the workspace, its classes, objects and calendars
            new WorkspaceTransferTable
            {
                Name = "PermissionAssignment",
                Role = WorkspaceTransferRole.Content,
                Owners =
                [
                    new("ScopeId", "Workspace", "Id"),
                    new("ScopeId", "Class", "Id"),
                    new("ScopeId", "Object", "Id"),
                    new("ScopeId", "Calendar", "Id")
                ]
            },

            // object level
            Owned("Object", "WorkspaceId", "Workspace"),
            Owned("Value", "ObjectId", "Object"),
            Owned("Commit", "ObjectId", "Object", new WorkspaceTransferReference("CreatedById", "Identity")),
            Owned("Change", "CommitId", "Commit"),
            Owned("Attachment", "ObjectId", "Object"),
            Owned("Comment", "ObjectId", "Object"),
            Owned("CommentLike", "CommentId", "Comment"),
            Owned("CommentReaction", "CommentId", "Comment"),
            Owned("ObjectDraft", "ObjectId", "Object"),
            Owned("ObjectKeyAlias", "ObjectId", "Object"),
            Owned("ObjectPlanning", "ObjectId", "Object"),
            // an empty target means an external relation: a relation whose target is not there is
            // left out rather than turned into one
            new WorkspaceTransferTable
            {
                Name = "ObjectRelation",
                Role = WorkspaceTransferRole.Content,
                Owners = [new("SourceObjectId", "Object", "Id")],
                References = [new("TypeKey", "ObjectRelationType", "Key")],
                Requires = ["TargetObjectId"]
            },
            Owned("ObjectShare", "ObjectId", "Object"),
            Owned("ObjectTag", "ObjectId", "Object"),
            Owned("Worklog", "ObjectId", "Object", new WorkspaceTransferReference("IdentityId", "Identity")),
            Owned("Approval", "ObjectId", "Object", new WorkspaceTransferReference("RequestedById", "Identity")),
            Owned("ApprovalDecision", "ApprovalId", "Approval", new WorkspaceTransferReference("IdentityId", "Identity")),
            Owned("Poll", "ObjectId", "Object", new WorkspaceTransferReference("ClosedById", "Identity")),
            Owned("PollOption", "ObjectId", "Object"),
            Owned("PollVote", "OptionId", "PollOption", new WorkspaceTransferReference("IdentityId", "Identity")),
            Owned("Appointment", "ObjectId", "Object", new WorkspaceTransferReference("OrganizerId", "Identity")),
            Owned("AppointmentParticipant", "ObjectId", "Object", new WorkspaceTransferReference("IdentityId", "Identity")),
            Owned("AppointmentResource", "ObjectId", "Object"),

            // reference data of the installation a workspace needs
            Shared("Category", "Name"),
            Shared("StatusCategory", "Name"),
            Shared("ObjectRelationType", "Key"),

            // data of the installation: referred to by id and name only
            External("Identity", "UserName"),
            External("Group", "Name"),
            External("Tenant", "Name"),

            // not part of a workspace
            Excluded("AccessToken", "credential of an account"),
            Excluded("PasswordReset", "credential of an account"),
            Excluded("IdentitySession", "session of an account"),
            Excluded("UserSession", "per-user setting"),
            Excluded("UserNotification", "per-user notification"),
            Excluded("WqlHistory", "per-user search history"),
            Excluded("SavedSearch", "per-user search"),
            Excluded("CustomQuickfilter", "per-user quickfilter"),
            Excluded("WorkspaceBookmark", "per-user bookmark"),
            Excluded("ObjectVisit", "per-user visit"),
            Excluded("ObjectWatcher", "per-user subscription"),
            Excluded("IdentityCapacity", "planning figure of an account"),
            Excluded("IdentityGroupMembership", "membership of an account"),
            Excluded("GroupPolicy", "policy of a group"),
            Excluded("AuditEvent", "the audit log is append-only and belongs to the installation"),
            Excluded("AuditDelta", "the audit log is append-only and belongs to the installation"),
            Excluded("AutomationJob", "runtime queue"),
            Excluded("ObjectCreation", "runtime record of a creation run"),
            Excluded("ObjectOperation", "runtime record of an operation"),
            Excluded("ObjectOperationItem", "runtime record of an operation"),
            Excluded("ObjectPath", "index, rebuilt after an import"),
            Excluded("Insight", "insights span workspaces"),
            Excluded("InsightCategory", "insights span workspaces"),
            Excluded("InsightView", "insights span workspaces"),
            Excluded("DashboardColumn", "insights span workspaces"),
            Excluded("Widget", "insights span workspaces"),
            Excluded("Branding", "configuration of the installation"),
            Excluded("Maintenance", "configuration of the installation"),
            Excluded("NavigatorLink", "configuration of the installation")
        ];

        private static readonly Dictionary<string, WorkspaceTransferTable> _byName =
            _tables.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Returns every classified table.
        /// </summary>
        public static IReadOnlyList<WorkspaceTransferTable> Tables => _tables;

        /// <summary>
        /// Returns the entry of a table, or <see langword="null"/> when the name is unknown.
        /// </summary>
        /// <param name="name">The entity name (compared ignoring case).</param>
        /// <returns>The entry, or <see langword="null"/>.</returns>
        public static WorkspaceTransferTable Find(string name)
        {
            return name is not null && _byName.TryGetValue(name, out var table) ? table : null;
        }

        private static WorkspaceTransferTable Root(string name, params WorkspaceTransferReference[] references)
        {
            return new WorkspaceTransferTable { Name = name, Role = WorkspaceTransferRole.Root, References = references };
        }

        private static WorkspaceTransferTable Owned(string name, string property, string owner, params WorkspaceTransferReference[] references)
        {
            return new WorkspaceTransferTable
            {
                Name = name,
                Role = WorkspaceTransferRole.Content,
                Owners = [new WorkspaceTransferLink(property, owner, "Id")],
                References = references
            };
        }

        private static WorkspaceTransferTable Shared(string name, params string[] matchBy)
        {
            return new WorkspaceTransferTable { Name = name, Role = WorkspaceTransferRole.Shared, MatchBy = matchBy };
        }

        private static WorkspaceTransferTable External(string name, params string[] matchBy)
        {
            return new WorkspaceTransferTable { Name = name, Role = WorkspaceTransferRole.External, MatchBy = matchBy };
        }

        private static WorkspaceTransferTable Excluded(string name, string reason)
        {
            return new WorkspaceTransferTable { Name = name, Role = WorkspaceTransferRole.Excluded, Reason = reason };
        }
    }
}
