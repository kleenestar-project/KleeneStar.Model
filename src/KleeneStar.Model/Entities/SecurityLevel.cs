using KleeneStar.Model.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebApp.WebAttribute;
using WebExpress.WebIndex.WebAttribute;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Represents a security level entity: a classification an object of the class may carry,
    /// together with the groups whose members are cleared to see objects carrying it.
    /// </summary>
    /// <remarks>
    /// A security level is defined per class, exactly the way a <see cref="Field"/> is - the
    /// class is the catalog, there is no enum of levels, and an administrator decides which
    /// levels a class knows.
    /// <para>
    /// <b>The clearance is the group list.</b> <see cref="PermittedGroupIds"/> names the groups
    /// whose members may see - and assign - the level. A level that names no group is closed:
    /// nobody but the identities cleared through a group sees it, and creating a level without
    /// naming a group therefore hides every object classified with it. An object that carries
    /// no level at all is unclassified and stays visible to everyone.
    /// </para>
    /// </remarks>
    public class SecurityLevel : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the security level.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the security level, e.g. <c>Vertraulich</c>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the security level.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current state of the security level.
        /// </summary>
        [RestConverter<SecurityLevelStateConverter>]
        public SecurityLevelState State { get; set; }

        /// <summary>
        /// Gets or sets the rank of the security level within its class, lower being less
        /// restrictive. The rank orders the levels in every list and selection; it carries no
        /// authority of its own - a higher rank does not imply the clearance of a lower one.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with this security level.
        /// </summary>
        public ImageIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the class this security level belongs to.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Gets or sets the class associated with the current security level.
        /// </summary>
        public Class Class { get; set; }

        /// <summary>
        /// Gets or sets the identifiers of the groups whose members are cleared for this level.
        /// An empty list closes the level: no identity is cleared for it.
        /// </summary>
        [RestConverter<GuidListConverter>]
        public List<Guid> PermittedGroupIds { get; set; } = [];

        /// <summary>
        /// Gets or sets a value indicating whether the level is preselected when an object of
        /// the class is created. At most one level of a class should carry the flag; where
        /// several do, the one with the lowest <see cref="Rank"/> wins.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SecurityLevel()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the class with the
        /// specified unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier to assign to the security level.
        /// </param>
        public SecurityLevel(Guid id)
        {
            Id = id;
        }
    }
}
