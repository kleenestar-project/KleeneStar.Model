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
        Assets
    }

    /// <summary>
    /// Provides extension methods for <see cref="ObjectViewType"/>.
    /// </summary>
    public static class ObjectViewTypeExtensions
    {
        /// <summary>
        /// Returns the stable string identifier of the view type. The identifier is the
        /// client-side id of the corresponding tab template fragment in
        /// <c>KleeneStar.Core</c>: the fragment's full type name, lower-cased, with dots
        /// replaced by dashes (the id under which <c>FragmentControlDataTabTemplate</c>
        /// renders its <c>&lt;template&gt;</c> element). <see cref="ObjectViewType.Table"/>
        /// and <see cref="ObjectViewType.List"/> deliberately share the composite
        /// <c>ObjectTabViewTemplateFragment</c>, which hosts the switchable
        /// table/tile/list object view.
        /// </summary>
        /// <param name="type">The view type.</param>
        /// <returns>The string identifier.</returns>
        public static string TemplateId(this ObjectViewType type)
        {
            return type switch
            {
                ObjectViewType.Table => "kleenestar-core-webfragment-object-objecttabviewtemplatefragment",
                ObjectViewType.List => "kleenestar-core-webfragment-object-objecttabviewtemplatefragment",
                ObjectViewType.Dashboard => "kleenestar-core-webfragment-object-objecttabdashboardtemplatefragment",
                ObjectViewType.Kanban => "kleenestar-core-webfragment-object-objecttabkanbantemplatefragment",
                ObjectViewType.ScrumSprint => "kleenestar-core-webfragment-object-objecttabscrumsprinttemplatefragment",
                ObjectViewType.ScrumBacklog => "kleenestar-core-webfragment-object-objecttabscrumbacklogtemplatefragment",
                ObjectViewType.Issues => "kleenestar-core-webfragment-object-issues-issueviewtemplatefragment",
                ObjectViewType.Assets => "kleenestar-core-webfragment-object-assets-assetviewtemplatefragment",
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
                ObjectViewType.Issues => Guid.Parse("7A2C41F8-90DE-4B6B-8D3A-1E5F72C4AE07"),
                ObjectViewType.Assets => Guid.Parse("8B3D52A9-A1EF-4C7C-9E4B-2F6A83D5BF08"),
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
                _ => null
            };
        }
    }
}
