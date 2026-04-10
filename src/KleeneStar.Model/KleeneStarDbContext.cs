using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace KleeneStar.Model;

/// <summary>
/// The database context for the KleeneStar domain model.
/// Provides access to all entity sets and applies entity configurations.
/// </summary>
public class KleeneStarDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the set of permissions.
    /// </summary>
    public DbSet<Permission> Permissions => Set<Permission>();

    /// <summary>
    /// Gets or sets the set of policies.
    /// </summary>
    public DbSet<Policy> Policies => Set<Policy>();

    /// <summary>
    /// Gets or sets the set of groups.
    /// </summary>
    public DbSet<Group> Groups => Set<Group>();

    /// <summary>
    /// Gets or sets the set of workspaces.
    /// </summary>
    public DbSet<Workspace> Workspaces => Set<Workspace>();

    /// <summary>
    /// Gets or sets the set of permission profiles.
    /// </summary>
    public DbSet<PermissionProfile> PermissionProfiles => Set<PermissionProfile>();

    /// <summary>
    /// Initializes a new instance of <see cref="KleeneStarDbContext"/>.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public KleeneStarDbContext(DbContextOptions<KleeneStarDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Applies all entity type configurations from the current assembly.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to construct the model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KleeneStarDbContext).Assembly);
    }
}
