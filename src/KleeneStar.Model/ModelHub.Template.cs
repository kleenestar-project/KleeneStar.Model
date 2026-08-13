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
            dbEntry.ParentId = templateEntry.ParentId;
            dbEntry.Order = templateEntry.Order;
            dbEntry.Updated = DateTime.UtcNow;

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