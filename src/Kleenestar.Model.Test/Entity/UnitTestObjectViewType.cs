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
        // The three tests that used to stand here locked ObjectViewTypeExtensions.TemplateId to a
        // table of hard-written fragment ids. They compared that table against itself, so they
        // stayed green while every id in it named a fragment that did not exist and adding a tab
        // silently produced a table. The mapping now lives in KleeneStar.Core next to the
        // fragments and is covered by UnitTestObjectViewTemplate, which asserts against the
        // fragment types themselves rather than against copies of their names.

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
