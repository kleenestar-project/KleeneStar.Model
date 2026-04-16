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
        /// Adds a predefined set of field entities to the specified database context.
        /// </summary>
        /// <remarks>
        /// This method is intended to seed the database with a standard set of fields. 
        /// It does not save changes to the database; callers must call SaveChanges on the 
        /// context to persist the additions.
        /// </remarks>
        /// <param name="db">The database context to which the category entities will be added. Cannot be null.</param>
        private static void SeedFields(KleeneStarDbContext db)
        {
            var classes = db.Classes
                .AsNoTracking()
                .ToList();

            foreach (var cls in classes)
            {
                var templates = GetFieldTemplatesForClass(cls.Name);

                foreach (var template in templates)
                {
                    db.Fields.Add(new Field
                    {
                        Id = Guid.NewGuid(),
                        Name = template.Name,
                        Description = template.Description,
                        HelpText = null,
                        Placeholder = null,
                        State = FieldState.Active,
                        Icon = ImageIcon.FromString(template.Icon),
                        ClassId = cls.Id,
                        FieldType = "Text",
                        Cardinality = FieldCardinality.Single,
                        ValidationRules = new List<string>(),
                        DefaultSpec = null,
                        Required = false,
                        Unique = false,
                        Deprecated = false,
                        AccessModifier = AccessModifier.Public,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    });
                }
            }
        }

        /// <summary>
        /// Returns field templates based on class name.
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <returns>A list of field templates.</returns>
        private static IReadOnlyList<(string Name, string Description, string Icon)> GetFieldTemplatesForClass(string className)
        {
            // use shared defaults to keep seed data compact and maintainable
            var defaults = new List<(string Name, string Description, string Icon)>
            {
                ("Title", "Short human-readable title of the entry.", "/kleenestar/assets/icons/field/title.svg"),
                ("Description", "Detailed description of the entry.", "/kleenestar/assets/icons/field/description.svg"),
                ("Status", "Lifecycle status of the entry.", "/kleenestar/assets/icons/field/status.svg"),
                ("Priority", "Priority or urgency classification.", "/kleenestar/assets/icons/field/priority.svg"),
                ("Owner", "Responsible person or team.", "/kleenestar/assets/icons/field/owner.svg"),
                ("Tags", "Keywords used for filtering and grouping.", "/kleenestar/assets/icons/field/tags.svg"),
                ("CreatedAt", "Timestamp when the entry was created.", "/kleenestar/assets/icons/field/created.svg"),
                ("UpdatedAt", "Timestamp of the last update.", "/kleenestar/assets/icons/field/updated.svg")
            };

            switch (className)
            {
                case "Incident":
                case "Problem":
                case "ServiceRequest":
                case "Ticket":
                    return
                    [
                        .. defaults,
                        ("Impact", "Business impact classification.", "/kleenestar/assets/icons/field/impact.svg"),
                        ("Urgency", "Urgency classification for triage.", "/kleenestar/assets/icons/field/urgency.svg"),
                        ("AffectedService", "Related affected service.", "/kleenestar/assets/icons/field/service.svg")
                    ];

                case "Change":
                case "ChangeRequest":
                    return
                    [
                        .. defaults,
                        ("Risk", "Risk level of the proposed change.", "/kleenestar/assets/icons/field/risk.svg"),
                        ("PlannedStart", "Planned start date and time.", "/kleenestar/assets/icons/field/calendar.svg"),
                        ("PlannedEnd", "Planned end date and time.", "/kleenestar/assets/icons/field/calendar.svg"),
                        ("Approval", "Approval status for the change.", "/kleenestar/assets/icons/field/approval.svg")
                    ];

                case "Asset":
                case "ConfigurationItem":
                    return
                    [
                        .. defaults,
                        ("SerialNumber", "Vendor serial number.", "/kleenestar/assets/icons/field/id.svg"),
                        ("Location", "Physical or logical location.", "/kleenestar/assets/icons/field/location.svg"),
                        ("Vendor", "Vendor or manufacturer.", "/kleenestar/assets/icons/field/vendor.svg"),
                        ("WarrantyUntil", "Warranty expiration date.", "/kleenestar/assets/icons/field/warranty.svg")
                    ];

                case "Employee":
                    return
                    [
                        .. defaults,
                        ("PersonnelNumber", "Internal employee number.", "/kleenestar/assets/icons/field/id.svg"),
                        ("Department", "Organizational department.", "/kleenestar/assets/icons/field/org.svg"),
                        ("Email", "Primary business email address.", "/kleenestar/assets/icons/field/email.svg"),
                        ("Phone", "Primary business phone number.", "/kleenestar/assets/icons/field/phone.svg")
                    ];

                case "Budget":
                case "Invoice":
                case "CostCenter":
                    return
                    [
                        .. defaults,
                        ("Amount", "Monetary amount.", "/kleenestar/assets/icons/field/currency.svg"),
                        ("Currency", "Currency code (e.g., EUR, USD).", "/kleenestar/assets/icons/field/currency.svg"),
                        ("Period", "Accounting period.", "/kleenestar/assets/icons/field/period.svg"),
                        ("ApprovedBy", "Approving manager or role.", "/kleenestar/assets/icons/field/approval.svg")
                    ];

                default:
                    return defaults;
            }
        }
    }
}
