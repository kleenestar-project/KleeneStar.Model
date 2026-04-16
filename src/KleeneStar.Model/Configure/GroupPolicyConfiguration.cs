using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the GroupPolicy entity type.
    /// </summary>
    internal class GroupPolicyConfiguration : IEntityTypeConfiguration<GroupPolicy>
    {
        /// <summary>
        /// Configures the GroupPolicy entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<GroupPolicy> builder)
        {
            builder.ToTable("GroupPolicy");

            // primary key
            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            // global Guid identifier (optional but supported)
            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            // foreign key to Group
            builder.Property(x => x.GroupId)
                .HasColumnName("GroupId")
                .IsRequired();

            // policy name (full type name or attribute name)
            builder.Property(x => x.Policy)
                .HasColumnName("Policy")
                .IsRequired()
                .HasMaxLength(256);

            // relation: many GroupPolicies → one Group
            builder.HasOne(x => x.Group)
                .WithMany(x => x.GroupPolicies)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}