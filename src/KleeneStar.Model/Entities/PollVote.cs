using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// The answer of one account to one option of a poll.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A voter has at most one answer per option. In a scheduling poll the voter answers every
    /// date with yes, if need be or no; in a decision the options chosen are answered yes, the
    /// others are not answered. An option without an answer of the voter counts as not chosen.
    /// </para>
    /// <para>
    /// The vote goes with its option (foreign key, cascade), the option with its poll object.
    /// <see cref="ObjectId"/> is kept beside it for reading a poll's votes in one statement; it
    /// carries no foreign key of its own, because SQL Server refuses a second cascading path into
    /// the object table. The identity is referenced without a foreign key: removing an account
    /// must not depend on the polls it voted in.
    /// </para>
    /// </remarks>
    public class PollVote : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the vote.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the poll object.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the option answered.
        /// </summary>
        public Guid OptionId { get; set; }

        /// <summary>
        /// Gets or sets the account that answered.
        /// </summary>
        public Guid IdentityId { get; set; }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        public PollAnswer Answer { get; set; } = PollAnswer.Yes;

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last change.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PollVote()
        {
            Id = Guid.NewGuid();
        }
    }

    /// <summary>
    /// The answer of a voter to an option of a poll. The values are persisted.
    /// </summary>
    public enum PollAnswer
    {
        /// <summary>
        /// The option does not suit the voter.
        /// </summary>
        No = 0,

        /// <summary>
        /// The option suits the voter if need be.
        /// </summary>
        Maybe = 1,

        /// <summary>
        /// The option suits the voter, or the voter chose it.
        /// </summary>
        Yes = 2
    }
}
