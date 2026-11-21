using System;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// Names the action a <see cref="Commit"/> records. The type is the reason the commit
    /// exists; the <see cref="Change"/> entries beneath it are what that reason did to the
    /// object's fields.
    /// </summary>
    /// <remarks>
    /// The values are persisted as their ordinal, so new entries are appended rather than
    /// inserted. <see cref="Created"/> is first because it is the genesis commit of every
    /// chain and therefore the value rows written before the column existed would read as.
    /// </remarks>
    public enum CommitType
    {
        /// <summary>
        /// The genesis commit of an object. It has no predecessor and carries a change
        /// entry for every populated field, so the chain is replayable from the start.
        /// </summary>
        Created,

        /// <summary>
        /// A field-level edit of an existing object.
        /// </summary>
        Updated,

        /// <summary>
        /// A workflow state change travelled along a transition.
        /// </summary>
        Transitioned,

        /// <summary>
        /// The object was archived. Carries no field changes of its own.
        /// </summary>
        Archived,

        /// <summary>
        /// A historical state was reapplied as a new commit, or an archived object was
        /// brought back.
        /// </summary>
        Restored,

        /// <summary>
        /// The terminal commit of a chain. The object row is gone; its history remains for
        /// audit purposes.
        /// </summary>
        Deleted
    }

    /// <summary>
    /// Provides extension methods for the <see cref="CommitType"/> enumeration.
    /// </summary>
    public static class CommitTypeExtensions
    {
        /// <summary>
        /// Returns a stable unique identifier for the specified commit type.
        /// </summary>
        /// <param name="type">The commit type.</param>
        /// <returns>The commit type's GUID.</returns>
        public static Guid Id(this CommitType type)
        {
            return type switch
            {
                CommitType.Created => Guid.Parse("2B7F41C6-9D53-4A08-8E1C-5F0D3A6B27E4"),
                CommitType.Updated => Guid.Parse("3C8052D7-AE64-4B19-9F2D-60E14B7C38F5"),
                CommitType.Transitioned => Guid.Parse("4D9163E8-BF75-4C2A-A03E-71F25C8D4906"),
                CommitType.Archived => Guid.Parse("5EA274F9-C086-4D3B-B14F-82036D9E5A17"),
                CommitType.Restored => Guid.Parse("6FB3850A-D197-4E4C-C250-93147EAF6B28"),
                CommitType.Deleted => Guid.Parse("70C4961B-E2A8-4F5D-D361-A4258FB07C39"),
                _ => Guid.Empty
            };
        }

        /// <summary>
        /// Returns the wire token the REST API exchanges the commit type as.
        /// </summary>
        /// <param name="type">The commit type.</param>
        /// <returns>The lower-case wire token.</returns>
        public static string Token(this CommitType type)
        {
            return type switch
            {
                CommitType.Created => "created",
                CommitType.Updated => "updated",
                CommitType.Transitioned => "transitioned",
                CommitType.Archived => "archived",
                CommitType.Restored => "restored",
                CommitType.Deleted => "deleted",
                _ => "updated"
            };
        }

        /// <summary>
        /// Returns the localized text key for the specified commit type, suitable for
        /// passing to <c>I18N.Translate</c>.
        /// </summary>
        /// <param name="type">The commit type.</param>
        /// <returns>The translation key, or <c>null</c> for unknown values.</returns>
        public static string Text(this CommitType type)
        {
            return type switch
            {
                CommitType.Created => "kleenestar.core:object.history.type.created",
                CommitType.Updated => "kleenestar.core:object.history.type.updated",
                CommitType.Transitioned => "kleenestar.core:object.history.type.transitioned",
                CommitType.Archived => "kleenestar.core:object.history.type.archived",
                CommitType.Restored => "kleenestar.core:object.history.type.restored",
                CommitType.Deleted => "kleenestar.core:object.history.type.deleted",
                _ => null
            };
        }

        /// <summary>
        /// Returns the CSS color-selection class associated with the commit type, used to
        /// tint the badge in the history list.
        /// </summary>
        /// <param name="type">The commit type.</param>
        /// <returns>The CSS class string.</returns>
        public static string Color(this CommitType type)
        {
            return type switch
            {
                CommitType.Created => TypeColorSelection.Success.ToClass(),
                CommitType.Updated => TypeColorSelection.Primary.ToClass(),
                CommitType.Transitioned => TypeColorSelection.Info.ToClass(),
                CommitType.Archived => TypeColorSelection.Warning.ToClass(),
                CommitType.Restored => TypeColorSelection.Info.ToClass(),
                CommitType.Deleted => TypeColorSelection.Danger.ToClass(),
                _ => TypeColorSelection.Default.ToClass()
            };
        }

        /// <summary>
        /// Parses a wire token into the matching commit type. An unknown, empty or
        /// <c>null</c> token reads as <see cref="CommitType.Updated"/>.
        /// </summary>
        /// <param name="token">The wire token.</param>
        /// <returns>The parsed commit type.</returns>
        public static CommitType Parse(string token)
        {
            return (token?.Trim() ?? string.Empty).ToLowerInvariant() switch
            {
                "created" => CommitType.Created,
                "transitioned" => CommitType.Transitioned,
                "archived" => CommitType.Archived,
                "restored" => CommitType.Restored,
                "deleted" => CommitType.Deleted,
                _ => CommitType.Updated
            };
        }
    }
}
