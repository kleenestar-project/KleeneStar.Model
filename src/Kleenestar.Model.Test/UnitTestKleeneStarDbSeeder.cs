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
