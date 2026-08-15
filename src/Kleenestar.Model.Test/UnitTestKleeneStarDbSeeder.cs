using Microsoft.EntityFrameworkCore;

namespace KleeneStar.Model.Test
{
    /// <summary>
    /// Contains unit tests for the database seeder.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestKleeneStarDbSeeder
    {
        /// <summary>
        /// Verifies that identity entities and their group assignments are seeded.
        /// </summary>
        [Fact]
        public async Task SeedIdentityEntities()
        {
            // arrange
            var connectionString = $"SeedIdentityEntities_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            Assert.Equal(4, db.Groups.Count());
            Assert.Equal(4, db.Identities.Count());

            var admin = db.Identities
                .Include(x => x.GroupMemberships)
                    .ThenInclude(x => x.Group)
                .Single(x => x.Email == "admin@kleenestar.org");

            Assert.Contains(admin.GroupMemberships, x => x.Group.Name == "Admin");
        }

        /// <summary>
        /// Verifies that seeding remains idempotent for identity entities.
        /// </summary>
        [Fact]
        public async Task SeedIdentityEntitiesIsIdempotent()
        {
            // arrange
            var connectionString = $"SeedIdentityEntitiesIsIdempotent_{Guid.NewGuid()}";

            await using (var db = InMemoryDbContextFactory.Create(connectionString))
            {
                await KleeneStarDbSeeder.SeedAsync(db);
            }

            // act
            await using (var db = InMemoryDbContextFactory.Create(connectionString))
            {
                await KleeneStarDbSeeder.SeedAsync(db);
            }

            // validation
            await using (var db = InMemoryDbContextFactory.Create(connectionString))
            {
                Assert.Equal(4, db.Groups.Count());
                Assert.Equal(4, db.Identities.Count());
                Assert.Equal(4, db.Identities.Include(x => x.GroupMemberships).Sum(x => x.GroupMemberships.Count));
            }
        }

        /// <summary>
        /// Verifies that repeated seeding does not change counts across all core seeded entity sets.
        /// </summary>
        [Fact]
        public async Task SeedAllEntitiesIsIdempotent()
        {
            // arrange
            var connectionString = $"SeedAllEntitiesIsIdempotent_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            var categories = db.Categories.Count();
            var tenants = db.Tenants.Count();
            var groups = db.Groups.Count();
            var identities = db.Identities.Count();
            var workspaces = db.Workspaces.Count();
            var classes = db.Classes.Count();
            var fields = db.Fields.Count();
            var forms = db.Forms.Count();
            var priorities = db.Priorities.Count();
            var statusCategories = db.StatusCategories.Count();
            var statuses = db.Statuses.Count();
            var workflows = db.Workflows.Count();
            var objects = db.Objects.Count();
            var sprints = db.Sprints.Count();
            var dashboards = db.Dashboards.Count();
            var slaPolicies = db.SlaPolicies.Count();
            var slaTargets = db.SlaTargets.Count();
            var slaScope = db.SlaScopeRules.Count();
            var slaEscalations = db.SlaEscalationLevels.Count();
            var calendars = db.Calendars.Count();
            var businessHours = db.BusinessHourSlots.Count();
            var holidays = db.Holidays.Count();
            var comments = db.Comments.Count();
            var commentLikes = db.CommentLikes.Count();
            var commentReactions = db.CommentReactions.Count();

            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            Assert.True(categories > 0);
            Assert.True(workspaces > 0);
            Assert.True(objects > 0);

            Assert.Equal(categories, db.Categories.Count());
            Assert.Equal(tenants, db.Tenants.Count());
            Assert.Equal(groups, db.Groups.Count());
            Assert.Equal(identities, db.Identities.Count());
            Assert.Equal(workspaces, db.Workspaces.Count());
            Assert.Equal(classes, db.Classes.Count());
            Assert.Equal(fields, db.Fields.Count());
            Assert.Equal(forms, db.Forms.Count());
            Assert.Equal(priorities, db.Priorities.Count());
            Assert.Equal(statusCategories, db.StatusCategories.Count());
            Assert.Equal(statuses, db.Statuses.Count());
            Assert.Equal(workflows, db.Workflows.Count());
            Assert.Equal(objects, db.Objects.Count());
            Assert.Equal(sprints, db.Sprints.Count());
            Assert.Equal(dashboards, db.Dashboards.Count());
            Assert.Equal(slaPolicies, db.SlaPolicies.Count());
            Assert.Equal(slaTargets, db.SlaTargets.Count());
            Assert.Equal(slaScope, db.SlaScopeRules.Count());
            Assert.Equal(slaEscalations, db.SlaEscalationLevels.Count());
            Assert.Equal(calendars, db.Calendars.Count());
            Assert.Equal(businessHours, db.BusinessHourSlots.Count());
            Assert.Equal(holidays, db.Holidays.Count());
            Assert.Equal(comments, db.Comments.Count());
            Assert.Equal(commentLikes, db.CommentLikes.Count());
            Assert.Equal(commentReactions, db.CommentReactions.Count());
        }

        /// <summary>
        /// Verifies that the calendar seeder produces at least one calendar per class,
        /// that each business-hours calendar has seven weekly slots, and that service-desk
        /// style classes additionally get a night-shift calendar.
        /// </summary>
        [Fact]
        public async Task SeedCalendars()
        {
            // arrange
            var connectionString = $"SeedCalendars_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            Assert.True(db.Calendars.Any(), "expected at least one calendar");
            Assert.True(db.BusinessHourSlots.Any(), "expected at least one business-hour slot");

            // every class should have at least one calendar
            var classes = db.Classes.ToList();
            foreach (var cls in classes)
            {
                Assert.True
                (
                    db.Calendars.Any(c => c.ClassId == cls.Id),
                    $"expected at least one calendar for class {cls.Name}"
                );
            }

            // Incident gets three calendars (Standard, 24/7, Night-shift)
            var incident = db.Classes.Single(c => c.Name == "Incident");
            var incidentCalendars = db.Calendars.Where(c => c.ClassId == incident.Id).ToList();
            Assert.Equal(3, incidentCalendars.Count);
            Assert.Contains(incidentCalendars, c => c.Name.Contains("Standard"));
            Assert.Contains(incidentCalendars, c => c.Name.Contains("24 / 7"));
            Assert.Contains(incidentCalendars, c => c.Name.Contains("Night shift"));

            // a Standard calendar should have 7 weekly slots and German holidays
            var standard = db.Calendars
                .Where(c => c.Name == "Standard · Europe/Berlin")
                .Select(c => c.Id)
                .First();
            Assert.Equal(7, db.BusinessHourSlots.Count(b => b.CalendarId == standard));
            Assert.Contains(db.Holidays.Where(h => h.CalendarId == standard).ToList(), h => h.Name == "New Year's Day");
        }

        /// <summary>
        /// Verifies that the SLA seeder produces a class-specific policy catalogue and that
        /// the support-desk style classes get multiple policies (P1, P2, P3, VIP, draft, inactive).
        /// </summary>
        [Fact]
        public async Task SeedSlaPolicies()
        {
            // arrange
            var connectionString = $"SeedSlaPolicies_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            Assert.True(db.SlaPolicies.Any(), "expected at least one SLA policy");
            Assert.True(db.SlaTargets.Any(),  "expected at least one SLA target");

            // each seeded class should have at least one policy
            var classes = db.Classes.ToList();
            foreach (var cls in classes)
            {
                Assert.True
                (
                    db.SlaPolicies.Any(p => p.ClassId == cls.Id),
                    $"expected at least one policy for class {cls.Name}"
                );
            }

            // Incident class gets the richer catalogue (6 policies including draft + inactive)
            var incident = db.Classes.Single(c => c.Name == "Incident");
            var incidentPolicies = db.SlaPolicies.Where(p => p.ClassId == incident.Id).ToList();
            Assert.Equal(6, incidentPolicies.Count);
            Assert.Contains(incidentPolicies, p => p.State == Entities.SlaPolicyState.Draft);
            Assert.Contains(incidentPolicies, p => p.State == Entities.SlaPolicyState.Inactive);
            Assert.Contains(incidentPolicies, p => p.State == Entities.SlaPolicyState.Active);

            // every Active/Inactive policy should be linked to a Calendar of its class
            var classCalendarIds = db.Calendars.ToDictionary(c => c.Id, c => c.ClassId);
            foreach (var policy in incidentPolicies)
            {
                Assert.True(policy.CalendarId.HasValue, $"policy {policy.Name} expected to reference a calendar");
                Assert.Equal(incident.Id, classCalendarIds[policy.CalendarId.Value]);
            }
        }

        /// <summary>
        /// Verifies that comments are seeded for every object and that the threads are
        /// class-flavoured (Incident gets the network/triage thread, Bug gets the
        /// triage-and-fix thread, etc.). Also asserts that at least one thread carries a
        /// reply so the parent/reply rendering path has live data in the seeded DB.
        /// </summary>
        [Fact]
        public async Task SeedComments()
        {
            // arrange
            var connectionString = $"SeedComments_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            Assert.True(db.Comments.Any(), "expected at least one seeded comment");

            // every seeded object should have at least one top-level comment
            var objects = db.Objects.ToList();
            foreach (var obj in objects)
            {
                Assert.True
                (
                    db.Comments.Any(c => c.ObjectId == obj.Id && c.ParentCommentId == null),
                    $"expected at least one top-level comment for object {obj.Key}"
                );
            }

            // at least one comment should carry a reply (exercises the parent FK path)
            Assert.True(db.Comments.Any(c => c.ParentCommentId.HasValue), "expected at least one nested reply across all seed threads");

            // at least one comment should be pinned (exercises the IsPinned path)
            Assert.True(db.Comments.Any(c => c.IsPinned), "expected at least one pinned comment");

            // at least one like and one reaction should be seeded (exercises the
            // CommentLike + CommentReaction tables)
            Assert.True(db.CommentLikes.Any(), "expected at least one seeded comment like");
            Assert.True(db.CommentReactions.Any(), "expected at least one seeded comment reaction");

            // the threads are class-flavoured: an Incident object should carry the
            // network-team narrative produced by GetCommentTemplatesForClass("Incident").
            var incidentClass = db.Classes.Single(c => c.Name == "Incident");
            var incidentObject = db.Objects.First(o => o.ClassId == incidentClass.Id);
            var threads = db.Comments
                .Where(c => c.ObjectId == incidentObject.Id && c.ParentCommentId == null)
                .Select(c => c.Content)
                .ToList();
            Assert.Contains(threads, t => t.Contains("VPN", StringComparison.OrdinalIgnoreCase) || t.Contains("workaround", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Verifies that the seeded fields span every <see cref="Entities.FieldType"/> rather
        /// than being all-string, so the forms rendered from seed data show a variety of types.
        /// </summary>
        [Fact]
        public async Task SeedFieldsCoverAllFieldTypes()
        {
            // arrange
            var connectionString = $"SeedFieldTypes_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            var seededTypes = db.Fields.Select(f => f.FieldType).Distinct().ToHashSet();

            foreach (var type in Enum.GetValues<Entities.FieldType>())
            {
                Assert.Contains(type, seededTypes);
            }
        }

        /// <summary>
        /// Verifies that no class receives two fields of the same name.
        /// </summary>
        /// <remarks>
        /// A field is unique per class and name (see the index on <c>ClassId, Name</c>), but
        /// the in-memory provider the seeder tests run against does not enforce a unique
        /// index — a duplicate therefore passes here and only fails on a real database, at
        /// startup, where it takes the whole seeding with it. The invariant is asserted
        /// explicitly instead.
        /// </remarks>
        [Fact]
        public async Task SeedFieldNamesAreUniquePerClass()
        {
            // arrange
            var connectionString = $"SeedFieldNames_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            var duplicates = db.Fields
                .ToList()
                .GroupBy(f => (f.ClassId, f.Name))
                .Where(g => g.Count() > 1)
                .Select(g => $"{g.Key.Name} ({g.Count()}x)")
                .ToList();

            Assert.True(duplicates.Count == 0, $"duplicate field names: {string.Join(", ", duplicates)}");
        }

        /// <summary>
        /// Verifies that the seeded workflow-typed field is linked to a workflow of its class,
        /// i.e. its <see cref="Entities.Field.WorkflowId"/> is populated and references an
        /// existing workflow.
        /// </summary>
        [Fact]
        public async Task SeedWorkflowFieldIsLinkedToWorkflow()
        {
            // arrange
            var connectionString = $"SeedWorkflowField_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            var incident = db.Classes.Single(c => c.Name == "Incident");
            var workflowFields = db.Fields
                .Where(f => f.ClassId == incident.Id && f.FieldType == Entities.FieldType.Workflow)
                .ToList();

            Assert.NotEmpty(workflowFields);

            var workflowIds = db.Workflows
                .Where(w => w.ClassId == incident.Id)
                .Select(w => w.Id)
                .ToHashSet();

            Assert.All(workflowFields, f =>
            {
                Assert.True(f.WorkflowId.HasValue, $"workflow field {f.Name} expected a WorkflowId");
                Assert.Contains(f.WorkflowId.Value, workflowIds);
            });
        }

        /// <summary>
        /// Verifies that the standard View form of a class references at least one field of
        /// every <see cref="Entities.FieldType"/>, i.e. the form contains the full type variety.
        /// </summary>
        [Fact]
        public async Task SeedViewFormCoversAllFieldTypes()
        {
            // arrange
            var connectionString = $"SeedViewFormTypes_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            var incident = db.Classes.Single(c => c.Name == "Incident");
            var viewForm = db.Forms.Single(f => f.ClassId == incident.Id && f.FormType == Entities.FormType.View);

            var tabIds = db.FormTabs
                .Where(t => t.FormId == viewForm.Id)
                .Select(t => t.Id)
                .ToHashSet();

            var fieldIdsInForm = db.FormElements
                .ToList()
                .OfType<Entities.FormFieldRefElement>()
                .Where(e => tabIds.Contains(e.FormTabId))
                .Select(e => e.FieldId)
                .ToHashSet();

            var typesInForm = db.Fields
                .ToList()
                .Where(f => fieldIdsInForm.Contains(f.Id))
                .Select(f => f.FieldType)
                .Distinct()
                .ToHashSet();

            foreach (var type in Enum.GetValues<Entities.FieldType>())
            {
                Assert.Contains(type, typesInForm);
            }
        }

        /// <summary>
        /// Verifies that the objects tab views are seeded per kind: every workspace that
        /// receives views gets one issue tab set (Issues, Table, List, Dashboard, Kanban,
        /// ScrumSprint, ScrumBacklog) and one asset tab set (Assets, Table, List, Dashboard,
        /// Kanban) — the asset set omits the two Scrum boards — each in its own display
        /// order starting at zero, and that all of them are active.
        /// </summary>
        [Fact]
        public async Task SeedObjectViews()
        {
            // arrange
            var connectionString = $"SeedObjectViews_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            Assert.True(db.ObjectViews.Any(), "expected at least one seeded object view");

            var expectedIssueOrder = new[]
            {
                Entities.ObjectViewType.Issues,
                Entities.ObjectViewType.Table,
                Entities.ObjectViewType.List,
                Entities.ObjectViewType.Dashboard,
                Entities.ObjectViewType.Kanban,

                // the sprint board and the backlog share one seeded tab now; it is persisted
                // as ScrumSprint, and ScrumBacklog is no longer seeded
                Entities.ObjectViewType.ScrumSprint
            };

            var expectedAssetOrder = new[]
            {
                Entities.ObjectViewType.Assets,
                Entities.ObjectViewType.Table,
                Entities.ObjectViewType.List,
                Entities.ObjectViewType.Dashboard,
                Entities.ObjectViewType.Kanban
            };

            // each kind keeps its own tab set within a workspace
            var byWorkspaceKind = db.ObjectViews
                .ToList()
                .GroupBy(v => (v.WorkspaceId, v.Kind))
                .ToList();

            Assert.NotEmpty(byWorkspaceKind);

            foreach (var group in byWorkspaceKind)
            {
                var ordered = group.OrderBy(v => v.Order).ToList();
                var expectedOrder = group.Key.Kind == Entities.ObjectKind.Asset
                    ? expectedAssetOrder
                    : expectedIssueOrder;

                // one view per type of the kind, in the canonical display order
                Assert.Equal(expectedOrder.Length, ordered.Count);
                Assert.Equal(expectedOrder, ordered.Select(v => v.ViewType).ToArray());
                Assert.Equal(Enumerable.Range(0, expectedOrder.Length).ToArray(), ordered.Select(v => v.Order).ToArray());

                // all seeded views are active and carry a name
                Assert.All(ordered, v => Assert.Equal(Entities.ObjectViewState.Active, v.State));
                Assert.All(ordered, v => Assert.False(string.IsNullOrWhiteSpace(v.Name)));
            }

            // both kinds are represented for every workspace that receives views
            var kindsPerWorkspace = db.ObjectViews
                .ToList()
                .GroupBy(v => v.WorkspaceId)
                .ToList();

            Assert.All(kindsPerWorkspace, g =>
            {
                var kinds = g.Select(v => v.Kind).Distinct().ToList();
                Assert.Contains(Entities.ObjectKind.Issue, kinds);
                Assert.Contains(Entities.ObjectKind.Asset, kinds);
            });
        }

        /// <summary>
        /// Verifies that the sprints are seeded: every workspace gets one completed, one
        /// active and one planned sprint, a share of the workspace objects is committed
        /// to the completed and active sprints with dense 1-based ranks per group, and
        /// most objects carry a story-point estimate.
        /// </summary>
        [Fact]
        public async Task SeedSprints()
        {
            // arrange
            var connectionString = $"SeedSprints_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            Assert.True(db.Sprints.Any(), "expected at least one seeded sprint");

            foreach (var workspace in db.Workspaces.ToList())
            {
                var sprints = db.Sprints.Where(s => s.WorkspaceId == workspace.Id).ToList();

                // one sprint per lifecycle state, exactly one active
                Assert.Equal(3, sprints.Count);
                Assert.Single(sprints, s => s.State == Entities.SprintState.Completed);
                Assert.Single(sprints, s => s.State == Entities.SprintState.Active);
                Assert.Single(sprints, s => s.State == Entities.SprintState.Planned);
                Assert.All(sprints, s => Assert.False(string.IsNullOrWhiteSpace(s.Name)));
                Assert.All(sprints, s => Assert.True(s.Start.HasValue && s.End.HasValue && s.Start < s.End));

                var active = sprints.Single(s => s.State == Entities.SprintState.Active);
                var objects = db.Objects.Where(o => o.WorkspaceId == workspace.Id).ToList();

                if (objects.Count == 0)
                {
                    continue;
                }

                // the active sprint carries objects, the backlog is non-empty, and the
                // ranks per group are dense and 1-based
                var committed = objects.Where(o => o.SprintId == active.Id).OrderBy(o => o.SprintRank).ToList();
                var backlog = objects.Where(o => o.SprintId == null).OrderBy(o => o.SprintRank).ToList();

                Assert.NotEmpty(committed);
                Assert.NotEmpty(backlog);
                Assert.Equal(Enumerable.Range(1, committed.Count), committed.Select(o => o.SprintRank));
                Assert.Equal(Enumerable.Range(1, backlog.Count), backlog.Select(o => o.SprintRank));

                // most objects are estimated, at least one is deliberately left open
                Assert.Contains(objects, o => o.StoryPoints.HasValue);
            }
        }

        /// <summary>
        /// Verifies that the object-kind partition is seeded: objects of the
        /// documentation-like classes become documents arranged into a page tree, the
        /// release objects become blog posts with creation dates spread over several
        /// months, the Asset class objects become assets, and every other object keeps
        /// the default issue kind.
        /// </summary>
        [Fact]
        public async Task SeedObjectKinds()
        {
            // arrange
            var connectionString = $"SeedObjectKinds_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation — the class is the source of the kind: the documentation-like
            // classes are document classes, the release class is a blog class, and
            // every other class keeps the default issue kind
            var documentClassIds = db.Classes
                .Where(c => c.Name == "Documentation" || c.Name == "Knowledge")
                .Select(c => c.Id)
                .ToHashSet();

            Assert.NotEmpty(documentClassIds);
            Assert.All(db.Classes.Where(c => documentClassIds.Contains(c.Id)).ToList(),
                c => Assert.Equal(Entities.ObjectKind.Document, c.Kind));
            Assert.All(db.Classes.Where(c => c.Name == "Release" || c.Name == "Announcement").ToList(),
                c => Assert.Equal(Entities.ObjectKind.Blog, c.Kind));
            Assert.All(db.Classes.Where(c => c.Name == "Asset").ToList(),
                c => Assert.Equal(Entities.ObjectKind.Asset, c.Kind));
            Assert.All(db.Classes.Where(c => c.Name != "Documentation" && c.Name != "Knowledge" && c.Name != "Release" && c.Name != "Announcement" && c.Name != "Asset").ToList(),
                c => Assert.Equal(Entities.ObjectKind.Issue, c.Kind));

            var documents = db.Objects.Where(o => documentClassIds.Contains(o.ClassId)).ToList();
            Assert.NotEmpty(documents);
            Assert.All(documents, o => Assert.Equal(Entities.ObjectKind.Document, o.Kind));

            // the documents form a tree: some roots, the rest nested beneath them
            var documentIds = documents.Select(o => o.Id).ToHashSet();
            Assert.Contains(documents, o => o.ParentId == null);
            Assert.Contains(documents, o => o.ParentId.HasValue);
            Assert.All(documents.Where(o => o.ParentId.HasValue), o => Assert.Contains(o.ParentId!.Value, documentIds));

            // validation — blog posts: every Release object is a blog post and the
            // staggered creation dates span more than one month (feeding the timeline)
            var blogClassIds = db.Classes
                .Where(c => c.Name == "Release" || c.Name == "Announcement")
                .Select(c => c.Id)
                .ToHashSet();

            Assert.NotEmpty(blogClassIds);

            var posts = db.Objects.Where(o => blogClassIds.Contains(o.ClassId)).ToList();
            Assert.NotEmpty(posts);
            Assert.All(posts, o => Assert.Equal(Entities.ObjectKind.Blog, o.Kind));
            Assert.True(posts.Select(o => (o.Created.Year, o.Created.Month)).Distinct().Count() > 1,
                "expected the blog posts to spread over more than one month");

            // validation — asset objects: the Asset class is an asset class, so every
            // object of it carries the asset kind
            var assetClassIds = db.Classes
                .Where(c => c.Name == "Asset")
                .Select(c => c.Id)
                .ToHashSet();

            Assert.NotEmpty(assetClassIds);

            var assets = db.Objects.Where(o => assetClassIds.Contains(o.ClassId)).ToList();
            Assert.NotEmpty(assets);
            Assert.All(assets, o => Assert.Equal(Entities.ObjectKind.Asset, o.Kind));

            // validation — everything else keeps the default issue kind
            var otherKinds = db.Objects
                .Where(o => !documentClassIds.Contains(o.ClassId) && !blogClassIds.Contains(o.ClassId) && !assetClassIds.Contains(o.ClassId))
                .Select(o => o.Kind)
                .Distinct()
                .ToList();

            Assert.Equal([Entities.ObjectKind.Issue], otherKinds);
        }

        /// <summary>
        /// Verifies that the customer-portal projection flags are seeded: the service-desk
        /// request types (Ticket, Incident, ServiceRequest) are portal-visible while
        /// internal classes are not, and exactly the "Self-Service Form" of each portal
        /// class is offered as a portal template.
        /// </summary>
        [Fact]
        public async Task SeedPortalFlags()
        {
            // arrange
            var connectionString = $"SeedPortalFlags_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation — class flags
            var portalClassNames = db.Classes
                .Where(c => c.PortalVisible)
                .Select(c => c.Name)
                .OrderBy(n => n)
                .ToList();

            Assert.Equal(["Incident", "ServiceRequest", "Ticket"], portalClassNames);

            // validation — template flags: every portal-template form is the
            // "Self-Service Form" and belongs to a service-desk-capable class.
            var portalTemplates = db.Forms
                .Where(f => f.PortalTemplate)
                .ToList();

            Assert.NotEmpty(portalTemplates);
            Assert.All(portalTemplates, f => Assert.Equal("Self-Service Form", f.Name));

            var portalVisibleClassIds = db.Classes
                .Where(c => c.PortalVisible)
                .Select(c => c.Id)
                .ToHashSet();

            // the portal-visible classes each carry exactly one portal template
            foreach (var classId in portalVisibleClassIds)
            {
                Assert.Single(portalTemplates, f => f.ClassId == classId);
            }
        }
    }
}
