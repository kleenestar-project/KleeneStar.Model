using System.ComponentModel.DataAnnotations;
using WebExpress.WebCore.WebDomain;
using WebExpress.WebIndex;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents an entity that provides information and supports indexing.
    /// </summary>
    public interface IEntity : IIndexItem, IDomain
    {
        /// <summary>
        /// Gets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        int RawId { get; }
    }
}