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
    }
}
