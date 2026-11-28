using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// One option of a poll: a proposed date of a scheduling poll or a choice of a decision.
    /// </summary>
    /// <remarks>
    /// A proposed date carries <see cref="Start"/> (and <see cref="End"/>, unless it is a mere
    /// point in time) in UTC; a choice carries <see cref="Text"/> only. A date may carry a text
    /// as well, a remark such as "in room 4.12".
    /// </remarks>
    public class PollOption : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the option.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the poll object.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the position of the option within the poll.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the text of a choice, or the remark of a proposed date.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the start of a proposed date (UTC), or <see langword="null"/> for a choice.
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// Gets or sets the end of a proposed date (UTC), or <see langword="null"/>.
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Gets or sets whether a proposed date covers whole days.
        /// </summary>
        public bool AllDay { get; set; }

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
        public PollOption()
        {
            Id = Guid.NewGuid();
        }
    }
}
