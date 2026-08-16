using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="UserNotification"/>.
    /// </summary>
    internal class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        /// <summary>
        /// Configures the entity type mapping for the UserNotification entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.ToTable("UserNotification");

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

            // the actor is optional: an event raised by a job or an expiring SLA has no person
            // behind it. Removing an identity clears the reference rather than taking the
            // notifications of everybody else with it.
            builder.Property(x => x.ActorId)
                .HasColumnName("Actor")
                .HasMaxLength(36);

            builder.HasOne(x => x.Actor)
                .WithMany()
                .HasForeignKey(x => x.ActorId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.TitleKey)
                .HasColumnName("TitleKey")
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.MessageKey)
                .HasColumnName("MessageKey")
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.Subject)
                .HasColumnName("Subject")
                .HasMaxLength(256);

            builder.Property(x => x.TargetUri)
                .HasColumnName("TargetUri")
                .HasMaxLength(512);

            builder.Property(x => x.SubjectIcon)
                .HasColumnName("SubjectIcon")
                .HasMaxLength(512);

            builder.Property(x => x.Read)
                .HasColumnName("Read")
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            // the center reads "my unread, newest first" on every page render, which is what
            // this index serves
            builder.HasIndex(x => new { x.OwnerId, x.Read });
        }
    }
}
