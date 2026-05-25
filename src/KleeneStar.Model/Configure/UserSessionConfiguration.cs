using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="UserSession"/>.
    /// </summary>
    internal class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        /// <summary>
        /// Configures the user-session entity. Establishes:
        /// <list type="bullet">
        /// <item>FK <see cref="UserSession.OwnerId"/> → <see cref="Identity"/> (cascade delete).</item>
        /// <item>Unique composite index on (Owner, Scope, Key) so each identity has at most one
        /// row per (scope, key) pair; upserts overwrite the existing row.</item>
        /// </list>
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSession");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.OwnerId)
                .HasColumnName("Owner")
                .IsRequired();

            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Scope)
                .HasColumnName("Scope")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Key)
                .HasColumnName("Key")
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.Value)
                .HasColumnName("Value");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => new { x.OwnerId, x.Scope, x.Key }).IsUnique();
        }
    }
}
