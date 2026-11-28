using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KleeneStar.Model.Configure
{
    /// <summary>
    /// Provides the Entity Framework Core configuration for the settings of polls.
    /// </summary>
    internal class PollConfiguration : IEntityTypeConfiguration<Poll>
    {
        /// <summary>
        /// Configures the entity type mapping for the Poll entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder.ToTable("Poll");

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

            // the settings go with their poll
            builder.HasOne<Object>()
                .WithMany()
                .HasForeignKey(x => x.ObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ObjectId)
                .IsUnique();

            builder.Property(x => x.Type)
                .HasColumnName("Type")
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.MultipleChoice)
                .HasColumnName("MultipleChoice");

            builder.Property(x => x.AllowMaybe)
                .HasColumnName("AllowMaybe");

            builder.Property(x => x.Anonymous)
                .HasColumnName("Anonymous");

            builder.Property(x => x.HideResults)
                .HasColumnName("HideResults");

            builder.Property(x => x.Deadline)
                .HasColumnName("Deadline");

            builder.Property(x => x.Closed)
                .HasColumnName("Closed");

            builder.Property(x => x.ClosedById)
                .HasColumnName("ClosedBy");

            // no foreign key: the option is removed with the poll anyway, and a second path into
            // the option table would be refused by SQL Server
            builder.Property(x => x.ChosenOptionId)
                .HasColumnName("ChosenOption");

            builder.Property(x => x.TimeZone)
                .HasColumnName("TimeZone")
                .HasMaxLength(64);

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            // the running polls are those not closed, by their deadline
            builder.HasIndex(x => new { x.Closed, x.Deadline });
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the options of polls.
    /// </summary>
    internal class PollOptionConfiguration : IEntityTypeConfiguration<PollOption>
    {
        /// <summary>
        /// Configures the entity type mapping for the PollOption entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PollOption> builder)
        {
            builder.ToTable("PollOption");

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

            // the options go with their poll
            builder.HasOne<Object>()
                .WithMany()
                .HasForeignKey(x => x.ObjectId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Order)
                .HasColumnName("Order");

            builder.Property(x => x.Text)
                .HasColumnName("Text")
                .HasMaxLength(512);

            builder.Property(x => x.Start)
                .HasColumnName("Start");

            builder.Property(x => x.End)
                .HasColumnName("End");

            builder.Property(x => x.AllDay)
                .HasColumnName("AllDay");

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => new { x.ObjectId, x.Order });
        }
    }

    /// <summary>
    /// Provides the Entity Framework Core configuration for the votes of polls.
    /// </summary>
    internal class PollVoteConfiguration : IEntityTypeConfiguration<PollVote>
    {
        /// <summary>
        /// Configures the entity type mapping for the PollVote entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PollVote> builder)
        {
            builder.ToTable("PollVote");

            builder.HasKey(x => x.RawId);

            builder.Property(x => x.RawId)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Id)
                .HasColumnName("Guid")
                .IsRequired()
                .HasMaxLength(36);

            // no foreign key: the vote goes with its option, which goes with the poll; a second
            // cascading path into the object table is refused by SQL Server
            builder.Property(x => x.ObjectId)
                .HasColumnName("Object")
                .IsRequired();

            builder.Property(x => x.OptionId)
                .HasColumnName("Option")
                .IsRequired();

            // the answers go with the option they answer
            builder.HasOne<PollOption>()
                .WithMany()
                .HasForeignKey(x => x.OptionId)
                .HasPrincipalKey(o => o.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // no foreign key: an account is removed without asking its votes
            builder.Property(x => x.IdentityId)
                .HasColumnName("Identity")
                .IsRequired();

            builder.Property(x => x.Answer)
                .HasColumnName("Answer")
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("Created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("Updated")
                .IsRequired();

            builder.HasIndex(x => new { x.OptionId, x.IdentityId })
                .IsUnique();
            builder.HasIndex(x => new { x.ObjectId, x.IdentityId });
        }
    }
}
