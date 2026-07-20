using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for <see cref="ObjectViewTypeExtensions"/>. These lock the contract that
    /// the objects tab control and its <see cref="ObjectView"/>-backed REST endpoint depend on:
    /// every view type maps to a stable, unique template id/icon/label/description and the
    /// template id round-trips back to the same view type.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestObjectViewType
    {
        /// <summary>
        /// Verifies that every view type exposes a non-null tab-template id.
        /// </summary>
        /// <param name="type">The view type under test.</param>
        [Theory]
        [InlineData(ObjectViewType.Table)]
        [InlineData(ObjectViewType.List)]
        [InlineData(ObjectViewType.Dashboard)]
        [InlineData(ObjectViewType.Kanban)]
        [InlineData(ObjectViewType.ScrumSprint)]
        [InlineData(ObjectViewType.ScrumBacklog)]
        [InlineData(ObjectViewType.Issues)]
        public void TemplateId_ReturnsNonNull(ObjectViewType type)
        {
            Assert.False(string.IsNullOrWhiteSpace(type.TemplateId()));
        }

        /// <summary>
        /// Verifies that each view type maps to the client-side id of its tab template
        /// fragment in <c>KleeneStar.Core</c> (full type name, lower-cased, dots replaced by
        /// dashes). These ids must match the <c>&lt;template&gt;</c> element ids rendered by
        /// <c>FragmentControlDataTabTemplate</c>, otherwise the client tab control falls back
        /// to the first registered template. <see cref="ObjectViewType.Table"/> and
        /// <see cref="ObjectViewType.List"/> deliberately share the composite object view
        /// template.
        /// </summary>
        /// <param name="type">The view type under test.</param>
        /// <param name="expected">The expected tab-template fragment id.</param>
        [Theory]
        [InlineData(ObjectViewType.Table, "kleenestar-core-webfragment-object-objecttabviewtemplatefragment")]
        [InlineData(ObjectViewType.List, "kleenestar-core-webfragment-object-objecttabviewtemplatefragment")]
        [InlineData(ObjectViewType.Dashboard, "kleenestar-core-webfragment-object-objecttabdashboardtemplatefragment")]
        [InlineData(ObjectViewType.Kanban, "kleenestar-core-webfragment-object-objecttabkanbantemplatefragment")]
        [InlineData(ObjectViewType.ScrumSprint, "kleenestar-core-webfragment-object-objecttabscrumsprinttemplatefragment")]
        [InlineData(ObjectViewType.ScrumBacklog, "kleenestar-core-webfragment-object-objecttabscrumbacklogtemplatefragment")]
        [InlineData(ObjectViewType.Issues, "kleenestar-core-webfragment-object-issues-issueviewtemplatefragment")]
        public void TemplateId_MatchesTabTemplateFragmentId(ObjectViewType type, string expected)
        {
            Assert.Equal(expected, type.TemplateId());
        }

        /// <summary>
        /// Verifies that a template id resolves back to its canonical view type, mirroring the
        /// first-match mapping the objects tab REST endpoint performs when creating a view.
        /// <see cref="ObjectViewType.List"/> shares its template with
        /// <see cref="ObjectViewType.Table"/> and therefore resolves to Table.
        /// </summary>
        /// <param name="type">The view type under test.</param>
        /// <param name="expected">The view type the template id is expected to resolve to.</param>
        [Theory]
        [InlineData(ObjectViewType.Table, ObjectViewType.Table)]
        [InlineData(ObjectViewType.List, ObjectViewType.Table)]
        [InlineData(ObjectViewType.Dashboard, ObjectViewType.Dashboard)]
        [InlineData(ObjectViewType.Kanban, ObjectViewType.Kanban)]
        [InlineData(ObjectViewType.ScrumSprint, ObjectViewType.ScrumSprint)]
        [InlineData(ObjectViewType.ScrumBacklog, ObjectViewType.ScrumBacklog)]
        [InlineData(ObjectViewType.Issues, ObjectViewType.Issues)]
        public void TemplateId_ResolvesToCanonicalType(ObjectViewType type, ObjectViewType expected)
        {
            var templateId = type.TemplateId();

            var resolved = Enum.GetValues<ObjectViewType>()
                .First(x => string.Equals(x.TemplateId(), templateId, StringComparison.OrdinalIgnoreCase));

            Assert.Equal(expected, resolved);
        }

        /// <summary>
        /// Verifies that every view type exposes a non-null label resource key.
        /// </summary>
        /// <param name="type">The view type under test.</param>
        [Theory]
        [InlineData(ObjectViewType.Table)]
        [InlineData(ObjectViewType.List)]
        [InlineData(ObjectViewType.Dashboard)]
        [InlineData(ObjectViewType.Kanban)]
        [InlineData(ObjectViewType.ScrumSprint)]
        [InlineData(ObjectViewType.ScrumBacklog)]
        [InlineData(ObjectViewType.Issues)]
        public void Text_ReturnsNonNull(ObjectViewType type)
        {
            Assert.False(string.IsNullOrWhiteSpace(type.Text()));
        }

        /// <summary>
        /// Verifies that every view type exposes a non-null description resource key for the
        /// "+ add view" template picker.
        /// </summary>
        /// <param name="type">The view type under test.</param>
        [Theory]
        [InlineData(ObjectViewType.Table)]
        [InlineData(ObjectViewType.List)]
        [InlineData(ObjectViewType.Dashboard)]
        [InlineData(ObjectViewType.Kanban)]
        [InlineData(ObjectViewType.ScrumSprint)]
        [InlineData(ObjectViewType.ScrumBacklog)]
        [InlineData(ObjectViewType.Issues)]
        public void Description_ReturnsNonNull(ObjectViewType type)
        {
            Assert.False(string.IsNullOrWhiteSpace(type.Description()));
        }

        /// <summary>
        /// Verifies that the label and description keys differ, so the picker never shows the
        /// same string for both lines.
        /// </summary>
        /// <param name="type">The view type under test.</param>
        [Theory]
        [InlineData(ObjectViewType.Table)]
        [InlineData(ObjectViewType.List)]
        [InlineData(ObjectViewType.Dashboard)]
        [InlineData(ObjectViewType.Kanban)]
        [InlineData(ObjectViewType.ScrumSprint)]
        [InlineData(ObjectViewType.ScrumBacklog)]
        [InlineData(ObjectViewType.Issues)]
        public void Text_And_Description_Differ(ObjectViewType type)
        {
            Assert.NotEqual(type.Text(), type.Description());
        }

        /// <summary>
        /// Verifies that every view type exposes a non-null icon for the tab header and picker.
        /// </summary>
        /// <param name="type">The view type under test.</param>
        [Theory]
        [InlineData(ObjectViewType.Table)]
        [InlineData(ObjectViewType.List)]
        [InlineData(ObjectViewType.Dashboard)]
        [InlineData(ObjectViewType.Kanban)]
        [InlineData(ObjectViewType.ScrumSprint)]
        [InlineData(ObjectViewType.ScrumBacklog)]
        [InlineData(ObjectViewType.Issues)]
        public void Icon_ReturnsNonNull(ObjectViewType type)
        {
            Assert.NotNull(type.Icon());
        }

        /// <summary>
        /// Verifies that every view type exposes a non-empty, unique well-known id.
        /// </summary>
        [Fact]
        public void Id_ReturnsNonEmptyUniqueGuids()
        {
            var ids = Enum.GetValues<ObjectViewType>()
                .Select(x => x.Id())
                .ToList();

            Assert.DoesNotContain(Guid.Empty, ids);
            Assert.Equal(ids.Count, ids.Distinct().Count());
        }
    }
}
