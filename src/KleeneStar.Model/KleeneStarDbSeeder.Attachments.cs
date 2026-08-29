using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Attaches a small, class-flavoured set of files to every seeded <see cref="Object"/>
        /// so the object attachment card is populated out of the box. The seed only writes
        /// attachment metadata — no binary payload is materialized on disk — which is enough
        /// for the file list to render names, sizes, dates and uploaders. Uploaders are
        /// resolved by e-mail and reuse the same <see cref="SeedAuthor"/> pool as the comment
        /// seeder.
        /// </summary>
        /// <param name="db">The database context.</param>
        private static void SeedAttachments(KleeneStarDbContext db)
        {
            var identities = db.Identities.AsNoTracking().ToList();
            if (identities.Count == 0)
            {
                return;
            }

            var admin = identities.First(i => i.Email == "admin@kleenestar.org");
            var alice = identities.First(i => i.Email == "alice.engineer@kleenestar.org");
            var support = identities.First(i => i.Email == "support@kleenestar.org");
            var marketer = identities.First(i => i.Email == "marketer@kleenestar.org");

            var objects = db.Objects
                .Include(o => o.Class)
                .AsNoTracking()
                .ToList();

            // anchor timestamps to a stable point so the seed produces chronologically
            // sensible results without rewriting every run.
            var anchor = new DateTime(2026, 5, 1, 9, 0, 0, DateTimeKind.Utc);

            foreach (var obj in objects)
            {
                var templates = GetAttachmentTemplatesForClass(obj.Class?.Name);
                if (templates.Count == 0)
                {
                    continue;
                }

                // the seed writes through the context rather than through ModelHub.Add, so the
                // version each row holds among the files of its name is counted here - a template
                // catalogue that ever repeats a name then seeds a version chain rather than two
                // files that collide
                var chains = new Dictionary<string, int>();

                var minute = 0;
                foreach (var t in templates)
                {
                    var id = Guid.NewGuid();
                    var uploader = ResolveSeedAuthor(t.Uploader, admin, alice, support, marketer);
                    var version = chains.TryGetValue(t.FileName, out var previous) ? previous + 1 : 1;
                    chains[t.FileName] = version;

                    // seed a small textual placeholder payload so the download endpoint
                    // returns a real (if tiny) file for every seeded attachment instead of
                    // a 404 — the seed never carries the original multi-kilobyte binary.
                    var content = System.Text.Encoding.UTF8.GetBytes(
                        $"KleeneStar seed attachment\r\n" +
                        $"File: {t.FileName}\r\n" +
                        $"Object: {obj.Key} — {obj.Summary}\r\n" +
                        $"{t.Description}\r\n");

                    db.Attachments.Add(new Attachment
                    {
                        Id = id,
                        ObjectId = obj.Id,
                        UploaderId = uploader.Id,
                        FileName = t.FileName,
                        // the seeded payload is a small text placeholder, so serve it as
                        // text/plain — the realistic file extension still drives the list
                        // icon, but the browser renders the placeholder cleanly on download
                        // instead of failing to parse it as a binary of the declared type.
                        ContentType = "text/plain",
                        Version = version,
                        Size = t.Size,
                        // nominal on-disk location; the payload itself lives in Content.
                        StoragePath = $"attachments/{obj.Key}/{id}/{t.FileName}",
                        Content = content,
                        Description = t.Description,
                        State = AttachmentState.Active,
                        Created = anchor.AddMinutes(minute),
                        Updated = anchor.AddMinutes(minute)
                    });

                    minute += 23;
                }
            }
        }

        /// <summary>
        /// Describes a single seed attachment: its file name, MIME type, size in bytes,
        /// a short human description and the seed identity that uploaded it.
        /// </summary>
        /// <param name="FileName">The original file name including extension.</param>
        /// <param name="ContentType">The MIME content type.</param>
        /// <param name="Size">The file size in bytes.</param>
        /// <param name="Description">A short description of the file.</param>
        /// <param name="Uploader">The seed identity that uploaded the file.</param>
        private sealed record AttachmentTemplate(string FileName, string ContentType, long Size, string Description, SeedAuthor Uploader);

        /// <summary>
        /// Returns the catalogue of seed attachments for the supplied class name. The
        /// content is intentionally short and class-flavoured so a fresh install shows a
        /// realistic file list instead of placeholder rows.
        /// </summary>
        /// <param name="className">The class name of the object (may be <c>null</c>).</param>
        /// <returns>The list of attachment templates; may be empty.</returns>
        private static IReadOnlyList<AttachmentTemplate> GetAttachmentTemplatesForClass(string className)
        {
            switch (className)
            {
                case "Incident":
                    return
                    [
                        new("incident-timeline.pdf", "application/pdf", 184_320, "Chronological timeline of the outage.", SeedAuthor.Support),
                        new("gateway-logs.txt", "text/plain", 51_200, "Raw VPN gateway log excerpt.", SeedAuthor.Alice),
                    ];

                case "Problem":
                    return
                    [
                        new("root-cause-analysis.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", 96_540, "5-whys root cause analysis write-up.", SeedAuthor.Admin),
                    ];

                case "Change":
                case "ChangeRequest":
                    return
                    [
                        new("change-plan.pdf", "application/pdf", 142_880, "Implementation and rollback plan.", SeedAuthor.Admin),
                        new("cab-approval.png", "image/png", 73_410, "Signed CAB approval screenshot.", SeedAuthor.Support),
                    ];

                case "ServiceRequest":
                    return
                    [
                        new("asset-quote.pdf", "application/pdf", 65_220, "Hardware quote from the supplier.", SeedAuthor.Support),
                    ];

                case "Bug":
                    return
                    [
                        new("stacktrace.txt", "text/plain", 12_540, "Full stack trace from the develop branch.", SeedAuthor.Alice),
                        new("repro-recording.gif", "image/gif", 1_048_576, "Screen recording reproducing the defect.", SeedAuthor.Alice),
                    ];

                case "Feature":
                    return
                    [
                        new("design-mockup.png", "image/png", 524_288, "High-fidelity mockup of the new feature.", SeedAuthor.Marketer),
                        new("implementation-plan.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 88_064, "Task breakdown and estimates.", SeedAuthor.Alice),
                    ];

                case "Knowledge":
                case "Documentation":
                    return
                    [
                        new("how-to-guide.pdf", "application/pdf", 210_944, "Step-by-step how-to guide.", SeedAuthor.Alice),
                    ];

                case "Onboarding":
                    return
                    [
                        new("welcome-pack.pdf", "application/pdf", 256_000, "New-hire welcome pack.", SeedAuthor.Admin),
                        new("first-week-schedule.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 41_984, "First-week onboarding schedule.", SeedAuthor.Support),
                    ];

                case "Invoice":
                case "PurchaseOrder":
                    return
                    [
                        new("invoice.pdf", "application/pdf", 78_336, "Scanned invoice document.", SeedAuthor.Admin),
                    ];

                default:
                    return
                    [
                        new("summary.pdf", "application/pdf", 102_400, "Summary document for the object.", SeedAuthor.Support),
                    ];
            }
        }
    }
}
