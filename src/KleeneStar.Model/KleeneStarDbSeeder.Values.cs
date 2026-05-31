using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Populates the <c>Value</c> table with data for every seeded object so that
        /// detail and edit views have something to display from the very first launch.
        /// For each object, one <see cref="Value"/> row is created per active field of
        /// the object's class. The payload is generated deterministically based on the
        /// field's name and the object's index so that successive seed runs produce the
        /// same data.
        /// </summary>
        /// <param name="db">The database context used for adding the new values.</param>
        private static void SeedValues(KleeneStarDbContext db)
        {
            var objects = db.Objects
                .AsNoTracking()
                .OrderBy(o => o.Created)
                .ThenBy(o => o.Key)
                .ToList();

            // group fields by class id so we only walk them once per class
            var fieldsByClass = db.Fields
                .AsNoTracking()
                .Where(f => f.State == FieldState.Active && !f.Deprecated)
                .ToList()
                .GroupBy(f => f.ClassId)
                .ToDictionary(g => g.Key, g => g.OrderBy(f => f.Name).ToList());

            // names of the priorities defined for each class (in display order) so that a
            // priority-typed field is seeded with one of its class's real priorities rather
            // than a generic placeholder.
            var priorityNamesByClass = db.Priorities
                .AsNoTracking()
                .ToList()
                .GroupBy(p => p.ClassId)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.OrderBy(p => p.Order).Select(p => p.Name).ToList());

            // sequence number per (object, field) lets us generate distinct values for
            // each row when the same field type recurs across objects of the same class.
            var indexByClass = new Dictionary<Guid, int>();

            foreach (var entity in objects)
            {
                if (!fieldsByClass.TryGetValue(entity.ClassId, out var fields))
                {
                    continue;
                }

                if (!priorityNamesByClass.TryGetValue(entity.ClassId, out var priorityNames))
                {
                    priorityNames = [];
                }

                if (!indexByClass.TryGetValue(entity.ClassId, out var sequence))
                {
                    sequence = 0;
                }

                foreach (var field in fields)
                {
                    var data = GenerateSampleData(field, entity, sequence, priorityNames);

                    if (data is null)
                    {
                        continue;
                    }

                    db.Values.Add(new Value
                    {
                        Id = Guid.NewGuid(),
                        ObjectId = entity.Id,
                        FieldId = field.Id,
                        Data = data,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    });
                }

                indexByClass[entity.ClassId] = sequence + 1;
            }
        }

        /// <summary>
        /// Produces a deterministic, semantically plausible string representation for the
        /// given (<paramref name="field"/>, <paramref name="entity"/>) pair. The seed values
        /// are designed to be human-readable so the UI looks populated out of the box.
        /// </summary>
        /// <param name="field">The class field for which a value should be produced.</param>
        /// <param name="entity">The object the value is being seeded for.</param>
        /// <param name="sequence">A per-class sequence index used to vary string contents.</param>
        /// <param name="priorityNames">The names of the priorities defined for the object's class, in display order, used to seed priority-typed fields.</param>
        /// <returns>The string payload to store in <see cref="Value.Data"/>, or <c>null</c> when the field should be left blank.</returns>
        private static string GenerateSampleData(Field field, Entities.Object entity, int sequence, IReadOnlyList<string> priorityNames)
        {
            // a few field names appear across many classes; treat them up-front so the
            // seeded data lines up with what the user would expect to see for that field.
            switch (field.Name)
            {
                case "Title":
                    return entity.Summary;

                case "Description":
                    return entity.Description;

                case "CreatedAt":
                    return entity.Created.ToString("o", CultureInfo.InvariantCulture);

                case "UpdatedAt":
                    return entity.Updated.ToString("o", CultureInfo.InvariantCulture);

                case "Status":
                    // Mirror the seeded workflow status names so the workflow status card can
                    // resolve the value to a real status.
                    return PickRoundRobin(["New", "In Progress", "Resolved", "Closed"], sequence);

                case "Priority":
                    return priorityNames.Count > 0
                        ? PickRoundRobin(priorityNames, sequence)
                        : PickRoundRobin(["Low", "Medium", "High", "Critical"], sequence);

                case "Category":
                    return PickRoundRobin(["Hardware", "Software", "Network", "Other"], sequence);

                case "Owner":
                    return PickRoundRobin(["Max Power", "Erika Mustermann", "John Doe", "Jane Smith"], sequence);

                case "Tags":
                    return PickRoundRobin(["urgent", "internal", "external", "review"], sequence);

                case "Impact":
                    return PickRoundRobin(["Low", "Medium", "High"], sequence);

                case "Urgency":
                    return PickRoundRobin(["Low", "Medium", "High"], sequence);

                case "AffectedService":
                    return PickRoundRobin(["VPN", "Email", "Intranet", "CRM", "ERP"], sequence);

                case "Risk":
                    return PickRoundRobin(["Low", "Medium", "High"], sequence);

                case "PlannedStart":
                    return entity.Created.AddDays(1).ToString("o", CultureInfo.InvariantCulture);

                case "PlannedEnd":
                    return entity.Created.AddDays(3).ToString("o", CultureInfo.InvariantCulture);

                case "Approval":
                    return PickRoundRobin(["Pending", "Approved", "Rejected"], sequence);

                case "SerialNumber":
                    return $"SN-{entity.Key}-{sequence:D4}";

                case "Location":
                    return PickRoundRobin(["Berlin DC", "Frankfurt DC", "Munich Office", "Remote"], sequence);

                case "Vendor":
                    return PickRoundRobin(["Dell", "HP", "Cisco", "Lenovo", "Microsoft"], sequence);

                case "WarrantyUntil":
                    return entity.Created.AddYears(3).ToString("o", CultureInfo.InvariantCulture);

                case "PersonnelNumber":
                    return $"P-{1000 + sequence:D5}";

                case "Department":
                    return PickRoundRobin(["Engineering", "Sales", "HR", "Finance", "Operations"], sequence);

                case "Email":
                    return $"user.{sequence}@example.com";

                case "Phone":
                    return $"+49 30 555-{sequence:D4}";

                case "Amount":
                    return ((sequence + 1) * 1234.56).ToString("F2", CultureInfo.InvariantCulture);

                case "Currency":
                    return PickRoundRobin(["EUR", "USD", "GBP"], sequence);

                case "Period":
                    return $"{DateTime.UtcNow.Year}-Q{(sequence % 4) + 1}";

                case "ApprovedBy":
                    return PickRoundRobin(["Lisa Robinson", "Thomas Allen", "Patricia Lee"], sequence);
            }

            // fall back to a value derived purely from the field type so unknown custom
            // fields still get a sensible seed.
            return GenerateFromType(field, entity, sequence, priorityNames);
        }

        /// <summary>
        /// Generates a seed value purely from the field's <see cref="FieldType"/> when no
        /// name-based mapping applies. Multi-cardinality fields (cardinality unlimited or
        /// max greater than one) are encoded as a comma-separated list to keep the seed
        /// readable; the API layer is free to interpret the data however it likes.
        /// </summary>
        /// <param name="field">The class field for which a value should be produced.</param>
        /// <param name="entity">The object the value is being seeded for.</param>
        /// <param name="sequence">A per-class sequence index used to vary string contents.</param>
        /// <param name="priorityNames">The names of the priorities defined for the object's class, in display order, used to seed priority-typed fields.</param>
        /// <returns>The string payload to store in <see cref="Value.Data"/>.</returns>
        private static string GenerateFromType(Field field, Entities.Object entity, int sequence, IReadOnlyList<string> priorityNames)
        {
            var multi = field.CardinalityUnlimited || field.CardinalityMax > 1;

            switch (field.FieldType)
            {
                case FieldType.Boolean:
                    return ((sequence % 2) == 0) ? "true" : "false";

                case FieldType.Number:
                    return (sequence + 1).ToString(CultureInfo.InvariantCulture);

                case FieldType.Date:
                    return entity.Created.AddDays(sequence).ToString("o", CultureInfo.InvariantCulture);

                case FieldType.Selection:
                    if (field.Options is { Count: > 0 })
                    {
                        return PickRoundRobin(field.Options, sequence);
                    }
                    return $"Option {(sequence % 3) + 1}";

                case FieldType.Tag:
                    return multi
                        ? "alpha,beta,gamma"
                        : "alpha";

                case FieldType.Reference:
                    return $"REF-{entity.Key}-{sequence:D3}";

                case FieldType.Workflow:
                    return PickRoundRobin(["new", "in_progress", "done"], sequence);

                case FieldType.Priority:
                    return priorityNames.Count > 0
                        ? PickRoundRobin(priorityNames, sequence)
                        : PickRoundRobin(["Low", "Medium", "High", "Critical"], sequence);

                case FieldType.User:
                    return PickRoundRobin(["max.power", "erika.mustermann", "john.doe"], sequence);

                case FieldType.Attachment:
                    // attachments are file references; we leave them empty so the UI shows the
                    // empty-state and the file picker rather than a broken link.
                    return null;

                case FieldType.Text:
                default:
                    return $"{field.Name} for {entity.Key}";
            }
        }

        /// <summary>
        /// Returns the sample at <c>index % options.Count</c> from the supplied options.
        /// </summary>
        private static string PickRoundRobin(IReadOnlyList<string> options, int index)
        {
            return options[((index % options.Count) + options.Count) % options.Count];
        }
    }
}
