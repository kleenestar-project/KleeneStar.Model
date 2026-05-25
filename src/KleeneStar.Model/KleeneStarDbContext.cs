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
        /// Gets or sets the collection of templates.
        /// </summary>
        public DbSet<Template> Templates { get; set; }

        /// <summary>
        /// Gets or sets the collection of forms.
        /// </summary>
        public DbSet<Form> Forms { get; set; }

        /// <summary>
        /// Gets or sets the collection of form tabs.
        /// </summary>
        public DbSet<FormTab> FormTabs { get; set; }

        /// <summary>
        /// Gets or sets the collection of form structural elements (groups and field references).
        /// </summary>
        public DbSet<FormElement> FormElements { get; set; }

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
        /// Gets or sets the collection of persisted object views (workspace tabs).
        /// </summary>
        public DbSet<ObjectView> ObjectViews { get; set; }

        /// <summary>
        /// Gets or sets the collection of field values associated with objects.
        /// </summary>
        public DbSet<Value> Values { get; set; }

        /// <summary>
        /// Gets or sets the collection of typed directional links between objects.
        /// </summary>
        public DbSet<ObjectLink> ObjectLinks { get; set; }

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
        /// Gets or sets the collection of identities.
        /// </summary>
        public DbSet<Identity> Identities { get; set; }

        /// <summary>
        /// Gets or sets the collection of groups.
        /// </summary>
        public DbSet<Group> Groups { get; set; }

        /// <summary>
        /// Gets or sets the collection of SLA policies.
        /// </summary>
        public DbSet<SlaPolicy> SlaPolicies { get; set; }

        /// <summary>
        /// Gets or sets the collection of SLA targets attached to policies.
        /// </summary>
        public DbSet<SlaTarget> SlaTargets { get; set; }

        /// <summary>
        /// Gets or sets the collection of SLA scope rules attached to policies.
        /// </summary>
        public DbSet<SlaScopeRule> SlaScopeRules { get; set; }

        /// <summary>
        /// Gets or sets the collection of SLA escalation levels attached to policies.
        /// </summary>
        public DbSet<SlaEscalationLevel> SlaEscalationLevels { get; set; }

        /// <summary>
        /// Gets or sets the collection of calendars.
        /// </summary>
        public DbSet<Calendar> Calendars { get; set; }

        /// <summary>
        /// Gets or sets the collection of weekly business-hour slots attached to calendars.
        /// </summary>
        public DbSet<BusinessHourSlot> BusinessHourSlots { get; set; }

        /// <summary>
        /// Gets or sets the collection of holidays attached to calendars.
        /// </summary>
        public DbSet<Holiday> Holidays { get; set; }

        /// <summary>
        /// Gets or sets the collection of comments posted on objects.
        /// </summary>
        public DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// Gets or sets the likes attached to comments.
        /// </summary>
        public DbSet<CommentLike> CommentLikes { get; set; }

        /// <summary>
        /// Gets or sets the emoji reactions attached to comments.
        /// </summary>
        public DbSet<CommentReaction> CommentReactions { get; set; }

        /// <summary>
        /// Gets or sets the per-identity generic session/preference entries.
        /// </summary>
        public DbSet<UserSession> UserSessions { get; set; }

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
