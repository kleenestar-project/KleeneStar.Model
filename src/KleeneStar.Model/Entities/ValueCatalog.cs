using System;
using System.ComponentModel.DataAnnotations;
using WebExpress.WebIndex.WebAttribute;

namespace KleeneStar.Model.Entities
{
    /// <summary>
    /// A central list of values - locations, cost centers, applications, suppliers - that
    /// selection fields of many classes draw their options from.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The values come from the catalog's <see cref="Source"/>: typed in, read from the objects a
    /// WQL query selects (every server of the CMDB), or synchronized from another system's
    /// address. Every change is a new, immutable <see cref="ValueCatalogVersion"/>, so what a
    /// list held at any time can be told and brought back.
    /// </para>
    /// <para>
    /// A catalog of a workspace serves the fields of that workspace; one without a workspace is
    /// installation-wide and administered by the installation's administrators.
    /// </para>
    /// </remarks>
    public class ValueCatalog : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the catalog.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the workspace, or <c>null</c> for an installation-wide catalog.
        /// </summary>
        public Guid? WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the name, unique within its workspace (or among the installation-wide ones).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets where the values come from.
        /// </summary>
        public ValueCatalogSource Source { get; set; }

        /// <summary>
        /// Gets or sets the WQL query of a query catalog.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the attribute of the selected objects a query catalog lists:
        /// <c>summary</c> or <c>key</c>.
        /// </summary>
        public string ValueAttribute { get; set; }

        /// <summary>
        /// Gets or sets the address an external catalog is read from.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets how often a query or external catalog is read again, in minutes, or
        /// <c>null</c> for on demand only.
        /// </summary>
        public int? RefreshMinutes { get; set; }

        /// <summary>
        /// Gets or sets whether the catalog is used.
        /// </summary>
        public ValueCatalogState State { get; set; }

        /// <summary>
        /// Gets or sets the number of the current version; 0 before the first.
        /// </summary>
        public int CurrentVersion { get; set; }

        /// <summary>
        /// Gets or sets when the source was last read, or <c>null</c>.
        /// </summary>
        public DateTime? LastRefresh { get; set; }

        /// <summary>
        /// Gets or sets why the source could not be read last time, or <c>null</c>.
        /// </summary>
        public string LastError { get; set; }

        /// <summary>
        /// Gets or sets who created the catalog; a query catalog reads as them.
        /// </summary>
        public Guid? CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time the catalog was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time the catalog was updated.
        /// </summary>
        public DateTime Updated { get; set; }
    }

    /// <summary>
    /// Where the values of a <see cref="ValueCatalog"/> come from. Persisted as the ordinal.
    /// </summary>
    public enum ValueCatalogSource
    {
        /// <summary>Typed in.</summary>
        Static,

        /// <summary>The objects a WQL query selects.</summary>
        Query,

        /// <summary>Another system's address.</summary>
        External
    }

    /// <summary>
    /// Whether a <see cref="ValueCatalog"/> is used. Persisted as the ordinal.
    /// </summary>
    public enum ValueCatalogState
    {
        /// <summary>The catalog is used and refreshed.</summary>
        Active,

        /// <summary>The catalog keeps its values but is not refreshed.</summary>
        Disabled
    }

    /// <summary>
    /// The values of a <see cref="ValueCatalog"/> at one point in time. Never changed once written.
    /// </summary>
    public class ValueCatalogVersion : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the version.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the catalog.
        /// </summary>
        public Guid CatalogId { get; set; }

        /// <summary>
        /// Gets or sets the number, counting from 1 per catalog.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the values (JSON: <c>[{ "value", "label" }]</c>).
        /// </summary>
        public string Values { get; set; }

        /// <summary>
        /// Gets or sets how many values were added against the version before.
        /// </summary>
        public int Added { get; set; }

        /// <summary>
        /// Gets or sets how many values were removed against the version before.
        /// </summary>
        public int Removed { get; set; }

        /// <summary>
        /// Gets or sets why the version exists: typed, refreshed, restored.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets who made the version; empty for a refresh.
        /// </summary>
        public Guid? CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time the version was made.
        /// </summary>
        public DateTime Created { get; set; }
    }

    /// <summary>
    /// A selection field that draws its options from a catalog.
    /// </summary>
    /// <remarks>
    /// The binding writes the values into the field's options, so every form, filter and
    /// renderer reads them as before. A field pinned to a version keeps it when the catalog
    /// moves on; an unpinned one follows the catalog.
    /// </remarks>
    public class FieldCatalogBinding : IEntity
    {
        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        [IndexIgnore]
        [Key]
        public int RawId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the binding.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the field; one binding per field.
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// Gets or sets the catalog.
        /// </summary>
        public Guid CatalogId { get; set; }

        /// <summary>
        /// Gets or sets the version the field is pinned to, or <c>null</c> to follow the catalog.
        /// </summary>
        public int? PinnedVersion { get; set; }
    }
}
