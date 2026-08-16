using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the in-app notifications listed in the
    /// notification center.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// The largest number of notifications kept per identity. Older ones are dropped as
        /// new ones arrive, so a long-running installation does not accumulate an unbounded
        /// history nobody reads.
        /// </summary>
        private const int NotificationRetention = 200;

        /// <summary>
        /// Returns the notifications addressed to the given identity, newest first.
        /// </summary>
        /// <param name="ownerId">The addressed identity.</param>
        /// <param name="unreadOnly">Whether to return only the ones not yet seen.</param>
        /// <param name="limit">The largest number of rows to return; 0 for all of them.</param>
        /// <returns>A materialized collection of notifications (possibly empty).</returns>
        public static IEnumerable<UserNotification> GetUserNotifications(Guid ownerId, bool unreadOnly = false, int limit = 0)
        {
            if (ownerId == Guid.Empty)
            {
                return [];
            }

            using var db = CreateDbContext();

            var query = db.UserNotifications
                .AsNoTracking()
                .Where(x => x.OwnerId == ownerId);

            if (unreadOnly)
            {
                query = query.Where(x => !x.Read);
            }

            query = query.OrderByDescending(x => x.Created);

            if (limit > 0)
            {
                query = query.Take(limit);
            }

            return [.. query];
        }

        /// <summary>
        /// Returns how many notifications the given identity has not seen yet.
        /// </summary>
        /// <param name="ownerId">The addressed identity.</param>
        /// <returns>The number of unread notifications.</returns>
        public static int GetUnreadUserNotificationCount(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
            {
                return 0;
            }

            using var db = CreateDbContext();

            return db.UserNotifications
                .AsNoTracking()
                .Count(x => x.OwnerId == ownerId && !x.Read);
        }

        /// <summary>
        /// Adds a notification and trims the addressee's history back to
        /// <see cref="NotificationRetention"/> entries.
        /// </summary>
        /// <param name="notification">The notification to add.</param>
        public static void Add(UserNotification notification)
        {
            ArgumentNullException.ThrowIfNull(notification);

            using var db = CreateDbContext();

            db.UserNotifications.Add(notification);
            db.SaveChanges();

            var surplus = db.UserNotifications
                .Where(x => x.OwnerId == notification.OwnerId)
                .OrderByDescending(x => x.Created)
                .Skip(NotificationRetention)
                .ToList();

            if (surplus.Count == 0)
            {
                return;
            }

            db.UserNotifications.RemoveRange(surplus);
            db.SaveChanges();
        }

        /// <summary>
        /// Marks the notification with the given id as seen.
        /// </summary>
        /// <param name="notificationId">The id of the notification.</param>
        /// <param name="ownerId">
        /// The identity the notification must belong to; a row owned by somebody else is left
        /// untouched.
        /// </param>
        public static void MarkUserNotificationRead(Guid notificationId, Guid ownerId)
        {
            using var db = CreateDbContext();

            var existing = db.UserNotifications
                .FirstOrDefault(x => x.Id == notificationId && x.OwnerId == ownerId);

            if (existing is null || existing.Read)
            {
                return;
            }

            existing.Read = true;
            db.SaveChanges();
        }

        /// <summary>
        /// Marks every notification of the given identity as seen.
        /// </summary>
        /// <param name="ownerId">The addressed identity.</param>
        /// <returns>The number of notifications that changed.</returns>
        public static int MarkAllUserNotificationsRead(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
            {
                return 0;
            }

            using var db = CreateDbContext();

            var unread = db.UserNotifications
                .Where(x => x.OwnerId == ownerId && !x.Read)
                .ToList();

            if (unread.Count == 0)
            {
                return 0;
            }

            unread.ForEach(x => x.Read = true);
            db.SaveChanges();

            return unread.Count;
        }

        /// <summary>
        /// Removes a single notification.
        /// </summary>
        /// <param name="notificationId">The id of the notification.</param>
        /// <param name="ownerId">
        /// The identity the notification must belong to; a row owned by somebody else is left
        /// untouched.
        /// </param>
        public static void RemoveUserNotification(Guid notificationId, Guid ownerId)
        {
            using var db = CreateDbContext();

            var existing = db.UserNotifications
                .FirstOrDefault(x => x.Id == notificationId && x.OwnerId == ownerId);

            if (existing is null)
            {
                return;
            }

            db.UserNotifications.Remove(existing);
            db.SaveChanges();
        }

        /// <summary>
        /// Removes every notification of the given identity.
        /// </summary>
        /// <param name="ownerId">The addressed identity.</param>
        /// <returns>The number of notifications that were removed.</returns>
        public static int RemoveUserNotifications(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
            {
                return 0;
            }

            using var db = CreateDbContext();

            var all = db.UserNotifications
                .Where(x => x.OwnerId == ownerId)
                .ToList();

            if (all.Count == 0)
            {
                return 0;
            }

            db.UserNotifications.RemoveRange(all);
            db.SaveChanges();

            return all.Count;
        }
    }
}
