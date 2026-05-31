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
        /// This method is intended to seed the database with a standard set of fields. The
        /// default field set deliberately spans every <see cref="FieldType"/> (text, number,
        /// date, boolean, selection, reference, workflow, attachment, user, tag, priority) so
        /// the forms rendered from the seed data contain at least one field of each type out
        /// of the box.
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
                        FieldType = template.Type,
                        Cardinality = FieldCardinality.Single,
                        Options = template.Options is null ? new List<string>() : new List<string>(template.Options),
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
        /// Returns field templates based on class name. Each template carries the
        /// <see cref="FieldType"/> and, for selection fields, the selectable options.
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <returns>A list of field templates.</returns>
        private static IReadOnlyList<(string Name, string Description, string Icon, FieldType Type, string[] Options)> GetFieldTemplatesForClass(string className)
        {
            // The shared default set covers every FieldType so any class's standard form
            // contains at least one field of each kind. Workflow fields are linked to the
            // class workflow later, in SeedWorkflows.
            var defaults = new List<(string Name, string Description, string Icon, FieldType Type, string[] Options)>
            {
                ("Title",          "Short human-readable title of the entry.",  "/kleenestar/assets/icons/field/title.svg",       FieldType.Text,       null),
                ("Description",    "Detailed description of the entry.",         "/kleenestar/assets/icons/field/description.svg", FieldType.Text,       null),
                ("Status",         "Lifecycle status of the entry.",            "/kleenestar/assets/icons/field/status.svg",      FieldType.Workflow,   null),
                ("Priority",       "Priority or urgency classification.",       "/kleenestar/assets/icons/field/priority.svg",    FieldType.Priority,   null),
                ("Category",       "Categorization of the entry.",              "/kleenestar/assets/icons/field/category.svg",    FieldType.Selection,  ["Hardware", "Software", "Network", "Other"]),
                ("Owner",          "Responsible person or team.",               "/kleenestar/assets/icons/field/owner.svg",       FieldType.User,       null),
                ("Tags",           "Keywords used for filtering and grouping.", "/kleenestar/assets/icons/field/tags.svg",        FieldType.Tag,        null),
                ("CreatedAt",      "Timestamp when the entry was created.",     "/kleenestar/assets/icons/field/created.svg",     FieldType.Date,       null),
                ("UpdatedAt",      "Timestamp of the last update.",             "/kleenestar/assets/icons/field/updated.svg",     FieldType.Date,       null),
                ("EstimatedHours", "Estimated effort in hours.",                "/kleenestar/assets/icons/field/period.svg",      FieldType.Number,     null),
                ("Confidential",   "Whether the entry is confidential.",        "/kleenestar/assets/icons/field/approval.svg",    FieldType.Boolean,    null),
                ("RelatedItem",    "Reference to a related entry.",             "/kleenestar/assets/icons/field/id.svg",          FieldType.Reference,  null),
                ("Attachment",     "Attached file or document.",                "/kleenestar/assets/icons/field/description.svg", FieldType.Attachment, null)
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
                        ("Impact",          "Business impact classification.",   "/kleenestar/assets/icons/field/impact.svg",  FieldType.Selection, ["Low", "Medium", "High"]),
                        ("Urgency",         "Urgency classification for triage.", "/kleenestar/assets/icons/field/urgency.svg", FieldType.Selection, ["Low", "Medium", "High"]),
                        ("AffectedService", "Related affected service.",         "/kleenestar/assets/icons/field/service.svg", FieldType.Reference, null)
                    ];

                case "Change":
                case "ChangeRequest":
                    return
                    [
                        .. defaults,
                        ("Risk",         "Risk level of the proposed change.", "/kleenestar/assets/icons/field/risk.svg",     FieldType.Selection, ["Low", "Medium", "High"]),
                        ("PlannedStart", "Planned start date and time.",       "/kleenestar/assets/icons/field/calendar.svg", FieldType.Date,      null),
                        ("PlannedEnd",   "Planned end date and time.",         "/kleenestar/assets/icons/field/calendar.svg", FieldType.Date,      null),
                        ("Approval",     "Approval status for the change.",    "/kleenestar/assets/icons/field/approval.svg", FieldType.Selection, ["Pending", "Approved", "Rejected"])
                    ];

                case "Asset":
                case "ConfigurationItem":
                    return
                    [
                        .. defaults,
                        ("SerialNumber",  "Vendor serial number.",         "/kleenestar/assets/icons/field/id.svg",       FieldType.Text,      null),
                        ("Location",      "Physical or logical location.", "/kleenestar/assets/icons/field/location.svg", FieldType.Text,      null),
                        ("Vendor",        "Vendor or manufacturer.",       "/kleenestar/assets/icons/field/vendor.svg",   FieldType.Reference, null),
                        ("WarrantyUntil", "Warranty expiration date.",     "/kleenestar/assets/icons/field/warranty.svg", FieldType.Date,      null)
                    ];

                case "Employee":
                    return
                    [
                        .. defaults,
                        ("PersonnelNumber", "Internal employee number.",      "/kleenestar/assets/icons/field/id.svg",    FieldType.Text, null),
                        ("Department",      "Organizational department.",     "/kleenestar/assets/icons/field/org.svg",   FieldType.Text, null),
                        ("Email",           "Primary business email address.","/kleenestar/assets/icons/field/email.svg", FieldType.Text, null),
                        ("Phone",           "Primary business phone number.", "/kleenestar/assets/icons/field/phone.svg", FieldType.Text, null)
                    ];

                case "Budget":
                case "Invoice":
                case "CostCenter":
                    return
                    [
                        .. defaults,
                        ("Amount",     "Monetary amount.",               "/kleenestar/assets/icons/field/currency.svg", FieldType.Number,    null),
                        ("Currency",   "Currency code (e.g., EUR, USD).", "/kleenestar/assets/icons/field/currency.svg", FieldType.Selection, ["EUR", "USD", "GBP"]),
                        ("Period",     "Accounting period.",             "/kleenestar/assets/icons/field/period.svg",   FieldType.Text,      null),
                        ("ApprovedBy", "Approving manager or role.",     "/kleenestar/assets/icons/field/approval.svg", FieldType.User,      null)
                    ];

                default:
                    return defaults;
            }
        }
    }
}
