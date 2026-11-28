using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    internal static partial class ModelHub
    {
        /// <summary>
        /// Returns the schedule of an appointment object.
        /// </summary>
        /// <param name="objectId">The appointment object.</param>
        /// <returns>The schedule, or <see langword="null"/>.</returns>
        public static Appointment GetAppointment(Guid objectId)
        {
            using var db = CreateDbContext();

            return db.Appointments.AsNoTracking().FirstOrDefault(x => x.ObjectId == objectId);
        }

        /// <summary>
        /// Returns the schedules of several appointment objects in one read.
        /// </summary>
        /// <param name="objectIds">The appointment objects.</param>
        /// <returns>The schedules found, keyed by object.</returns>
        public static IReadOnlyDictionary<Guid, Appointment> GetAppointments(IEnumerable<Guid> objectIds)
        {
            var ids = (objectIds ?? []).Distinct().ToList();

            if (ids.Count == 0)
            {
                return new Dictionary<Guid, Appointment>();
            }

            using var db = CreateDbContext();

            return db.Appointments
                .AsNoTracking()
                .Where(x => ids.Contains(x.ObjectId))
                .ToList()
                .GroupBy(x => x.ObjectId)
                .ToDictionary(x => x.Key, x => x.First());
        }

        /// <summary>
        /// Returns the schedules of the appointments of some workspaces that may have an
        /// occurrence in a period: those starting before its end whose series has not ended
        /// before its start.
        /// </summary>
        /// <param name="workspaceIds">The workspaces, or <see langword="null"/> for all.</param>
        /// <param name="from">The start of the period (UTC).</param>
        /// <param name="to">The end of the period (UTC).</param>
        /// <returns>The candidate schedules; the occurrences are for the caller to expand.</returns>
        public static IReadOnlyList<Appointment> GetAppointmentsOfWorkspacesInPeriod(IReadOnlyCollection<Guid> workspaceIds, DateTime from, DateTime to)
        {
            using var db = CreateDbContext();

            var objects = db.Objects.AsNoTracking().Where(o => o.Kind == ObjectKind.Appointment);

            if (workspaceIds is not null)
            {
                var ids = workspaceIds.ToList();
                objects = objects.Where(o => ids.Contains(o.WorkspaceId));
            }

            return [.. db.Appointments
                .AsNoTracking()
                .Where(x => x.Start < to && (x.SeriesEnd == null || x.SeriesEnd > from))
                .Where(x => objects.Any(o => o.Id == x.ObjectId))];
        }

        /// <summary>
        /// Returns the schedules among some appointment objects that may have an occurrence in a
        /// period.
        /// </summary>
        /// <param name="objectIds">The appointment objects to look at.</param>
        /// <param name="from">The start of the period (UTC).</param>
        /// <param name="to">The end of the period (UTC).</param>
        /// <returns>The candidate schedules.</returns>
        public static IReadOnlyList<Appointment> GetAppointmentsOfObjectsInPeriod(IEnumerable<Guid> objectIds, DateTime from, DateTime to)
        {
            var ids = (objectIds ?? []).Distinct().ToList();

            if (ids.Count == 0)
            {
                return [];
            }

            using var db = CreateDbContext();

            return [.. db.Appointments
                .AsNoTracking()
                .Where(x => ids.Contains(x.ObjectId))
                .Where(x => x.Start < to && (x.SeriesEnd == null || x.SeriesEnd > from))];
        }

        /// <summary>
        /// Returns the schedules whose last occurrence ended before a point in time and that
        /// are to be closed then, but were not closed yet.
        /// </summary>
        /// <param name="before">The point in time (UTC).</param>
        /// <param name="limit">The most schedules answered in one call.</param>
        /// <returns>The schedules, the longest over first.</returns>
        public static IReadOnlyList<Appointment> GetAppointmentsToClose(DateTime before, int limit)
        {
            using var db = CreateDbContext();

            return [.. db.Appointments
                .AsNoTracking()
                .Where(x => x.AutoClose && x.Closed == null && x.SeriesEnd != null && x.SeriesEnd < before)
                .OrderBy(x => x.SeriesEnd)
                .Take(Math.Max(1, limit))];
        }

        /// <summary>
        /// Returns the schedule a workspace holds for an event of an external calendar.
        /// </summary>
        /// <param name="workspaceId">The workspace.</param>
        /// <param name="externalId">The identifier of the event in the external calendar.</param>
        /// <returns>The schedule, or <see langword="null"/>.</returns>
        public static Appointment GetAppointmentByExternalId(Guid workspaceId, string externalId)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                return null;
            }

            using var db = CreateDbContext();

            var objects = db.Objects.AsNoTracking().Where(o => o.WorkspaceId == workspaceId);

            return db.Appointments
                .AsNoTracking()
                .Where(x => x.ExternalId == externalId)
                .FirstOrDefault(x => objects.Any(o => o.Id == x.ObjectId));
        }

        /// <summary>
        /// Writes the schedule of an appointment: adds it when the object has none, replaces the
        /// stored one otherwise.
        /// </summary>
        /// <param name="appointment">The schedule.</param>
        public static void SaveAppointment(Appointment appointment)
        {
            ArgumentNullException.ThrowIfNull(appointment);

            using var db = CreateDbContext();

            var stored = db.Appointments.FirstOrDefault(x => x.ObjectId == appointment.ObjectId);

            if (stored is null)
            {
                appointment.Created = appointment.Created == default ? DateTime.UtcNow : appointment.Created;
                appointment.Updated = DateTime.UtcNow;
                db.Appointments.Add(appointment);
            }
            else
            {
                stored.Start = appointment.Start;
                stored.End = appointment.End;
                stored.AllDay = appointment.AllDay;
                stored.TimeZone = appointment.TimeZone;
                stored.Location = appointment.Location;
                stored.Recurrence = appointment.Recurrence;
                stored.Exceptions = appointment.Exceptions;
                stored.SeriesEnd = appointment.SeriesEnd;
                stored.AutoClose = appointment.AutoClose;
                stored.Closed = appointment.Closed;
                stored.OrganizerId = appointment.OrganizerId;
                stored.ExternalId = appointment.ExternalId;
                stored.Updated = DateTime.UtcNow;

                appointment.Id = stored.Id;
                appointment.RawId = stored.RawId;
                appointment.Created = stored.Created;
                appointment.Updated = stored.Updated;
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Returns the participants of an appointment, in the order they were invited.
        /// </summary>
        /// <param name="objectId">The appointment object.</param>
        /// <returns>The participants.</returns>
        public static IReadOnlyList<AppointmentParticipant> GetAppointmentParticipants(Guid objectId)
        {
            using var db = CreateDbContext();

            return [.. db.AppointmentParticipants
                .AsNoTracking()
                .Where(x => x.ObjectId == objectId)
                .OrderBy(x => x.Created)
                .ThenBy(x => x.RawId)];
        }

        /// <summary>
        /// Returns the participants of several appointments in one read.
        /// </summary>
        /// <param name="objectIds">The appointment objects.</param>
        /// <returns>The participants, grouped by object.</returns>
        public static IReadOnlyDictionary<Guid, IReadOnlyList<AppointmentParticipant>> GetAppointmentParticipants(IEnumerable<Guid> objectIds)
        {
            var ids = (objectIds ?? []).Distinct().ToList();

            if (ids.Count == 0)
            {
                return new Dictionary<Guid, IReadOnlyList<AppointmentParticipant>>();
            }

            using var db = CreateDbContext();

            return db.AppointmentParticipants
                .AsNoTracking()
                .Where(x => ids.Contains(x.ObjectId))
                .ToList()
                .GroupBy(x => x.ObjectId)
                .ToDictionary(x => x.Key, x => (IReadOnlyList<AppointmentParticipant>)[.. x.OrderBy(p => p.Created).ThenBy(p => p.RawId)]);
        }

        /// <summary>
        /// Returns the appointment objects some identities are invited to and have not declined.
        /// </summary>
        /// <param name="identityIds">The identities.</param>
        /// <returns>The appointment objects.</returns>
        public static IReadOnlyList<Guid> GetAppointmentsAttendedBy(IEnumerable<Guid> identityIds)
        {
            var ids = (identityIds ?? []).Distinct().ToList();

            if (ids.Count == 0)
            {
                return [];
            }

            using var db = CreateDbContext();

            return [.. db.AppointmentParticipants
                .AsNoTracking()
                .Where(x => x.IdentityId != null && ids.Contains(x.IdentityId.Value))
                .Where(x => x.Response != AppointmentResponse.Declined)
                .Select(x => x.ObjectId)
                .Distinct()];
        }

        /// <summary>
        /// Replaces the participants of an appointment. A participant invited before keeps the
        /// answer they gave; one no longer named is removed.
        /// </summary>
        /// <param name="objectId">The appointment object.</param>
        /// <param name="participants">The participants it is to have.</param>
        /// <returns>The participants invited by this call (new to the appointment).</returns>
        public static IReadOnlyList<AppointmentParticipant> SetAppointmentParticipants(Guid objectId, IEnumerable<AppointmentParticipant> participants)
        {
            using var db = CreateDbContext();

            var stored = db.AppointmentParticipants.Where(x => x.ObjectId == objectId).ToList();
            var wanted = (participants ?? []).Where(x => x is not null).ToList();
            var added = new List<AppointmentParticipant>();
            var kept = new HashSet<int>();

            foreach (var participant in wanted)
            {
                var match = stored.FirstOrDefault(x => !kept.Contains(x.RawId) && Same(x, participant));

                if (match is null)
                {
                    participant.ObjectId = objectId;
                    participant.Created = participant.Created == default ? DateTime.UtcNow : participant.Created;
                    participant.Updated = DateTime.UtcNow;
                    db.AppointmentParticipants.Add(participant);
                    added.Add(participant);

                    continue;
                }

                kept.Add(match.RawId);

                if (match.Role != participant.Role || (participant.Name is not null && match.Name != participant.Name))
                {
                    match.Role = participant.Role;
                    match.Name = participant.Name ?? match.Name;
                    match.Email = participant.Email ?? match.Email;
                    match.Updated = DateTime.UtcNow;
                }
            }

            db.AppointmentParticipants.RemoveRange(stored.Where(x => !kept.Contains(x.RawId)));
            db.SaveChanges();

            return added;

            static bool Same(AppointmentParticipant a, AppointmentParticipant b)
            {
                if (a.IdentityId is not null || b.IdentityId is not null)
                {
                    return a.IdentityId == b.IdentityId;
                }

                return !string.IsNullOrWhiteSpace(a.Email)
                    ? string.Equals(a.Email, b.Email, StringComparison.OrdinalIgnoreCase)
                    : string.Equals(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Writes the answer of a participant.
        /// </summary>
        /// <param name="objectId">The appointment object.</param>
        /// <param name="identityId">The participant's account.</param>
        /// <param name="response">The answer.</param>
        /// <param name="comment">The reservation or reason, or <see langword="null"/>.</param>
        /// <returns>The participant as stored, or <see langword="null"/> when the account is
        /// not invited.</returns>
        public static AppointmentParticipant RespondToAppointment(Guid objectId, Guid identityId, AppointmentResponse response, string comment)
        {
            using var db = CreateDbContext();

            var participant = db.AppointmentParticipants.FirstOrDefault(x => x.ObjectId == objectId && x.IdentityId == identityId);

            if (participant is null)
            {
                return null;
            }

            participant.Response = response;
            participant.Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
            participant.Responded = DateTime.UtcNow;
            participant.Updated = DateTime.UtcNow;

            db.SaveChanges();

            return participant;
        }

        /// <summary>
        /// Resets the answers of the participants of an appointment, because what they answered
        /// to has moved. The organizer's own answer stays.
        /// </summary>
        /// <param name="objectId">The appointment object.</param>
        /// <param name="keep">The identity whose answer stays, or <see langword="null"/>.</param>
        /// <returns>The number of answers reset.</returns>
        public static int ResetAppointmentResponses(Guid objectId, Guid? keep)
        {
            using var db = CreateDbContext();

            var participants = db.AppointmentParticipants
                .Where(x => x.ObjectId == objectId && x.Response != AppointmentResponse.Pending)
                .ToList()
                .Where(x => keep is null || x.IdentityId != keep)
                .ToList();

            foreach (var participant in participants)
            {
                participant.Response = AppointmentResponse.Pending;
                participant.Responded = null;
                participant.Updated = DateTime.UtcNow;
            }

            db.SaveChanges();

            return participants.Count;
        }

        /// <summary>
        /// Returns the resources booked for several appointments in one read.
        /// </summary>
        /// <param name="objectIds">The appointment objects.</param>
        /// <returns>The booked resources, grouped by appointment object.</returns>
        public static IReadOnlyDictionary<Guid, IReadOnlyList<Guid>> GetAppointmentResources(IEnumerable<Guid> objectIds)
        {
            var ids = (objectIds ?? []).Distinct().ToList();

            if (ids.Count == 0)
            {
                return new Dictionary<Guid, IReadOnlyList<Guid>>();
            }

            using var db = CreateDbContext();

            return db.AppointmentResources
                .AsNoTracking()
                .Where(x => ids.Contains(x.ObjectId))
                .OrderBy(x => x.RawId)
                .ToList()
                .GroupBy(x => x.ObjectId)
                .ToDictionary(x => x.Key, x => (IReadOnlyList<Guid>)[.. x.Select(b => b.ResourceId)]);
        }

        /// <summary>
        /// Returns the appointment objects some resources are booked for.
        /// </summary>
        /// <param name="resourceIds">The resources.</param>
        /// <returns>The appointment objects.</returns>
        public static IReadOnlyList<Guid> GetAppointmentsBooking(IEnumerable<Guid> resourceIds)
        {
            var ids = (resourceIds ?? []).Distinct().ToList();

            if (ids.Count == 0)
            {
                return [];
            }

            using var db = CreateDbContext();

            return [.. db.AppointmentResources
                .AsNoTracking()
                .Where(x => ids.Contains(x.ResourceId))
                .Select(x => x.ObjectId)
                .Distinct()];
        }

        /// <summary>
        /// Replaces the resources booked for an appointment.
        /// </summary>
        /// <param name="objectId">The appointment object.</param>
        /// <param name="resourceIds">The resources it is to book.</param>
        public static void SetAppointmentResources(Guid objectId, IEnumerable<Guid> resourceIds)
        {
            var wanted = (resourceIds ?? []).Where(x => x != Guid.Empty && x != objectId).Distinct().ToList();

            using var db = CreateDbContext();

            var stored = db.AppointmentResources.Where(x => x.ObjectId == objectId).ToList();

            db.AppointmentResources.RemoveRange(stored.Where(x => !wanted.Contains(x.ResourceId)));

            foreach (var resourceId in wanted.Where(x => !stored.Any(s => s.ResourceId == x)))
            {
                db.AppointmentResources.Add(new AppointmentResource
                {
                    ObjectId = objectId,
                    ResourceId = resourceId,
                    Created = DateTime.UtcNow
                });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Removes every booking of a resource, because the resource is gone.
        /// </summary>
        /// <param name="resourceId">The resource.</param>
        /// <returns>The number of bookings removed.</returns>
        public static int RemoveAppointmentResourceBookings(Guid resourceId)
        {
            using var db = CreateDbContext();

            var bookings = db.AppointmentResources.Where(x => x.ResourceId == resourceId).ToList();

            db.AppointmentResources.RemoveRange(bookings);
            db.SaveChanges();

            return bookings.Count;
        }

        /// <summary>
        /// Returns the further layers of the calendar of a workspace, in their order.
        /// </summary>
        /// <param name="workspaceId">The workspace.</param>
        /// <returns>The sources.</returns>
        public static IReadOnlyList<AppointmentCalendarSource> GetAppointmentCalendarSources(Guid workspaceId)
        {
            using var db = CreateDbContext();

            return [.. db.AppointmentCalendarSources
                .AsNoTracking()
                .Where(x => x.WorkspaceId == workspaceId)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Name)];
        }

        /// <summary>
        /// Returns a layer of a workspace calendar.
        /// </summary>
        /// <param name="sourceId">The source.</param>
        /// <returns>The source, or <see langword="null"/>.</returns>
        public static AppointmentCalendarSource GetAppointmentCalendarSource(Guid sourceId)
        {
            using var db = CreateDbContext();

            return db.AppointmentCalendarSources.AsNoTracking().FirstOrDefault(x => x.Id == sourceId);
        }

        /// <summary>
        /// Adds a layer to a workspace calendar.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void AddAppointmentCalendarSource(AppointmentCalendarSource source)
        {
            ArgumentNullException.ThrowIfNull(source);

            using var db = CreateDbContext();

            db.AppointmentCalendarSources.Add(source);
            db.SaveChanges();
        }

        /// <summary>
        /// Writes the changes of a layer of a workspace calendar.
        /// </summary>
        /// <param name="source">The source carrying the new values.</param>
        public static void UpdateAppointmentCalendarSource(AppointmentCalendarSource source)
        {
            ArgumentNullException.ThrowIfNull(source);

            using var db = CreateDbContext();

            db.UpdateEntity(source);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes a layer from a workspace calendar.
        /// </summary>
        /// <param name="sourceId">The source.</param>
        public static void RemoveAppointmentCalendarSource(Guid sourceId)
        {
            using var db = CreateDbContext();

            db.AppointmentCalendarSources.RemoveRange(db.AppointmentCalendarSources.Where(x => x.Id == sourceId));
            db.SaveChanges();
        }
    }
}
