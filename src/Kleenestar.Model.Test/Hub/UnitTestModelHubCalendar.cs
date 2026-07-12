using KleeneStar.Model;
using KleeneStar.Model.Entities;
using WebExpress.WebIndex.Queries;

namespace Kleenestar.Model.Test.Hub
{
    /// <summary>
    /// Provides unit tests for the <see cref="ModelHub"/> calendar surface.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestModelHubCalendar
    {
        private static readonly Guid WorkspaceId = Guid.Parse("8C53B30E-1B0D-4C53-9211-D71F7CC4F22A");
        private static readonly Guid ClassId = Guid.Parse("E2A0BCD0-2A82-4D24-9F2C-2E1A2A4F8B7A");

        private static void SeedClassFor(string connectionString)
        {
            ModelHub.DatabaseConfig = new KleeneStar.Model.Config.DbConfig
            {
                ConnectionString = connectionString,
                Assembly = "KleeneStar.Model.Test"
            };

            using var db = ModelHub.CreateDbContext();

            if (!db.Workspaces.Any(x => x.Id == WorkspaceId))
            {
                db.Workspaces.Add(new Workspace { Id = WorkspaceId, Key = "ws-cal", Name = "workspace" });
            }

            if (!db.Classes.Any(x => x.Id == ClassId))
            {
                db.Classes.Add(new Class { Id = ClassId, Name = "Incident", WorkspaceId = WorkspaceId });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verifies all calendars in the database can be retrieved.
        /// </summary>
        [Fact]
        public void AllCalendars()
        {
            var connectionString = nameof(AllCalendars);
            SeedClassFor(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.Calendars.Add(new Calendar { Id = Guid.NewGuid(), Name = "Alpha", ClassId = ClassId });
                db.Calendars.Add(new Calendar { Id = Guid.NewGuid(), Name = "Beta",  ClassId = ClassId });
                db.SaveChanges();
            }

            var result = ModelHub.GetCalendars(new Query<Calendar>()).ToList();

            Assert.Equal(2, result.Count);
        }

        /// <summary>
        /// Verifies that the filter pipeline returns only matching entries.
        /// </summary>
        [Fact]
        public void FilteredCalendars()
        {
            var connectionString = nameof(FilteredCalendars);
            SeedClassFor(connectionString);

            using (var db = ModelHub.CreateDbContext())
            {
                db.Calendars.Add(new Calendar { Id = Guid.NewGuid(), Name = "Alpha", ClassId = ClassId });
                db.Calendars.Add(new Calendar { Id = Guid.NewGuid(), Name = "Beta",  ClassId = ClassId });
                db.SaveChanges();
            }

            var result = ModelHub.GetCalendars(new Query<Calendar>().Where(x => x.Name.StartsWith("A"))).ToList();

            Assert.Single(result);
            Assert.Equal("Alpha", result[0].Name);
        }

        /// <summary>
        /// Verifies adding a calendar persists the calendar and its child collections.
        /// </summary>
        [Fact]
        public void AddCalendarPersistsChildren()
        {
            var connectionString = nameof(AddCalendarPersistsChildren);
            SeedClassFor(connectionString);

            var calendar = new Calendar
            {
                Id = Guid.NewGuid(),
                Name = "Standard",
                ClassId = ClassId,
                State = CalendarState.Active,
                TimeZone = "Europe/Berlin",
                Region = "DE",
                IsDefault = true,
                BusinessHours =
                {
                    new BusinessHourSlot { DayOfWeek = DayOfWeek.Monday,    Enabled = true,  StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(18, 0) },
                    new BusinessHourSlot { DayOfWeek = DayOfWeek.Saturday,  Enabled = false, StartTime = new TimeOnly(0, 0), EndTime = new TimeOnly(0,  0) },
                },
                Holidays =
                {
                    new Holiday { Date = new DateOnly(2026, 1, 1), Name = "Neujahr", Region = "DE" }
                }
            };

            ModelHub.Add(calendar);

            var loaded = ModelHub.GetCalendars(new Query<Calendar>().WhereEquals(x => x.Id, calendar.Id)).Single();

            Assert.Equal("Standard", loaded.Name);
            Assert.Equal("Europe/Berlin", loaded.TimeZone);
            Assert.True(loaded.IsDefault);
            Assert.Equal(2, loaded.BusinessHours.Count);
            Assert.Single(loaded.Holidays);
            Assert.Contains(loaded.BusinessHours, b => b.DayOfWeek == DayOfWeek.Monday && b.Enabled);
        }

        /// <summary>
        /// Verifies that adding a calendar whose id already exists is a no-op.
        /// </summary>
        [Fact]
        public void AddCalendarWhenIdExistsIsNoOp()
        {
            var connectionString = nameof(AddCalendarWhenIdExistsIsNoOp);
            SeedClassFor(connectionString);

            var id = Guid.NewGuid();

            ModelHub.Add(new Calendar { Id = id, Name = "Alpha", ClassId = ClassId });
            ModelHub.Add(new Calendar { Id = id, Name = "Beta",  ClassId = ClassId });

            using var db = ModelHub.CreateDbContext();
            var calendars = db.Calendars.Where(x => x.Id == id).ToList();
            Assert.Single(calendars);
            Assert.Equal("Alpha", calendars[0].Name);
        }

        /// <summary>
        /// Verifies that updating a calendar replaces scalar properties and child collections.
        /// </summary>
        [Fact]
        public void UpdateCalendarReplacesChildren()
        {
            var connectionString = nameof(UpdateCalendarReplacesChildren);
            SeedClassFor(connectionString);

            var calendar = new Calendar
            {
                Id = Guid.NewGuid(),
                Name = "Initial",
                ClassId = ClassId,
                BusinessHours = { new BusinessHourSlot { DayOfWeek = DayOfWeek.Monday, Enabled = true, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0) } },
                Holidays = { new Holiday { Date = new DateOnly(2026, 1, 1), Name = "old", Region = "DE" } }
            };
            ModelHub.Add(calendar);

            var update = new Calendar
            {
                Id = calendar.Id,
                Name = "Updated",
                ClassId = ClassId,
                TimeZone = "UTC",
                IsDefault = true,
                BusinessHours =
                {
                    new BusinessHourSlot { DayOfWeek = DayOfWeek.Monday,    Enabled = true, StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(19, 0) },
                    new BusinessHourSlot { DayOfWeek = DayOfWeek.Tuesday,   Enabled = true, StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(19, 0) },
                },
                Holidays =
                {
                    new Holiday { Date = new DateOnly(2026, 12, 25), Name = "1. Weihnachtstag", Region = "DE" }
                }
            };

            ModelHub.Update(update);

            var loaded = ModelHub.GetCalendars(new Query<Calendar>().WhereEquals(x => x.Id, calendar.Id)).Single();

            Assert.Equal("Updated", loaded.Name);
            Assert.Equal("UTC", loaded.TimeZone);
            Assert.True(loaded.IsDefault);
            Assert.Equal(2, loaded.BusinessHours.Count);
            Assert.Single(loaded.Holidays);
            Assert.Contains(loaded.Holidays, h => h.Name == "1. Weihnachtstag");
        }

        /// <summary>
        /// Verifies that removing a calendar cascades to its children.
        /// </summary>
        [Fact]
        public void RemoveCalendarCascadesChildren()
        {
            var connectionString = nameof(RemoveCalendarCascadesChildren);
            SeedClassFor(connectionString);

            var calendar = new Calendar
            {
                Id = Guid.NewGuid(),
                Name = "ToRemove",
                ClassId = ClassId,
                BusinessHours = { new BusinessHourSlot { DayOfWeek = DayOfWeek.Monday, Enabled = true, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(18, 0) } },
                Holidays = { new Holiday { Date = new DateOnly(2026, 1, 1), Name = "Neujahr", Region = "DE" } }
            };
            ModelHub.Add(calendar);

            ModelHub.Remove(calendar);

            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Calendars.Where(x => x.Id == calendar.Id));
            Assert.Empty(db.BusinessHourSlots.Where(x => x.CalendarId == calendar.Id));
            Assert.Empty(db.Holidays.Where(x => x.CalendarId == calendar.Id));
        }

        /// <summary>
        /// Verifies that removing a non-existent calendar is a no-op.
        /// </summary>
        [Fact]
        public void RemoveCalendarWhenNotExistsIsNoOp()
        {
            var connectionString = nameof(RemoveCalendarWhenNotExistsIsNoOp);
            SeedClassFor(connectionString);

            ModelHub.Remove(new Calendar { Id = Guid.NewGuid(), ClassId = ClassId });

            using var db = ModelHub.CreateDbContext();
            Assert.Empty(db.Calendars);
        }
    }
}
