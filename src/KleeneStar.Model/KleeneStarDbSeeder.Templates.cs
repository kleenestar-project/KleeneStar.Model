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
        /// Describes a template to seed, including its place in the two template hierarchies.
        /// </summary>
        /// <param name="Name">The template name, unique within its workspace across the seeded set.</param>
        /// <param name="Description">The description, or null to inherit it.</param>
        /// <param name="Category">The category, or null to inherit it.</param>
        /// <param name="Icon">The icon uri, or null to inherit it.</param>
        /// <param name="State">The lifecycle state.</param>
        /// <param name="Parent">The name of the template this one is a part of, or null.</param>
        /// <param name="Order">The position among the siblings of the parent template.</param>
        /// <param name="Presets">The serialized field presets, or null.</param>
        private sealed record TemplateSeed
        (
            string Name,
            string Description,
            string Category,
            string Icon,
            TemplateState State = TemplateState.Active,
            string Parent = null,
            int Order = 0,
            string Presets = null
        );

        /// <summary>
        /// Adds a predefined set of templates to the specified database context, bound to the
        /// classes they instantiate and wired into the template hierarchy.
        /// </summary>
        /// <remarks>
        /// A template is the guided starting point of object creation: it carries presentation
        /// metadata (name, description, category, icon) and field presets for a class its objects
        /// are created from. Beyond the flat set, the templates below exercise the composition
        /// hierarchy: a template with a parent becomes a sub-object whenever an object is created
        /// from that parent. The references are resolved in a second pass, so a template may point
        /// at one that the first pass creates later. This method does not save changes to the
        /// database; callers must call SaveChanges on the context to persist the additions.
        /// </remarks>
        /// <param name="db">The database context to which the templates will be added. Cannot be null.</param>
        private static void SeedTemplates(KleeneStarDbContext db)
        {
            // retrieve all classes including their associated workspaces
            var classes = db.Classes
                .Include(c => c.Workspace)
                .AsNoTracking()
                .ToList();

            // the seeded names are unique per workspace, so a workspace-qualified name is enough
            // to resolve the references of the second pass
            var created = new Dictionary<string, Template>(StringComparer.OrdinalIgnoreCase);
            var seeds = new List<(TemplateSeed Seed, string WorkspaceKey)>();

            foreach (var cls in classes)
            {
                foreach (var seed in GetTemplatesForClassAndWorkspace(cls.Name, cls.Workspace?.Key))
                {
                    var template = new Template
                    {
                        Id = Guid.NewGuid(),
                        Name = seed.Name,
                        Description = seed.Description,
                        Category = seed.Category,
                        Icon = seed.Icon is null ? null : ImageIcon.FromString(seed.Icon),
                        State = seed.State,
                        Order = seed.Order,
                        Presets = seed.Presets,
                        ClassId = cls.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    };

                    db.Templates.Add(template);
                    created[Reference(cls.Workspace?.Key, seed.Name)] = template;
                    seeds.Add((seed, cls.Workspace?.Key));
                }
            }

            // second pass: every template exists now, so the parent references resolve
            foreach (var (seed, workspaceKey) in seeds)
            {
                created[Reference(workspaceKey, seed.Name)].ParentId = Resolve(created, workspaceKey, seed.Parent);
            }
        }

        /// <summary>
        /// Builds the lookup key of a seeded template.
        /// </summary>
        /// <param name="workspaceKey">The key of the workspace the template belongs to.</param>
        /// <param name="name">The template name.</param>
        /// <returns>The workspace-qualified name.</returns>
        private static string Reference(string workspaceKey, string name)
        {
            return $"{workspaceKey}/{name}";
        }

        /// <summary>
        /// Resolves a hierarchy reference of a seeded template to the id it points at.
        /// </summary>
        /// <param name="created">The templates created by the first pass.</param>
        /// <param name="workspaceKey">The key of the workspace the reference is resolved within.</param>
        /// <param name="name">The referenced template name, or null.</param>
        /// <returns>The id of the referenced template, or null when the reference is absent.</returns>
        private static Guid? Resolve(IReadOnlyDictionary<string, Template> created, string workspaceKey, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return created.TryGetValue(Reference(workspaceKey, name), out var template)
                ? template.Id
                : null;
        }

        /// <summary>
        /// Returns the templates defined for the given class within the given workspace. A class
        /// the set says nothing about carries no templates, so object creation for it starts from
        /// a blank form.
        /// </summary>
        /// <param name="className">The name of the class the templates instantiate.</param>
        /// <param name="workspaceKey">The key of the workspace the class belongs to.</param>
        /// <returns>The templates of the class, which may be empty.</returns>
        private static IReadOnlyList<TemplateSeed> GetTemplatesForClassAndWorkspace(string className, string workspaceKey)
        {
            return (workspaceKey, className) switch
            {
                ("SD", "Incident") =>
                [
                    new("Software Issue", "Report a bug or software problem with an application or service.", "IT Support", "/kleenestar/assets/icons/incident.svg",
                        Presets: """{"Priority":"Medium","Impact":"Medium","Urgency":"Medium"}"""),
                    new("Outage Report", "Report a service that is unreachable or degraded for multiple users.", "IT Support", "/kleenestar/assets/icons/incident.svg",
                        Presets: """{"Priority":"Critical","Impact":"High","Urgency":"High"}""")
                ],
                ("SD", "ServiceRequest") =>
                [
                    new("Hardware Request", "Request new IT equipment such as a laptop, monitor or headset.", "IT Support", "/kleenestar/assets/icons/servicerequest.svg",
                        Presets: """{"Priority":"Medium","Impact":"Medium","Urgency":"Medium"}"""),
                    new("Software License", "Request a license for a commercial application.", "IT Support", "/kleenestar/assets/icons/servicerequest.svg",
                        Presets: """{"Priority":"Medium","Impact":"Medium","Urgency":"Medium"}"""),
                    new("Network Access", "Request access to a network share, VPN profile or wireless network.", "Operations", "/kleenestar/assets/icons/servicerequest.svg",
                        State: TemplateState.Archived)
                ],
                ("SD", "Change") =>
                [
                    new("Standard Change", "A pre-approved, low-risk change following an established procedure.", "Operations", "/kleenestar/assets/icons/change.svg",
                        Presets: """{"Priority":"Low","Impact":"Low"}"""),
                    new("Emergency Change", "An urgent change required to restore or protect a service.", "Operations", "/kleenestar/assets/icons/change.svg",
                        Presets: """{"Priority":"Critical","Impact":"High","Urgency":"High"}""")
                ],
                ("SD", "Problem") =>
                [
                    new("Root Cause Analysis", "Investigate the underlying cause of one or more recurring incidents.", "Operations", "/kleenestar/assets/icons/problem.svg")
                ],
                ("SD", "Knowledge") =>
                [
                    new("How-To Article", "Step-by-step instructions for a recurring user task.", "Knowledge Base", "/kleenestar/assets/icons/knowledge.svg"),
                    new("Known Error", "A documented workaround for a known defect.", "Knowledge Base", "/kleenestar/assets/icons/knowledge.svg")
                ],

                ("DEV", "Bug") =>
                [
                    new("Bug Report", "Report a defect with steps to reproduce, expected and actual behaviour.", "Engineering", "/kleenestar/assets/icons/bug.svg"),
                    new("Regression", "A defect in behaviour that worked in an earlier release.", "Engineering", "/kleenestar/assets/icons/bug.svg",
                        Presets: """{"Priority":"High"}""")
                ],
                ("DEV", "Task") =>
                [
                    new("Feature Work", "Implementation work for a planned feature.", "Engineering", "/kleenestar/assets/icons/feature.svg",
                        Presets: """{"Priority":"Medium","EstimatedHours":"8"}"""),
                    new("Technical Debt", "Clean-up or refactoring work with no user-visible change.", "Engineering", "/kleenestar/assets/icons/task.svg",
                        Presets: """{"Priority":"Low","EstimatedHours":"8"}"""),
                    new("Spike", "A time-boxed investigation to reduce uncertainty before planning.", "Engineering", "/kleenestar/assets/icons/task.svg",
                        Presets: """{"Priority":"Medium","EstimatedHours":"4"}"""),

                    // the three steps of the release checklist below; each becomes an object
                    // beneath the release when one is created from "Release Checklist"
                    new("Cut Release Branch", "Branch off and freeze the release candidate.", "Delivery", "/kleenestar/assets/icons/repo.svg",
                        Parent: "Release Checklist", Order: 1),
                    new("Run Regression Suite", "Execute the full regression suite against the candidate.", "Delivery", "/kleenestar/assets/icons/build.svg",
                        Parent: "Release Checklist", Order: 2,
                        Presets: """{"EstimatedHours":"6"}""")
                ],
                ("DEV", "Release") =>
                [
                    new("Release Checklist", "Prepare, verify and announce a product release.", "Delivery", "/kleenestar/assets/icons/release.svg")
                ],
                ("DEV", "Documentation") =>
                [
                    new("API Reference", "Document an endpoint, its parameters and its responses.", "Documentation", "/kleenestar/assets/icons/doc.svg"),
                    new("Publish Release Notes", "Summarize the changes of the release for its audience.", "Delivery", "/kleenestar/assets/icons/doc.svg",
                        Parent: "Release Checklist", Order: 3)
                ],

                ("HR", "Onboarding") =>
                [
                    new("New Employee", "Start the onboarding process for a new colleague.", "HR & Onboarding", "/kleenestar/assets/icons/onboarding.svg"),
                    new("Internal Transfer", "Move a colleague to another team or position.", "HR & Onboarding", "/kleenestar/assets/icons/onboarding.svg"),

                    // the onboarding steps; each becomes an object beneath the onboarding when one
                    // is created from "New Employee"
                    new("Prepare Workplace", "Order desk, hardware and access badge.", "HR & Onboarding", "/kleenestar/assets/icons/asset.svg",
                        Parent: "New Employee", Order: 1,
                        Presets: """{"Priority":"High","EstimatedHours":"2"}"""),
                    new("Set Up Accounts", "Create the accounts and group memberships the role needs.", "HR & Onboarding", "/kleenestar/assets/icons/employee.svg",
                        Parent: "New Employee", Order: 2,
                        Presets: """{"Priority":"High"}"""),
                    new("Schedule Intro Day", "Book the introduction day and inform the team.", "HR & Onboarding", "/kleenestar/assets/icons/orgunit.svg",
                        Parent: "New Employee", Order: 3)
                ],
                ("HR", "Absence") =>
                [
                    new("Leave Request", "Apply for paid time off.", "People", "/kleenestar/assets/icons/absence.svg"),
                    new("Sick Leave", "Report an absence due to illness.", "People", "/kleenestar/assets/icons/absence.svg")
                ],
                ("HR", "Training") =>
                [
                    new("Training Request", "Request a course, conference or certification.", "People", "/kleenestar/assets/icons/training.svg"),
                    new("Assign Onboarding Training", "Enrol the new colleague in the mandatory introduction courses.", "HR & Onboarding", "/kleenestar/assets/icons/training.svg",
                        Parent: "New Employee", Order: 4)
                ],

                ("FIN", "Invoice") =>
                [
                    new("Supplier Invoice", "Record an incoming invoice for review and payment.", "Accounting", "/kleenestar/assets/icons/invoice.svg")
                ],
                ("FIN", "Budget") =>
                [
                    new("Budget Request", "Request budget for a project or purchase.", "Accounting", "/kleenestar/assets/icons/budget.svg")
                ],
                ("FIN", "Contract") =>
                [
                    new("Contract Renewal", "Review and renew an expiring supplier contract.", "Procurement", "/kleenestar/assets/icons/contract.svg")
                ],

                ("CMDB", "Asset") =>
                [
                    new("Workstation", "Register a laptop or desktop as a configuration item.", "Inventory", "/kleenestar/assets/icons/asset.svg",
                        Presets: """{"Priority":"Low"}"""),
                    new("Server", "Register a physical or virtual server as a configuration item.", "Inventory", "/kleenestar/assets/icons/asset.svg",
                        Presets: """{"Priority":"High"}""")
                ],
                ("CMDB", "Vulnerability") =>
                [
                    new("Security Finding", "Record a vulnerability discovered on a configuration item.", "Security", "/kleenestar/assets/icons/vuln.svg",
                        Presets: """{"Priority":"High"}""")
                ],
                ("CMDB", "ChangeRequest") =>
                [
                    new("Configuration Change", "Request a change to a configuration item.", "Operations", "/kleenestar/assets/icons/change.svg")
                ],

                _ => []
            };
        }
    }
}
