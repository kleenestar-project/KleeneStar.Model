using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// The home pages of the seeded workspaces: the workspace, the id of the <c>Page</c> class
        /// written for a workspace that has no prose document class, and the body.
        /// </summary>
        /// <remarks>
        /// The home page is the dedicated document of a workspace (<see cref="Workspace.HomeId"/>).
        /// It is not part of the page tree, takes no comments and is read and edited on the
        /// document overview. The seeded workspaces correspond to the shipped workspace templates;
        /// their home pages carry the English content of the templates
        /// (<c>KleeneStar.Templates</c>, <c>WorkspaceTemplateBase.HomePage</c>). They carry no
        /// banner, because the banner address is a route of the core application, which the model
        /// does not know.
        /// </remarks>
        private static readonly (string WorkspaceId, string PageClassId, string Body)[] WorkspaceHomes =
        [
            (
                "F027A791-4219-4B1D-BA7C-2E7757091AAA",
                "734D29A7-5E80-5117-A335-190EE28EA84D",
                "<p>This is the IT service desk, the single point of contact for everything IT. Incidents bring services back, service requests deliver what was asked for, problems remove the causes, and changes are approved before they go live.</p>"
                + "<h2>How to reach us</h2>"
                + "<p>Customers report through the customer portal; the service team works every request here.</p>"
                + "<ul><li>Something is broken or slow: report an incident.</li><li>You need access, hardware or software: file a service request.</li><li>Look in the knowledge base first, many questions are answered there.</li></ul>"
                + "<h2>How the team works</h2>"
                + "<ol><li>Every new ticket is categorized and prioritized within the first hour.</li><li>Incidents are worked by priority, and the service level clock shows how much time is left.</li><li>Recurring incidents are linked to a problem, whose root cause is removed with a change.</li><li>Changes pass the change advisory board before they are scheduled for a release.</li></ol>"
            ),
            (
                "660E9B11-2D54-4A36-84F9-F3BF5C78B748",
                "F260FCCD-31FB-5A13-AD61-E4441F9B4636",
                "<p>This is where the software is built. Tasks and bugs are planned in sprints, the code lives in the linked repositories, and the build pipelines report back here.</p>"
                + "<h2>How we work</h2>"
                + "<ol><li>Plan: new work is written as a task, a defect as a bug, and both land in the backlog.</li><li>Commit: the sprint takes what the team can finish in two weeks.</li><li>Build: every merge runs the build pipeline, and a red build is fixed before new work starts.</li><li>Release: what is done goes out with the next release and is announced in the release notes.</li></ol>"
                + "<h2>Rules of thumb</h2>"
                + "<ul><li>A bug report says what was expected, what happened and how to reproduce it.</li><li>Documentation is kept next to the work it describes, in the page tree of this workspace.</li><li>Specifications are captured through their own form, so they all answer the same questions.</li></ul>"
            ),
            (
                "D651799A-690E-4CFF-AE0C-D3341CA3BBB4",
                "442EC8C3-8A7D-5284-B9E6-FCEAF25E9479",
                "<p>The configuration management database keeps the configuration items of the IT landscape and how they depend on each other, so the impact of a change or an outage is known before it happens.</p>"
                + "<h2>What belongs here</h2>"
                + "<ul><li>Every configuration item that is managed: servers, applications, network components and licences.</li><li>The relationships between them, so dependencies can be followed in both directions.</li><li>The policies and compliance rules the landscape is held to, and the vulnerabilities found in it.</li></ul>"
                + "<h2>Keeping it accurate</h2>"
                + "<ol><li>Change a configuration item only through an approved change request.</li><li>Record a new item when it goes into operation, and retire it when it is switched off.</li><li>Review the items you own regularly, an outdated record is worse than none.</li></ol>"
            ),
            (
                "9994445E-FDBE-42E2-A3A0-65DF13CB453B",
                "8F308051-5DAB-5835-B772-58FCB908C59F",
                "<p>Finance and Controlling plans and steers the money. Budgets are set and tracked per cost center, invoices are checked and approved, and forecasts show early where the year is heading.</p>"
                + "<h2>What happens here</h2>"
                + "<ul><li>Budgets are planned per cost center and fiscal year and compared with the actual spending.</li><li>Incoming invoices are checked against their order and approved before they are paid.</li><li>Contracts are kept with their terms and renewal dates.</li><li>Forecasts are updated every month.</li></ul>"
                + "<h2>Approvals</h2>"
                + "<p>Spending is approved by the person responsible for the cost center; larger amounts need a second approval from controlling.</p>"
            ),
            (
                "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F",
                "9A5744BA-CEC7-5795-9182-951843AD2E37",
                "<p>Human Resources looks after the people of the organization, from the open position to the onboarding, from absences to training.</p>"
                + "<h2>What happens here</h2>"
                + "<ul><li>Open positions are created here and followed until they are filled.</li><li>Every new colleague gets an onboarding, with tasks for HR, IT and the team.</li><li>Absences are requested and approved here.</li><li>Training is planned and recorded per employee.</li></ul>"
                + "<h2>Please note</h2>"
                + "<p>Personnel data is confidential. Classify a record that contains personal information, so only those who need it for their work can see it.</p>"
            ),
            (
                "D35FDCD6-5B11-4043-98D6-215DF414D99C",
                "278E96EA-AF9C-5BE9-BE45-90C1BE014686",
                "<p>Procurement buys what the organization needs, from the tender to the order and from the delivery to the invoice, together with the suppliers who deliver it.</p>"
                + "<h2>How purchasing works</h2>"
                + "<ol><li>Larger purchases start with a tender, in which suppliers submit their offers.</li><li>The chosen offer becomes a purchase order, which is approved before it is placed.</li><li>Goods received are recorded as a delivery, with what arrived and in what state.</li><li>The invoice is matched against the order and the delivery before it is paid.</li></ol>"
                + "<h2>Suppliers</h2>"
                + "<p>Every supplier is kept with its contacts, agreed terms and contracts. Rate a supplier after each delivery, so the next decision can build on it.</p>"
            ),
            (
                "25F04599-C02A-4BEF-80B1-5957FBBB1FED",
                "AA416411-C143-5301-BE96-EED68010F2F5",
                "<p>Product management decides what gets built and why. Features and requirements are collected and prioritized here, and the roadmap shows when they are planned to ship.</p>"
                + "<h2>From idea to release</h2>"
                + "<ol><li>Capture an idea as a feature and describe the problem it solves.</li><li>Break it down into requirements and the use cases they serve.</li><li>Prioritize it with the stakeholders and place it on the roadmap.</li><li>Plan the release, and hand it over to development.</li></ol>"
                + "<h2>Stakeholders</h2>"
                + "<p>Every stakeholder is recorded with their interests, so it is clear whom a decision affects and who has to be heard.</p>"
            )
        ];

        /// <summary>
        /// The closing line of every seeded home page.
        /// </summary>
        private const string WorkspaceHomeFooter =
            "<p><em>This is the home page of the workspace. It opens the document overview, stands outside "
            + "the page tree and can be edited by everyone who may write in the workspace.</em></p>";

        /// <summary>
        /// Gives every seeded workspace its home page, unless it already has one.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Runs on every start as the last seeding step. Running last keeps the comments, labels
        /// and shares of the earlier steps off the home pages.
        /// </para>
        /// <para>
        /// A <see cref="Workspace.HomeId"/> that references a document of the workspace without a
        /// parent, children or comments is kept. A referenced document with a parent, children or
        /// comments stays a page, and a new home page is created. The rule matches
        /// <c>IWorkspaceManager.EnsureHome</c>.
        /// </para>
        /// <para>
        /// The page is written into the first document class of the workspace rendered as prose;
        /// without such a class the class <c>Page</c> is created with its three empty standard
        /// forms.
        /// </para>
        /// </remarks>
        /// <param name="db">The database context.</param>
        private static void SeedWorkspaceHomes(KleeneStarDbContext db)
        {
            var creatorId = Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD");
            var creator = db.Identities.AsNoTracking().Any(x => x.Id == creatorId) ? creatorId : (Guid?)null;

            foreach (var (workspaceText, pageClassText, body) in WorkspaceHomes)
            {
                var workspaceId = Guid.Parse(workspaceText);
                var workspace = db.Workspaces.FirstOrDefault(x => x.Id == workspaceId);

                if (workspace is null || HasDedicatedHome(db, workspace))
                {
                    continue;
                }

                var host = db.Classes
                    .AsNoTracking()
                    .Where(x => x.WorkspaceId == workspaceId && x.Kind == ObjectKind.Document && x.State == ClassState.Active)
                    .Where(x => x.Renderer == null || x.Renderer == "" || x.Renderer == ObjectRenderer.Prose)
                    .OrderBy(x => x.Name)
                    .FirstOrDefault();

                if (host is null)
                {
                    var pageClassId = Guid.Parse(pageClassText);

                    host = db.Classes.AsNoTracking().FirstOrDefault(x => x.Id == pageClassId);

                    if (host is null)
                    {
                        host = new Class
                        {
                            Id = pageClassId,
                            Name = "Page",
                            Description = "Pages of the workspace: documentation, guides and notes.",
                            Icon = ImageIcon.FromString("/kleenestar/assets/icons/doc.svg"),
                            Kind = ObjectKind.Document,
                            WorkspaceId = workspaceId,
                            State = ClassState.Active,
                            AccessModifier = AccessModifier.Public,
                            Created = DateTime.UtcNow,
                            Updated = DateTime.UtcNow
                        };

                        db.Classes.Add(host);

                        AddStandardForm(db, host, "Create", FormType.Create, "/kleenestar/assets/icons/form/create.svg", []);
                        AddStandardForm(db, host, "Edit", FormType.Edit, "/kleenestar/assets/icons/form/edit.svg", []);
                        AddStandardForm(db, host, "View", FormType.View, "/kleenestar/assets/icons/form/view.svg", []);
                    }
                }

                var home = new Entities.Object
                {
                    Id = Guid.NewGuid(),
                    Key = NextSeedKey(db, workspace),
                    Summary = "Welcome to " + workspace.Name,
                    Description = body + WorkspaceHomeFooter,
                    Icon = host.Icon,
                    Kind = ObjectKind.Document,
                    State = WorkspaceState.Active,
                    WorkspaceId = workspaceId,
                    ClassId = host.Id,
                    CreatorId = creator,
                    UpdaterId = creator,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                db.Objects.Add(home);

                workspace.HomeId = home.Id;
                workspace.Updated = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Determines whether the workspace's <see cref="Workspace.HomeId"/> names a dedicated
        /// home page: a document of the workspace without parent, children and comments.
        /// </summary>
        /// <param name="db">The database context.</param>
        /// <param name="workspace">The workspace.</param>
        /// <returns><see langword="true"/> when the workspace keeps its home page.</returns>
        private static bool HasDedicatedHome(KleeneStarDbContext db, Workspace workspace)
        {
            if (workspace.HomeId is not { } homeId || homeId == Guid.Empty)
            {
                return false;
            }

            var home = db.Objects
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == homeId && x.WorkspaceId == workspace.Id && x.Kind == ObjectKind.Document);

            return home is not null
                && !home.ParentId.HasValue
                && !db.Objects.AsNoTracking().Any(x => x.ParentId == homeId)
                && !db.Comments.AsNoTracking().Any(x => x.ObjectId == homeId);
        }

        /// <summary>
        /// Returns the next free object key of a workspace, counting the keys an object gave up
        /// when it moved away, the way the object manager numbers.
        /// </summary>
        /// <param name="db">The database context.</param>
        /// <param name="workspace">The workspace.</param>
        /// <returns>The key.</returns>
        private static string NextSeedKey(KleeneStarDbContext db, Workspace workspace)
        {
            var prefix = workspace.Key;
            var pattern = new Regex($"^{Regex.Escape(prefix)}-(\\d+)$", RegexOptions.IgnoreCase);

            var keys = db.Objects.AsNoTracking()
                .Where(x => x.WorkspaceId == workspace.Id)
                .Select(x => x.Key)
                .ToList()
                .Concat(db.ObjectKeyAliases.AsNoTracking()
                    .Where(x => x.Key.StartsWith(prefix + "-"))
                    .Select(x => x.Key)
                    .ToList())
                .Concat(db.Objects.Local
                    .Where(x => x.WorkspaceId == workspace.Id)
                    .Select(x => x.Key));

            var next = keys
                .Select(x => pattern.Match(x ?? string.Empty))
                .Where(m => m.Success)
                .Select(m => int.TryParse(m.Groups[1].Value, out var number) ? number : 0)
                .DefaultIfEmpty(0)
                .Max() + 1;

            return $"{prefix}-{next.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}
