namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// The keys of the tab types the core ships for an insight (<see cref="InsightView.ViewType"/>).
    /// </summary>
    /// <remarks>
    /// This is not the catalog of tab types - that is the core's open registry, which a plugin
    /// extends without touching this list. The keys are written down here because the seeder and
    /// the migration that turned the dashboards into tabs have to name them too.
    /// </remarks>
    public static class InsightViewTypes
    {
        /// <summary>
        /// The objects as a table or a list, with search, quickfilters and paging.
        /// </summary>
        public const string Objects = "objects";

        /// <summary>
        /// The dashboard of the insight: columns of freely arranged widgets.
        /// </summary>
        public const string Dashboard = Insight.DashboardType;

        /// <summary>
        /// A Kanban board of the objects.
        /// </summary>
        public const string Kanban = "kanban";

        /// <summary>
        /// The sprint board and the backlog of the sprints the objects are planned in.
        /// </summary>
        public const string Scrum = "scrum";

        /// <summary>
        /// The objects on a timeline.
        /// </summary>
        public const string Gantt = "gantt";

        /// <summary>
        /// The objects on a month, week or agenda grid.
        /// </summary>
        public const string Calendar = "calendar";

        /// <summary>
        /// Charts computed from the history of the objects: cumulative flow, velocity, average
        /// age, created vs. resolved and resolution time.
        /// </summary>
        public const string Reports = "reports";
    }
}
