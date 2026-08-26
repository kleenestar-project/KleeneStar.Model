using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// The label that pins an object to the landing page.
        /// </summary>
        /// <remarks>
        /// The landing page reads these four labels through
        /// <c>KleeneStar.Core.WebFragment.Landing.LandingLabel</c>. They are repeated here
        /// rather than referenced because the model layer does not know the core plugin; the
        /// strings have to stay in step, which is why they are named in one place on each side
        /// and nowhere else.
        /// </remarks>
        private const string LandingLabelPinned = "Pinned";

        /// <summary>
        /// The label of the compact how-to pages of the help area.
        /// </summary>
        private const string LandingLabelHelp = "Help";

        /// <summary>
        /// The label of the frequently-asked-questions pages.
        /// </summary>
        private const string LandingLabelFaq = "FAQ";

        /// <summary>
        /// The label of the first-steps pages.
        /// </summary>
        private const string LandingLabelFirstSteps = "First Steps";

        /// <summary>
        /// The pages the help area of the landing page shows out of the box, as
        /// (key, label, summary, description).
        /// </summary>
        /// <remarks>
        /// One page per entry rather than one page per column: the three columns read their
        /// pages as a list of how-tos, a list of questions and a checklist of first steps, so a
        /// single page holding everything would collapse each column into one row. The keys sit
        /// in a range of their own (9000+) so they cannot collide with the per-class ranges
        /// <c>SeedObjects</c> hands out.
        /// <para>
        /// The descriptions are written as plain prose. They are read inside a card - the FAQ
        /// answer opens directly beneath its question - and markup would be shown as written.
        /// </para>
        /// </remarks>
        private static readonly (string Key, string Label, string Summary, string Description)[] LandingPages =
        [
            (
                "SD-9000", LandingLabelHelp,
                "Create an issue",
                "Use the create button in the header, pick the class the issue belongs to, and fill in the summary. Everything else can follow later - the workflow asks for what it needs when it needs it."
            ),
            (
                "SD-9001", LandingLabelHelp,
                "Work with templates",
                "A template pre-fills a form so only what is specific to your case is left to write. Templates belong to a class and are offered when you create an issue of that class."
            ),
            (
                "SD-9002", LandingLabelHelp,
                "Assign and hand over an issue",
                "Set the assignee on the people card of the issue. Handing over means changing that field; the previous assignee keeps seeing the issue in their history, and the new one finds it under my issues."
            ),
            (
                "SD-9003", LandingLabelHelp,
                "Filter and save a view",
                "Narrow a list with the quick filters or the search field, then save the result as a saved search. A saved view belongs to you until you share it."
            ),
            (
                "SD-9004", LandingLabelHelp,
                "Comments and attachments",
                "Every issue carries a comment thread and a list of attachments. Both are part of the record: they are versioned with the issue and visible to everybody who can see it."
            ),
            (
                "SD-9005", LandingLabelFaq,
                "Who can see my issue?",
                "Everybody with access to the workspace the issue lives in. Fields marked confidential stay restricted to the responsible team."
            ),
            (
                "SD-9006", LandingLabelFaq,
                "What happens when a deadline passes?",
                "The SLA clock of the issue turns red and, depending on the policy, the issue is escalated to the next level. Nothing is deleted or closed automatically."
            ),
            (
                "SD-9007", LandingLabelFaq,
                "Can I replace this start page?",
                "Yes. Choose start page in the head of this page leads to the dashboards; any of them can take the place of this page for you."
            ),
            (
                "SD-9008", LandingLabelFaq,
                "How do I find an old issue?",
                "Search across all workspaces from the header, or open the issue list and reset the quick filters - a closed issue leaves the default filters but stays in the list."
            ),
            (
                "SD-9009", LandingLabelFirstSteps,
                "Your first issue in three steps",
                "Pick a workspace, press create, and describe what happened in one sentence. That is enough to start; the rest is filled in as the issue moves."
            ),
            (
                "SD-9010", LandingLabelFirstSteps,
                "Understand templates",
                "A template is a pre-filled form, not a separate kind of issue. What it fills in can be changed before saving."
            ),
            (
                "SD-9011", LandingLabelFirstSteps,
                "The basic functions at a glance",
                "Reading an issue, changing it, commenting on it, and following it. Everything else in KleeneStar is built from those four."
            ),
            (
                "SD-9012", LandingLabelFirstSteps,
                "Class, view, workspace",
                "A workspace holds the work of a team. A class says which fields an issue has. A view is one way of looking at a set of issues - a list, a board, a calendar."
            )
        ];

        /// <summary>
        /// Adds the pages the help area of the landing page shows out of the box.
        /// </summary>
        /// <remarks>
        /// They are ordinary document objects of the service-desk knowledge class, because that
        /// is what help is in KleeneStar: a page, editable, versioned and searchable like every
        /// other one. Each page is added only when its key is absent, so the set can grow later
        /// without disturbing the pages an installation already carries.
        /// </remarks>
        /// <param name="db">The database context to which the pages are added. Cannot be null.</param>
        private static void SeedLandingPages(KleeneStarDbContext db)
        {
            var knowledgeClassId = Guid.Parse("4F80098A-C986-415A-9288-1C85232C27C9");
            var serviceDeskWorkspaceId = Guid.Parse("F027A791-4219-4B1D-BA7C-2E7757091AAA");
            var creatorId = Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD");
            var icon = ImageIcon.FromString("/kleenestar/assets/icons/knowledge.svg");

            // without the class the workspace was not seeded either - nothing to attach to
            if (!db.Classes.AsNoTracking().Any(x => x.Id == knowledgeClassId))
            {
                return;
            }

            var known = db.Objects
                .AsNoTracking()
                .Where(x => x.Key.StartsWith("SD-9"))
                .Select(x => x.Key)
                .ToHashSet();

            foreach (var page in LandingPages)
            {
                if (known.Contains(page.Key))
                {
                    continue;
                }

                db.Objects.Add(new Entities.Object
                {
                    Id = Guid.NewGuid(),
                    Key = page.Key,
                    Summary = page.Summary,
                    Description = page.Description,
                    Icon = icon,
                    Kind = ObjectKind.Document,
                    State = WorkspaceState.Active,
                    WorkspaceId = serviceDeskWorkspaceId,
                    ClassId = knowledgeClassId,
                    CreatorId = creatorId,
                    UpdaterId = creatorId,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Attaches the reserved landing-page labels: each seeded help page gets the label of
        /// the column it belongs to, and a small set of documents is pinned.
        /// </summary>
        /// <remarks>
        /// This runs after <see cref="SeedLandingPages"/> has been committed, because a label
        /// names its object by id. The pinned set is picked by summary rather than by id: those
        /// documents are generated by <c>SeedObjects</c>, so their ids are not known here. A
        /// document that is not present is simply not pinned.
        /// </remarks>
        /// <param name="db">The database context to which the labels are added. Cannot be null.</param>
        private static void SeedLandingLabels(KleeneStarDbContext db)
        {
            void label(Guid objectId, string name, string color)
                => db.ObjectTags.Add(new ObjectTag
                {
                    ObjectId = objectId,
                    Name = name,
                    Color = color,
                    Created = DateTime.UtcNow
                });

            var labelsByKey = LandingPages.ToDictionary(x => x.Key, x => x.Label);
            var keys = labelsByKey.Keys.ToList();

            var pages = db.Objects
                .AsNoTracking()
                .Where(x => keys.Contains(x.Key))
                .Select(x => new { x.Id, x.Key })
                .ToList();

            var tagged = db.ObjectTags
                .AsNoTracking()
                .Select(x => new { x.ObjectId, x.Name })
                .ToHashSet();

            foreach (var page in pages)
            {
                var name = labelsByKey[page.Key];

                // the composite unique index on (ObjectId, Name) makes a second attempt fatal,
                // so an already labelled page is skipped rather than labelled again
                if (tagged.Contains(new { ObjectId = page.Id, Name = name }))
                {
                    continue;
                }

                label(page.Id, name, "#0dcaf0");
            }

            // the documents an organization would keep in reach: the big picture, the manual,
            // and the way in for a new colleague
            var pinnedSummaries = new[] { "Architecture Diagram", "User Manual", "Setup Guide" };

            var pinned = db.Objects
                .AsNoTracking()
                .Where(x => x.Kind == ObjectKind.Document && pinnedSummaries.Contains(x.Summary))
                .Select(x => x.Id)
                .ToList();

            foreach (var objectId in pinned)
            {
                if (tagged.Contains(new { ObjectId = objectId, Name = LandingLabelPinned }))
                {
                    continue;
                }

                label(objectId, LandingLabelPinned, "#fd7e14");
            }
        }
    }
}
