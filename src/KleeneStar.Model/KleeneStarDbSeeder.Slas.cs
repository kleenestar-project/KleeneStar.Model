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
        /// Identity used as fallback owner when no specific identity is available.
        /// </summary>
        private static readonly Guid SeedAdminIdentityId = Guid.Parse("77087646-B13A-44B1-9BAC-6E66443CEDFD");
        private static readonly Guid SeedAliceIdentityId = Guid.Parse("BBF45E5D-AA35-4382-9B84-6055193CE544");
        private static readonly Guid SeedSupportIdentityId = Guid.Parse("D1C5AED2-78D3-45F7-BB19-E87B8F134301");

        /// <summary>
        /// Adds a class-specific catalogue of SLA policies to the database, including targets,
        /// scope rules, and escalation levels. One or more policies are generated for every
        /// existing class, with extra variety (multiple priorities, VIP, draft, inactive)
        /// for support-desk style classes such as Incident, Problem, Change and Request.
        /// </summary>
        /// <param name="db">The database context to which the policies will be added.</param>
        private static void SeedSlas(KleeneStarDbContext db)
        {
            var classes = db.Classes
                .Include(c => c.Workspace)
                .AsNoTracking()
                .ToList();

            foreach (var cls in classes)
            {
                var templates = GetSlaTemplatesForClass(cls.Name, cls.Workspace?.Key);

                foreach (var template in templates)
                {
                    var policy = new SlaPolicy
                    {
                        Id = Guid.NewGuid(),
                        Name = template.Name,
                        Description = template.Description,
                        State = template.State,
                        Priority = template.Priority,
                        Calendar = template.Calendar,
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
            SlaCalendar Calendar,
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
        /// Returns the SLA template catalogue for a class within a workspace.
        /// </summary>
        /// <remarks>
        /// Support-desk style classes (Incident, Problem, Change, ServiceRequest) get a rich
        /// catalogue mirroring the design prototype (P1 Enterprise, P2 Standard, P3 Basic,
        /// VIP, plus drafts and an inactive legacy policy). All other classes receive a
        /// single sensible default so the SLA management page is never empty.
        /// </remarks>
        private static IReadOnlyList<SlaTemplate> GetSlaTemplatesForClass(string className, string workspaceKey)
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
            Description: "24/7-Abdeckung für unternehmenskritische Vorfälle bei Enterprise-Verträgen.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Critical,
            Calendar: SlaCalendar.TwentyFourSeven,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.Slack | SlaNotificationChannels.Sms | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for customer, Scheduled maintenance",
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Erstreaktion",   SlaTargetKind.Response,   30, SlaTargetUnit.Minutes),
                new SlaTargetTemplate("Lösung",         SlaTargetKind.Resolution,  4, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Update-Pflicht", SlaTargetKind.Update,      2, SlaTargetUnit.Hours),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Priority, "High"),
                new SlaScopeTemplate(SlaScopeRuleType.Contract, "Enterprise"),
                new SlaScopeTemplate(SlaScopeRuleType.System,   "Produktiv"),
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
            Description: "Geschäftszeiten-SLA für Standard-Verträge.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.High,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.Slack | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for customer",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Erstreaktion",   SlaTargetKind.Response,    2, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Lösung",         SlaTargetKind.Resolution,  1, SlaTargetUnit.Days),
                new SlaTargetTemplate("Update-Pflicht", SlaTargetKind.Update,      4, SlaTargetUnit.Hours),
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
            Description: "Best-effort-SLA für niedrigpriorisierte Tickets.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Low,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for customer, Waiting for 3rd-party",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Erstreaktion", SlaTargetKind.Response,   1, SlaTargetUnit.Days),
                new SlaTargetTemplate("Lösung",       SlaTargetKind.Resolution, 5, SlaTargetUnit.Days),
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
            Name: "Incident · VIP-User · Premium",
            Description: "Verschärfte Reaktionszeiten für VIP-User (C-Level, Vorstand).",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Critical,
            Calendar: SlaCalendar.TwentyFourSeven,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.Slack | SlaNotificationChannels.Sms | SlaNotificationChannels.InApp,
            PauseOn: null,
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Erstreaktion",   SlaTargetKind.Response,    5, SlaTargetUnit.Minutes),
                new SlaTargetTemplate("Lösung",         SlaTargetKind.Resolution,  1, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Update-Pflicht", SlaTargetKind.Update,     30, SlaTargetUnit.Minutes),
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
            Name: "Batch-Job · Nightly-Recovery",
            Description: "Entwurf für automatisierte Recovery-SLAs (Pilotphase).",
            State: SlaPolicyState.Draft,
            Priority: SlaPriority.Medium,
            Calendar: SlaCalendar.NightShift,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.Slack | SlaNotificationChannels.InApp,
            PauseOn: null,
            OwnerId: SeedAliceIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Erstreaktion", SlaTargetKind.Response,  10, SlaTargetUnit.Minutes),
                new SlaTargetTemplate("Recovery",     SlaTargetKind.Resolution, 2, SlaTargetUnit.Hours),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Category, "Batch-Job-Fehler"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(15, SlaTargetUnit.Minutes, "Batch Operations"),
            ]
        );

        private static SlaTemplate LegacyInactive() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A001-000000000006"),
            Name: "Legacy · Old Service Desk Tickets",
            Description: "Alte SLA für migrierte Tickets aus dem ServiceNow-Bestand. Nicht mehr verwendet.",
            State: SlaPolicyState.Inactive,
            Priority: SlaPriority.Low,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.None,
            PauseOn: null,
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Erstreaktion", SlaTargetKind.Response,   8, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Lösung",       SlaTargetKind.Resolution, 5, SlaTargetUnit.Days),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Source, "ServiceNow-Migration"),
            ],
            Escalations: []
        );

        private static SlaTemplate ProblemRootCause() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A002-000000000001"),
            Name: "Problem · Root-Cause-Analyse",
            Description: "Mittelfristige SLA für Root-Cause-Analyse nach kritischen Incidents.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.High,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.Slack | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for 3rd-party, Waiting for vendor",
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Reaktion",   SlaTargetKind.Response,    1, SlaTargetUnit.Days),
                new SlaTargetTemplate("RCA fertig", SlaTargetKind.Resolution, 10, SlaTargetUnit.Days),
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
            Name: "Change · Standard-Change",
            Description: "Approval- und Implementierungs-Fristen für Standard-Changes.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "CAB approval pending",
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("CAB-Approval",    SlaTargetKind.Approval,       3, SlaTargetUnit.Days),
                new SlaTargetTemplate("Implementierung", SlaTargetKind.Implementation, 7, SlaTargetUnit.Days),
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
            Name: "Change · Emergency-Change",
            Description: "Verkürzte Fristen für Emergency-Changes mit hoher Dringlichkeit.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Critical,
            Calendar: SlaCalendar.TwentyFourSeven,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.Slack | SlaNotificationChannels.Sms | SlaNotificationChannels.InApp,
            PauseOn: null,
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("CAB-Approval",    SlaTargetKind.Approval,       2, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Implementierung", SlaTargetKind.Implementation, 8, SlaTargetUnit.Hours),
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
            Name: "Request · Onboarding-Hardware",
            Description: "Service-Request für neues Onboarding inkl. Laptop, Konten, Berechtigungen.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for HR approval, Waiting for delivery",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Fulfillment", SlaTargetKind.Fulfillment, 5, SlaTargetUnit.BusinessDays),
            ],
            Scope:
            [
                new SlaScopeTemplate(SlaScopeRuleType.Catalog, "Hardware-Onboarding"),
            ],
            Escalations:
            [
                new SlaEscalationTemplate(4, SlaTargetUnit.Days, "HR · IT Support Lead"),
            ]
        );

        private static SlaTemplate ServiceRequestStandard() => new
        (
            Id: Guid.Parse("A1B2C3D4-E5F6-4711-A004-000000000002"),
            Name: "Request · Standard-Service",
            Description: "Standard service request fulfilment SLA.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Low,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for customer",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Erstreaktion", SlaTargetKind.Response,    4, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Fulfillment",  SlaTargetKind.Fulfillment, 3, SlaTargetUnit.BusinessDays),
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
            Description: "Allgemeine SLA für unklassifizierte Service-Desk-Tickets.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for customer",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Erstreaktion", SlaTargetKind.Response,    4, SlaTargetUnit.Hours),
                new SlaTargetTemplate("Lösung",       SlaTargetKind.Resolution,  2, SlaTargetUnit.BusinessDays),
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
            Name: "Knowledge · Review-Pflicht",
            Description: "Knowledge-Artikel werden regelmäßig auf Aktualität geprüft.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Low,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: null,
            OwnerId: SeedAliceIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Review",  SlaTargetKind.Custom,     90, SlaTargetUnit.Days),
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
            Name: "Bug · Triage & Fix",
            Description: "SLA für die Triage und Behebung von Bugs.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.High,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.Slack | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for reporter",
            OwnerId: SeedAliceIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Triage",         SlaTargetKind.Response,    1, SlaTargetUnit.BusinessDays),
                new SlaTargetTemplate("Fix verfügbar",  SlaTargetKind.Resolution, 14, SlaTargetUnit.Days),
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
            Name: "Feature · Delivery-Commitment",
            Description: "Tracking der zugesagten Feature-Liefertermine.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            Calendar: SlaCalendar.BusinessHours,
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
            Name: "Invoice · Approval-Cycle",
            Description: "Genehmigungs- und Zahlungsfristen für eingehende Rechnungen.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for supplier",
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Approval", SlaTargetKind.Approval,    5,  SlaTargetUnit.BusinessDays),
                new SlaTargetTemplate("Zahlung",  SlaTargetKind.Fulfillment, 30, SlaTargetUnit.Days),
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
            Name: "Approval · Standard-Cycle",
            Description: "Standard-SLA für Approval-Workflows.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            Calendar: SlaCalendar.BusinessHours,
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
            Description: "Bearbeitungs-SLA für eingehende Bestellungen.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.Medium,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for budget approval",
            OwnerId: SeedAdminIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Order Approval", SlaTargetKind.Approval,    2, SlaTargetUnit.BusinessDays),
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
            Name: "Onboarding · Day-1-Ready",
            Description: "Vollständige Bereitstellung neuer Mitarbeiter zum ersten Arbeitstag.",
            State: SlaPolicyState.Active,
            Priority: SlaPriority.High,
            Calendar: SlaCalendar.BusinessHours,
            Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
            PauseOn: "Waiting for IT, Waiting for HR",
            OwnerId: SeedSupportIdentityId,
            Targets:
            [
                new SlaTargetTemplate("Vertrag versandt", SlaTargetKind.Response,    5, SlaTargetUnit.BusinessDays),
                new SlaTargetTemplate("Day-1-Ready",      SlaTargetKind.Fulfillment, 1, SlaTargetUnit.BusinessDays),
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
                Name: $"{className} · Default-SLA",
                Description: $"Standard-SLA für die Klasse {className}.",
                State: SlaPolicyState.Active,
                Priority: SlaPriority.Medium,
                Calendar: SlaCalendar.BusinessHours,
                Notifications: SlaNotificationChannels.Email | SlaNotificationChannels.InApp,
                PauseOn: null,
                OwnerId: SeedAdminIdentityId,
                Targets:
                [
                    new SlaTargetTemplate("Erstreaktion", SlaTargetKind.Response,   1, SlaTargetUnit.BusinessDays),
                    new SlaTargetTemplate("Lösung",       SlaTargetKind.Resolution, 5, SlaTargetUnit.BusinessDays),
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
