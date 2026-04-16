using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the IdentityGroupMembership entity type.
    /// </summary>
    internal class IdentityGroupMembershipConfiguration : IEntityTypeConfiguration<IdentityGroupMembership>
    {
        /// <summary>
        /// Configuration of the field entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<IdentityGroupMembership> builder)
        {
            builder.ToTable("IdentityGroupMembership");

            // Composite primary key (IdentityId + GroupId)
            builder.HasKey(x => new { x.IdentityId, x.GroupId });

            builder.Property(x => x.IdentityId)
                .HasColumnName("IdentityId")
                .IsRequired();

            builder.Property(x => x.GroupId)
                .HasColumnName("GroupId")
                .IsRequired();

            // Relation: Identity 1..n Memberships
            builder.HasOne(x => x.Identity)
                .WithMany(x => x.GroupMemberships)
                .HasForeignKey(x => x.IdentityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Group 1..n Memberships
            builder.HasOne(x => x.Group)
                .WithMany(x => x.GroupMemberships)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}