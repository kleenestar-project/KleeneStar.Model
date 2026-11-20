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
        /// Returns a queryable collection of templates from the database, optionally filtered 
        /// by one or more query criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned templates. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of templates.
        /// </returns>
        public static IEnumerable<Template> GetTemplates(IQuery<Template> query)
        {
            using var db = CreateDbContext();

            return [.. GetTemplates(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of templates from the database, optionally filtered 
        /// by one or more query criteria.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned templates. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of templates.
        /// </returns>
        public static IEnumerable<Template> GetTemplates(IQuery<Template> query, KleeneStarDbContext context)
        {
            var data = context.Templates
                .Include(x => x.Class)
                .ThenInclude(c => c.Workspace)
                .Include(x => x.Parent)
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified template to the database if it does not already exist.
        /// </summary>
        /// <param name="templateEntry">
        /// The template to add. The template's id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Template templateEntry)
        {
            ArgumentNullException.ThrowIfNull(templateEntry);

            using var db = CreateDbContext();

            var query = new Query<Template>()
                .WhereEquals(x => x.Id, templateEntry.Id);

            if (query.Apply(db.Templates).Any())
            {
                return;
            }

            db.AddEntity(templateEntry);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the properties of an existing template in the database.
        /// Only specific properties are mapped during the update.
        /// </summary>
        /// <param name="templateEntry">
        /// The template object tracking the changes to apply.
        /// Cannot be null.
        /// </param>
        public static void Update(Template templateEntry)
        {
            ArgumentNullException.ThrowIfNull(templateEntry);

            using var db = CreateDbContext();

            var query = new Query<Template>()
                .WhereEquals(x => x.Id, templateEntry.Id);

            var dbEntry = query.Apply(db.Templates).FirstOrDefault();

            if (dbEntry is null)
            {
                return;
            }

            dbEntry.Name = templateEntry.Name;
            dbEntry.Description = templateEntry.Description;
            dbEntry.Category = templateEntry.Category;
            dbEntry.Icon = templateEntry.Icon;
            dbEntry.State = templateEntry.State;
            dbEntry.Presets = templateEntry.Presets;
            dbEntry.Order = templateEntry.Order;

            // the empty guid names nobody and nothing (the "none" entry of a selection); written
            // into a reference it would violate the foreign key
            dbEntry.ParentId = templateEntry.ParentId == Guid.Empty ? null : templateEntry.ParentId;
            dbEntry.AssigneeId = templateEntry.AssigneeId == Guid.Empty ? null : templateEntry.AssigneeId;
            dbEntry.PredecessorId = templateEntry.PredecessorId == Guid.Empty ? null : templateEntry.PredecessorId;

            if (templateEntry.ClassId != Guid.Empty)
            {
                dbEntry.ClassId = templateEntry.ClassId;
            }

            dbEntry.Updated = DateTime.UtcNow;

            db.SaveChanges();
        }

        /// <summary>
        /// Returns the further classes of the given templates, keyed by template id.
        /// </summary>
        /// <param name="templateIds">The templates; null reads every assignment.</param>
        /// <returns>
        /// The further class ids per template, in the order they were assigned. A template
        /// without further classes has no entry.
        /// </returns>
        public static IReadOnlyDictionary<Guid, IReadOnlyList<Guid>> GetTemplateClassIds(IEnumerable<Guid> templateIds = null)
        {
            using var db = CreateDbContext();

            var rows = db.TemplateClasses.AsNoTracking();

            if (templateIds is not null)
            {
                var ids = templateIds.Distinct().ToList();

                rows = rows.Where(x => ids.Contains(x.TemplateId));
            }

            return rows
                .OrderBy(x => x.RawId)
                .Select(x => new { x.TemplateId, x.ClassId })
                .AsEnumerable()
                .GroupBy(x => x.TemplateId)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<Guid>)[.. g.Select(x => x.ClassId)]);
        }

        /// <summary>
        /// Returns the templates that serve the given class as a further class.
        /// </summary>
        /// <param name="classId">The class.</param>
        /// <returns>The template ids.</returns>
        public static IReadOnlyList<Guid> GetTemplateIdsServingClass(Guid classId)
        {
            using var db = CreateDbContext();

            return [.. db.TemplateClasses
                .AsNoTracking()
                .Where(x => x.ClassId == classId)
                .Select(x => x.TemplateId)];
        }

        /// <summary>
        /// Replaces the further classes of a template.
        /// </summary>
        /// <remarks>
        /// The owning class of the template is never stored as a further class, and a class is
        /// stored once; the order of the given ids is kept. Rows already present are left
        /// alone, so the assignments that stay keep their ids.
        /// </remarks>
        /// <param name="templateId">The template.</param>
        /// <param name="classIds">The further classes.</param>
        public static void SetTemplateClasses(Guid templateId, IEnumerable<Guid> classIds)
        {
            using var db = CreateDbContext();

            var template = db.Templates.AsNoTracking().FirstOrDefault(x => x.Id == templateId);

            if (template is null)
            {
                return;
            }

            var wanted = (classIds ?? [])
                .Where(x => x != Guid.Empty && x != template.ClassId)
                .Distinct()
                .ToList();
            var existing = db.TemplateClasses
                .Where(x => x.TemplateId == templateId)
                .ToList();

            db.TemplateClasses.RemoveRange(existing.Where(x => !wanted.Contains(x.ClassId)));

            foreach (var classId in wanted.Where(x => existing.All(e => e.ClassId != x)))
            {
                db.TemplateClasses.Add(new TemplateClass
                {
                    TemplateId = templateId,
                    ClassId = classId
                });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified template from the database and persists the changes.
        /// </summary>
        /// <param name="templateEntry">
        /// The template object representing the entity to be removed based on its ID.
        /// Cannot be null.
        /// </param>
        public static void Remove(Template templateEntry)
        {
            ArgumentNullException.ThrowIfNull(templateEntry);

            using var db = CreateDbContext();

            var query = new Query<Template>()
                .WhereEquals(x => x.Id, templateEntry.Id);

            var dbEntry = query.Apply(db.Templates).FirstOrDefault();

            if (dbEntry is null)
            {
                return;
            }

            db.Remove(dbEntry);
            db.SaveChanges();
        }
    }
}