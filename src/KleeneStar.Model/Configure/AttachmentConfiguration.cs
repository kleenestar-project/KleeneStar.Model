using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Entity Framework Core configuration for <see cref="Attachment"/>.
    /// </summary>
    internal class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        /// <summary>
        /// Configures the attachment entity. Establishes:
        /// <list type="bullet">
        /// <item>FK <see cref="Attachment.ObjectId"/> → <see cref="Object"/> (cascade delete —
        /// removing an object removes its attachments).</item>
        /// <item>FK <see cref="Attachment.UploaderId"/> → <see cref="Identity"/> (restrict —
        /// identities must not be hard-deleted while they still own attachments).</item>
        /// <item>Composite index on (Object, Created) so the file-list query can retrieve
        /// attachments per object in chronological order without a full scan.</item>
        /// <item>Composite index on (Object, FileName, Version) so the version chain of a file is
        /// resolved without a scan — it is read on every upload to assign the next number.</item>
        /// </list>
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ToTable("Attachment");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.FileName)
                .HasColumnName("FileName")
                .IsRequired();

            builder.Property(x => x.ContentType)
                .HasColumnName("ContentType");

            builder.Property(x => x.Version)
                .HasColumnName("Version");

            builder.Property(x => x.Size)
                .HasColumnName("Size");

            builder.Property(x => x.StoragePath)
                .HasColumnName("StoragePath");

            builder.Property(x => x.Content)
                .HasColumnName("Content");

            builder.Property(x => x.Description)
                .HasColumnName("Description");

            builder.Property(x => x.State)
                .HasColumnName("State");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.Property(x => x.ObjectId)
                .HasColumnName("Object")
                .IsRequired();

            builder.HasOne(x => x.Object)
                .WithMany()
                .HasForeignKey(x => x.ObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.UploaderId)
                .HasColumnName("Uploader");

            builder.HasOne(x => x.Uploader)
                .WithMany()
                .HasForeignKey(x => x.UploaderId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ObjectId, x.Created });

            // the version chain of a file is looked up by its name on every upload, and the file
            // surfaces group by the same pair - without this the next version number costs a scan
            builder.HasIndex(x => new { x.ObjectId, x.FileName, x.Version });
        }
    }
}
