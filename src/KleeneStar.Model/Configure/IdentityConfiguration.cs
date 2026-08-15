using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebExpress.WebCore.WebIdentity;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the Identity entity type.
    /// </summary>
    internal class IdentityConfiguration : IEntityTypeConfiguration<Identity>
    {
        /// <summary>
        /// Configuration of the field entity.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Configure(EntityTypeBuilder<Identity> builder)
        {
            builder.ToTable("Identity");

            // Primary key
            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            // business GUID
            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.Avatar)
                .HasColumnName("Icon")
                .HasMaxLength(256)
                .HasConversion
                (
                    icon => icon != null && icon.Uri != null ? icon.Uri.ToString() : null,
                    uri => string.IsNullOrEmpty(uri) ? null : ImageIcon.FromString(uri)
                );

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Email)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.State)
                .HasColumnName("State")
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .HasColumnName("PasswordHash")
                .IsRequired()
                .HasMaxLength(512);

            // profile page — publicly visible inside the tenant
            builder.Property(x => x.UserName)
                .HasColumnName("UserName")
                .HasMaxLength(64);

            builder.Property(x => x.EmailVerified)
                .HasColumnName("EmailVerified")
                .IsRequired();

            builder.Property(x => x.Bio)
                .HasColumnName("Bio")
                .HasMaxLength(1024);

            builder.Property(x => x.PhoneCountry)
                .HasColumnName("PhoneCountry")
                .HasMaxLength(8);

            builder.Property(x => x.Phone)
                .HasColumnName("Phone")
                .HasMaxLength(64);

            builder.Property(x => x.Website)
                .HasColumnName("Website")
                .HasMaxLength(256);

            builder.Property(x => x.Location)
                .HasColumnName("Location")
                .HasMaxLength(128);

            builder.Property(x => x.Position)
                .HasColumnName("Position")
                .HasMaxLength(128);

            // account page — login, language, time zone and regional formats
            builder.Property(x => x.Language)
                .HasColumnName("Language")
                .HasMaxLength(16);

            builder.Property(x => x.TimeZone)
                .HasColumnName("TimeZone")
                .HasMaxLength(64);

            builder.Property(x => x.DateFormat)
                .HasColumnName("DateFormat")
                .HasMaxLength(32);

            builder.Property(x => x.WeekStart)
                .HasColumnName("WeekStart")
                .IsRequired();

            // tenant & role page — business data of the identity's active tenant
            builder.Property(x => x.Role)
                .HasColumnName("Role")
                .HasMaxLength(128);

            builder.Property(x => x.RoleSince)
                .HasColumnName("RoleSince");

            builder.Property(x => x.Department)
                .HasColumnName("Department")
                .HasMaxLength(128);

            builder.Property(x => x.CostCenter)
                .HasColumnName("CostCenter")
                .HasMaxLength(64);

            builder.Property(x => x.PersonnelNumber)
                .HasColumnName("PersonnelNumber")
                .HasMaxLength(64);

            // self-referencing, nullable FK: the deputy is another identity, and removing it
            // must not cascade into the identities it stands in for.
            builder.Property(x => x.DeputyId)
                .HasColumnName("Deputy")
                .HasMaxLength(36);

            builder.HasOne(x => x.Deputy)
                .WithMany()
                .HasForeignKey(x => x.DeputyId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.UserName);

            // nullable FK: portal-side accounts carry the tenant they belong to;
            // operator-side accounts (the seeded admin, integration users) stay
            // tenant-less and are simply excluded from IssueScope.Organization
            // queries by the portal.
            builder.Property(x => x.TenantId)
                .HasColumnName("Tenant")
                .HasMaxLength(36);

            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .HasPrincipalKey(t => t.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TenantId);

            // m:n relation via IdentityGroupMembership
            builder.HasMany(x => x.GroupMemberships)
                .WithOne(x => x.Identity)
                .HasForeignKey(x => x.IdentityId)
                .OnDelete(DeleteBehavior.Cascade);

            // ignore interface projection
            builder.Ignore(x => ((IIdentity)x).Groups);
        }
    }
}