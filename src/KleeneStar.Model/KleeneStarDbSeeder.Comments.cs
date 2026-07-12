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
        /// Adds personalized discussion threads to every seeded <see cref="Object"/>.
        /// Each object receives 2-4 comments authored by different identities, with the
        /// content tailored to the class of the object (Incident gets triage notes,
        /// Bug gets investigation steps, Onboarding gets HR progress, etc.). A subset
        /// of comments include nested replies to exercise the reply path.
        /// </summary>
        /// <param name="db">The database context.</param>
        private static void SeedComments(KleeneStarDbContext db)
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

            // anchor timestamps to a stable "now-minus-N-days" so the seed produces
            // chronologically sensible results without rewriting every run.
            var anchor = new DateTime(2026, 5, 1, 9, 0, 0, DateTimeKind.Utc);

            foreach (var obj in objects)
            {
                var templates = GetCommentTemplatesForClass(obj.Class?.Name);
                if (templates.Count == 0)
                {
                    continue;
                }

                var minute = 0;
                var templateIndex = 0;
                foreach (var t in templates)
                {
                    var topLevel = new Comment
                    {
                        Id = Guid.NewGuid(),
                        ObjectId = obj.Id,
                        AuthorId = ResolveSeedAuthor(t.Author, admin, alice, support, marketer).Id,
                        Content = t.Content,
                        State = CommentState.Active,
                        Created = anchor.AddMinutes(minute),
                        Updated = anchor.AddMinutes(minute),
                        // pin the first template of every thread so the UI shows the
                        // "pinned" badge on at least one comment per object.
                        IsPinned = templateIndex == 0
                    };
                    db.Comments.Add(topLevel);
                    minute += 17;

                    // seed two likes per top-level comment so the like count is visible
                    // immediately on first load.
                    foreach (var liker in PickLikers(t.Author, admin, alice, support, marketer))
                    {
                        db.CommentLikes.Add(new CommentLike
                        {
                            Id = Guid.NewGuid(),
                            CommentId = topLevel.Id,
                            AuthorId = liker.Id,
                            Created = anchor.AddMinutes(minute - 1)
                        });
                    }

                    // seed one emoji reaction on every other top-level comment so the
                    // reaction chip row is exercised.
                    if (templateIndex % 2 == 0)
                    {
                        var reactor = ResolveSeedAuthor(SeedAuthor.Alice, admin, alice, support, marketer);
                        db.CommentReactions.Add(new CommentReaction
                        {
                            Id = Guid.NewGuid(),
                            CommentId = topLevel.Id,
                            AuthorId = reactor.Id,
                            Emoji = PickEmojiForClass(obj.Class?.Name),
                            Created = anchor.AddMinutes(minute - 1)
                        });
                    }

                    foreach (var reply in t.Replies)
                    {
                        db.Comments.Add(new Comment
                        {
                            Id = Guid.NewGuid(),
                            ObjectId = obj.Id,
                            AuthorId = ResolveSeedAuthor(reply.Author, admin, alice, support, marketer).Id,
                            Content = reply.Content,
                            State = CommentState.Active,
                            ParentCommentId = topLevel.Id,
                            Created = anchor.AddMinutes(minute),
                            Updated = anchor.AddMinutes(minute)
                        });
                        minute += 11;
                    }

                    templateIndex++;
                }
            }
        }

        private static IEnumerable<Identity> PickLikers(SeedAuthor author, Identity admin, Identity alice, Identity support, Identity marketer)
        {
            // every comment is liked by two identities OTHER than its author so the
            // like list isn't biased by self-likes.
            var pool = new[] { admin, alice, support, marketer };
            var authorId = ResolveSeedAuthor(author, admin, alice, support, marketer).Id;
            return pool.Where(i => i.Id != authorId).Take(2);
        }

        private static string PickEmojiForClass(string className) =>
            className switch
            {
                "Incident" => "🔥",
                "Problem" => "🔍",
                "Bug" => "🐞",
                "Feature" => "✨",
                "ServiceRequest" => "📨",
                "Change" or "ChangeRequest" => "🔧",
                "Onboarding" => "👋",
                "Knowledge" => "📚",
                "Approval" => "✅",
                "Invoice" or "PurchaseOrder" => "💰",
                _ => "👍"
            };

        private enum SeedAuthor
        {
            Admin,
            Alice,
            Support,
            Marketer
        }

        private sealed record CommentTemplate(SeedAuthor Author, string Content, IReadOnlyList<CommentTemplate> Replies)
        {
            public CommentTemplate(SeedAuthor author, string content)
                : this(author, content, [])
            {
            }
        }

        private static Identity ResolveSeedAuthor(SeedAuthor key, Identity admin, Identity alice, Identity support, Identity marketer) =>
            key switch
            {
                SeedAuthor.Alice => alice,
                SeedAuthor.Support => support,
                SeedAuthor.Marketer => marketer,
                _ => admin
            };

        /// <summary>
        /// Returns the catalogue of seed comments for the supplied class name. The
        /// content is intentionally short and class-flavoured so a fresh install shows
        /// realistic discussion threads instead of placeholder text.
        /// </summary>
        /// <param name="className">The class name of the object (may be <c>null</c>).</param>
        /// <returns>The list of comment templates; may be empty.</returns>
        private static IReadOnlyList<CommentTemplate> GetCommentTemplatesForClass(string className)
        {
            switch (className)
            {
                case "Incident":
                    return
                    [
                        new(SeedAuthor.Support, "Reported by the affected user. Reproducible on the production VPN gateway.", [
                            new(SeedAuthor.Alice,   "Confirmed — I see the same behavior from the office segment."),
                            new(SeedAuthor.Admin,   "Escalated to the network team. They are on it."),
                        ]),
                        new(SeedAuthor.Alice, "Workaround documented in the knowledge base. Closing the ticket once the patch is rolled out."),
                    ];

                case "Problem":
                    return
                    [
                        new(SeedAuthor.Admin, "Root cause analysis kick-off. We will run a 5-whys session with the on-call team tomorrow."),
                        new(SeedAuthor.Alice, "Initial finding: the recent firmware update changed default keepalive values."),
                    ];

                case "Change":
                case "ChangeRequest":
                    return
                    [
                        new(SeedAuthor.Admin,   "CAB review scheduled for Thursday. Risk class: medium."),
                        new(SeedAuthor.Support, "Maintenance window communicated to all stakeholders."),
                    ];

                case "ServiceRequest":
                    return
                    [
                        new(SeedAuthor.Support, "Hardware ordered, delivery expected within 3 business days.", [
                            new(SeedAuthor.Admin, "Please attach the asset tag once the laptop arrives."),
                        ]),
                    ];

                case "Ticket":
                    return
                    [
                        new(SeedAuthor.Support, "Investigation underway. Will update once we have logs."),
                    ];

                case "Knowledge":
                    return
                    [
                        new(SeedAuthor.Alice, "Reviewed for accuracy on 2026-04-12. Still matches the current build."),
                        new(SeedAuthor.Admin, "Marked as evergreen — next review due in 90 days."),
                    ];

                case "Bug":
                    return
                    [
                        new(SeedAuthor.Alice, "Reproduced locally on the develop branch. Stack trace attached."),
                        new(SeedAuthor.Admin, "Triage: Blocker. Holds the next release.", [
                            new(SeedAuthor.Alice, "Patch ready — opening pull request shortly."),
                        ]),
                    ];

                case "Feature":
                    return
                    [
                        new(SeedAuthor.Marketer, "Customer feedback strongly in favour. Promoting to Must-Have."),
                        new(SeedAuthor.Alice,    "Design spike completed. Implementation plan attached."),
                    ];

                case "Task":
                    return
                    [
                        new(SeedAuthor.Alice, "Picked this up for the current sprint."),
                    ];

                case "Sprint":
                    return
                    [
                        new(SeedAuthor.Alice, "Velocity matches the previous sprint. Burndown looks healthy."),
                    ];

                case "Onboarding":
                    return
                    [
                        new(SeedAuthor.Support, "Day-1 readiness check complete. Accounts provisioned."),
                        new(SeedAuthor.Admin,   "Welcome e-mail sent. First-week schedule attached."),
                    ];

                case "Invoice":
                    return
                    [
                        new(SeedAuthor.Admin, "Approval routed to the cost-center owner. Payment expected by month-end."),
                    ];

                case "PurchaseOrder":
                    return
                    [
                        new(SeedAuthor.Admin, "Procurement check passed. Order released to supplier."),
                    ];

                case "Approval":
                    return
                    [
                        new(SeedAuthor.Admin, "Approved subject to compliance sign-off."),
                    ];

                case "Asset":
                case "Repository":
                case "Release":
                case "Documentation":
                    return
                    [
                        new(SeedAuthor.Alice, "Asset inventory cross-checked with the CMDB extract."),
                    ];

                default:
                    return
                    [
                        new(SeedAuthor.Support, "Initial review complete. No blocking issues found."),
                    ];
            }
        }
    }
}
