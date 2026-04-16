using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebCore.WebIdentity;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the group entity type.
    /// </summary>
    internal class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        /// <summary>
        /// Configuration of the group entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Group");

            // Primary key
            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            // 1:n relation to GroupPolicy
            builder.HasMany(x => x.GroupPolicies)
                .WithOne(x => x.Group)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:n relation to IdentityGroupMembership (m:n join)
            builder.HasMany(x => x.GroupMemberships)
                .WithOne(x => x.Group)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore interface property (derived, not persisted)
            builder.Ignore(x => ((IIdentityGroup)x).Policies);

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
