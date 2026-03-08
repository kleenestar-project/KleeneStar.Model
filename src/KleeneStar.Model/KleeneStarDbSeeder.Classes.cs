using KleeneStar.Model.Entities;
using System;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
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
            add("083DA1FE-9E75-47B4-8CC4-39106000847B", "Compliance", "Compliance rules and audits.", "/kleenestar/assets/icons/compliance.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("7320C34D-F58A-4D7F-9761-8A998B9BFEC3", "Asset", "Configuration items and assets.", "/kleenestar/assets/icons/asset.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("4112E505-B27F-427B-91E6-756C95FC307F", "Relationship", "Dependencies and relations between assets.", "/kleenestar/assets/icons/rel.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("23998D21-5457-4779-921F-0420B3D41967", "Policy", "Configuration and governance policies.", "/kleenestar/assets/icons/policy.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("2D562E0C-CCF0-4484-8085-CC9FCA57E163", "Approval", "Approval workflows for configuration changes.", "/kleenestar/assets/icons/approval.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("237B8990-4BD2-4606-AC49-9BE35B769EF5", "Vulnerability", "Security vulnerabilities and risk items.", "/kleenestar/assets/icons/vuln.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");
            add("969A4DAF-6B4D-4908-BA94-E4A46FD97BD0", "ChangeRequest", "Requests for configuration changes.", "/kleenestar/assets/icons/change.svg", "D651799A-690E-4CFF-AE0C-D3341CA3BBB4");

            // workspace: Software Development (DEV)
            add("4411D773-811A-4242-95CC-22CFC9B78CDA", "Repository", "Source code repositories.", "/kleenestar/assets/icons/repo.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("B95B82D2-4C70-4908-ABF6-2525F9AC378B", "BuildPipeline", "Automated build pipelines.", "/kleenestar/assets/icons/build.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("41BF8EF5-03B8-4CCD-8666-83D91450ABC5", "Release", "Release management and deployment.", "/kleenestar/assets/icons/release.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("0657665A-051F-4DB5-A782-265A1FE373DD", "Task", "Development tasks and work items.", "/kleenestar/assets/icons/task.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("C43355BF-2EE1-4B0C-8632-9F9C17FDCAF4", "Bug", "Bug tracking and issue management.", "/kleenestar/assets/icons/bug.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("1CF19C71-6FFA-4006-8014-929D3D0079F8", "Documentation", "Technical documentation and specs.", "/kleenestar/assets/icons/doc.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");
            add("D3F7D136-F214-4E4C-8CB5-9F43659F46A7", "Sprint", "Agile sprint planning and tracking.", "/kleenestar/assets/icons/sprint.svg", "660E9B11-2D54-4A36-84F9-F3BF5C78B748");

            // workspace: Finance and Controlling (FIN)
            add("9BDF73BB-9777-4FA5-9335-DA00A55D5337", "Budget", "Budget planning and allocation.", "/kleenestar/assets/icons/budget.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");
            add("EFD694A1-5E7E-4C21-BC4A-1F270A9BDE01", "Invoice", "Invoices and billing documents.", "/kleenestar/assets/icons/invoice.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");
            add("2DC257FD-74D5-4187-B845-E36BAAC30694", "Approval", "Financial approval workflows.", "/kleenestar/assets/icons/approval.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");
            add("20C2A713-0593-4A01-8E54-44FCC8D09035", "CostCenter", "Cost center management.", "/kleenestar/assets/icons/costcenter.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");
            add("C9CF53BA-1114-463F-B053-18469914E3E6", "Contract", "Financial contracts and agreements.", "/kleenestar/assets/icons/contract.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");
            add("2113E9E7-5F5F-48B7-8E16-57BD1EBBDA0B", "Forecast", "Financial forecasting.", "/kleenestar/assets/icons/forecast.svg", "9994445E-FDBE-42E2-A3A0-65DF13CB453B");

            // workspace: Human Resources (HR)
            add("FAF023A8-9DEF-49E5-A043-3E9C547F3BC8", "Employee", "Employee master data.", "/kleenestar/assets/icons/employee.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");
            add("6362F942-9B91-4352-A177-E1A0E6FEF673", "Onboarding", "Onboarding workflows.", "/kleenestar/assets/icons/onboarding.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");
            add("091A41C4-80A3-40C2-A607-B9F2069D613D", "Position", "Job positions and roles.", "/kleenestar/assets/icons/position.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");
            add("E54E59DB-48E5-4357-9B9B-839F5074E0E1", "Absence", "Absence and vacation tracking.", "/kleenestar/assets/icons/absence.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");
            add("EA0BA82B-16C7-4422-AE2E-E56EBBD447AB", "Training", "Employee training and certifications.", "/kleenestar/assets/icons/training.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");
            add("5435188C-F44D-4D2D-B664-2CF689F3BAA3", "OrganizationUnit", "Departments and org structure.", "/kleenestar/assets/icons/orgunit.svg", "8C8F10A2-4A98-4A8C-B0C4-12359CAC3C5F");

            // workspace: Product Management(PM)
            add("4C1473C2-F2FB-4D94-84C5-9673F237EB9C", "Feature", "Product features and epics.", "/kleenestar/assets/icons/feature.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");
            add("C1B890BA-8BF0-42E9-9787-C3C499175B6F", "ReleasePlan", "Release planning and timelines.", "/kleenestar/assets/icons/releaseplan.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");
            add("23A68A06-5045-49F9-B81A-A30A7B163248", "Roadmap", "Product roadmap items.", "/kleenestar/assets/icons/roadmap.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");
            add("9289A89E-672F-4273-84EC-90B227C74252", "Requirement", "Functional and non-functional requirements.", "/kleenestar/assets/icons/requirement.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");
            add("737EDAC2-DB73-4C21-A937-93836E3C704A", "Stakeholder", "Stakeholder management.", "/kleenestar/assets/icons/stakeholder.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");
            add("AB898A6D-8008-4425-BC00-F2D98F61ACF6", "UseCase", "Use cases and scenarios.", "/kleenestar/assets/icons/usecase.svg", "25F04599-C02A-4BEF-80B1-5957FBBB1FED");

            // workspace: Procurement (PROC)
            add("EBA77F0B-0EF1-4548-AC2C-9F6669D54AA4", "PurchaseOrder", "Purchase orders and requests.", "/kleenestar/assets/icons/po.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");
            add("858ADDB3-3A1B-4995-A340-1BF761259FA5", "Supplier", "Supplier and vendor data.", "/kleenestar/assets/icons/supplier.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");
            add("8E6647BB-E2CF-416C-B0E3-C445686E271D", "Contract", "Procurement contracts.", "/kleenestar/assets/icons/contract.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");
            add("CF323A7C-FDC2-486C-895B-C7AFA4978C1D", "Tender", "Tender and bidding processes.", "/kleenestar/assets/icons/tender.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");
            add("AD4EF26A-B871-4000-B5F1-89D4510F4992", "Invoice", "Procurement-related invoices.", "/kleenestar/assets/icons/invoice.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");
            add("E8E3A5FD-BD60-40AC-8590-4C24C0E5A7ED", "Delivery", "Delivery and logistics tracking.", "/kleenestar/assets/icons/delivery.svg", "D35FDCD6-5B11-4043-98D6-215DF414D99C");

            // workspace: IT Service Desk (SD)
            add("02A1933C-3209-4E17-B228-3468A18543E8", "Ticket", "Service desk tickets.", "/kleenestar/assets/icons/ticket.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
            add("D16D1B15-C83F-4489-9B1F-CCC46855D619", "Incident", "Incident management.", "/kleenestar/assets/icons/incident.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
            add("14CF0195-8E7E-443A-A9CB-469C4C3C3DCC", "Problem", "Problem management.", "/kleenestar/assets/icons/problem.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
            add("481C4914-EA75-4B6B-96DC-6B594A1A7F3A", "Change", "Change management.", "/kleenestar/assets/icons/change.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
            add("4F80098A-C986-415A-9288-1C85232C27C9", "Knowledge", "Knowledge base articles.", "/kleenestar/assets/icons/knowledge.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
            add("52A740B7-1978-454E-9A46-DF2D20888E2E", "ServiceRequest", "Service request items.", "/kleenestar/assets/icons/servicerequest.svg", "F027A791-4219-4B1D-BA7C-2E7757091AAA");
        }
    }
}
