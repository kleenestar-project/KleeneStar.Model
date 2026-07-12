using KleeneStar.Model.Entities;
using KleeneStar.Model.Forms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides utility methods for working with the KleeneStar.
    /// </summary>
    internal static partial class ModelHub
    {
        /// <summary>
        /// Replaces the structural tree (tabs, groups, field references) of the form with
        /// the contents of the supplied snapshot, transactionally and with optimistic
        /// concurrency.
        /// </summary>
        /// <remarks>
        /// The editor PUTs the entire aggregate, so the cheapest correct strategy is to
        /// drop and re-create the structure for the form. Form metadata (name, description,
        /// updated timestamp) and the version counter are updated in the same transaction.
        /// </remarks>
        /// <param name="formId">The unique identifier of the form to update.</param>
        /// <param name="snapshot">The new structure to persist.</param>
        /// <param name="expectedVersion">
        /// The version the caller saw when loading the form. The save fails with a
        /// <see cref="DbUpdateConcurrencyException"/> if the server has moved on.
        /// </param>
        /// <returns>The new version number after the save.</returns>
        public static int SaveFormStructure(Guid formId, FormStructureSnapshot snapshot, int expectedVersion)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            using var db = CreateDbContext();
            using var tx = db.Database.BeginTransaction();

            var form = db.Forms.FirstOrDefault(f => f.Id == formId)
                ?? throw new InvalidOperationException($"Form '{formId}' not found.");

            if (form.Version != expectedVersion)
            {
                throw new DbUpdateConcurrencyException(
                    $"Form structure version mismatch (db={form.Version}, sent={expectedVersion}).");
            }

            var oldTabIds = db.FormTabs
                .Where(t => t.FormId == formId)
                .Select(t => t.Id)
                .ToList();

            db.FormElements.RemoveRange(
                db.FormElements.Where(e => oldTabIds.Contains(e.FormTabId)));
            db.FormTabs.RemoveRange(
                db.FormTabs.Where(t => t.FormId == formId));

            db.SaveChanges();

            int tabPos = 0;
            foreach (var ts in snapshot.Tabs)
            {
                var tab = new FormTab
                {
                    Id = Guid.NewGuid(),
                    FormId = formId,
                    Name = ts.Name,
                    Position = tabPos++
                };
                db.FormTabs.Add(tab);
                AppendElements(db, tab.Id, parentId: null, ts.Children);
            }

            form.Name = snapshot.FormName;
            form.Description = snapshot.FormDescription;
            form.Updated = DateTime.UtcNow;
            form.Version = expectedVersion + 1;

            db.SaveChanges();
            tx.Commit();

            return form.Version;
        }

        /// <summary>
        /// Recursively materializes a list of node snapshots as <see cref="FormElement"/>
        /// rows beneath the specified parent. Sets the position to the index in the input
        /// sequence.
        /// </summary>
        /// <param name="db">The active context.</param>
        /// <param name="tabId">The tab the elements belong to.</param>
        /// <param name="parentId">The parent element id, or <c>null</c> for top-level.</param>
        /// <param name="children">The ordered children to add.</param>
        private static void AppendElements(KleeneStarDbContext db, Guid tabId, Guid? parentId, IEnumerable<NodeSnapshot> children)
        {
            int pos = 0;
            foreach (var node in children)
            {
                FormElement entity = node switch
                {
                    GroupSnapshot g => new FormGroupElement
                    {
                        Id = Guid.NewGuid(),
                        Label = g.Label,
                        Layout = g.Layout
                    },
                    FieldRefSnapshot f => new FormFieldRefElement
                    {
                        Id = Guid.NewGuid(),
                        FieldId = f.FieldId
                    },
                    _ => throw new InvalidOperationException(
                        $"Unknown node snapshot type '{node?.GetType().Name ?? "null"}'.")
                };

                entity.FormTabId = tabId;
                entity.ParentElementId = parentId;
                entity.Position = pos++;
                db.FormElements.Add(entity);

                if (node is GroupSnapshot grp && grp.Children.Count > 0)
                {
                    AppendElements(db, tabId, entity.Id, grp.Children);
                }
            }
        }
    }
}
