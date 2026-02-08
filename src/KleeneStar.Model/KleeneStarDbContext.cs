using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using WebExpress.WebIndex.Queries;

namespace KleeneStar.Model
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the KleeneStar 
    /// application, providing access to the application's data entities and 
    /// database operations.
    /// </summary>
    public class KleeneStarDbContext : DbContext, IQueryContext
    {
        /// <summary>
        /// Returns or sets the collection of categories.
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Returns or sets the collection of workspaces.
        /// </summary>
        public DbSet<Workspace> Workspaces { get; set; }

        /// <summary>
        /// Initializes a new instance of the class using the specified options.
        /// </summary>
        /// <param name="options">
        /// The options to be used by the DbContext. Must not be null.
        /// </param>
        public KleeneStarDbContext(DbContextOptions<KleeneStarDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Configures the schema needed for the context by using the specified model builder.
        /// </summary>
        /// <param name="modelBuilder">
        /// The builder used to construct the model for the context. Provides configuration 
        /// of entity types, relationships, and database mappings.
        /// </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(KleeneStarDbContext).Assembly);
        }
    }
}
