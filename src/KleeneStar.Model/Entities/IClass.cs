using WebExpress.WebCore.WebDomain;
using WebExpress.WebIndex;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a class entity.
    /// </summary>
    public interface IClass : IIndexItem, IDomain
    {
        /// <summary>
        /// Returns the name of the class.
        /// </summary>
        string Name { get; }
    }
}
