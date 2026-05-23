using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the KleeneStar.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns a queryable collection of calendars from the database.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <returns>The materialized collection of calendars.</returns>
        public static IEnumerable<Calendar> GetCalendars(IQuery<Calendar> query)
        {
            using var db = CreateDbContext();

            return [.. GetCalendars(query, db)];
        }

        /// <summary>
        /// Returns a queryable collection of calendars from the database using the supplied context.
        /// </summary>
        /// <param name="query">The query criteria.</param>
        /// <param name="context">The DbContext.</param>
        /// <returns>The filtered collection.</returns>
        public static IEnumerable<Calendar> GetCalendars(IQuery<Calendar> query, KleeneStarDbContext context)
        {
            var data = context.Calendars
                .Include(x => x.Class)
                .Include(x => x.BusinessHours)
                .Include(x => x.Holidays)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Adds the specified calendar to the database when no calendar with the same id exists.
        /// </summary>
        /// <param name="calendarEntry">The calendar to add.</param>
        public static void Add(Calendar calendarEntry)
        {
            ArgumentNullException.ThrowIfNull(calendarEntry);

            using var db = CreateDbContext();

            var query = new Query<Calendar>()
                .WhereEquals(x => x.Id, calendarEntry.Id);

            if (query.Apply(db.Calendars).Any())
            {
                return;
            }

            // detach child collections and re-attach them via explicit DbSet.Add so that EF
            // does not try to graph them through the calendar's not-yet-tracked navigation.
            var hours = calendarEntry.BusinessHours?.ToList() ?? [];
            var holidays = calendarEntry.Holidays?.ToList() ?? [];

            calendarEntry.BusinessHours = [];
            calendarEntry.Holidays = [];

            if (calendarEntry.Created == default)
            {
                calendarEntry.Created = DateTime.UtcNow;
            }

            calendarEntry.Updated = DateTime.UtcNow;

            db.Calendars.Add(calendarEntry);

            foreach (var slot in hours)
            {
                slot.CalendarId = calendarEntry.Id;
                db.BusinessHourSlots.Add(slot);
            }

            foreach (var h in holidays)
            {
                h.CalendarId = calendarEntry.Id;
                db.Holidays.Add(h);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Updates an existing calendar in the database, replacing its business-hour and
        /// holiday collections with the supplied ones.
        /// </summary>
        /// <param name="calendarEntry">The calendar to update.</param>
        public static void Update(Calendar calendarEntry)
        {
            ArgumentNullException.ThrowIfNull(calendarEntry);

            using var db = CreateDbContext();

            var existing = db.Calendars
                .Include(x => x.BusinessHours)
                .Include(x => x.Holidays)
                .FirstOrDefault(x => x.Id == calendarEntry.Id);

            if (existing is null)
            {
                return;
            }

            existing.Name = calendarEntry.Name;
            existing.Description = calendarEntry.Description;
            existing.TimeZone = calendarEntry.TimeZone;
            existing.Region = calendarEntry.Region;
            existing.State = calendarEntry.State;
            existing.IsDefault = calendarEntry.IsDefault;
            existing.Icon = calendarEntry.Icon;
            existing.ClassId = calendarEntry.ClassId;
            existing.Updated = DateTime.UtcNow;

            db.BusinessHourSlots.RemoveRange(existing.BusinessHours);
            db.Holidays.RemoveRange(existing.Holidays);

            foreach (var slot in calendarEntry.BusinessHours ?? [])
            {
                db.BusinessHourSlots.Add(new BusinessHourSlot
                {
                    Id = slot.Id == Guid.Empty ? Guid.NewGuid() : slot.Id,
                    DayOfWeek = slot.DayOfWeek,
                    Enabled = slot.Enabled,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    CalendarId = existing.Id
                });
            }

            foreach (var h in calendarEntry.Holidays ?? [])
            {
                db.Holidays.Add(new Holiday
                {
                    Id = h.Id == Guid.Empty ? Guid.NewGuid() : h.Id,
                    Date = h.Date,
                    Name = h.Name,
                    Region = h.Region,
                    Enabled = h.Enabled,
                    CalendarId = existing.Id
                });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified calendar and its child collections.
        /// </summary>
        /// <param name="calendarEntry">The calendar to remove.</param>
        public static void Remove(Calendar calendarEntry)
        {
            ArgumentNullException.ThrowIfNull(calendarEntry);

            using var db = CreateDbContext();

            var existing = db.Calendars
                .Include(x => x.BusinessHours)
                .Include(x => x.Holidays)
                .FirstOrDefault(x => x.Id == calendarEntry.Id);

            if (existing is null)
            {
                return;
            }

            db.BusinessHourSlots.RemoveRange(existing.BusinessHours);
            db.Holidays.RemoveRange(existing.Holidays);
            db.Calendars.Remove(existing);

            db.SaveChanges();
        }
    }
}
