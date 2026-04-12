using Microsoft.EntityFrameworkCore;

namespace KleeneStar.Model.Test
{
    /// <summary>
    /// Contains unit tests for the database seeder.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestKleeneStarDbSeeder
    {
        /// <summary>
        /// Verifies that identity entities and their group assignments are seeded.
        /// </summary>
        [Fact]
        public async Task SeedIdentityEntities()
        {
            // arrange
            var connectionString = $"SeedIdentityEntities_{Guid.NewGuid()}";

            await using var db = InMemoryDbContextFactory.Create(connectionString);

            // act
            await KleeneStarDbSeeder.SeedAsync(db);

            // validation
            Assert.Equal(4, db.Groups.Count());
            Assert.Equal(4, db.Identities.Count());

            var admin = db.Identities
                .Include(x => x.GroupMemberships)
                    .ThenInclude(x => x.Group)
                .Single(x => x.Email == "admin@kleenestar.local");

            Assert.Contains(admin.GroupMemberships, x => x.Group.Name == "Admin");
        }

        /// <summary>
        /// Verifies that seeding remains idempotent for identity entities.
        /// </summary>
        [Fact]
        public async Task SeedIdentityEntitiesIsIdempotent()
        {
            // arrange
            var connectionString = $"SeedIdentityEntitiesIsIdempotent_{Guid.NewGuid()}";

            await using (var db = InMemoryDbContextFactory.Create(connectionString))
            {
                await KleeneStarDbSeeder.SeedAsync(db);
            }

            // act
            await using (var db = InMemoryDbContextFactory.Create(connectionString))
            {
                await KleeneStarDbSeeder.SeedAsync(db);
            }

            // validation
            await using (var db = InMemoryDbContextFactory.Create(connectionString))
            {
                Assert.Equal(4, db.Groups.Count());
                Assert.Equal(4, db.Identities.Count());
                Assert.Equal(4, db.Identities.Include(x => x.GroupMemberships).Sum(x => x.GroupMemberships.Count));
            }
        }
    }
}
