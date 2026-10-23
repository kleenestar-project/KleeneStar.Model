using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the webhooks.
    /// </summary>
    internal class WebhookConfiguration : IEntityTypeConfiguration<Webhook>
    {
        /// <summary>
        /// Configures the entity type mapping for the Webhook entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Webhook> builder)
        {
            builder.ToTable("Webhook");

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

            // the connections of a workspace go with it
            builder.HasOne<Workspace>()
                .WithMany()
                .HasForeignKey(x => x.WorkspaceId)
                .HasPrincipalKey(w => w.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(64);

            builder.HasIndex(x => new { x.WorkspaceId, x.Name })
                .IsUnique();

            builder.Property(x => x.Direction)
                .HasColumnName("Direction")
                .IsRequired();

            builder.Property(x => x.Url)
                .HasColumnName("Url")
                .HasMaxLength(1024);

            builder.Property(x => x.Integration)
                .HasColumnName("Integration")
                .HasMaxLength(64);

            builder.Property(x => x.Secret)
                .HasColumnName("Secret")
                .HasMaxLength(256);

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.LastDelivery)
                .HasColumnName("LastDelivery");

            builder.Property(x => x.LastResult)
                .HasColumnName("LastResult")
                .HasMaxLength(256);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();
        }
    }
}
