using KleeneStar.Model.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model
{
    public static class KleeneStarDbSeeder
    {
        /// <summary>
        /// Ensures that the database is populated with required initial data if it is not 
        /// already present.
        /// </summary>
        /// <remarks>
        /// This method should be invoked during application startup to guarantee that the database
        /// contains the minimum necessary data for the application to operate. If the required data
        /// already exists, no changes are made.
        /// </remarks>
        /// <param name="db">
        /// The database context used for performing the seeding operation. Cannot be null.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous seeding process.
        /// </returns>
        public static async Task SeedAsync(KleeneStarDbContext db)
        {
            if (!db.Categories.Any())
            {
                SeedCategories(db);
                await db.SaveChangesAsync();
            }

            if (!db.Workspaces.Any())
            {
                SeedWorkspaces(db);
                await db.SaveChangesAsync();
            }

            if (!db.Classes.Any())
            {
                SeedClasses(db);
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Adds a predefined set of category entities to the specified database context.
        /// </summary>
        /// <remarks>
        /// This method is intended to seed the database with a standard set of categories. 
        /// It does not save changes to the database; callers must call SaveChanges on the 
        /// context to persist the additions.
        /// </remarks>
        /// <param name="db">The database context to which the category entities will be added. Cannot be null.</param>
        private static void SeedCategories(KleeneStarDbContext db)
        {
            void add(string id, string name) => db.Categories.Add(new Category
            {
                Id = Guid.Parse(id),
                Name = name
            });

            add("6B90F75F-93C6-4186-965F-C2F974A04E05", "Compliance");
            add("AF7C999B-334F-4745-9799-AD46BCF7D7E0", "Development");
            add("DB91D6FD-1FB3-4F3B-B982-EDFCC733DEAF", "Engineering");
            add("9B08E835-4B55-4748-84BA-14AC1417C52E", "Finance");
            add("8CC28181-5DBC-4F29-8016-41885FF5A323", "HumanResources");
            add("26AC5FB6-CBAB-40A8-9D2F-5DA9E91910C9", "Infrastructure");
            add("9ECA0869-E563-41DB-81A2-50DC7D09478B", "Operations");
            add("F56D0256-4C67-47D7-AEF3-563D05EF9CEB", "Support");
        }

        /// <summary>
        /// Adds a default workspace to the specified database context if it 
        /// does not already exist.
        /// </summary>
        /// <param name="db">
        /// The database context to which the default workspace will be added. Cannot be null.
        /// </param>
        private static void SeedWorkspaces(KleeneStarDbContext db)
        {
            void add(string id, string name, string key, string description, string icon, params string[] categories) =>
                db.Workspaces.Add(new Workspace
                {
                    Id = Guid.Parse(id),
                    Name = name,
                    Key = key,
                    Description = description,
                    Icon = ImageIcon.FromString(icon),
                    Categories = [.. db.Categories.Where(x => categories.Contains(x.Name))],
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });

            add
            (
                "D651799A-690E-4CFF-AE0C-D3341CA3BBB4",
                "Configuration Database",
                "CMDB",
                "Workspace for managing configuration items and asset relationships.",
                "/kleenestar/assets/icons/cmdb.svg",
                "Infrastructure", "Compliance"
            );

            add
            (
                "660E9B11-2D54-4A36-84F9-F3BF5C78B748",
                "Software Development",
                "DEV",
                "Workspace for managing source code, repositories, development tasks, and release pipelines.",
                "/kleenestar/assets/icons/dev.svg",
                "Engineering"
            );

            add
            (
                "9994445E-FDBE-42E2-A3A0-65DF13CB453B",
                "Finance and Controlling",
                "FIN",
                "Workspace for managing budgets, invoices, and financial approvals.",
                "/kleenestar/assets/icons/fin.svg",
                "Finance"
            );

            add
            (
                "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F",
                "Human Resources",
                "HR",
                "Workspace for managing employee records, onboarding, and organizational structure.",
                "/kleenestar/assets/icons/hr.svg",
                "HumanResources"
            );

            add
            (
                "25F04599-C02A-4BEF-80B1-5957FBBB1FED",
                "Product Management",
                "PM",
                "Workspace for planning features, tracking releases, and coordinating product strategy.",
                "/kleenestar/assets/icons/pm.svg",
                "Development"
            );

            add
            (
                "D35FDCD6-5B11-4043-98D6-215DF414D99C",
                "Procurement",
                "PROC",
                "Workspace for managing purchase orders, supplier relations, and procurement workflows.",
                "/kleenestar/assets/icons/proc.svg",
                "Operations"
            );

            add
            (
                "F027A791-4219-4B1D-BA7C-2E7757091AAA",
                "IT Service Desk",
                "SD",
                "Workspace for handling service desk operations.",
                "/kleenestar/assets/icons/sd.svg",
                "Support"
            );
        }

        /// <summary>
        /// Adds default classes for each workspace if they do not already exist.
        /// </summary>
        /// <param name="db">The database context.</param>
        private static void SeedClasses(KleeneStarDbContext db)
        {
            void add(string id, string name, string description, string icon, string workspaceId)
                => db.Classes.Add(new Class
                {
                    Id = Guid.Parse(id),
                    Name = name,
                    Description = description,
                    Icon = ImageIcon.FromString(icon),
                    WorkspaceId = Guid.Parse(workspaceId),
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });

            // workspace: Configuration Database (CMDB)
            add("083DA1FE-9E75-47B4-8CC4-39106000847B", "Compliance", "Compliance rules and audits.", "/kleenestar/assets/icons/class-compliance.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("7320C34D-F58A-4D7F-9761-8A998B9BFEC3", "Asset", "Configuration items and assets.", "/kleenestar/assets/icons/class-asset.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("4112E505-B27F-427B-91E6-756C95FC307F", "Relationship", "Dependencies and relations between assets.", "/kleenestar/assets/icons/class-rel.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("23998D21-5457-4779-921F-0420B3D41967", "Policy", "Configuration and governance policies.", "/kleenestar/assets/icons/class-policy.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("2D562E0C-CCF0-4484-8085-CC9FCA57E163", "Approval", "Approval workflows for configuration changes.", "/kleenestar/assets/icons/class-approval.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("237B8990-4BD2-4606-AC49-9BE35B769EF5", "Vulnerability", "Security vulnerabilities and risk items.", "/kleenestar/assets/icons/class-vuln.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("969A4DAF-6B4D-4908-BA94-E4A46FD97BD0", "ChangeRequest", "Requests for configuration changes.", "/kleenestar/assets/icons/class-change.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");

            // workspace: Software Development (DEV)
            add("4411D773-811A-4242-95CC-22CFC9B78CDA", "Repository", "Source code repositories.", "/kleenestar/assets/icons/class-repo.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("B95B82D2-4C70-4908-ABF6-2525F9AC378B", "BuildPipeline", "Automated build pipelines.", "/kleenestar/assets/icons/class-build.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("41BF8EF5-03B8-4CCD-8666-83D91450ABC5", "Release", "Release management and deployment.", "/kleenestar/assets/icons/class-release.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("0657665A-051F-4DB5-A782-265A1FE373DD", "Task", "Development tasks and work items.", "/kleenestar/assets/icons/class-task.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("C43355BF-2EE1-4B0C-8632-9F9C17FDCAF4", "Bug", "Bug tracking and issue management.", "/kleenestar/assets/icons/class-bug.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("1CF19C71-6FFA-4006-8014-929D3D0079F8", "Documentation", "Technical documentation and specs.", "/kleenestar/assets/icons/class-doc.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("D3F7D136-F214-4E4C-8CB5-9F43659F46A7", "Sprint", "Agile sprint planning and tracking.", "/kleenestar/assets/icons/class-sprint.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");

            // workspace: Finance and Controlling (FIN)
            add("9BDF73BB-9777-4FA5-9335-DA00A55D5337", "Budget", "Budget planning and allocation.", "/kleenestar/assets/icons/class-budget.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");
            add("EFD694A1-5E7E-4C21-BC4A-1F270A9BDE01", "Invoice", "Invoices and billing documents.", "/kleenestar/assets/icons/class-invoice.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");
            add("2DC257FD-74D5-4187-B845-E36BAAC30694", "Approval", "Financial approval workflows.", "/kleenestar/assets/icons/class-approval.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");
            add("20C2A713-0593-4A01-8E54-44FCC8D09035", "CostCenter", "Cost center management.", "/kleenestar/assets/icons/class-costcenter.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");
            add("C9CF53BA-1114-463F-B053-18469914E3E6", "Contract", "Financial contracts and agreements.", "/kleenestar/assets/icons/class-contract.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");
            add("2113E9E7-5F5F-48B7-8E16-57BD1EBBDA0B", "Forecast", "Financial forecasting.", "/kleenestar/assets/icons/class-forecast.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");

            // workspace: Human Resources (HR)
            add("FAF023A8-9DEF-49E5-A043-3E9C547F3BC8", "Employee", "Employee master data.", "/kleenestar/assets/icons/class-employee.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");
            add("6362F942-9B91-4352-A177-E1A0E6FEF673", "Onboarding", "Onboarding workflows.", "/kleenestar/assets/icons/class-onboarding.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");
            add("091A41C4-80A3-40C2-A607-B9F2069D613D", "Position", "Job positions and roles.", "/kleenestar/assets/icons/class-position.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");
            add("E54E59DB-48E5-4357-9B9B-839F5074E0E1", "Absence", "Absence and vacation tracking.", "/kleenestar/assets/icons/class-absence.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");
            add("EA0BA82B-16C7-4422-AE2E-E56EBBD447AB", "Training", "Employee training and certifications.", "/kleenestar/assets/icons/class-training.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");
            add("5435188C-F44D-4D2D-B664-2CF689F3BAA3", "OrganizationUnit", "Departments and org structure.", "/kleenestar/assets/icons/class-orgunit.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");

            // workspace: Product Management(PM)
            add("4C1473C2-F2FB-4D94-84C5-9673F237EB9C", "Feature", "Product features and epics.", "/kleenestar/assets/icons/class-feature.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");
            add("C1B890BA-8BF0-42E9-9787-C3C499175B6F", "ReleasePlan", "Release planning and timelines.", "/kleenestar/assets/icons/class-releaseplan.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");
            add("23A68A06-5045-49F9-B81A-A30A7B163248", "Roadmap", "Product roadmap items.", "/kleenestar/assets/icons/class-roadmap.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");
            add("9289A89E-672F-4273-84EC-90B227C74252", "Requirement", "Functional and non-functional requirements.", "/kleenestar/assets/icons/class-requirement.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");
            add("737EDAC2-DB73-4C21-A937-93836E3C704A", "Stakeholder", "Stakeholder management.", "/kleenestar/assets/icons/class-stakeholder.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");
            add("AB898A6D-8008-4425-BC00-F2D98F61ACF6", "UseCase", "Use cases and scenarios.", "/kleenestar/assets/icons/class-usecase.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");

            // workspace: Procurement (PROC)
            add("EBA77F0B-0EF1-4548-AC2C-9F6669D54AA4", "PurchaseOrder", "Purchase orders and requests.", "/kleenestar/assets/icons/class-po.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");
            add("858ADDB3-3A1B-4995-A340-1BF761259FA5", "Supplier", "Supplier and vendor data.", "/kleenestar/assets/icons/class-supplier.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");
            add("8E6647BB-E2CF-416C-B0E3-C445686E271D", "Contract", "Procurement contracts.", "/kleenestar/assets/icons/class-contract.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");
            add("CF323A7C-FDC2-486C-895B-C7AFA4978C1D", "Tender", "Tender and bidding processes.", "/kleenestar/assets/icons/class-tender.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");
            add("AD4EF26A-B871-4000-B5F1-89D4510F4992", "Invoice", "Procurement-related invoices.", "/kleenestar/assets/icons/class-invoice.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");
            add("E8E3A5FD-BD60-40AC-8590-4C24C0E5A7ED", "Delivery", "Delivery and logistics tracking.", "/kleenestar/assets/icons/class-delivery.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");

            // workspace: IT Service Desk (SD)
            add("02A1933C-3209-4E17-B228-3468A18543E8", "Ticket", "Service desk tickets.", "/kleenestar/assets/icons/class-ticket.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
            add("D16D1B15-C83F-4489-9B1F-CCC46855D619", "Incident", "Incident management.", "/kleenestar/assets/icons/class-incident.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
            add("14CF0195-8E7E-443A-A9CB-469C4C3C3DCC", "Problem", "Problem management.", "/kleenestar/assets/icons/class-problem.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
            add("481C4914-EA75-4B6B-96DC-6B594A1A7F3A", "Change", "Change management.", "/kleenestar/assets/icons/class-change.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
            add("4F80098A-C986-415A-9288-1C85232C27C9", "Knowledge", "Knowledge base articles.", "/kleenestar/assets/icons/class-knowledge.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
            add("52A740B7-1978-454E-9A46-DF2D20888E2E", "ServiceRequest", "Service request items.", "/kleenestar/assets/icons/class-servicerequest.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
        }
    }
}
