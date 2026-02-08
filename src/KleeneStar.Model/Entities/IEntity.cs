using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
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
        /// Returns the database id.
        /// </summary>
        [RestTableColumnHidden]
        [IndexIgnore]
        [Key]
        int RawId { get; }
    }
}