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
        /// Adds a predefined set of form entities to the specified database context.
        /// </summary>
        /// <remarks>
        /// This method is intended to seed the database with a standard set of forms.
        /// Each class receives the three standard forms (Create, Edit, View). Every standard
        /// form is given a default tab "General" containing field references to all active
        /// fields previously seeded for that class, so the dynamic form rendering pipeline
        /// has structure to display from the moment the database is first provisioned.
        /// All additional forms beyond the three standard ones are created as empty
        /// templates that can be configured later via the form editor.
        /// It does not save changes to the database; callers must call SaveChanges on the
        /// context to persist the additions.
        /// </remarks>
        /// <param name="db">The database context to which the form entities will be added. Cannot be null.</param>
        private static void SeedForms(KleeneStarDbContext db)
        {
            var classes = db.Classes
                .AsNoTracking()
                .ToList();

            // group fields by class id once so we can attach field references to the tab
            // of every standard form without re-querying inside the loop.
            var fieldsByClass = db.Fields
                .AsNoTracking()
                .Where(f => f.State == FieldState.Active && !f.Deprecated)
                .ToList()
                .GroupBy(f => f.ClassId)
                .ToDictionary(g => g.Key, g => g.OrderBy(f => f.Name).ToList());

            foreach (var cls in classes)
            {
                fieldsByClass.TryGetValue(cls.Id, out var fields);
                fields ??= [];

                AddStandardForm(db, cls, "Create", FormType.Create, "/kleenestar/assets/icons/form/create.svg", fields);
                AddStandardForm(db, cls, "Edit", FormType.Edit, "/kleenestar/assets/icons/form/edit.svg", fields);
                AddStandardForm(db, cls, "View", FormType.View, "/kleenestar/assets/icons/form/view.svg", fields);

                // retrieve the additional form templates for the current class
                var templates = GetFormsTemplatesForClass(cls.Name);

                foreach (var template in templates)
                {
                    // additional forms are created without a tab structure; they are
                    // intended to be configured in the form designer by an administrator.
                    db.Forms.Add(new Form
                    {
                        Id = Guid.NewGuid(),
                        Name = template.Name,
                        Description = template.Description,
                        FormType = FormType.Default,
                        State = FormState.Active,
                        Icon = ImageIcon.FromString(template.Icon),
                        ClassId = cls.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    });
                }
            }
        }

        /// <summary>
        /// Adds a single standard form (Create, Edit, or View) for the given class together
        /// with its initial structure: one tab labelled "General" that contains a
        /// <see cref="FormFieldRefElement"/> per active field of the class.
        /// </summary>
        /// <param name="db">The database context to mutate.</param>
        /// <param name="cls">The class the form belongs to.</param>
        /// <param name="name">The display name of the form (also used as a unique key per class).</param>
        /// <param name="formType">The form type (<see cref="FormType.Create"/>, <see cref="FormType.Edit"/>, <see cref="FormType.View"/>).</param>
        /// <param name="iconUri">Relative URI of the form icon.</param>
        /// <param name="fields">Active fields of the class to be referenced from the tab, in render order.</param>
        private static void AddStandardForm
        (
            KleeneStarDbContext db,
            Class cls,
            string name,
            FormType formType,
            string iconUri,
            IReadOnlyList<Field> fields
        )
        {
            var formId = Guid.NewGuid();
            var tabId = Guid.NewGuid();

            db.Forms.Add(new Form
            {
                Id = formId,
                Name = name,
                Description = "Standard form for the class.",
                FormType = formType,
                State = FormState.Active,
                Icon = ImageIcon.FromString(iconUri),
                ClassId = cls.Id,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            });

            db.FormTabs.Add(new FormTab
            {
                Id = tabId,
                FormId = formId,
                Name = "General",
                Position = 0
            });

            for (var i = 0; i < fields.Count; i++)
            {
                db.FormElements.Add(new FormFieldRefElement
                {
                    Id = Guid.NewGuid(),
                    FormTabId = tabId,
                    ParentElementId = null,
                    Position = i,
                    FieldId = fields[i].Id
                });
            }
        }

        /// <summary>
        /// Returns form templates based on the given class name.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <returns>A list of form templates.</returns>
        private static IReadOnlyList<(string Name, string Description, string Icon)> GetFormsTemplatesForClass(string className)
        {
            // use shared default forms for all classes to keep seed data maintainable
            var defaults = new List<(string Name, string Description, string Icon)>
            {
                (
                    "Workflow Overview",
                    "General overview form showing key workflow information, status and metadata.",
                    "/kleenestar/assets/icons/form/workflow-overview.svg"
                ),
                (
                    "Transition Form",
                    "Form used when performing a workflow transition, including required fields and actions.",
                    "/kleenestar/assets/icons/form/workflow-transition.svg"
                ),
                (
                    "Decision Form",
                    "Form for decision points within the workflow, such as approvals or branching steps.",
                    "/kleenestar/assets/icons/form/workflow-decision.svg"
                )
            };

            switch (className)
            {
                case "Incident":
                case "Problem":
                case "ServiceRequest":
                case "Ticket":
                    {
                        return
                        [
                            .. defaults,
                        ("Self-Service Form", "Simplified form for end users.", "/kleenestar/assets/icons/form/selfservice.svg"),
                        ("Resolver Form", "Detailed form for support agents.", "/kleenestar/assets/icons/form/resolver.svg")
                        ];
                    }

                case "Change":
                case "ChangeRequest":
                    {
                        return
                        [
                            .. defaults,
                        ("Approval Form", "Form displaying details required for approval.", "/kleenestar/assets/icons/form/approval.svg")
                        ];
                    }

                case "Employee":
                    {
                        return
                        [
                            .. defaults,
                        ("Onboarding Form", "Form for new employee registration.", "/kleenestar/assets/icons/form/onboarding.svg")
                        ];
                    }

                default:
                    {
                        return defaults;
                    }
            }
        }
    }
}
