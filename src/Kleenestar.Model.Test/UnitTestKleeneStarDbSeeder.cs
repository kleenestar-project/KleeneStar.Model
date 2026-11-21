using KleeneStar.Model.Entities;
using KleeneStar.Model.Integrity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KleeneStar.Model.Test
{
    /// <summary>
    /// Contains unit tests for the base seed (<see cref="KleeneStarDbSeeder"/>): the data every
    /// database is created with. The demo data is tested in <c>KleeneStar.Seeder.Test</c>.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestKleeneStarDbSeeder
    {
        /// <summary>
        /// Verifies that the base seed writes the base data and nothing of the demo data.
        /// </summary>
        [Fact]
        public async Task SeedWritesTheBaseDataOnly()
        {
            await using var db = InMemoryDbContextFactory.Create($"SeedBaseData_{Guid.NewGuid()}");

            await KleeneStarDbSeeder.SeedAsync(db);

            Assert.Single(db.Maintenances);
            Assert.Single(db.Brandings);
            Assert.Equal
            (
                new[] { Group.AdministratorsId, Group.AuthenticatedId, Group.AnonymousId }.OrderBy(x => x),
                db.Groups.Select(x => x.Id).ToList().OrderBy(x => x)
            );
            Assert.Equal(KleeneStarDbSeeder.AdministratorId, Assert.Single(db.Identities).Id);
            Assert.Equal(4, db.StatusCategories.Count());
            Assert.Single(db.StatusCategories, x => x.IsDefault);
            Assert.Equal(8, db.ObjectRelationTypes.Count());
            Assert.Single(db.AuditEvents);

            Assert.Empty(db.Categories);
            Assert.Empty(db.Tenants);
            Assert.Empty(db.Workspaces);
            Assert.Empty(db.Classes);
            Assert.Empty(db.Objects);
            Assert.Empty(db.PermissionAssignments);
        }

        /// <summary>
        /// Verifies that the administrator is internal, a member of the administrators group, and
        /// signs in with the initial password.
        /// </summary>
        [Fact]
        public async Task SeedAdministratorSignsInWithTheInitialPassword()
        {
            await using var db = InMemoryDbContextFactory.Create($"SeedAdministrator_{Guid.NewGuid()}");

            await KleeneStarDbSeeder.SeedAsync(db);

            var admin = db.Identities
                .Include(x => x.GroupMemberships)
                    .ThenInclude(x => x.Group)
                .Single();

            Assert.Equal(KleeneStarDbSeeder.AdministratorUserName, admin.UserName);
            Assert.Equal(KleeneStarDbSeeder.AdministratorName, admin.Name);
            Assert.Equal("admin@kleenestar.org", admin.Email);
            Assert.Null(admin.AuthenticationSource);
            Assert.Null(admin.TenantId);
            Assert.NotNull(admin.PasswordChanged);
            Assert.Contains(admin.GroupMemberships, x => x.Group.Id == Group.AdministratorsId);
            Assert.NotEqual(PasswordVerificationResult.Failed, IdentityPassword.Verify(admin, KleeneStarDbSeeder.InitialPassword));
            Assert.Equal(PasswordVerificationResult.Failed, IdentityPassword.Verify(admin, "not the initial password"));
        }

        /// <summary>
        /// Verifies that a placeholder hash is replaced by the initial password while a password
        /// somebody set is left unchanged.
        /// </summary>
        [Fact]
        public async Task SeedReplacesPasswordPlaceholders()
        {
            var name = $"SeedReplacesPasswordPlaceholders_{Guid.NewGuid()}";
            var otherId = Guid.NewGuid();

            await using (var db = InMemoryDbContextFactory.Create(name))
            {
                await KleeneStarDbSeeder.SeedAsync(db);

                var admin = db.Identities.Single(x => x.Id == KleeneStarDbSeeder.AdministratorId);
                admin.PasswordHash = "$seed$v1$fb4e111dbf8b4c1cb95e0f6579f7f72f";

                var other = new Identity(otherId) { Name = "Other", UserName = "other", Email = "other@example.org" };
                other.PasswordHash = IdentityPassword.Hash(other, "own password");
                db.Identities.Add(other);

                await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            await using (var db = InMemoryDbContextFactory.Create(name))
            {
                await KleeneStarDbSeeder.SeedAsync(db);
            }

            await using (var db = InMemoryDbContextFactory.Create(name))
            {
                var admin = db.Identities.Single(x => x.Id == KleeneStarDbSeeder.AdministratorId);
                var other = db.Identities.Single(x => x.Id == otherId);

                Assert.True(IdentityPassword.IsUsable(admin.PasswordHash));
                Assert.NotEqual(PasswordVerificationResult.Failed, IdentityPassword.Verify(admin, KleeneStarDbSeeder.InitialPassword));
                Assert.NotEqual(PasswordVerificationResult.Failed, IdentityPassword.Verify(other, "own password"));
                Assert.Equal(PasswordVerificationResult.Failed, IdentityPassword.Verify(other, KleeneStarDbSeeder.InitialPassword));
            }
        }

        /// <summary>
        /// Verifies that a second run changes no count.
        /// </summary>
        [Fact]
        public async Task SeedIsIdempotent()
        {
            var name = $"SeedIsIdempotent_{Guid.NewGuid()}";

            await using (var db = InMemoryDbContextFactory.Create(name))
            {
                await KleeneStarDbSeeder.SeedAsync(db);
            }

            await using (var db = InMemoryDbContextFactory.Create(name))
            {
                await KleeneStarDbSeeder.SeedAsync(db);
            }

            await using (var db = InMemoryDbContextFactory.Create(name))
            {
                Assert.Single(db.Maintenances);
                Assert.Single(db.Brandings);
                Assert.Equal(3, db.Groups.Count());
                Assert.Single(db.Identities);
                Assert.Equal(1, db.Identities.Include(x => x.GroupMemberships).Sum(x => x.GroupMemberships.Count));
                Assert.Equal(4, db.StatusCategories.Count());
                Assert.Equal(8, db.ObjectRelationTypes.Count());
                Assert.Single(db.AuditEvents);
            }
        }

        /// <summary>
        /// Verifies that the built-in groups are added to a store that lacks them, without any
        /// permission grant.
        /// </summary>
        [Fact]
        public async Task SeedBuiltInGroupsIntoAnEarlierStore()
        {
            var name = $"SeedBuiltInGroups_{Guid.NewGuid()}";

            await using (var db = InMemoryDbContextFactory.Create(name))
            {
                await KleeneStarDbSeeder.SeedAsync(db);

                db.Groups.RemoveRange(db.Groups.Where(x => x.Id == Group.AuthenticatedId || x.Id == Group.AnonymousId));
                await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            await using (var db = InMemoryDbContextFactory.Create(name))
            {
                await KleeneStarDbSeeder.SeedAsync(db);
            }

            await using (var db = InMemoryDbContextFactory.Create(name))
            {
                Assert.Contains(db.Groups, x => x.Id == Group.AuthenticatedId);
                Assert.Contains(db.Groups, x => x.Id == Group.AnonymousId);
                Assert.Empty(db.PermissionAssignments);
            }
        }

        /// <summary>
        /// Verifies that the genesis event anchors the chain and counts the base data, and that
        /// <see cref="KleeneStarDbSeeder.RecordSeed"/> appends a verifiable event onto it.
        /// </summary>
        [Fact]
        public async Task SeedAuditGenesisAndRecordSeed()
        {
            await using var db = InMemoryDbContextFactory.Create($"SeedAudit_{Guid.NewGuid()}");

            await KleeneStarDbSeeder.SeedAsync(db);

            var genesis = db.AuditEvents.Include(x => x.Deltas).Single();

            Assert.Equal(1, genesis.Sequence);
            Assert.Equal(string.Empty, genesis.PreviousHash);
            Assert.Equal(AuditAction.Seeded, genesis.Action);
            Assert.Equal(KleeneStarDbSeeder.AuditAgent, genesis.Agent);
            Assert.True(AuditSeal.Verify(genesis, string.Empty));
            Assert.Equal("1", genesis.Deltas.Single(x => x.Attribute == "identities").NewValue);
            Assert.Equal("0", genesis.Deltas.Single(x => x.Attribute == "workspaces").NewValue);

            KleeneStarDbSeeder.RecordSeed(db, "test.seed");

            var appended = db.AuditEvents.AsNoTracking().Include(x => x.Deltas).Single(x => x.Sequence == 2);

            Assert.Equal("test.seed", appended.Agent);
            Assert.Equal(genesis.Hash, appended.PreviousHash);
            Assert.True(AuditSeal.Verify(appended, genesis.Hash));
        }
    }
}
