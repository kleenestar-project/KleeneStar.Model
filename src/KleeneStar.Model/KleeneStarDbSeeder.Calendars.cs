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
        /// Adds a default catalogue of <see cref="Calendar"/> entries per class, including
        /// a "Standard · Europe/Berlin" business-hours calendar (Mon-Fri 08-18) and a "24/7"
        /// always-on calendar. Service-desk style classes (Incident, ServiceRequest, Ticket)
        /// additionally get a "Night shift" calendar. All business-hours calendars receive
        /// the public holidays for their region.
        /// </summary>
        /// <param name="db">The database context to which the calendars will be added.</param>
        private static void SeedCalendars(KleeneStarDbContext db)
        {
            var classes = db.Classes.AsNoTracking().ToList();

            foreach (var cls in classes)
            {
                var templates = GetCalendarTemplatesForClass(cls.Name);

                foreach (var template in templates)
                {
                    var calendar = new Calendar
                    {
                        Id = Guid.NewGuid(),
                        Name = template.Name,
                        Description = template.Description,
                        TimeZone = template.TimeZone,
                        Region = template.Region,
                        State = template.State,
                        IsDefault = template.IsDefault,
                        Icon = ImageIcon.FromString("/kleenestar/assets/icons/calendar.svg"),
                        ClassId = cls.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    };

                    foreach (var slot in template.BusinessHours)
                    {
                        calendar.BusinessHours.Add(new BusinessHourSlot
                        {
                            Id = Guid.NewGuid(),
                            DayOfWeek = slot.Day,
                            Enabled = slot.Enabled,
                            StartTime = slot.Start,
                            EndTime = slot.End
                        });
                    }

                    foreach (var holiday in template.Holidays)
                    {
                        calendar.Holidays.Add(new Holiday
                        {
                            Id = Guid.NewGuid(),
                            Date = holiday.Date,
                            Name = holiday.Name,
                            Region = holiday.Region,
                            Enabled = true
                        });
                    }

                    db.Calendars.Add(calendar);
                }
            }
        }

        private sealed record CalendarTemplate
        (
            string Name,
            string Description,
            string TimeZone,
            string Region,
            CalendarState State,
            bool IsDefault,
            IReadOnlyList<BusinessHourTemplate> BusinessHours,
            IReadOnlyList<HolidayTemplate> Holidays
        );

        private sealed record BusinessHourTemplate(DayOfWeek Day, bool Enabled, TimeOnly Start, TimeOnly End);

        private sealed record HolidayTemplate(DateOnly Date, string Name, string Region);

        /// <summary>
        /// Returns the calendar catalogue for a given class. Service-desk style classes
        /// receive an additional "Night shift" calendar; every class always has a
        /// Standard and a 24/7 calendar.
        /// </summary>
        private static IReadOnlyList<CalendarTemplate> GetCalendarTemplatesForClass(string className)
        {
            var standard = StandardBusinessCalendar();
            var twentyFourSeven = TwentyFourSevenCalendar();

            switch (className)
            {
                case "Incident":
                case "Problem":
                case "ServiceRequest":
                case "Ticket":
                    return [standard, twentyFourSeven, NightShiftCalendar()];

                default:
                    return [standard, twentyFourSeven];
            }
        }

        private static CalendarTemplate StandardBusinessCalendar() => new
        (
            Name: "Standard · Europe/Berlin",
            Description: "Business hours Mon-Fri 08-18 including German public holidays.",
            TimeZone: "Europe/Berlin",
            Region: "DE",
            State: CalendarState.Active,
            IsDefault: true,
            BusinessHours:
            [
                new(DayOfWeek.Monday,    true,  new TimeOnly(8, 0), new TimeOnly(18, 0)),
                new(DayOfWeek.Tuesday,   true,  new TimeOnly(8, 0), new TimeOnly(18, 0)),
                new(DayOfWeek.Wednesday, true,  new TimeOnly(8, 0), new TimeOnly(18, 0)),
                new(DayOfWeek.Thursday,  true,  new TimeOnly(8, 0), new TimeOnly(18, 0)),
                new(DayOfWeek.Friday,    true,  new TimeOnly(8, 0), new TimeOnly(18, 0)),
                new(DayOfWeek.Saturday,  false, new TimeOnly(0, 0), new TimeOnly(0, 0)),
                new(DayOfWeek.Sunday,    false, new TimeOnly(0, 0), new TimeOnly(0, 0)),
            ],
            Holidays: GermanHolidays2026()
        );

        private static CalendarTemplate TwentyFourSevenCalendar() => new
        (
            Name: "24 / 7 · Always on",
            Description: "Always-on calendar that never pauses for holidays or weekends.",
            TimeZone: "UTC",
            Region: null,
            State: CalendarState.Active,
            IsDefault: false,
            BusinessHours:
            [
                new(DayOfWeek.Monday,    true, new TimeOnly(0, 0), new TimeOnly(23, 59)),
                new(DayOfWeek.Tuesday,   true, new TimeOnly(0, 0), new TimeOnly(23, 59)),
                new(DayOfWeek.Wednesday, true, new TimeOnly(0, 0), new TimeOnly(23, 59)),
                new(DayOfWeek.Thursday,  true, new TimeOnly(0, 0), new TimeOnly(23, 59)),
                new(DayOfWeek.Friday,    true, new TimeOnly(0, 0), new TimeOnly(23, 59)),
                new(DayOfWeek.Saturday,  true, new TimeOnly(0, 0), new TimeOnly(23, 59)),
                new(DayOfWeek.Sunday,    true, new TimeOnly(0, 0), new TimeOnly(23, 59)),
            ],
            Holidays: []
        );

        private static CalendarTemplate NightShiftCalendar() => new
        (
            Name: "Night shift · 22-06",
            Description: "Night-shift calendar for batch and maintenance SLAs (22 to 06).",
            TimeZone: "Europe/Berlin",
            Region: "DE",
            State: CalendarState.Active,
            IsDefault: false,
            BusinessHours:
            [
                new(DayOfWeek.Monday,    true, new TimeOnly(22, 0), new TimeOnly( 6, 0)),
                new(DayOfWeek.Tuesday,   true, new TimeOnly(22, 0), new TimeOnly( 6, 0)),
                new(DayOfWeek.Wednesday, true, new TimeOnly(22, 0), new TimeOnly( 6, 0)),
                new(DayOfWeek.Thursday,  true, new TimeOnly(22, 0), new TimeOnly( 6, 0)),
                new(DayOfWeek.Friday,    true, new TimeOnly(22, 0), new TimeOnly( 6, 0)),
                new(DayOfWeek.Saturday,  true, new TimeOnly(22, 0), new TimeOnly( 6, 0)),
                new(DayOfWeek.Sunday,    true, new TimeOnly(22, 0), new TimeOnly( 6, 0)),
            ],
            Holidays: []
        );

        private static IReadOnlyList<HolidayTemplate> GermanHolidays2026() =>
        [
            new(new DateOnly(2026,  1,  1), "New Year's Day",        "DE"),
            new(new DateOnly(2026,  4,  3), "Good Friday",           "DE"),
            new(new DateOnly(2026,  4,  6), "Easter Monday",         "DE"),
            new(new DateOnly(2026,  5,  1), "Labour Day",            "DE"),
            new(new DateOnly(2026,  5, 14), "Ascension Day",         "DE"),
            new(new DateOnly(2026,  5, 25), "Whit Monday",           "DE"),
            new(new DateOnly(2026, 10,  3), "German Unity Day",      "DE"),
            new(new DateOnly(2026, 12, 25), "Christmas Day",         "DE"),
            new(new DateOnly(2026, 12, 26), "Boxing Day",            "DE"),
        ];
    }
}
