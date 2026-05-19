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
        ScrumBacklog
    }

    /// <summary>
    /// Provides extension methods for <see cref="ObjectViewType"/>.
    /// </summary>
    public static class ObjectViewTypeExtensions
    {
        /// <summary>
        /// Returns the stable string identifier of the view type. The identifier matches
        /// the <c>TemplateId</c> of the corresponding tab template fragment.
        /// </summary>
        /// <param name="type">The view type.</param>
        /// <returns>The string identifier.</returns>
        public static string TemplateId(this ObjectViewType type)
        {
            return type switch
            {
                ObjectViewType.Table => "tab-objects-table",
                ObjectViewType.List => "tab-objects-list",
                ObjectViewType.Dashboard => "tab-objects-dashboard",
                ObjectViewType.Kanban => "tab-objects-kanban",
                ObjectViewType.ScrumSprint => "tab-objects-scrum-sprint",
                ObjectViewType.ScrumBacklog => "tab-objects-scrum-backlog",
                _ => null
            };
        }

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
                _ => null
            };
        }
    }
}
