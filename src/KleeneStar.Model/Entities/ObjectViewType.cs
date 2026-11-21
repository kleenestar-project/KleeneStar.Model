using System;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Specifies the kind of view that an <see cref="ObjectView"/> renders inside the
    /// objects tab control of a workspace.
    /// </summary>
    public enum ObjectViewType
    {
        /// <summary>
        /// Tabular view of objects with sortable, filterable columns.
        /// </summary>
        Table,

        /// <summary>
        /// Compact one-line-per-item list view of objects.
        /// </summary>
        List,

        /// <summary>
        /// Dashboard view aggregating widgets that summarise the objects.
        /// </summary>
        Dashboard,

        /// <summary>
        /// Kanban board view that groups objects in columns by status.
        /// </summary>
        Kanban,

        /// <summary>
        /// Scrum sprint board for the current iteration.
        /// </summary>
        ScrumSprint,

        /// <summary>
        /// Scrum product backlog view.
        /// </summary>
        ScrumBacklog,

        /// <summary>
        /// Curated issue list: the most recently updated issues of the workspace with
        /// search, personal quickfilters (starred, assigned to me, created by me,
        /// archived), and pagination. Appended after <see cref="ScrumBacklog"/> because
        /// the enum is persisted by ordinal and therefore append-only.
        /// </summary>
        Issues,

        /// <summary>
        /// Curated asset list: the most recently updated assets of the workspace with
        /// search, personal quickfilters (starred, assigned to me, created by me,
        /// archived), and pagination. Appended after <see cref="Issues"/> because the
        /// enum is persisted by ordinal and therefore append-only.
        /// </summary>
        Assets,

        /// <summary>
        /// Gantt chart view that lays the objects out on a timeline, with the bars taken
        /// from the date fields of their class. Appended after <see cref="Assets"/>
        /// because the enum is persisted by ordinal and therefore append-only.
        /// </summary>
        Gantt,

        /// <summary>
        /// Calendar view that places the objects on a month, week or agenda grid by the
        /// date fields of their class. Appended after <see cref="Gantt"/> because the enum
        /// is persisted by ordinal and therefore append-only.
        /// </summary>
        Scheduler
    }

    /// <summary>
    /// Provides extension methods for <see cref="ObjectViewType"/>.
    /// </summary>
    public static class ObjectViewTypeExtensions
    {
        // The mapping from a view type to the tab template that renders it used to live here, as a
        // table of hard-written fragment ids. It named fragments that no longer existed, so no id
        // matched and every tab a user added came out as a table. Naming types of KleeneStar.Core
        // from the model could only ever be checked by eye, and the templates differ per object
        // kind anyway, so the mapping now lives next to those fragments in
        // KleeneStar.Core.WebFragment.Object.ObjectViewTemplate and is derived from the types.

        /// <summary>
        /// Returns the well-known identifier of the view type as a <see cref="Guid"/>
        /// for selection endpoints.
        /// </summary>
        public static Guid Id(this ObjectViewType type)
        {
            return type switch
            {
                ObjectViewType.Table => Guid.Parse("1F4E80AA-AC2B-4B73-9F12-1A0E15B8F901"),
                ObjectViewType.List => Guid.Parse("2C8B6A95-1F22-4E54-8E0E-AE15B4F1D902"),
                ObjectViewType.Dashboard => Guid.Parse("3D9E22C1-A2FA-4FCD-B731-D6A07F9C9E03"),
                ObjectViewType.Kanban => Guid.Parse("4A6F90BD-3E8C-46B5-9F2A-19D6E0AC8E04"),
                ObjectViewType.ScrumSprint => Guid.Parse("5B5C18D3-7AAB-481C-A8E6-A95F4D2B7E05"),
                ObjectViewType.ScrumBacklog => Guid.Parse("6E7D9342-5CBE-409B-9A1F-0C5BAD3E2F06"),
                ObjectViewType.Issues => Guid.Parse("7A2C41F8-90DE-4B6B-8D3A-1E5F72C4AE07"),
                ObjectViewType.Assets => Guid.Parse("8B3D52A9-A1EF-4C7C-9E4B-2F6A83D5BF08"),
                ObjectViewType.Gantt => Guid.Parse("9C4E63BA-B2FA-4D8D-AF5C-306B94E6C019"),
                ObjectViewType.Scheduler => Guid.Parse("AD5F74CB-C30B-4E9E-B06D-417CA5F7D12A"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the resource key label associated with the specified view type.
        /// </summary>
        public static string Text(this ObjectViewType type)
        {
            return type switch
            {
                ObjectViewType.Table => "kleenestar.core:object.view.table.label",
                ObjectViewType.List => "kleenestar.core:object.view.list.label",
                ObjectViewType.Dashboard => "kleenestar.core:object.view.dashboard.label",
                ObjectViewType.Kanban => "kleenestar.core:object.view.kanban.label",
                ObjectViewType.ScrumSprint => "kleenestar.core:object.view.scrum.sprint.label",
                ObjectViewType.ScrumBacklog => "kleenestar.core:object.view.scrum.backlog.label",
                ObjectViewType.Issues => "kleenestar.core:object.view.issues.label",
                ObjectViewType.Assets => "kleenestar.core:object.view.assets.label",
                ObjectViewType.Gantt => "kleenestar.core:object.view.gantt.label",
                ObjectViewType.Scheduler => "kleenestar.core:object.view.scheduler.label",
                _ => null
            };
        }

        /// <summary>
        /// Returns the resource key of the short description shown for the view type in the
        /// "add view" template picker of the objects tab control.
        /// </summary>
        /// <param name="type">The view type.</param>
        /// <returns>The description resource key.</returns>
        public static string Description(this ObjectViewType type)
        {
            return type switch
            {
                ObjectViewType.Table => "kleenestar.core:object.view.table.description",
                ObjectViewType.List => "kleenestar.core:object.view.list.description",
                ObjectViewType.Dashboard => "kleenestar.core:object.view.dashboard.description",
                ObjectViewType.Kanban => "kleenestar.core:object.view.kanban.description",
                ObjectViewType.ScrumSprint => "kleenestar.core:object.view.scrum.sprint.description",
                ObjectViewType.ScrumBacklog => "kleenestar.core:object.view.scrum.backlog.description",
                ObjectViewType.Issues => "kleenestar.core:object.view.issues.description",
                ObjectViewType.Assets => "kleenestar.core:object.view.assets.description",
                ObjectViewType.Gantt => "kleenestar.core:object.view.gantt.description",
                ObjectViewType.Scheduler => "kleenestar.core:object.view.scheduler.description",
                _ => null
            };
        }

        /// <summary>
        /// Returns the icon associated with the specified view type.
        /// </summary>
        public static IIcon Icon(this ObjectViewType type)
        {
            return type switch
            {
                ObjectViewType.Table => new IconTable(),
                ObjectViewType.List => new IconList(),
                ObjectViewType.Dashboard => new IconDashboard(),
                ObjectViewType.Kanban => new IconColumns(),
                ObjectViewType.ScrumSprint => new IconBolt(),
                ObjectViewType.ScrumBacklog => new IconListCheck(),
                ObjectViewType.Issues => new IconClipboardList(),
                ObjectViewType.Assets => new IconCubes(),
                ObjectViewType.Gantt => new IconChartGantt(),
                ObjectViewType.Scheduler => new IconCalendarDays(),
                _ => null
            };
        }
    }
}
