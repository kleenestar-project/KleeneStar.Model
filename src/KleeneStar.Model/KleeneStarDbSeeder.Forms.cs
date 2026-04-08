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
        /// It does not save changes to the database; callers must call SaveChanges on the 
        /// context to persist the additions.
        /// </remarks>
        /// <param name="db">The database context to which the form entities will be added. Cannot be null.</param>
        private static void SeedForms(KleeneStarDbContext db)
        {
            var classes = db.Classes
                .AsNoTracking()
                .ToList();

            foreach (var cls in classes)
            {
                // retrieve the specific form templates for the current class
                var templates = GetFormsTemplatesForClass(cls.Name);

                foreach (var template in templates)
                {
                    // add the instantiated form entity to the context
                    db.Forms.Add(new Form
                    {
                        Id = Guid.NewGuid(),
                        Name = template.Name,
                        Description = template.Description,
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
        /// Returns form templates based on the given class name.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <returns>A list of form templates.</returns>
        private static IReadOnlyList<(string Name, string Description, string Icon)> GetFormsTemplatesForClass(string className)
        {
            // use shared default forms for all classes to keep seed data maintainable
            var defaults = new List<(string Name, string Description, string Icon)>
            {
                ("Default Form", "Standard view for the entry.", "/kleenestar/assets/icons/form/default.svg"),
                ("Create Form", "Form used when creating a new entry.", "/kleenestar/assets/icons/form/create.svg"),
                ("Edit Form", "Form used when modifying an existing entry.", "/kleenestar/assets/icons/form/edit.svg")
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