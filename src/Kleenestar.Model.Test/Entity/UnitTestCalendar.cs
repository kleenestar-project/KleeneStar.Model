using KleeneStar.Model.Entities;

namespace Kleenestar.Model.Test.Entity
{
    /// <summary>
    /// Contains unit tests for the <see cref="Calendar"/>, <see cref="BusinessHourSlot"/>,
    /// and <see cref="Holiday"/> entities.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestCalendar
    {
        /// <summary>
        /// Verifies that a new <see cref="Calendar"/> is assigned a non-empty id.
        /// </summary>
        [Fact]
        public void CalendarInitializeId()
        {
            var calendar = new Calendar();

            Assert.NotEqual(Guid.Empty, calendar.Id);
        }

        /// <summary>
        /// Verifies that the explicit-id constructor sets the supplied id.
        /// </summary>
        [Fact]
        public void CalendarInitializeWithExplicitId()
        {
            var id = Guid.NewGuid();

            var calendar = new Calendar(id);

            Assert.Equal(id, calendar.Id);
        }

        /// <summary>
        /// Verifies that the scalar properties round-trip.
        /// </summary>
        [Theory]
        [InlineData("Standard", "Mon-Fri 08-18", "Europe/Berlin", "DE", CalendarState.Active, true)]
        [InlineData("Archived", null,            "UTC",           null, CalendarState.Archived, false)]
        public void CalendarSetScalarProperties(string name, string description, string tz, string region, CalendarState state, bool isDefault)
        {
            var calendar = new Calendar
            {
                Name = name,
                Description = description,
                TimeZone = tz,
                Region = region,
                State = state,
                IsDefault = isDefault
            };

            Assert.Equal(name, calendar.Name);
            Assert.Equal(description, calendar.Description);
            Assert.Equal(tz, calendar.TimeZone);
            Assert.Equal(region, calendar.Region);
            Assert.Equal(state, calendar.State);
            Assert.Equal(isDefault, calendar.IsDefault);
        }

        /// <summary>
        /// Verifies that child collections start empty and accept entries.
        /// </summary>
        [Fact]
        public void CalendarChildCollectionsAcceptEntries()
        {
            var calendar = new Calendar();

            Assert.Empty(calendar.BusinessHours);
            Assert.Empty(calendar.Holidays);

            calendar.BusinessHours.Add(new BusinessHourSlot
            {
                DayOfWeek = DayOfWeek.Monday,
                Enabled = true,
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(18, 0)
            });
            calendar.Holidays.Add(new Holiday
            {
                Date = new DateOnly(2026, 1, 1),
                Name = "Neujahr",
                Region = "DE"
            });

            Assert.Single(calendar.BusinessHours);
            Assert.Single(calendar.Holidays);
        }

        /// <summary>
        /// Verifies state extensions (IsActive/Id/Text/Color).
        /// </summary>
        [Theory]
        [InlineData(CalendarState.Active, true)]
        [InlineData(CalendarState.Archived, false)]
        public void CalendarStateExtensions(CalendarState state, bool expectedActive)
        {
            Assert.Equal(expectedActive, state.IsActive());
            Assert.NotEqual(Guid.Empty, state.Id());
            Assert.False(string.IsNullOrEmpty(state.Text()));
            Assert.False(string.IsNullOrEmpty(state.Color()));
        }

        /// <summary>
        /// Verifies the slot defaults its id and persists scalar values.
        /// </summary>
        [Fact]
        public void BusinessHourSlotInitializesIdAndSetsProperties()
        {
            var slot = new BusinessHourSlot
            {
                DayOfWeek = DayOfWeek.Wednesday,
                Enabled = true,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 30)
            };

            Assert.NotEqual(Guid.Empty, slot.Id);
            Assert.Equal(DayOfWeek.Wednesday, slot.DayOfWeek);
            Assert.True(slot.Enabled);
            Assert.Equal(new TimeOnly(9, 0), slot.StartTime);
            Assert.Equal(new TimeOnly(17, 30), slot.EndTime);
        }

        /// <summary>
        /// Verifies the holiday defaults its id and is enabled by default.
        /// </summary>
        [Fact]
        public void HolidayInitializesIdAndIsEnabledByDefault()
        {
            var holiday = new Holiday
            {
                Date = new DateOnly(2026, 12, 25),
                Name = "1. Weihnachtstag",
                Region = "DE"
            };

            Assert.NotEqual(Guid.Empty, holiday.Id);
            Assert.True(holiday.Enabled);
            Assert.Equal(new DateOnly(2026, 12, 25), holiday.Date);
            Assert.Equal("1. Weihnachtstag", holiday.Name);
            Assert.Equal("DE", holiday.Region);
        }
    }
}
