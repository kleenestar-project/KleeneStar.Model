using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
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

            if (!db.Objects.Any())
            {
                SeedObjects(db);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    Console.WriteLine($"Error seeding objects: {ex.InnerException.Message}");
                    throw; // Re-throwing the exception after logging
                }
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

        /// <summary>
        /// Populates the database with initial object data across multiple classes.
        /// </summary>
        /// <param name="db">The database context used for adding the new objects.</param>
        private static void SeedObjects(KleeneStarDbContext db)
        {
            var c = 1000;
            var classes = db.Classes
                .AsNoTracking()
                .Include(c => c.Workspace)
                .ToList();

            foreach (var cls in classes)
            {
                // creates exactly 20 distinct items per class based on the extended seeding logic
                for (int i = 0; i < 20; i++)
                {
                    var (summary, description) = GetRandomContentForClass(cls.Name, i);
                    var entity = new Entities.Object
                    {
                        Id = Guid.NewGuid(),
                        Key = $"{cls.Workspace.Key}-{c + i}",
                        Summary = summary,
                        Description = description,
                        Icon = cls.Icon,
                        State = TypeWorkspaceState.Active,
                        WorkspaceId = cls.WorkspaceId,
                        ClassId = cls.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    };

                    db.Objects.Add(entity);
                }

                c += 1000;
            }
        }

        /// <summary>
        /// Generates a name and a thematic description based on the class name.
        /// Returns 20 unique variations per class type for all known classes.
        /// </summary>
        /// <param name="className">The name of the class to generate content for.</param>
        /// <param name="index">The index (0-19) of the item being generated.</param>
        /// <returns>A tuple containing the generated name and description.</returns>
        private static (string Summary, string Description) GetRandomContentForClass(string className, int index)
        {
            string summary;
            string descriptionPrefix;
            string lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam.";

            // define a safe index accessor to avoid out of bounds if array lengths differ
            (string Summary, string Desc) Get(params (string, string)[] items) => items[index % items.Length];

            switch (className)
            {
                // --- CMDB Classes ---
                case "Compliance":
                    (summary, descriptionPrefix) = Get(
                        ("GDPR Audit 2025", "Annual audit for General Data Protection Regulation compliance."),
                        ("ISO 27001 Check", "Verification of information security management systems."),
                        ("SOC2 Type II", "Service Organization Control audit report."),
                        ("HIPAA Review", "Health Insurance Portability and Accountability Act assessment."),
                        ("PCI-DSS Scan", "Payment Card Industry Data Security Standard vulnerability scan."),
                        ("License Compliance", "Review of software license usage against entitlements."),
                        ("Accessibility Audit", "WCAG 2.1 AA compliance check for public portals."),
                        ("Data Retention", "Verification of data retention policies and deletion logs."),
                        ("Privacy Shield", "Assessment of cross-border data transfer mechanisms."),
                        ("Code Quality Audit", "Static analysis report for critical codebases."),
                        ("Security Training", "Completion rates for mandatory security awareness training."),
                        ("Vendor Assessment", "Third-party risk management assessment."),
                        ("Disaster Recovery", "Results from the annual disaster recovery simulation."),
                        ("Access Control", "Audit of privileged access management and logs."),
                        ("Physical Security", "Inspection of data center physical access controls."),
                        ("Cloud Governance", "Review of AWS/Azure IAM policies and spending."),
                        ("Ethical Guidelines", "Compliance check against corporate ethical standards."),
                        ("Anti-Money Laundering", "AML protocol verification for financial transactions."),
                        ("Environmental Audit", "ISO 14001 environmental management check."),
                        ("Safety Regulations", "OSHA workplace safety compliance review.")
                    );
                    break;
                case "Asset":
                    (summary, descriptionPrefix) = Get(
                        ("Core Switch Chicago", "High-performance L3 switch located in the Chicago datacenter."),
                        ("Firewall Perimeter A", "Primary perimeter firewall handling ingress traffic."),
                        ("Backup Server Node 1", "Dedicated storage node for nightly incremental backups."),
                        ("Application Server 04", "Virtual machine hosting the legacy CRM backend."),
                        ("Employee Laptop", "Standard issue ultrabook for remote development work."),
                        ("Meeting Room Display", "Interactive smart display in conference room B."),
                        ("Router Edge West", "Edge router connecting western branch offices."),
                        ("Load Balancer NGINX", "Software load balancer for the main web portal."),
                        ("Database Cluster", "Master node of the PostgreSQL production cluster."),
                        ("Print Server Main", "Centralized print management server."),
                        ("Wifi Access Point", "Wireless access point covering the lobby area."),
                        ("UPS Battery Array", "Uninterruptible power supply for rack 4."),
                        ("Virtual Host", "Physical host running 40+ virtual instances."),
                        ("Storage Area Network", "Fiber channel SAN for high-speed data access."),
                        ("Dev Workstation", "High-end workstation for graphics rendering."),
                        ("VPN Concentrator", "Hardware appliance for secure remote access."),
                        ("Mail Gateway Relay", "SMTP relay for outgoing corporate email."),
                        ("Proxy Server Squid", "Caching proxy to optimize web traffic."),
                        ("Security Camera", "IP camera monitoring the main entrance."),
                        ("Biometric Scanner", "Fingerprint scanner for server room access.")
                    );
                    break;
                case "Relationship":
                    (summary, descriptionPrefix) = Get(
                        ("Hosted On Host-01", "Defines the physical host for this virtual machine."),
                        ("Connected To Switch-A", "Physical uplink connection to the core switch."),
                        ("Depends On DB-01", "Critical dependency for application startup."),
                        ("Backup Of Vol-X", "Snapshot relationship for disaster recovery."),
                        ("Managed By Team-Ops", "Ownership assignment for the asset."),
                        ("Located In Rack-4", "Physical location within the datacenter."),
                        ("Powered By UPS-2", "Power redundancy dependency."),
                        ("Replicated To DR-Site", "Data replication link to secondary site."),
                        ("Route Through FW-1", "Network path dependency."),
                        ("Load Balanced By LB-1", "Association with the load balancer pool."),
                        ("Monitored By Nagios", "Monitoring agent assignment."),
                        ("Part Of Cluster-X", "Membership in high-availability cluster."),
                        ("Subnet Of VLAN-10", "Network segment assignment."),
                        ("Peer Of Node-2", "Cluster peer relationship."),
                        ("Client Of Auth-Svc", "Service dependency for authentication."),
                        ("Issued To User-J", "Hardware assignment to specific employee."),
                        ("Contracted Via Dell", "Procurement relationship."),
                        ("Patched By WSUS", "Update management association."),
                        ("Protected By AV", "Antivirus policy association."),
                        ("Audited By Ext-Firm", "External audit scope relationship.")
                    );
                    break;
                case "Policy":
                    (summary, descriptionPrefix) = Get(
                        ("Password Policy", "Rules regarding password complexity and rotation."),
                        ("Remote Work", "Guidelines for working from home and VPN usage."),
                        ("Acceptable Use", "Rules for personal use of company equipment."),
                        ("Data Classification", "Framework for labeling confidential data."),
                        ("Clean Desk", "Policy requiring desks to be cleared of sensitive docs."),
                        ("BYOD Policy", "Rules for bringing your own device to work."),
                        ("Social Media", "Guidelines for employees posting on social platforms."),
                        ("Incident Response", "Steps to take during a security breach."),
                        ("Cloud Usage", "Approved cloud providers and service tiers."),
                        ("Guest Access", "Rules for visitors connecting to office wifi."),
                        ("Encryption Standard", "Mandatory encryption levels for data at rest."),
                        ("Backup Retention", "Schedule for keeping and deleting backups."),
                        ("Code of Conduct", "General behavioral expectations."),
                        ("Software Install", "Whitelisted software allowed on workstations."),
                        ("Mobile Device Mgmt", "Required security profiles for mobile phones."),
                        ("Travel Policy", "Approved expenses and booking procedures."),
                        ("Gift Policy", "Limits on accepting gifts from vendors."),
                        ("Whistleblower", "Protections for reporting misconduct."),
                        ("Email Usage", "Guidelines for professional email communication."),
                        ("Termination Proc", "Steps for revoking access upon departure.")
                    );
                    break;
                case "Vulnerability":
                    (summary, descriptionPrefix) = Get(
                        ("CVE-2024-1234", "Remote code execution vulnerability in web server."),
                        ("Log4j Exploit", "Critical vulnerability in logging library."),
                        ("Weak Cipher Suite", "SSL/TLS configuration uses outdated ciphers."),
                        ("XSS on Login", "Cross-site scripting flaw on the login page."),
                        ("SQL Injection", "Input validation error allowing SQL injection."),
                        ("Default Password", "Device found with default factory credentials."),
                        ("Unpatched OS", "Server missing critical security updates."),
                        ("Open Port 21", "FTP service exposed to the public internet."),
                        ("Insecure Direct Ref", "IDOR vulnerability in API endpoint."),
                        ("CSRF Missing", "Cross-site request forgery token missing."),
                        ("Plaintext Auth", "Authentication sent over HTTP."),
                        ("Expired Certificate", "SSL certificate has expired."),
                        ("Privilege Escalation", "Local user can gain root access."),
                        ("Information Leak", "Debug mode enabled in production."),
                        ("Directory Listing", "Web server allows browsing directories."),
                        ("Weak SSH Key", "SSH key length falls below standards."),
                        ("DNS Zone Transfer", "DNS server allows full zone transfer."),
                        ("SMB Signing", "SMB signing not enforced on domain controller."),
                        ("Outdated PHP", "Running end-of-life PHP version."),
                        ("XML External Entity", "XXE vulnerability in parser.")
                    );
                    break;
                case "ChangeRequest":
                case "Change": // Covers both CMDB and SD
                    (summary, descriptionPrefix) = Get(
                        ("Upgrade DB RAM", "Request to double RAM on the main database server."),
                        ("Switch Replacement", "Scheduled replacement of end-of-life core switch."),
                        ("Firewall Rule Add", "Open port 443 for new marketing site."),
                        ("OS Patch Cycle", "Monthly patching of Windows servers."),
                        ("Deploy Hotfix", "Emergency deployment to fix production bug."),
                        ("Migrate to Cloud", "Moving legacy app to Azure."),
                        ("Update SSL Cert", "Renewing wildcard certificate."),
                        ("Decommission Server", "Turning off old file server."),
                        ("Install Docker", "Installing container runtime on dev nodes."),
                        ("Upgrade VPN", "Updating VPN client software."),
                        ("Change DNS TTL", "Lowering TTL before migration."),
                        ("Add Storage Vol", "Expanding SAN storage for backups."),
                        ("Reboot Cluster", "Rolling reboot of Kubernetes nodes."),
                        ("Update Firmware", "Firmware update for storage array."),
                        ("Config Change", "Modifying NGINX config for caching."),
                        ("User Access Revoke", "Removing admin rights for old group."),
                        ("Network Segment", "Creating new VLAN for IoT devices."),
                        ("Office Move", "Relocating IT assets to new floor."),
                        ("Vendor Access", "Granting temporary access to consultant."),
                        ("Disaster Test", "Failover test for critical systems.")
                    );
                    break;

                // --- DEV Classes ---
                case "Repository":
                    (summary, descriptionPrefix) = Get(
                        ("frontend-react", "Main customer-facing single page application."),
                        ("backend-api", "API Gateway handling routing and authentication."),
                        ("auth-service", "Microservice dedicated to OAuth2 logic."),
                        ("shared-utils", "Shared utility library used across projects."),
                        ("docker-images", "Collection of hardened Dockerfiles."),
                        ("legacy-billing", "Maintained COBOL logic for invoices."),
                        ("mobile-ios", "Native iOS application source code."),
                        ("mobile-android", "Native Android application source code."),
                        ("terraform-infra", "IaC definitions for AWS."),
                        ("internal-wiki", "Markdown source for documentation."),
                        ("notification-svc", "Service handling email/push."),
                        ("db-migrations", "SQL scripts for schema versioning."),
                        ("cli-tools", "CLI for internal ops tasks."),
                        ("admin-panel", "Web dashboard for support staff."),
                        ("payment-stripe", "Stripe integration layer."),
                        ("config-repo", "Centralized config files."),
                        ("e2e-tests", "Cypress testing suite."),
                        ("analytics-pipe", "Data processing pipeline."),
                        ("ml-engine", "Recommendation engine model."),
                        ("landing-pages", "Static marketing site.")
                    );
                    break;
                case "BuildPipeline":
                    (summary, descriptionPrefix) = Get(
                        ("CI-Main-Branch", "Continuous integration for main branch commits."),
                        ("Nightly-Build", "Comprehensive build and test run every night."),
                        ("Deploy-Staging", "Auto-deployment to staging environment."),
                        ("Release-Prod", "Manual trigger pipeline for production release."),
                        ("Lint-Check", "Static code analysis and linting."),
                        ("Security-Scan", "Dependency vulnerability scanning."),
                        ("Docker-Publish", "Build and push images to registry."),
                        ("Mobile-Beta", "Build and upload to TestFlight."),
                        ("Docs-Generate", "Auto-generation of API documentation."),
                        ("DB-Migrate-Test", "Test database migration scripts."),
                        ("Performance-Test", "Load testing pipeline."),
                        ("Frontend-Asset-Build", "Webpack build for static assets."),
                        ("Backup-Verification", "Restores backup to test integrity."),
                        ("Infrastructure-Plan", "Terraform plan generation."),
                        ("Infrastructure-Apply", "Terraform apply execution."),
                        ("Clean-Artifacts", "Cleanup of old build artifacts."),
                        ("SonarQube-Analysis", "Code quality metrics upload."),
                        ("Integration-Tests", "Running heavy integration tests."),
                        ("Feature-Branch-CI", "Lightweight CI for pull requests."),
                        ("Rollback-Pipeline", "Emergency rollback automation.")
                    );
                    break;
                case "Release":
                    (summary, descriptionPrefix) = Get(
                        ("v1.0.0 Initial", "First public release of the platform."),
                        ("v1.1.0 Feature", "Added user profiles and settings."),
                        ("v1.1.1 Hotfix", "Fixed critical login bug."),
                        ("v2.0.0 Major", "Complete UI overhaul."),
                        ("v2.1.0 Beta", "Beta release for testing new search."),
                        ("v2.1.0 Stable", "Stable release of search features."),
                        ("Mobile v3.4", "iOS and Android update."),
                        ("Backend v5.0", "Migration to .NET 8."),
                        ("Q1 Rollup", "Quarterly feature rollup."),
                        ("Security Patch Jan", "January security updates."),
                        ("API v2 Deprecation", "Removing old endpoints."),
                        ("Admin Tool v1", "Internal tool release."),
                        ("Holiday Theme", "Temporary UI theme for holidays."),
                        ("Performance Pack", "Optimization focused release."),
                        ("Localization Update", "Added Spanish and French."),
                        ("Hotfix-Auth", "Fixing OAuth token issue."),
                        ("Analytics v2", "New dashboard charts."),
                        ("Payment v3", "SCA compliance update."),
                        ("Chatbot v1", "Initial AI assistant release."),
                        ("Legacy Sunset", "Final release for legacy app.")
                    );
                    break;
                case "Task":
                    (summary, descriptionPrefix) = Get(
                        ("Refactor Auth", "Switch authentication to new provider."),
                        ("Update Icons", "Replace PNG icons with SVGs."),
                        ("Fix Typos", "Correct spelling in " + className + " module."),
                        ("Optimize SQL", "Improve query performance on index."),
                        ("Write Unit Tests", "Increase coverage for service layer."),
                        ("Update Readme", "Add setup instructions to repo."),
                        ("Investigate Crash", "Analyze logs for random 500 error."),
                        ("Design Login", "Create mockups for login screen."),
                        ("Setup Linter", "Configure ESLint rules."),
                        ("Migrate Data", "Move user data to new schema."),
                        ("Review PR #42", "Code review for feature branch."),
                        ("Update Dep", "Upgrade React to latest version."),
                        ("Config Logging", "Set up structured logging."),
                        ("Create Seeder", "Write database seed script."),
                        ("Fix CSS Z-Index", "Modal appearing behind header."),
                        ("Add Tooltip", "Add help text to form fields."),
                        ("Test on IE11", "Verify compatibility (or remove support)."),
                        ("Clean Up Code", "Remove commented out code."),
                        ("Document API", "Update Swagger definition."),
                        ("Research Lib", "Evaluate libraries for charts.")
                    );
                    break;
                case "Bug":
                    (summary, descriptionPrefix) = Get(
                        ("NullRefException", "Crash when user object is null."),
                        ("UI Glitch", "Button misaligned on mobile."),
                        ("Data Loss", "Form not saving all fields."),
                        ("Slow Load", "Page takes >5s to render."),
                        ("Search Broken", "Search returns no results."),
                        ("Login Loop", "User redirected to login repeatedly."),
                        ("Wrong Color", "Header uses old brand color."),
                        ("Memory Leak", "Server RAM usage increases over time."),
                        ("API 404", "Endpoint returns 404 incorrectly."),
                        ("Date Format", "Date showing as MM/DD instead of DD/MM."),
                        ("Calculation Error", "Total price off by 1 cent."),
                        ("Image Broken", "Profile picture not loading."),
                        ("Sort Order", "List not sorting alphabetically."),
                        ("Filter Fail", "Filtering by active status fails."),
                        ("Export Empty", "CSV export contains no data."),
                        ("Print Layout", "Print view cuts off text."),
                        ("Email Bounced", "System emails rejected by spam filter."),
                        ("Dead Link", "Footer link goes to 404."),
                        ("Session Timeout", "User logged out too quickly."),
                        ("Double Submit", "Form submits twice on click.")
                    );
                    break;
                case "Documentation":
                    (summary, descriptionPrefix) = Get(
                        ("API Reference", "Swagger documentation for public API."),
                        ("User Manual", "Guide for end-users of the portal."),
                        ("Architecture Diagram", "High-level system design."),
                        ("Setup Guide", "Developer onboarding instructions."),
                        ("Database Schema", "ERD diagram of the database."),
                        ("Deployment Steps", "Checklist for production release."),
                        ("Style Guide", "UI/UX branding guidelines."),
                        ("Troubleshooting", "Common issues and fixes."),
                        ("Release Notes", "Changelog for recent versions."),
                        ("Security Policy", "Internal security coding standards."),
                        ("Network Map", "Topology of the infrastructure."),
                        ("Disaster Plan", "Recovery procedures."),
                        ("Meeting Notes", "Minutes from architecture review."),
                        ("Product Spec", "Requirements for new feature."),
                        ("API Authentication", "How to use OAuth tokens."),
                        ("Coding Standards", "C# naming conventions."),
                        ("Git Workflow", "Branching strategy guide."),
                        ("License File", "Open source license text."),
                        ("Vendor Docs", "Documentation from third parties."),
                        ("Tutorial Video", "Script for training video.")
                    );
                    break;
                case "Sprint":
                    (summary, descriptionPrefix) = Get(
                        ("Sprint 24", "Focus on technical debt."),
                        ("Sprint 25", "Feature development for Q1."),
                        ("Sprint 26", "Bug bashing week."),
                        ("Sprint 27", "UI polish and refinement."),
                        ("Sprint 28", "Backend performance tuning."),
                        ("Sprint 29", "Security hardening."),
                        ("Sprint 30", "Mobile app beta prep."),
                        ("Sprint 31", "Integration testing."),
                        ("Sprint 32", "User feedback implementation."),
                        ("Sprint 33", "Documentation update."),
                        ("Sprint 34", "Holiday freeze preparation."),
                        ("Sprint 35", "New year roadmap start."),
                        ("Sprint 36", "Search engine optimization."),
                        ("Sprint 37", "Reporting module overhaul."),
                        ("Sprint 38", "Payment gateway switch."),
                        ("Sprint 39", "Accessibility fixes."),
                        ("Sprint 40", "Infrastructure upgrade."),
                        ("Sprint 41", "Admin tool features."),
                        ("Sprint 42", "Pre-release stabilization."),
                        ("Sprint 43", "Post-release support.")
                    );
                    break;

                // --- FIN Classes ---
                case "Budget":
                    (summary, descriptionPrefix) = Get(
                        ("IT Budget 2025", "Allocated funds for IT department."),
                        ("Marketing Q1", "Campaign spend for first quarter."),
                        ("R&D Allocation", "Investment in new product research."),
                        ("HR Training", "Budget for employee development."),
                        ("Sales Travel", "Travel expenses for sales team."),
                        ("Office Ops", "Rent and utilities budget."),
                        ("Hardware Refresh", "Capital for laptop upgrades."),
                        ("Cloud Spend", "Monthly Azure/AWS cost cap."),
                        ("Events Budget", "Conference and trade show funds."),
                        ("Legal Fees", "Retainer for external counsel."),
                        ("Software Lic", "SaaS subscription budget."),
                        ("Bonuses", "Year-end performance bonus pool."),
                        ("Consulting", "Budget for external contractors."),
                        ("Security Upgrades", "Funds for firewall replacement."),
                        ("Team Building", "Quarterly team event funds."),
                        ("Charity", "Corporate social responsibility fund."),
                        ("Recruiting", "Headhunter and job board fees."),
                        ("Production", "Manufacturing cost budget."),
                        ("Logistics", "Shipping and warehouse budget."),
                        ("Contingency", "Emergency reserve fund.")
                    );
                    break;
                case "CostCenter":
                    (summary, descriptionPrefix) = Get(
                        ("1001 IT-Ops", "Information Technology Operations."),
                        ("1002 Dev", "Software Development."),
                        ("2001 HR-Main", "Human Resources Headquarters."),
                        ("3001 Sales-US", "Sales Department North America."),
                        ("3002 Sales-EU", "Sales Department Europe."),
                        ("4001 Mark-Dig", "Digital Marketing."),
                        ("4002 Mark-Prt", "Print and Event Marketing."),
                        ("5001 Exec", "Executive Leadership Team."),
                        ("6001 Fin-Acc", "Accounting and Payroll."),
                        ("6002 Fin-Ctrl", "Controlling and Audit."),
                        ("7001 Leg-Gen", "General Legal Counsel."),
                        ("8001 R&D-AI", "Artificial Intelligence Research."),
                        ("8002 R&D-Hard", "Hardware Prototyping."),
                        ("9001 Fac-NY", "New York Facility Management."),
                        ("9002 Fac-LDN", "London Facility Management."),
                        ("9999 Misc", "Miscellaneous Overhead."),
                        ("1100 Supp-L1", "Customer Support Tier 1."),
                        ("1101 Supp-L2", "Customer Support Tier 2."),
                        ("1200 Sec-Phy", "Physical Security."),
                        ("1201 Sec-Cyb", "Cyber Security Team.")
                    );
                    break;
                case "Forecast":
                    (summary, descriptionPrefix) = Get(
                        ("Q3 Revenue", "Projected income for Q3."),
                        ("Q4 Hiring", "Headcount growth forecast."),
                        ("2026 Growth", "Long-term market expansion plan."),
                        ("Cash Flow Jan", "Expected liquidity for January."),
                        ("Expense Forecast", "Predicted operational costs."),
                        ("Profit Margin", "Estimated net profit margin."),
                        ("Sales Pipeline", "Value of current leads."),
                        ("Churn Rate", "Expected customer attrition."),
                        ("Market Share", "Target market penetration."),
                        ("Inflation Adj", "Impact of inflation on costs."),
                        ("Tax Liability", "Estimated tax payments."),
                        ("Inventory Level", "Stock depletion forecast."),
                        ("Interest Rate", "Impact of rate changes."),
                        ("Currency Exch", "Forex risk assessment."),
                        ("Product Launch", "Revenue from new product."),
                        ("Upgrade Rate", "Expected plan upgrades."),
                        ("Support Vol", "Projected ticket volume."),
                        ("Server Load", "Expected infrastructure scaling."),
                        ("Energy Cost", "Utility price forecast."),
                        ("Dividend Est", "Projected shareholder payout.")
                    );
                    break;

                // --- HR Classes ---
                case "Employee":
                    (summary, descriptionPrefix) = Get(
                        ("John Smith", "Senior Backend Developer."),
                        ("Sarah Johnson", "HR Manager."),
                        ("Michael Brown", "DevOps Engineer."),
                        ("Emily Davis", "UI/UX Designer."),
                        ("David Wilson", "Sales Director."),
                        ("Jessica Garcia", "Support Specialist."),
                        ("Daniel Martinez", "QA Lead."),
                        ("Lisa Robinson", "CFO."),
                        ("James Clark", "Sysadmin."),
                        ("Maria Rodriguez", "Product Owner."),
                        ("Robert Lewis", "Legal Counsel."),
                        ("Patricia Lee", "Marketing Manager."),
                        ("Joseph Walker", "Frontend Dev."),
                        ("Karen Hall", "Cloud Architect."),
                        ("Thomas Allen", "CISO."),
                        ("Nancy Young", "Benefits Coord."),
                        ("Charles King", "Account Exec."),
                        ("Margaret Wright", "Graphic Designer."),
                        ("Christopher Scott", "Database Admin."),
                        ("Betty Green", "Customer Success.")
                    );
                    break;
                case "Onboarding":
                    (summary, descriptionPrefix) = Get(
                        ("J. Doe Setup", "Hardware provisioning for new hire."),
                        ("Access Grant", "Creating AD and email accounts."),
                        ("Kit Prep", "Preparing welcome swag bag."),
                        ("Orientation", "Scheduling first day intro."),
                        ("Badge Issue", "Printing security badge."),
                        ("Training Assign", "Assigning mandatory videos."),
                        ("Mentor Intro", "Pairing with team mentor."),
                        ("Payroll Setup", "Collecting tax forms."),
                        ("Benefits Enroll", "Signing up for health insurance."),
                        ("Desk Setup", "Allocating seating."),
                        ("Slack Invite", "Adding to communication channels."),
                        ("Github Access", "Granting repo permissions."),
                        ("VPN Config", "Setting up remote access."),
                        ("Intro Meeting", "Team lunch coordination."),
                        ("Hardware Ship", "Mailing laptop to remote user."),
                        ("Policy Sign", "Collecting signatures."),
                        ("Photo Day", "Taking profile picture."),
                        ("Bio Writeup", "Writing intro for newsletter."),
                        ("30 Day Plan", "Setting initial goals."),
                        ("Feedback Sess", "First week check-in.")
                    );
                    break;
                case "Position":
                    (summary, descriptionPrefix) = Get(
                        ("Senior Dev", "Lead software engineer."),
                        ("Junior Admin", "Entry level systems administrator."),
                        ("VP Sales", "Vice President of Sales."),
                        ("CTO", "Chief Technology Officer."),
                        ("Intern", "Summer engineering intern."),
                        ("Office Mgr", "Office manager and receptionist."),
                        ("Data Scientist", "ML and analytics specialist."),
                        ("Product Mgr", "Strategic product lead."),
                        ("Scrum Master", "Agile process facilitator."),
                        ("QA Engineer", "Automated testing specialist."),
                        ("UX Research", "User experience researcher."),
                        ("Content Writer", "Marketing copywriter."),
                        ("Accountant", "Financial bookkeeper."),
                        ("HR Generalist", "Employee relations specialist."),
                        ("Sec Analyst", "SOC analyst."),
                        ("Network Eng", "Network infrastructure engineer."),
                        ("Solution Arch", "Pre-sales technical expert."),
                        ("Support Lead", "Team lead for helpdesk."),
                        ("Recruiter", "Talent acquisition specialist."),
                        ("Legal Asst", "Assistant to general counsel.")
                    );
                    break;
                case "Absence":
                    (summary, descriptionPrefix) = Get(
                        ("Sick Leave", "Unplanned medical absence."),
                        ("Vacation Sum", "Summer holiday booking."),
                        ("Vacation Win", "Winter holiday booking."),
                        ("Paternity", "Parental leave."),
                        ("Maternity", "Maternity leave."),
                        ("Bereavement", "Compassionate leave."),
                        ("Jury Duty", "Civic service."),
                        ("Unpaid", "Sabbatical or personal time."),
                        ("Remote Day", "Working from home (logged)."),
                        ("Half Day", "Leaving early for appointment."),
                        ("Doctor Appt", "Medical checkup."),
                        ("Conference", "Attending industry event."),
                        ("Training Day", "External workshop."),
                        ("Bank Holiday", "Public holiday."),
                        ("Voting", "Time off to vote."),
                        ("Moving Day", "Relocation day."),
                        ("Study Leave", "Exam preparation."),
                        ("Comp Time", "Time off in lieu."),
                        ("Stress Leave", "Medical recovery."),
                        ("Quarantine", "Health isolation.")
                    );
                    break;
                case "Training":
                    (summary, descriptionPrefix) = Get(
                        ("Security 101", "Basic cyber hygiene."),
                        ("Phishing Sim", "Identifying fake emails."),
                        ("C# Advanced", "Deep dive into .NET."),
                        ("React Hooks", "Modern frontend patterns."),
                        ("Diversity", "Inclusivity in the workplace."),
                        ("Leadership", "Management skills for leads."),
                        ("Sales Tech", "Negotiation strategies."),
                        ("Agile Basics", "Intro to Scrum/Kanban."),
                        ("GDPR Rules", "Data privacy handling."),
                        ("First Aid", "Emergency response."),
                        ("Fire Safety", "Evacuation procedures."),
                        ("Cloud Cert", "AWS/Azure certification prep."),
                        ("SQL Tuning", "Database optimization."),
                        ("Design Thinking", "Creative problem solving."),
                        ("Public Speaking", "Presentation skills."),
                        ("Time Mgmt", "Productivity hacks."),
                        ("Conflict Res", "Handling difficult conversations."),
                        ("Excel Master", "Advanced spreadsheet formulas."),
                        ("Git Workflow", "Version control best practices."),
                        ("Onboarding", "New hire orientation.")
                    );
                    break;
                case "OrganizationUnit":
                    (summary, descriptionPrefix) = Get(
                        ("Engineering", "Software and hardware R&D."),
                        ("Sales", "Global sales force."),
                        ("Marketing", "Brand and growth."),
                        ("Finance", "Accounting and control."),
                        ("HR", "People operations."),
                        ("Legal", "Compliance and contracts."),
                        ("Support", "Customer service."),
                        ("Operations", "Facilities and logistics."),
                        ("Executive", "C-Suite and assistants."),
                        ("IT", "Internal tech support."),
                        ("Product", "Product management."),
                        ("Design", "Creative studio."),
                        ("Data", "Business intelligence."),
                        ("Security", "InfoSec team."),
                        ("Procurement", "Purchasing department."),
                        ("Logistics", "Shipping and receiving."),
                        ("Research", "Advanced prototyping."),
                        ("QA", "Quality assurance."),
                        ("Remote", "Distributed workforce."),
                        ("Consultants", "External contractors.")
                    );
                    break;

                // --- PM Classes ---
                case "Feature":
                    (summary, descriptionPrefix) = Get(
                        ("Dark Mode", "System-wide dark theme."),
                        ("2FA Support", "Two-factor authentication."),
                        ("PDF Export", "Report generation."),
                        ("Avatar Upload", "Profile picture support."),
                        ("Autocomplete", "Predictive search."),
                        ("Rate Limit", "API protection."),
                        ("Social Login", "Google/GitHub auth."),
                        ("Audit Log", "Activity tracking."),
                        ("Mobile View", "Responsive layout."),
                        ("Bulk Edit", "Multi-row actions."),
                        ("Email Digest", "Weekly summaries."),
                        ("Widgets", "Dashboard customization."),
                        ("Localization", "German language support."),
                        ("Perf Boost", "Faster page loads."),
                        ("Offline Mode", "App caching."),
                        ("Pay History", "Invoice list."),
                        ("Webhooks", "Event callbacks."),
                        ("Drag Drop", "File upload UI."),
                        ("Accessibility", "WCAG compliance."),
                        ("Chatbot", "AI support assistant.")
                    );
                    break;
                case "ReleasePlan":
                    (summary, descriptionPrefix) = Get(
                        ("Q1 Launch", "First quarter deliverables."),
                        ("Mobile Beta", "Testflight release schedule."),
                        ("Enterprise Ed", "Features for big clients."),
                        ("SaaS v2", "Cloud platform update."),
                        ("On-Prem Dep", "Removing local install option."),
                        ("API V3", "New REST API rollout."),
                        ("Security Wk", "Focus on hardening."),
                        ("UI Refresh", "Design system update."),
                        ("Perf Sprint", "Optimization timeline."),
                        ("Data Mig", "Database transition plan."),
                        ("Holiday Sale", "E-commerce features."),
                        ("GDPR Patch", "Compliance updates."),
                        ("Integration", "Partner API connectors."),
                        ("Reporting", "New analytics engine."),
                        ("User Roles", "RBAC implementation."),
                        ("Notifications", "Alert system overhaul."),
                        ("Search Upg", "Elasticsearch integration."),
                        ("Payment", "New gateway support."),
                        ("Cleanup", "Tech debt removal."),
                        ("Sunset", "End of life planning.")
                    );
                    break;
                case "Roadmap":
                    (summary, descriptionPrefix) = Get(
                        ("2025 Vision", "Long term strategic goals."),
                        ("Q1 Goals", "Immediate priorities."),
                        ("Q2 Goals", "Spring objectives."),
                        ("Q3 Goals", "Summer objectives."),
                        ("Q4 Goals", "End of year push."),
                        ("Mobile First", "Shift to mobile strategy."),
                        ("AI Integr", "Machine learning adoption."),
                        ("Global Exp", "Entering Asian market."),
                        ("Cloud Native", "Full serverless shift."),
                        ("Ent Sales", "Focus on B2B features."),
                        ("Community", "Open source engagement."),
                        ("Partnership", "Strategic alliances."),
                        ("Acquisition", "Tech integration."),
                        ("Sustainability", "Green IT initiatives."),
                        ("Security", "Zero trust architecture."),
                        ("UX Excellence", "Design award targeting."),
                        ("Dev Exp", "Internal tooling improvements."),
                        ("Perf Scale", "Handling 10x traffic."),
                        ("Data Lake", "Big data infrastructure."),
                        ("Innovation", "R&D lab projects.")
                    );
                    break;
                case "Requirement":
                    (summary, descriptionPrefix) = Get(
                        ("Load < 200ms", "Performance requirement."),
                        ("Mobile Friend", "Responsive design constraint."),
                        ("IE11 Support", "Legacy browser constraint."),
                        ("99.9% Uptime", "Availability SLA."),
                        ("AES-256", "Encryption standard."),
                        ("SSO SAML", "Enterprise auth req."),
                        ("Export CSV", "Data portability."),
                        ("Access Log", "Security auditing."),
                        ("Backup Daily", "Disaster recovery."),
                        ("Role Access", "Granular permissions."),
                        ("Color Blind", "Accessibility contrast."),
                        ("Touch UI", "Tablet support."),
                        ("Offline Cap", "PWA capabilities."),
                        ("API Doc", "OpenAPI spec."),
                        ("Unit Tests", "Code coverage > 80%."),
                        ("Dockerized", "Container deployment."),
                        ("No SQL Inj", "Security validation."),
                        ("Scalability", "Horizontal scaling."),
                        ("Cost Cap", "Infrastructure budget."),
                        ("Legal Sign", "Terms of service.")
                    );
                    break;
                case "Stakeholder":
                    (summary, descriptionPrefix) = Get(
                        ("CTO", "Chief Technology Officer."),
                        ("Users", "End users of the platform."),
                        ("Investors", "Venture capital board."),
                        ("Support Team", "Helpdesk staff."),
                        ("Sales Team", "Revenue generators."),
                        ("Marketing", "Brand managers."),
                        ("Legal", "Compliance officers."),
                        ("Partners", "Integration partners."),
                        ("Dev Team", "Engineering squad."),
                        ("Ops Team", "Infrastructure maintainers."),
                        ("Product", "Feature owners."),
                        ("Design", "UX guardians."),
                        ("Security", "CISO office."),
                        ("Finance", "Budget approvers."),
                        ("HR", "Employee advocates."),
                        ("Beta Testers", "Early access group."),
                        ("Regulators", "Government bodies."),
                        ("Competitors", "Market tracking."),
                        ("Suppliers", "Vendors."),
                        ("Community", "Forum users.")
                    );
                    break;
                case "UseCase":
                    (summary, descriptionPrefix) = Get(
                        ("Login Flow", "User authenticates."),
                        ("Checkout", "User buys item."),
                        ("Forgot Pass", "User resets credential."),
                        ("Search Item", "User finds product."),
                        ("Edit Profile", "User changes details."),
                        ("Upload File", "User attaches doc."),
                        ("Delete Acct", "User leaves platform."),
                        ("View Report", "User sees stats."),
                        ("Export Data", "User downloads CSV."),
                        ("Change Role", "Admin promotes user."),
                        ("Ban User", "Admin blocks access."),
                        ("Approve PO", "Manager signs off."),
                        ("Reject Inv", "Finance denies bill."),
                        ("Book Leave", "Employee requests time."),
                        ("Review Code", "Dev checks PR."),
                        ("Deploy App", "Ops releases build."),
                        ("Scan Badge", "Staff enters building."),
                        ("Connect VPN", "Remote work start."),
                        ("Print Doc", "Sending to printer."),
                        ("Chat Supp", "Talking to bot.")
                    );
                    break;

                // --- PROC Classes ---
                case "PurchaseOrder":
                    (summary, descriptionPrefix) = Get(
                        ("PO-2025-01", "New laptops."),
                        ("PO-2025-02", "Office chairs."),
                        ("PO-2025-03", "Server rack."),
                        ("PO-2025-04", "Software licenses."),
                        ("PO-2025-05", "Cleaning supplies."),
                        ("PO-2025-06", "Coffee beans."),
                        ("PO-2025-07", "Marketing swag."),
                        ("PO-2025-08", "Printer paper."),
                        ("PO-2025-09", "Monitors."),
                        ("PO-2025-10", "Keyboards."),
                        ("PO-2025-11", "Network cables."),
                        ("PO-2025-12", "Conference table."),
                        ("PO-2025-13", "Projector."),
                        ("PO-2025-14", "Whiteboards."),
                        ("PO-2025-15", "Desks."),
                        ("PO-2025-16", "Phones."),
                        ("PO-2025-17", "Headsets."),
                        ("PO-2025-18", "Webcams."),
                        ("PO-2025-19", "SSD drives."),
                        ("PO-2025-20", "RAM upgrades.")
                    );
                    break;
                case "Supplier":
                    (summary, descriptionPrefix) = Get(
                        ("Microsoft", "Software vendor."),
                        ("Dell", "Hardware vendor."),
                        ("AWS", "Cloud provider."),
                        ("Cleaning Co", "Facility services."),
                        ("Catering Inc", "Food services."),
                        ("Staples", "Office supplies."),
                        ("ISP Corp", "Internet provider."),
                        ("Lease Mgmt", "Real estate."),
                        ("Legal Firm", "External counsel."),
                        ("Recruiters", "Hiring agency."),
                        ("Sec Guard", "Security personnel."),
                        ("Courier X", "Shipping."),
                        ("Travel Agcy", "Flight booking."),
                        ("Event Org", "Party planner."),
                        ("Print Shop", "Business cards."),
                        ("Furnishings", "Office furniture."),
                        ("Water Co", "Beverage delivery."),
                        ("Power Utility", "Electricity."),
                        ("Telecom", "Phone lines."),
                        ("Bank", "Financial services.")
                    );
                    break;
                case "Tender":
                    (summary, descriptionPrefix) = Get(
                        ("Office Reno", "Bid for renovation."),
                        ("Server Refresh", "Bid for new hardware."),
                        ("Cleaning Svc", "Bid for janitorial."),
                        ("Security Sys", "Bid for cameras."),
                        ("Catering", "Bid for canteen."),
                        ("Transport", "Bid for logistics."),
                        ("Software Dev", "Bid for outsourcing."),
                        ("Marketing", "Bid for agency."),
                        ("Audit", "Bid for financial audit."),
                        ("Legal", "Bid for retainer."),
                        ("Internet", "Bid for fiber."),
                        ("Mobile", "Bid for fleet."),
                        ("Furniture", "Bid for desks."),
                        ("Printer", "Bid for lease."),
                        ("Cloud", "Bid for hosting."),
                        ("Training", "Bid for courses."),
                        ("Recruiting", "Bid for headhunters."),
                        ("Waste Mgmt", "Bid for disposal."),
                        ("Energy", "Bid for green power."),
                        ("Insurance", "Bid for liability.")
                    );
                    break;
                case "Delivery":
                    (summary, descriptionPrefix) = Get(
                        ("Truck 1", "Hardware arrival."),
                        ("Courier A", "Documents."),
                        ("FedEx 123", "Swag delivery."),
                        ("UPS 456", "Monitor shipment."),
                        ("DHL 789", "Server parts."),
                        ("Food Van", "Lunch catering."),
                        ("Water Truck", "Cooler refill."),
                        ("Mail Bag", "Daily letters."),
                        ("Furniture", "Desk assembly."),
                        ("Supplies", "Paper refill."),
                        ("Return", "RMA pickup."),
                        ("Sample", "Vendor demo."),
                        ("Gift", "Client present."),
                        ("Laptop", "Remote worker kit."),
                        ("Phone", "Mobile replacement."),
                        ("Sim Card", "Data plan."),
                        ("Badge", "ID card batch."),
                        ("Key", "Office keys."),
                        ("Token", "Security fobs."),
                        ("Cable", "Patch cords.")
                    );
                    break;
                case "Invoice": // Covers FIN and PROC
                    (summary, descriptionPrefix) = Get(
                        ("AWS Bill", "Cloud hosting."),
                        ("Cleaning", "Janitorial."),
                        ("Dell", "Laptops."),
                        ("Rent", "Office lease."),
                        ("Consulting", "Audit fee."),
                        ("Ad Spend", "Google Ads."),
                        ("Travel", "Flights."),
                        ("Internet", "Fiber line."),
                        ("Coffee", "Kitchen."),
                        ("Legal", "Retainer."),
                        ("Server", "Hardware."),
                        ("Stationery", "Pens."),
                        ("Training", "Workshop."),
                        ("SaaS", "Subscription."),
                        ("Furniture", "Chairs."),
                        ("Power", "Electricity."),
                        ("Water", "Bottles."),
                        ("Shipping", "Courier."),
                        ("Phone", "Mobile bill."),
                        ("Event", "Party cost.")
                    );
                    break;
                case "Contract": // Covers FIN and PROC
                    (summary, descriptionPrefix) = Get(
                        ("Lease", "Office rent."),
                        ("NDA", "Non-disclosure."),
                        ("SLA", "Service level."),
                        ("Employment", "Job offer."),
                        ("Vendor", "Supply terms."),
                        ("License", "Software EULA."),
                        ("Maintenance", "Support deal."),
                        ("Insurance", "Liability."),
                        ("Partnership", "Joint venture."),
                        ("Loan", "Bank credit."),
                        ("Reseller", "Distributor."),
                        ("Consulting", "SOW."),
                        ("Agency", "Marketing."),
                        ("Privacy", "DPA."),
                        ("Cleaning", "Service."),
                        ("Security", "Guarding."),
                        ("Internet", "ISP term."),
                        ("Mobile", "Fleet plan."),
                        ("Catering", "Food svc."),
                        ("Waste", "Disposal.")
                    );
                    break;
                case "Approval": // Covers CMDB and FIN
                    (summary, descriptionPrefix) = Get(
                        ("Budget Appr", "Manager signoff."),
                        ("Deploy Appr", "Lead signoff."),
                        ("Hire Appr", "VP signoff."),
                        ("Vacation", "HR signoff."),
                        ("Expense", "Finance signoff."),
                        ("Access", "Security signoff."),
                        ("Contract", "Legal signoff."),
                        ("Design", "Product signoff."),
                        ("Content", "Marketing signoff."),
                        ("Hardware", "IT signoff."),
                        ("Software", "Arch signoff."),
                        ("Policy", "Board signoff."),
                        ("Audit", "Compliance signoff."),
                        ("Risk", "CISO signoff."),
                        ("Change", "CAB signoff."),
                        ("Release", "QA signoff."),
                        ("Invoice", "AP signoff."),
                        ("Quote", "Sales signoff."),
                        ("Partner", "CEO signoff."),
                        ("Strategy", "Board signoff.")
                    );
                    break;

                // --- SD Classes ---
                case "Ticket":
                case "Incident":
                case "Problem":
                case "ServiceRequest":
                case "Knowledge": // Grouping SD generic content
                    (summary, descriptionPrefix) = Get(
                        ("VPN Timeout", "Connection dropped."),
                        ("Printer Jam", "Hallway printer."),
                        ("Outlook Sync", "Email error."),
                        ("404 Login", "Page not found."),
                        ("Slow DB", "Query timeout."),
                        ("Locked Out", "Password reset."),
                        ("Flickering", "Screen issue."),
                        ("Install VS", "Software request."),
                        ("Wifi Weak", "Signal drop."),
                        ("Keyboard", "Broken key."),
                        ("Access Denied", "Folder perm."),
                        ("BSOD", "System crash."),
                        ("Mobile Email", "Phone setup."),
                        ("Zoom Crash", "App freeze."),
                        ("Firewall", "Site blocked."),
                        ("Restore", "File recovery."),
                        ("Mouse", "Bluetooth fail."),
                        ("Resolution", "Screen size."),
                        ("Macro Err", "Excel crash."),
                        ("New User", "Account create.")
                    );
                    break;

                default:
                    // default fallback
                    summary = $"{className} Item {index + 1}";
                    descriptionPrefix = $"Generic description for {className}.";
                    break;
            }

            return (summary, $"{descriptionPrefix} {lorem}");
        }
    }
}
