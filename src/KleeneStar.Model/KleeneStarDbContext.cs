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
        /// Gets or sets the collection of categories.
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Gets or sets the collection of workspaces.
        /// </summary>
        public DbSet<Workspace> Workspaces { get; set; }

        /// <summary>
        /// Gets or sets the collection of classes.
        /// </summary>
        public DbSet<Class> Classes { get; set; }

        /// <summary>
        /// Gets or sets the collection of fields.
        /// </summary>
        public DbSet<Field> Fields { get; set; }

        /// <summary>
        /// Gets or sets the collection of forms.
        /// </summary>
        public DbSet<Form> Forms { get; set; }

        /// <summary>
        /// Gets or sets the collection of priorities.
        /// </summary>
        public DbSet<Priority> Priorities { get; set; }

        /// <summary>
        /// Gets or sets the collection of workflows.
        /// </summary>
        public DbSet<Workflow> Workflows { get; set; }

        /// <summary>
        /// Gets or sets the collection of workflow states.
        /// </summary>
        public DbSet<Status> Statuses { get; set; }

        /// <summary>
        /// Gets or sets the collection of status categories.
        /// </summary>
        public DbSet<StatusCategory> StatusCategories { get; set; }

        /// <summary>
        /// Gets or sets the collection of workflow transitions.
        /// </summary>
        public DbSet<Transition> Transitions { get; set; }

        /// <summary>
        /// Gets or sets the collection of objects.
        /// </summary>
        public DbSet<Object> Objects { get; set; }

        /// <summary>
        /// Gets or sets the collection of dashboards.
        /// </summary>
        public DbSet<Dashboard> Dashboards { get; set; }

        /// <summary>
        /// Gets or sets the collection of dashboard columns.
        /// </summary>
        public DbSet<DashboardColumn> DashboardColumns { get; set; }

        /// <summary>
        /// Gets or sets the collection of widgets.
        /// </summary>
        public DbSet<Widget> Widgets { get; set; }

        /// <summary>
        /// Gets or sets the collection of tenants.
        /// </summary>
        public DbSet<Tenant> Tenants { get; set; }

        /// <summary>
        /// Gets or sets the collection of permissions.
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// Gets or sets the collection of policies.
        /// </summary>
        public DbSet<Policy> Policies { get; set; }

        /// <summary>
        /// Gets or sets the collection of groups.
        /// </summary>
        public DbSet<Group> Groups { get; set; }

        /// <summary>
        /// Gets or sets the collection of permission profiles.
        /// </summary>
        public DbSet<PermissionProfile> PermissionProfiles { get; set; }

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
