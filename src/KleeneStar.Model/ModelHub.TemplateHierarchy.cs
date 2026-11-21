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
        /// Returns the template hierarchies matching a query.
        /// </summary>
        /// <param name="query">The query criteria. Must not be null.</param>
        /// <returns>The hierarchies, materialized.</returns>
        public static IEnumerable<TemplateHierarchy> GetTemplateHierarchies(IQuery<TemplateHierarchy> query)
        {
            using var db = CreateDbContext();

            return [.. GetTemplateHierarchies(query, db)];
        }

        /// <summary>
        /// Returns the template hierarchies matching a query within a context.
        /// </summary>
        /// <param name="query">The query criteria. Must not be null.</param>
        /// <param name="context">The context the query is executed in.</param>
        /// <returns>The hierarchies, not materialized.</returns>
        public static IEnumerable<TemplateHierarchy> GetTemplateHierarchies(IQuery<TemplateHierarchy> query, KleeneStarDbContext context)
        {
            var data = context.TemplateHierarchies
                .Include(x => x.Workspace)
                .Include(x => x.RootTemplate)
                .ThenInclude(t => t.Class)
                .AsNoTracking();

            return query.Apply(data);
        }

        /// <summary>
        /// Adds a template hierarchy unless one with its id exists.
        /// </summary>
        /// <param name="hierarchy">The hierarchy. Cannot be null.</param>
        public static void Add(TemplateHierarchy hierarchy)
        {
            ArgumentNullException.ThrowIfNull(hierarchy);

            using var db = CreateDbContext();

            if (db.TemplateHierarchies.Any(x => x.Id == hierarchy.Id))
            {
                return;
            }

            hierarchy.Created = hierarchy.Created == default ? DateTime.UtcNow : hierarchy.Created;
            hierarchy.Updated = DateTime.UtcNow;

            db.AddEntity(hierarchy);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the attributes of a template hierarchy.
        /// </summary>
        /// <param name="hierarchy">The hierarchy carrying the changes. Cannot be null.</param>
        public static void Update(TemplateHierarchy hierarchy)
        {
            ArgumentNullException.ThrowIfNull(hierarchy);

            using var db = CreateDbContext();

            var dbEntry = db.TemplateHierarchies.FirstOrDefault(x => x.Id == hierarchy.Id);

            if (dbEntry is null)
            {
                return;
            }

            dbEntry.Name = hierarchy.Name;
            dbEntry.Description = hierarchy.Description;
            dbEntry.Category = hierarchy.Category;
            dbEntry.Icon = hierarchy.Icon;
            dbEntry.State = hierarchy.State;

            if (hierarchy.RootTemplateId != Guid.Empty)
            {
                dbEntry.RootTemplateId = hierarchy.RootTemplateId;
            }

            // the empty guid names no class (the bound class of the root template)
            dbEntry.RootClassId = hierarchy.RootClassId == Guid.Empty ? null : hierarchy.RootClassId;
            dbEntry.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Removes a template hierarchy with its nodes.
        /// </summary>
        /// <param name="hierarchy">The hierarchy. Cannot be null.</param>
        public static void Remove(TemplateHierarchy hierarchy)
        {
            ArgumentNullException.ThrowIfNull(hierarchy);

            using var db = CreateDbContext();

            var dbEntry = db.TemplateHierarchies.FirstOrDefault(x => x.Id == hierarchy.Id);

            if (dbEntry is null)
            {
                return;
            }

            // the database would cascade the nodes, but only where it enforces the key
            db.TemplateHierarchyNodes.RemoveRange(db.TemplateHierarchyNodes.Where(x => x.HierarchyId == dbEntry.Id));
            db.Remove(dbEntry);
            db.SaveChanges();
        }

        /// <summary>
        /// Returns the nodes of the given hierarchies, keyed by hierarchy id.
        /// </summary>
        /// <param name="hierarchyIds">The hierarchies; null reads every hierarchy.</param>
        /// <returns>The nodes per hierarchy in their order (depth first).</returns>
        public static IReadOnlyDictionary<Guid, IReadOnlyList<TemplateHierarchyNode>> GetTemplateHierarchyNodes(IEnumerable<Guid> hierarchyIds = null)
        {
            using var db = CreateDbContext();

            var rows = db.TemplateHierarchyNodes.AsNoTracking();

            if (hierarchyIds is not null)
            {
                var ids = hierarchyIds.Distinct().ToList();

                rows = rows.Where(x => ids.Contains(x.HierarchyId));
            }

            return rows
                .OrderBy(x => x.Order)
                .ThenBy(x => x.RawId)
                .AsEnumerable()
                .GroupBy(x => x.HierarchyId)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<TemplateHierarchyNode>)[.. g]);
        }

        /// <summary>
        /// Returns the hierarchies that use a template, as root or as a node.
        /// </summary>
        /// <param name="templateId">The template.</param>
        /// <returns>The hierarchy ids, each once.</returns>
        public static IReadOnlyList<Guid> GetTemplateHierarchyIdsUsing(Guid templateId)
        {
            using var db = CreateDbContext();

            return [.. db.TemplateHierarchies.AsNoTracking().Where(x => x.RootTemplateId == templateId).Select(x => x.Id)
                .Concat(db.TemplateHierarchyNodes.AsNoTracking().Where(x => x.TemplateId == templateId).Select(x => x.HierarchyId))
                .Distinct()];
        }

        /// <summary>
        /// Replaces the nodes of a hierarchy.
        /// </summary>
        /// <remarks>
        /// The tree is written as a whole: nodes whose id the list carries again are updated in
        /// place, the others are removed and new ones are added. The position follows the order
        /// of the given nodes. A parent naming no earlier node of the list is dropped (the node
        /// moves below the root); a link keeps the targets that name the root or another node of
        /// the list and is dropped without any; the empty guid is written as no reference.
        /// </remarks>
        /// <param name="hierarchyId">The hierarchy.</param>
        /// <param name="nodes">The nodes depth first.</param>
        public static void SetTemplateHierarchyNodes(Guid hierarchyId, IEnumerable<TemplateHierarchyNode> nodes)
        {
            using var db = CreateDbContext();

            if (!db.TemplateHierarchies.Any(x => x.Id == hierarchyId))
            {
                return;
            }

            var wanted = (nodes ?? [])
                .Where(x => x is not null && x.TemplateId != Guid.Empty)
                .DistinctBy(x => x.Id)
                .ToList();
            var ids = wanted.Select(x => x.Id).ToHashSet();
            var existing = db.TemplateHierarchyNodes
                .Where(x => x.HierarchyId == hierarchyId)
                .ToList();

            db.TemplateHierarchyNodes.RemoveRange(existing.Where(x => !ids.Contains(x.Id)));

            // a parent counts only when it comes earlier in the depth first order
            var seen = new HashSet<Guid>();
            var parents = new Dictionary<Guid, Guid?>();

            foreach (var node in wanted)
            {
                parents[node.Id] = node.ParentNodeId is Guid parent && seen.Contains(parent) ? parent : null;
                seen.Add(node.Id);
            }

            for (var i = 0; i < wanted.Count; i++)
            {
                var node = wanted[i];
                var row = existing.FirstOrDefault(x => x.Id == node.Id);

                if (row is null)
                {
                    row = new TemplateHierarchyNode { Id = node.Id, HierarchyId = hierarchyId };
                    db.TemplateHierarchyNodes.Add(row);
                }

                var parentId = parents[node.Id];

                row.ParentNodeId = parentId;
                row.TemplateId = node.TemplateId;
                row.ClassId = node.ClassId == Guid.Empty ? null : node.ClassId;
                row.Order = i + 1;
                row.AsChild = node.AsChild;
                row.AssigneeId = node.AssigneeId == Guid.Empty ? null : node.AssigneeId;
                row.Links = NormalizeLinks(node, ids);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Returns the links of a node as they are stored: each with a relation type and at least
        /// one target, a target being the root or another node of the list, each named once.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="ids">The ids of the nodes of the list.</param>
        /// <returns>The links.</returns>
        private static List<TemplateHierarchyNodeLink> NormalizeLinks(TemplateHierarchyNode node, HashSet<Guid> ids)
        {
            var links = new List<TemplateHierarchyNodeLink>();

            foreach (var link in node.Links ?? [])
            {
                if (link is null || string.IsNullOrWhiteSpace(link.Type))
                {
                    continue;
                }

                var targets = (link.Targets ?? [])
                    .Select(x => x?.Trim())
                    .Where(x => string.Equals(x, TemplateHierarchyNodeLink.RootTarget, StringComparison.OrdinalIgnoreCase)
                        || (Guid.TryParse(x, out var target) && target != node.Id && ids.Contains(target)))
                    .Select(x => Guid.TryParse(x, out var target) ? target.ToString() : TemplateHierarchyNodeLink.RootTarget)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (targets.Count > 0)
                {
                    links.Add(new TemplateHierarchyNodeLink { Type = link.Type.Trim(), Inverse = link.Inverse, Targets = targets });
                }
            }

            return links;
        }
    }
}
