using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the schedules of appointments.
    /// </summary>
    internal class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        /// <summary>
        /// Configures the entity type mapping for the Appointment entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointment");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.ObjectId)
                .HasColumnName("Object")
                .IsRequired();

            // the schedule goes with its appointment
            builder.HasOne<Object>()
                .WithMany()
                .HasForeignKey(x => x.ObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ObjectId)
                .IsUnique();

            builder.Property(x => x.Start)
                .HasColumnName("Start")
                .IsRequired();

            builder.Property(x => x.End)
                .HasColumnName("End")
                .IsRequired();

            builder.Property(x => x.AllDay)
                .HasColumnName("AllDay");

            builder.Property(x => x.TimeZone)
                .HasColumnName("TimeZone")
                .HasMaxLength(64);

            builder.Property(x => x.Location)
                .HasColumnName("Location")
                .HasMaxLength(256);

            builder.Property(x => x.Recurrence)
                .HasColumnName("Recurrence")
                .HasMaxLength(512);

            builder.Property(x => x.Exceptions)
                .HasColumnName("Exceptions");

            builder.Property(x => x.SeriesEnd)
                .HasColumnName("SeriesEnd");

            builder.Property(x => x.AutoClose)
                .HasColumnName("AutoClose");

            builder.Property(x => x.Closed)
                .HasColumnName("Closed");

            builder.Property(x => x.OrganizerId)
                .HasColumnName("Organizer");

            builder.Property(x => x.ExternalId)
                .HasColumnName("ExternalId")
                .HasMaxLength(256);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            // a period is narrowed by the start and the end of the series
            builder.HasIndex(x => new { x.Start, x.SeriesEnd });
            builder.HasIndex(x => x.ExternalId);
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the participants of appointments.
    /// </summary>
    internal class AppointmentParticipantConfiguration : IEntityTypeConfiguration<AppointmentParticipant>
    {
        /// <summary>
        /// Configures the entity type mapping for the AppointmentParticipant entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AppointmentParticipant> builder)
        {
            builder.ToTable("AppointmentParticipant");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.ObjectId)
                .HasColumnName("Object")
                .IsRequired();

            // the invitations go with their appointment
            builder.HasOne<Object>()
                .WithMany()
                .HasForeignKey(x => x.ObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // no foreign key: an account is removed without asking its invitations
            builder.Property(x => x.IdentityId)
                .HasColumnName("Identity");

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .HasMaxLength(256);

            builder.Property(x => x.Email)
                .HasColumnName("Email")
                .HasMaxLength(256);

            builder.Property(x => x.Role)
                .HasColumnName("Role")
                .IsRequired();

            builder.Property(x => x.Response)
                .HasColumnName("Response")
                .IsRequired();

            builder.Property(x => x.Comment)
                .HasColumnName("Comment")
                .HasMaxLength(1024);

            builder.Property(x => x.Responded)
                .HasColumnName("Responded");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => x.ObjectId);
            builder.HasIndex(x => x.IdentityId);
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the resource bookings of
    /// appointments.
    /// </summary>
    internal class AppointmentResourceConfiguration : IEntityTypeConfiguration<AppointmentResource>
    {
        /// <summary>
        /// Configures the entity type mapping for the AppointmentResource entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AppointmentResource> builder)
        {
            builder.ToTable("AppointmentResource");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.ObjectId)
                .HasColumnName("Object")
                .IsRequired();

            // the bookings go with their appointment
            builder.HasOne<Object>()
                .WithMany()
                .HasForeignKey(x => x.ObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // no foreign key: a second cascading path into the object table is refused by
            // SQL Server; a booking of a removed resource is dropped by the plugin
            builder.Property(x => x.ResourceId)
                .HasColumnName("Resource")
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.HasIndex(x => new { x.ObjectId, x.ResourceId })
                .IsUnique();
            builder.HasIndex(x => x.ResourceId);
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the sources of workspace calendars.
    /// </summary>
    internal class AppointmentCalendarSourceConfiguration : IEntityTypeConfiguration<AppointmentCalendarSource>
    {
        /// <summary>
        /// Configures the entity type mapping for the AppointmentCalendarSource entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AppointmentCalendarSource> builder)
        {
            builder.ToTable("AppointmentCalendarSource");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.WorkspaceId)
                .HasColumnName("Workspace")
                .IsRequired();

            // the layers of a calendar go with its workspace
            builder.HasOne<Workspace>()
                .WithMany()
                .HasForeignKey(x => x.WorkspaceId)
                .HasPrincipalKey(w => w.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Kind)
                .HasColumnName("Kind")
                .IsRequired()
                .HasMaxLength(16);

            // no foreign key: a second path from the workspace table; a source naming a removed
            // workspace shows nothing
            builder.Property(x => x.SourceWorkspaceId)
                .HasColumnName("SourceWorkspace");

            builder.Property(x => x.Query)
                .HasColumnName("Query");

            builder.Property(x => x.Color)
                .HasColumnName("Color")
                .HasMaxLength(32);

            builder.Property(x => x.Enabled)
                .HasColumnName("Enabled");

            builder.Property(x => x.Order)
                .HasColumnName("Order");

            builder.Property(x => x.CreatedById)
                .HasColumnName("CreatedBy");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => x.WorkspaceId);
        }
    }
}
