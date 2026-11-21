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
        /// Returns the webhooks of a workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace.</param>
        /// <returns>The webhooks, ordered by name.</returns>
        public static IReadOnlyList<Webhook> GetWebhooks(Guid workspaceId)
        {
            using var db = CreateDbContext();

            return [.. db.Webhooks.AsNoTracking().Where(x => x.WorkspaceId == workspaceId).OrderBy(x => x.Name)];
        }

        /// <summary>
        /// Returns a webhook.
        /// </summary>
        /// <param name="webhookId">The webhook.</param>
        /// <returns>The webhook, or <see langword="null"/>.</returns>
        public static Webhook GetWebhook(Guid webhookId)
        {
            using var db = CreateDbContext();

            return db.Webhooks.AsNoTracking().FirstOrDefault(x => x.Id == webhookId);
        }

        /// <summary>
        /// Adds a webhook.
        /// </summary>
        /// <param name="webhook">The webhook.</param>
        public static void AddWebhook(Webhook webhook)
        {
            ArgumentNullException.ThrowIfNull(webhook);

            using var db = CreateDbContext();

            db.Webhooks.Add(webhook);
            db.SaveChanges();
        }

        /// <summary>
        /// Writes the changes of a webhook, its secret included.
        /// </summary>
        /// <param name="webhook">The webhook carrying the new values.</param>
        public static void UpdateWebhook(Webhook webhook)
        {
            ArgumentNullException.ThrowIfNull(webhook);

            using var db = CreateDbContext();

            var stored = db.Webhooks.FirstOrDefault(x => x.Id == webhook.Id);

            if (stored is null)
            {
                return;
            }

            stored.Name = webhook.Name;
            stored.Direction = webhook.Direction;
            stored.Url = webhook.Url;
            stored.Integration = webhook.Integration;
            stored.Secret = webhook.Secret;
            stored.State = webhook.State;
            stored.LastDelivery = webhook.LastDelivery;
            stored.LastResult = webhook.LastResult;
            stored.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Removes a webhook.
        /// </summary>
        /// <param name="webhookId">The webhook.</param>
        public static void RemoveWebhook(Guid webhookId)
        {
            using var db = CreateDbContext();

            db.Webhooks.RemoveRange(db.Webhooks.Where(x => x.Id == webhookId));
            db.SaveChanges();
        }
    }
}
