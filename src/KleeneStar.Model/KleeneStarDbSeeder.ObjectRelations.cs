using KleeneStar.Model.Entities;
using System;
using System.Linq;
using WebExpress.WebApp.WebRelation;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Adds a starting catalog of relations, so an installation begins with something an
        /// administrator recognises rather than with an empty table.
        /// </summary>
        /// <remarks>
        /// These eight rows are ordinary data, not a fixed set: every one of them can be
        /// renamed, re-scoped, deactivated or dropped, and an administrator adds their own
        /// beside them. Nothing in the application knows a relation by name - the eight are
        /// simply the ones most installations turn out to want.
        /// <para>
        /// Their labels are i18n keys rather than prose, so the catalog of a fresh
        /// installation reads in the language of the request; the first edit of a row replaces
        /// its key with what the administrator typed, which is the point at which the relation
        /// stops being a suggestion and becomes theirs.
        /// </para>
        /// </remarks>
        /// <param name="db">The database context to which the relations will be added. Cannot be null.</param>
        private static void SeedObjectRelationTypes(KleeneStarDbContext db)
        {
            void add
            (
                string id,
                string key,
                string icon,
                int order,
                RelationCardinality cardinality,
                RelationEffect effect = RelationEffect.None,
                bool symmetric = false,
                bool oneSided = false,
                string system = null
            )
            {
                var prefix = $"webexpress.webapp:relation.type.{key}";

                db.ObjectRelationTypes.Add(new ObjectRelationType
                {
                    Id = Guid.Parse(id),
                    Key = key,

                    // a one-sided relation has no counterpart to name - the address a web link
                    // points at never reads the relation back - so the column stays empty
                    // rather than holding a caption nothing would ever render
                    Label = $"{prefix}.label",
                    InverseLabel = oneSided ? null : symmetric ? $"{prefix}.label" : $"{prefix}.inverse",
                    Description = $"{prefix}.description",
                    Symmetric = symmetric,
                    System = system ?? RelationSystem.Object,
                    Cardinality = cardinality,
                    Effect = effect,
                    Active = true,
                    Icon = icon,
                    Order = order,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                });
            }

            add("1C9B4A6E-5E0F-4A2E-9D5B-9A31C0D5F001", RelationType.Blocks, "flag", 1, RelationCardinality.ManyToMany, RelationEffect.BlocksCompletion);
            add("1C9B4A6E-5E0F-4A2E-9D5B-9A31C0D5F002", RelationType.Causes, "bolt", 2, RelationCardinality.OneToMany);
            add("1C9B4A6E-5E0F-4A2E-9D5B-9A31C0D5F003", RelationType.References, "file", 3, RelationCardinality.ManyToMany);
            add("1C9B4A6E-5E0F-4A2E-9D5B-9A31C0D5F004", RelationType.Similar, "clone", 4, RelationCardinality.ManyToMany, symmetric: true);
            add("1C9B4A6E-5E0F-4A2E-9D5B-9A31C0D5F005", RelationType.Duplicate, "copy", 5, RelationCardinality.ManyToOne, RelationEffect.ClosesItem);
            add("1C9B4A6E-5E0F-4A2E-9D5B-9A31C0D5F006", RelationType.Parent, "sitemap", 6, RelationCardinality.OneToMany, RelationEffect.AggregatesProgress);
            add("1C9B4A6E-5E0F-4A2E-9D5B-9A31C0D5F007", RelationType.Replaces, "arrow-right-arrow-left", 7, RelationCardinality.OneToOne);
            add("1C9B4A6E-5E0F-4A2E-9D5B-9A31C0D5F008", RelationType.WebLink, "arrow-up-right-from-square", 8, RelationCardinality.ManyToMany, oneSided: true, system: RelationSystem.Web);
        }

        /// <summary>
        /// Adds a small set of demonstration relations so the link surface of a fresh installation
        /// shows what it is for instead of an empty state.
        /// </summary>
        /// <remarks>
        /// The ends are resolved from whatever objects the object seeder produced rather than
        /// named by key, because the keys are derived from the workspace and the running count
        /// and would go stale the moment that seeder changes. Objects of one workspace are
        /// paired, which keeps every seeded relation inside one permission scope.
        /// </remarks>
        /// <param name="db">The database context to which the relations will be added. Cannot be null.</param>
        private static void SeedObjectRelations(KleeneStarDbContext db)
        {
            var relations = new[]
            {
                (Type: RelationType.Blocks, Comment: "Same gateway - the change has to land first."),
                (Type: RelationType.Causes, Comment: "The regression appeared with this rollout."),
                (Type: RelationType.References, Comment: "Background for the decision."),
                (Type: RelationType.Similar, Comment: "Reported twice within the same hour."),
                (Type: RelationType.Duplicate, Comment: "Same symptom, same component.")
            };

            var author = db.Identities.OrderBy(x => x.RawId).FirstOrDefault();
            var seeded = 0;

            foreach (var workspace in db.Workspaces.OrderBy(x => x.RawId).Take(3).ToList())
            {
                var objects = db.Objects
                    .Where(x => x.WorkspaceId == workspace.Id)
                    .OrderBy(x => x.RawId)
                    .Take(8)
                    .ToList();

                // a relation needs two ends, so a workspace the object seeder left thin is
                // simply skipped rather than relationed against itself
                for (var i = 0; i + 1 < objects.Count && i < relations.Length * 2; i += 2)
                {
                    var relation = relations[(i / 2) % relations.Length];

                    db.ObjectRelations.Add(new ObjectRelation
                    {
                        Id = Guid.NewGuid(),
                        System = RelationSystem.Object,
                        TypeKey = relation.Type,
                        Direction = RelationDirection.Bidirectional,
                        Status = RelationStatus.Active,
                        SourceObjectId = objects[i].Id,
                        TargetObjectId = objects[i + 1].Id,
                        Comment = relation.Comment,
                        CreatedById = author?.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    });

                    seeded++;
                }

                // one external relation per workspace, so both categories of the hybrid model are
                // visible on a fresh installation
                if (objects.Count > 0)
                {
                    db.ObjectRelations.Add(new ObjectRelation
                    {
                        Id = Guid.NewGuid(),
                        System = RelationSystem.Web,
                        TypeKey = RelationType.WebLink,
                        Direction = RelationDirection.Unidirectional,
                        Status = RelationStatus.Active,
                        SourceObjectId = objects[0].Id,
                        TargetUri = "https://github.com/kleenestar-project",
                        TargetTitle = "KleeneStar on GitHub",
                        Comment = "Project home.",
                        CreatedById = author?.Id,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow
                    });

                    seeded++;
                }
            }

            if (seeded == 0)
            {
                Console.WriteLine("No objects available - object relations were not seeded.");
            }
        }
    }
}
