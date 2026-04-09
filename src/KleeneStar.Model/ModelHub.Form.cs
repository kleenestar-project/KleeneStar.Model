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
        /// Returns a queryable collection of forms from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <remarks>
        /// The returned query is not executed until enumerated. Multiple predicates are combined
        /// using logical AND. The query includes related category data for each form.
        /// </remarks>
        /// <param name="query">
        /// The query criteria used to filter the returned forms. Must not be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of forms.
        /// </returns>
        public static IEnumerable<Form> GetForms(IQuery<Form> query)
        {
            using var db = CreateDbContext();

            return [.. GetForms(query, db)]; // materialize query
        }

        /// <summary>
        /// Returns a queryable collection of forms from the database, optionally filtered 
        /// by one or more predicate expressions.
        /// </summary>
        /// <param name="query">
        /// The query criteria used to filter the returned forms. Must not be null.
        /// </param>
        /// <param name="context">
        /// The context in which the query is executed. Provides additional information or constraints 
        /// for the retrieval operation. Cannot be null.
        /// </param>
        /// <returns>
        /// An enumeration representing the filtered collection of forms.
        /// </returns>
        public static IEnumerable<Form> GetForms(IQuery<Form> query, KleeneStarDbContext context)
        {
            var data = context.Forms
                .Include(form => form.Class)
                    .ThenInclude(@class => @class.Workspace)
                .AsNoTracking();

            return query.Apply(data); // none materialize query
        }

        /// <summary>
        /// Adds the specified form to the database if it does not already exist.
        /// </summary>
        /// <remarks>
        /// If a form with the same key (case-insensitive) already exists in the 
        /// database, this method does nothing.
        /// </remarks>
        /// <param name="formEntry">
        /// The form to add. The form's id property is used to determine uniqueness. 
        /// Cannot be null.
        /// </param>
        public static void Add(Form formEntry)
        {
            ArgumentNullException.ThrowIfNull(formEntry);

            using var db = CreateDbContext();

            var query = new Query<Form>()
                .WhereEquals(x => x.Id, formEntry.Id);

            if (query.Apply(db.Forms).Any())
            {
                return;
            }

            db.AddEntity(formEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the specified form in the database.
        /// </summary>
        /// <param name="formEntry">
        /// The form to update. Cannot be null.
        /// </param>
        public static void Update(Form formEntry)
        {
            ArgumentNullException.ThrowIfNull(formEntry);

            using var db = CreateDbContext();

            db.UpdateEntity(formEntry);

            // persist changes
            db.SaveChanges();
        }

        /// <summary>
        /// Removes the specified form from the data store if it exists.
        /// </summary>
        /// <remarks>
        /// Standard forms cannot be removed. Only additional forms can be deleted.
        /// </remarks>
        /// <param name="formEntry">
        /// The form entity to remove.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when attempting to remove a standard form.
        /// </exception>
        public static void Remove(Form formEntry)
        {
            ArgumentNullException.ThrowIfNull(formEntry);

            if (formEntry.FormType == FormType.Standard)
            {
                throw new InvalidOperationException("A standard form cannot be deleted.");
            }

            using var db = CreateDbContext();

            // verify the form in the database is not a standard form
            var existing = db.Forms.FirstOrDefault(f => f.Id == formEntry.Id);
            if (existing != null && existing.FormType == FormType.Standard)
            {
                throw new InvalidOperationException("A standard form cannot be deleted.");
            }

            db.RemoveEntity(formEntry);

            // persist changes
            db.SaveChanges();
        }
    }
}
