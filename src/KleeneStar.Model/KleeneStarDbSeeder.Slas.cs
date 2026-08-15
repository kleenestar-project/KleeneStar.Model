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
        // Identities used as fallback owners.
        private static readonly Guid SeedAdminIdentityId = Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD");
        private static readonly Guid SeedAliceIdentityId = Guid.Parse("BBF45E5D-AA35-4382-9B84-6055193CE544");
        private static readonly Guid SeedSupportIdentityId = Guid.Parse("D1C5AED2-78D3-45F7-BB19-E87B8F134301");

        // Calendar names available in the seeded catalogue. Used by the SLA seeder to
        // resolve the per-class calendar by name without hard-coding GUIDs.
        private const string StandardCalendarName = "Standard · Europe/Berlin";
        private const string AlwaysOnCalendarName = "24 / 7 · Always on";
        private const string NightShiftCalendarName = "Night shift · 22-06";

        /// <summary>
        /// Adds a class-specific catalogue of SLA policies to the database, including targets,
        /// scope rules, and escalation levels. Service-desk style classes get a rich catalogue
        /// (Incident P1-P3, VIP, Problem, Change, Request, plus a draft and an inactive legacy
        /// policy); all other classes receive a single sensible default so the SLA management
        /// page is never empty. Each policy is linked to the class's matching seeded calendar.
        /// </summary>
        /// <param name="db">The database context to which the policies will be added.</param>
        private static void SeedSlas(KleeneStarDbContext db)
        {
            var classes = db.Classes
                .Include(c => c.Workspace)
                .AsNoTracking()
                .ToList();

            // Pre-load all seeded calendars grouped by class so we can resolve them by
            // (classId, name) without firing a query per policy.
            var calendarsByClassAndName = db.Calendars
                .AsNoTracking()
                .ToDictionary(c => (c.ClassId, c.Name), c => c.Id);

            Guid? ResolveCalendar(Guid classId, string name)
            {
                return calendarsByClassAndName.TryGetValue((classId, name), out var id) ? id : null;
            }

            foreach (var cls in classes)
            {
                var templates = GetSlaTemplatesForClass(cls.Name);

                foreach (var template in templates)
                {
                    var policy = new SlaPolicy
                    {
                        Id = Guid.NewGuid(),
                        Name = template.Name,
                        Description = template.Description,
                        State = template.State,
                        Priority = template.Priority,
                        CalendarId = ResolveCalendar(cls.Id, template.CalendarName),
                        Notifications = template.Notifications,
                        PauseOn = template.PauseOn,
                        Icon = ImageIcon.FromString("/kleenestar/assets/icons/sla.svg"),
                        ClassId = cls.Id,
                        OwnerId = template.OwnerId,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    };

                    foreach (var t in template.Targets)
                    {
                        policy.Targets.Add(new SlaTarget
                        {
                            Id = Guid.NewGuid(),
                            Name = t.Name,
                            Kind = t.Kind,
                            TargetValue = t.Value,
                            Unit = t.Unit,
                            Created = DateTime.UtcNow,
                            Updated = DateTime.UtcNow
                        });
                    }

                    foreach (var s in template.Scope)
                    {
                        policy.Scope.Add(new SlaScopeRule
                        {
                            Id = Guid.NewGuid(),
                            RuleType = s.Type,
                            Value = s.Value
                        });
                    }

                    var level = 1;
                    foreach (var e in template.Escalations)
                    {
                        policy.Escalations.Add(new SlaEscalationLevel
                        {
                            Id = Guid.NewGuid(),
                            Level = level++,
                            AfterValue = e.AfterValue,
                            Unit = e.Unit,
                            Notify = e.Notify
                        });
                    }

                    db.SlaPolicies.Add(policy);
                }
            }
        }

        private sealed record SlaTemplate
        (
            Guid Id,
            string Name,
            string Description,
            SlaPolicyState State,
            SlaPriority Priority,
            string CalendarName,
            SlaNotificationChannels Notifications,
            string PauseOn,
            Guid? OwnerId,
            IReadOnlyList<SlaTargetTemplate> Targets,
            IReadOnlyList<SlaScopeTemplate> Scope,
            IReadOnlyList<SlaEscalationTemplate> Escalations
        );

        private sealed record SlaTargetTemplate
        (
            string Name,
            SlaTargetKind Kind,
            int Value,
            SlaTargetUnit Unit
        );

        private sealed record SlaScopeTemplate(SlaScopeRuleType Type, string Value);

        private sealed record SlaEscalationTemplate(int AfterValue, SlaTargetUnit Unit, string Notify);

        /// <summary>
        /// Returns the SLA template catalogue for a class.
        /// </summary>
        /// <remarks>
        /// Support-desk style classes (Incident, Problem, Change, ServiceRequest, etc.) get a
        /// rich catalogue mirroring the design prototype (P1 Enterprise, P2 Standard, P3 Basic,
        /// VIP, plus drafts and an inactive legacy policy). All other classes receive a single
        /// sensible default so the SLA management page is never empty.
        /// </remarks>
        private static IReadOnlyList<SlaTemplate> GetSlaTemplatesForClass(string className)
        {
            switch (className)
            {
                case "Incident":
                    return
                    [
                        IncidentP1Enterprise(),
                        IncidentP2Standard(),
                        IncidentP3Basic(),
                        IncidentVipPremium(),
                        BatchJobDraft(),
                        LegacyInactive(),
                    ];

                case "Problem":
                    return [ProblemRootCause()];

                case "Change":
                    return [ChangeStandard(), ChangeEmergency()];

                case "ServiceRequest":
                    return [ServiceRequestOnboarding(), ServiceRequestStandard()];

                case "Ticket":
                    return [TicketDefault()];

                case "Knowledge":
                    return [KnowledgeReview()];

                case "Bug":
                    return [BugTriage()];

                case "Feature":
                    return [FeatureDelivery()];

                case "Invoice":
                    return [InvoiceApproval()];

                case "Approval":
                    return [ApprovalStandard()];

                case "PurchaseOrder":
                    return [PurchaseOrderProcessing()];

                case "Onboarding":
                    return [OnboardingFulfillment()];

                default:
                    return [DefaultPolicy(className)];
            }
        }

        // ───────── support desk ─────────

        private static SlaTemplate IncidentP1Enterprise() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A001-000000000001"),
            Name: "Incident · Priority 1 · Enterprise",
            Description: "24/7 coverage for business-critical incidents on enterprise contracts.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Critical,
            CalendarName: AlwaysOnCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for customer, Scheduled maintenance",
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("First response", SlaTargetKind.Response,   30, SlaTargetUnit.Minutes),
                new SlaTargetTemplate("Resolution",     SlaTargetKind.Resolution,  4, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Status update",  SlaTargetKind.Update,      2, SlaTargetUnit.Hours),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Priority, "High"),
                new SlaScopeTemplate(SlaScopeRuleType.Contract, "Enterprise"),
                new SlaScopeTemplate(SlaScopeRuleType.System,   "Production"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(15, SlaTargetUnit.Minutes, "Team Lead Network"),
                new SlaEscalationTemplate(45, SlaTargetUnit.Minutes, "Head of IT Operations"),
                new SlaEscalationTemplate(90, SlaTargetUnit.Minutes, "CIO, On-Call Manager"),
            ]
        );

        private static SlaTemplate IncidentP2Standard() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A001-000000000002"),
            Name: "Incident · Priority 2 · Standard",
            Description: "Business-hours SLA for standard contracts.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.High,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for customer",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("First response", SlaTargetKind.Response,    2, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Resolution",     SlaTargetKind.Resolution,  1, SlaTargetUnit.Days),
                new SlaTargetTemplate("Status update",  SlaTargetKind.Update,      4, SlaTargetUnit.Hours),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Priority, "Medium"),
                new SlaScopeTemplate(SlaScopeRuleType.Contract, "Standard"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(4, SlaTargetUnit.Hours, "Service Desk Lead"),
                new SlaEscalationTemplate(1, SlaTargetUnit.Days,  "IT Operations Manager"),
            ]
        );

        private static SlaTemplate IncidentP3Basic() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A001-000000000003"),
            Name: "Incident · Priority 3 · Basic",
            Description: "Best-effort SLA for low-priority tickets.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Low,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for customer, Waiting for 3rd-party",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("First response", SlaTargetKind.Response,   1, SlaTargetUnit.Days),
                new SlaTargetTemplate("Resolution",     SlaTargetKind.Resolution, 5, SlaTargetUnit.Days),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Priority, "Low"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(3, SlaTargetUnit.Days, "Service Desk Lead"),
            ]
        );

        private static SlaTemplate IncidentVipPremium() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A001-000000000004"),
            Name: "Incident · VIP user · Premium",
            Description: "Tightened response targets for VIP users (C-level, board).",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Critical,
            CalendarName: AlwaysOnCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: null,
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("First response", SlaTargetKind.Response,    5, SlaTargetUnit.Minutes),
                new SlaTargetTemplate("Resolution",     SlaTargetKind.Resolution,  1, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Status update",  SlaTargetKind.Update,     30, SlaTargetUnit.Minutes),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Tag, "VIP-User"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate( 3, SlaTargetUnit.Minutes, "VIP Support Lead"),
                new SlaEscalationTemplate(15, SlaTargetUnit.Minutes, "Head of IT Operations"),
                new SlaEscalationTemplate(30, SlaTargetUnit.Minutes, "CIO"),
            ]
        );

        private static SlaTemplate BatchJobDraft() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A001-000000000005"),
            Name: "Batch job · Nightly recovery",
            Description: "Draft policy for automated nightly recovery (pilot).",
            State: SlaPolicyState.Draft,
            Priority: SlaPriority.Medium,
            CalendarName: NightShiftCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: null,
            OwnerId: SeedAliceIdentityId,
            Targets:
            [
                new SlaTargetTemplate("First response", SlaTargetKind.Response,  10, SlaTargetUnit.Minutes),
                new SlaTargetTemplate("Recovery",       SlaTargetKind.Resolution, 2, SlaTargetUnit.Hours),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Category, "Batch job failure"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(15, SlaTargetUnit.Minutes, "Batch Operations"),
            ]
        );

        private static SlaTemplate LegacyInactive() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A001-000000000006"),
            Name: "Legacy · Old service desk tickets",
            Description: "Legacy SLA for tickets migrated from the ServiceNow installation. No longer in use.",
            State: SlaPolicyState.Inactive,
            Priority: SlaPriority.Low,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.None,
            PauseOn: null,
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("First response", SlaTargetKind.Response,   8, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Resolution",     SlaTargetKind.Resolution, 5, SlaTargetUnit.Days),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Source, "ServiceNow migration"),
            ],
            Escalations: []
        );

        private static SlaTemplate ProblemRootCause() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A002-000000000001"),
            Name: "Problem · Root-cause analysis",
            Description: "Mid-term SLA for root-cause analysis following critical incidents.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.High,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for 3rd-party, Waiting for vendor",
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Response", SlaTargetKind.Response,    1, SlaTargetUnit.Days),
                new SlaTargetTemplate("RCA done", SlaTargetKind.Resolution, 10, SlaTargetUnit.Days),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Priority, "High"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(5, SlaTargetUnit.Days, "Problem Manager"),
                new SlaEscalationTemplate(8, SlaTargetUnit.Days, "Head of IT Operations"),
            ]
        );

        private static SlaTemplate ChangeStandard() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A003-000000000001"),
            Name: "Change · Standard change",
            Description: "Approval and implementation deadlines for standard changes.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "CAB approval pending",
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("CAB approval",   SlaTargetKind.Approval,       3, SlaTargetUnit.Days),
                new SlaTargetTemplate("Implementation", SlaTargetKind.Implementation, 7, SlaTargetUnit.Days),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Type, "Standard"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(2, SlaTargetUnit.Days, "Change Manager"),
            ]
        );

        private static SlaTemplate ChangeEmergency() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A003-000000000002"),
            Name: "Change · Emergency change",
            Description: "Tightened deadlines for emergency changes with high urgency.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Critical,
            CalendarName: AlwaysOnCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: null,
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("CAB approval",   SlaTargetKind.Approval,       2, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Implementation", SlaTargetKind.Implementation, 8, SlaTargetUnit.Hours),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Type, "Emergency"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(30, SlaTargetUnit.Minutes, "Emergency CAB"),
                new SlaEscalationTemplate( 1, SlaTargetUnit.Hours,   "Head of IT Operations"),
            ]
        );

        private static SlaTemplate ServiceRequestOnboarding() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A004-000000000001"),
            Name: "Request · Hardware onboarding",
            Description: "Service request for new-hire onboarding including laptop, accounts, and permissions.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for HR approval, Waiting for delivery",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Fulfillment", SlaTargetKind.Fulfillment, 5, SlaTargetUnit.BusinessDays),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Catalog, "Hardware onboarding"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(4, SlaTargetUnit.Days, "HR · IT Support Lead"),
            ]
        );

        private static SlaTemplate ServiceRequestStandard() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A004-000000000002"),
            Name: "Request · Standard service",
            Description: "Standard service request fulfilment SLA.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Low,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for customer",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("First response", SlaTargetKind.Response,    4, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Fulfillment",    SlaTargetKind.Fulfillment, 3, SlaTargetUnit.BusinessDays),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Catalog, "Standard"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(2, SlaTargetUnit.Days, "Service Desk Lead"),
            ]
        );

        private static SlaTemplate TicketDefault() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A005-000000000001"),
            Name: "Ticket · Default",
            Description: "Catch-all SLA for unclassified service-desk tickets.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for customer",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("First response", SlaTargetKind.Response,    4, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Resolution",     SlaTargetKind.Resolution,  2, SlaTargetUnit.BusinessDays),
            ],
            Scope: [],
            Escalations:
            [
                new SlaEscalationTemplate(1, SlaTargetUnit.Days, "Service Desk Lead"),
            ]
        );

        private static SlaTemplate KnowledgeReview() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A006-000000000001"),
            Name: "Knowledge · Review cadence",
            Description: "Knowledge articles must be reviewed for accuracy on a recurring cadence.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Low,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: null,
            OwnerId: SeedAliceIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Review", SlaTargetKind.Custom, 90, SlaTargetUnit.Days),
            ],
            Scope: [],
            Escalations:
            [
                new SlaEscalationTemplate(7, SlaTargetUnit.Days, "Knowledge Manager"),
            ]
        );

        // ───────── development ─────────

        private static SlaTemplate BugTriage() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-B001-000000000001"),
            Name: "Bug · Triage & fix",
            Description: "SLA for triaging and fixing bugs.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.High,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for reporter",
            OwnerId: SeedAliceIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Triage",        SlaTargetKind.Response,    1, SlaTargetUnit.BusinessDays),
                new SlaTargetTemplate("Fix available", SlaTargetKind.Resolution, 14, SlaTargetUnit.Days),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Priority, "Blocker"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(2, SlaTargetUnit.Days, "Engineering Lead"),
                new SlaEscalationTemplate(7, SlaTargetUnit.Days, "Head of Engineering"),
            ]
        );

        private static SlaTemplate FeatureDelivery() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-B002-000000000001"),
            Name: "Feature · Delivery commitment",
            Description: "Tracks the committed delivery date for features.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: null,
            OwnerId: SeedAliceIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Review",   SlaTargetKind.Approval,        3, SlaTargetUnit.BusinessDays),
                new SlaTargetTemplate("Delivery", SlaTargetKind.Implementation, 30, SlaTargetUnit.Days),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Priority, "Must Have"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(5, SlaTargetUnit.Days, "Product Owner"),
            ]
        );

        // ───────── finance / procurement ─────────

        private static SlaTemplate InvoiceApproval() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-C001-000000000001"),
            Name: "Invoice · Approval cycle",
            Description: "Approval and payment deadlines for incoming invoices.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for supplier",
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Approval", SlaTargetKind.Approval,    5,  SlaTargetUnit.BusinessDays),
                new SlaTargetTemplate("Payment",  SlaTargetKind.Fulfillment, 30, SlaTargetUnit.Days),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Priority, "Standard"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(7, SlaTargetUnit.BusinessDays, "Finance Lead"),
            ]
        );

        private static SlaTemplate ApprovalStandard() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-C002-000000000001"),
            Name: "Approval · Standard cycle",
            Description: "Standard SLA for approval workflows.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: null,
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Approval", SlaTargetKind.Approval, 3, SlaTargetUnit.BusinessDays),
            ],
            Scope: [],
            Escalations:
            [
                new SlaEscalationTemplate(5, SlaTargetUnit.BusinessDays, "Approver Manager"),
            ]
        );

        private static SlaTemplate PurchaseOrderProcessing() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-C003-000000000001"),
            Name: "PurchaseOrder · Processing",
            Description: "Processing SLA for incoming purchase orders.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for budget approval",
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Order approval", SlaTargetKind.Approval,     2, SlaTargetUnit.BusinessDays),
                new SlaTargetTemplate("Delivery",       SlaTargetKind.Fulfillment, 10, SlaTargetUnit.BusinessDays),
            ],
            Scope: [],
            Escalations:
            [
                new SlaEscalationTemplate(3, SlaTargetUnit.BusinessDays, "Procurement Lead"),
            ]
        );

        // ───────── hr ─────────

        private static SlaTemplate OnboardingFulfillment() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-D001-000000000001"),
            Name: "Onboarding · Day-1 ready",
            Description: "Complete provisioning of new hires by their first working day.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.High,
            CalendarName: StandardCalendarName,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for IT, Waiting for HR",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Contract sent", SlaTargetKind.Response,    5, SlaTargetUnit.BusinessDays),
                new SlaTargetTemplate("Day-1 ready",   SlaTargetKind.Fulfillment, 1, SlaTargetUnit.BusinessDays),
            ],
            Scope: [],
            Escalations:
            [
                new SlaEscalationTemplate(2, SlaTargetUnit.BusinessDays, "HR Lead"),
            ]
        );

        // ───────── generic fallback ─────────

        private static SlaTemplate DefaultPolicy(string className)
        {
            return new SlaTemplate
            (
                Id: Guid.NewGuid(),
                Name: $"{className} · Default SLA",
                Description: $"Standard SLA for class {className}.",
                State: SlaPolicyState.Active,
                Priority: SlaPriority.Medium,
                CalendarName: StandardCalendarName,
                Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
                PauseOn: null,
                OwnerId: SeedAdminIdentityId,
                Targets:
                [
                    new SlaTargetTemplate("First response", SlaTargetKind.Response,   1, SlaTargetUnit.BusinessDays),
                    new SlaTargetTemplate("Resolution",     SlaTargetKind.Resolution, 5, SlaTargetUnit.BusinessDays),
                ],
                Scope: [],
                Escalations:
                [
                    new SlaEscalationTemplate(2, SlaTargetUnit.BusinessDays, "Team Lead"),
                ]
            );
        }
    }
}
