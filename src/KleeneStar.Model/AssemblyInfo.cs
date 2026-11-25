using System.Runtime.CompilerServices;

/// <summary>
/// Exposes this assembly’s internal types to the
/// “KleeneStar.Core” assembly.
/// </summary>
[assembly: InternalsVisibleTo("KleeneStar.Core")]
[assembly: InternalsVisibleTo("Kleenestar.Model.Test")]
[assembly: InternalsVisibleTo("KleeneStar.Core.Test")]
[assembly: InternalsVisibleTo("KleeneStar.Portal.Test")]

// the automation plugin keeps its rules and job queue in the model (ModelHub.Automation)
[assembly: InternalsVisibleTo("KleeneStar.Automation")]
[assembly: InternalsVisibleTo("KleeneStar.Automation.Test")]

// the integration plugin keeps its webhooks in the model (ModelHub.Webhook)
[assembly: InternalsVisibleTo("KleeneStar.Integration")]
[assembly: InternalsVisibleTo("KleeneStar.Integration.Test")]

// the calendar plugin keeps schedules, participants, bookings and calendar layers in the
// model (ModelHub.Appointment)
[assembly: InternalsVisibleTo("KleeneStar.Calendar")]
[assembly: InternalsVisibleTo("KleeneStar.Calendar.Test")]